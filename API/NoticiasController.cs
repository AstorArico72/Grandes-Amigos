using System.Net;
using System.Net.Mime;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
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
    //[Authorize(Policy = "Ministerio")]
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
        // Pedido preliminar al proveedor del RSS remoto
        LectorRSS = XmlReader.Create("https://www.lanacion.com.ar/arc/outboundfeeds/rss/");
        HttpClient client = new HttpClient();
        HttpRequestMessage message = new HttpRequestMessage();
        message.Method = HttpMethod.Get;
        message.RequestUri = new Uri(LectorRSS.BaseURI);
        int respuesta = (int)client.Send(message).StatusCode;
        switch (respuesta)
        {
            case 500:
            case 503:
            case 204:
                return StatusCode(502, "Error en el servidor de noticias.");
            case 404:
                return BadRequest(
                    "El enlace al RSS remoto es inválido - El servidor dió una respuesta HTTP 404."
                );
            case 403:
                return BadRequest(
                    "El RSS remoto está bloqueado - El servidor dió una respuesta HTTP 403."
                );
            case 202:
                return StatusCode(504, "No se recibió respuesta del servidor de noticias.");
            case 200:
                break;
            default:
                return BadRequest("Error al traer el RSS remoto.");
        }

        SyndicationFeed Feed = SyndicationFeed.Load(LectorRSS);

        // Noticias de la última hora
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

                // Autor
                List<SyndicationPerson> Autores = item.Authors.ToList();
                List<string> NombresAutor = Autores.Select(a => a.Name).ToList();
                if (Autores.Count > 1)
                    NuevaNoticia.Autor = string.Join(", ", NombresAutor);
                else
                    NuevaNoticia.Autor =
                        NombresAutor.Count > 0 ? NombresAutor.First() : "Desconocido";

                // Enlace
                NuevaNoticia.Enlace = item.Links.Any() ? item.Links.First().Uri.AbsoluteUri : "";

                // Categoría
                NuevaNoticia.Categoría = item.Categories.Any()
                    ? item.Categories.First().Name
                    : "General";

                // Datos principales
                NuevaNoticia.FechaPublicación = item.PublishDate.DateTime;
                NuevaNoticia.Título = item.Title.Text;

                // 🔽 Contenido limpio (sin etiquetas HTML)
                if (!string.IsNullOrEmpty(item.Summary?.Text))
                {
                    NuevaNoticia.Contenido = Regex.Replace(item.Summary.Text, "<.*?>", "").Trim();
                }
                else
                {
                    NuevaNoticia.Contenido = "";
                }

                // EXTRAER IMAGEN
                string? imagenUrl = null;
                var media = item.ElementExtensions.FirstOrDefault(e =>
                    e.OuterName == "content" || e.OuterName == "thumbnail"
                );
                if (media != null)
                {
                    var attr = media.GetObject<XElement>().Attribute("url");
                    if (attr != null)
                        imagenUrl = attr.Value;
                }

                if (imagenUrl == null && item.Summary != null)
                {
                    var html = item.Summary.Text;
                    var match = Regex.Match(html, "<img.+?src=[\"'](.+?)[\"']");
                    if (match.Success)
                        imagenUrl = match.Groups[1].Value;
                }

                NuevaNoticia.ImagenUrl = imagenUrl;

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

        if (!exito)
        {
            return StatusCode(500, "Error interno.");
        }
        else
        {
            return Ok();
        }
    }
}
