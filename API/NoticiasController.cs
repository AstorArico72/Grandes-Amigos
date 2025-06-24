using System.Net;
using System.Net.Mime;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using Grandes_Amigos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;

[ApiController]
[ApiVersionNeutral]
[Route("/Api/Noticias")]
public class NoticiasController : Controller
{
    private readonly ILogger<NoticiasController> _logger;
    private ContextoDb Contexto;
    private XmlReader LectorRSS;

    public NoticiasController(ILogger<NoticiasController> logger, ContextoDb contexto)
    {
        _logger = logger;
        Contexto = contexto;
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Retorna una noticia.",
        Description = "Toma el ID de la noticia de la ruta; y si una noticia con ése ID existe en la base de datos, la lee."
    )]
    [SwaggerResponse(200, "La noticia existe en la base de datos.")]
    [SwaggerResponse(404, "La noticia no existe en la base de datos.")]
    public IActionResult LeerArticulo([FromRoute] int id)
    {
        Noticia? item = Contexto.Noticias.Find(id);
        if (item == null)
        {
            return NotFound("El ID seleccionado no existe.");
        }
        else
        {
            return Ok(item);
        }
    }

    [HttpGet("Cargar")]
    [Authorize(Policy = "Ministerio")]
    [SwaggerOperation(
        Summary = "Carga noticias a la base de datos.",
        Description = "Ésto lee el contenido de un archivo RSS y lo carga en la base de datos, artículo por artículo. Ésto sólo carga los artículos de la última hora."
    )]
    [SwaggerResponse(201, "Todas las nuevas noticias se añadieron con éxito.")]
    [SwaggerResponse(204, "El RSS está vacío o no hay artículos recientes.")]
    [SwaggerResponse(500, "Ocurrió una excepción MySQL. Lee la respuesta atentamente.")]
    [SwaggerResponse(502, "El servidor de noticias está caído o devolvió una respuesta inválida.")]
    public IActionResult CargarNoticias()
    {
        //Ésto hace un pedido preliminar al proveedor del RSS remoto, para evitar gastar recursos en cargar las noticias si pasa un error.
        LectorRSS = XmlReader.Create("https://www.lanacion.com.ar/arc/outboundfeeds/rss/");
        HttpClient client = new HttpClient();
        HttpRequestMessage message = new HttpRequestMessage();
        message.Method = HttpMethod.Get;
        message.RequestUri = new Uri(LectorRSS.BaseURI);
        int respuesta = (int)client.Send(message).StatusCode;
        switch (respuesta)
        {
            case 500:
                // 500: El servidor devolvió un error genérico.
                return StatusCode(502, "Error en el servidor de noticias.");
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Status/502
            // Al leer el RSS del proveedor, nuestro servidor hace de intermediario, por lo que correspondería devolver 502 si se recibe una respuesta inválida, o al menos éso creo.
            case 503:
                // 503: El servidor está caído.
                return StatusCode(502, "Error en el servidor de noticias.");
            case 404:
                // 404: La URI es inválida.
                return BadRequest(
                    "El enlace al RSS remoto es inválido - El servidor dió una respuesta HTTP 404."
                );
            case 403:
                // 403: La URI es válida, pero el contenido está bloqueado.
                return BadRequest(
                    "El RSS remoto está bloqueado - El servidor dió una respuesta HTTP 403."
                );
            case 204:
                // 204: La respuesta del servidor está vacía.
                return StatusCode(502, "Error en el servidor de noticias.");
            case 202:
                // 202: El servidor aceptó el pedido, pero no lo procesó.
                return StatusCode(504, "No se recibió respuesta del servidor de noticias.");
            case 200:
                // 200: El servidor devolvió una respuesta válida.
                break;
            default:
                return BadRequest("Error al traer el RSS remoto.");
        }
        ;

        SyndicationFeed Feed = SyndicationFeed.Load(LectorRSS);

        //Ésto trae las noticias de hoy.
        var UltimasNoticias = Feed
            .Items.Where(item => item.PublishDate >= DateTime.Now.AddHours(-1))
            .ToArray();
        int TotalArticulos = UltimasNoticias.Length;
        bool exito = true;
        int i = 0;

        if (Feed.Items.Count() == 0)
        {
            return NoContent();
        }

        try
        {
            do
            {
                var item = UltimasNoticias[i];
                Noticia NuevaNoticia = new Noticia();

                List<SyndicationPerson> Autores = item.Authors.ToList();
                List<string> NombresAutor = new List<string>();

                foreach (var autor in Autores)
                {
                    //En la teoría debería ser así.
                    //NombresAutor.Add (autor.Name);
                    NombresAutor.Add(autor.Name); //Pero funciona así para el RSS de Perfil.
                }

                if (Autores.Count > 1)
                {
                    NuevaNoticia.Autor = string.Join(", ", NombresAutor);
                }
                else
                {
                    NuevaNoticia.Autor = NombresAutor.First();
                }

                NuevaNoticia.Enlace = item.Links.First().Uri.AbsoluteUri;
                NuevaNoticia.FechaPublicación = item.PublishDate.DateTime;
                NuevaNoticia.Título = item.Title.Text;
                NuevaNoticia.Contenido = item.Summary.Text;
                if (ModelState.IsValid)
                {
                    Contexto.Noticias.Add(NuevaNoticia);
                    Contexto.SaveChanges();
                    i++;
                }
            } while (i < TotalArticulos);
        }
        catch (MySqlException ex)
        {
            exito = false;
            return StatusCode(500, ex);
        }

        if (exito == false)
        {
            return StatusCode(500, "Error interno.");
        }
        else
        {
            return Ok();
        }
    }
}
