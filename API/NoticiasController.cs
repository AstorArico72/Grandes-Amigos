using System.Xml;
using System.Xml.Linq;
using System.ServiceModel.Syndication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mime;
using Swashbuckle.AspNetCore.SwaggerGen;
using Grandes_Amigos.Models;
using MySqlConnector;
using Microsoft.AspNetCore.Http.Features;

[ApiController]
[ApiVersionNeutral]
[Route("/Api/Noticias")]
public class NoticiasController : Controller {
    private readonly ILogger<NoticiasController> _logger;
    private ContextoDb Contexto;
    private XmlReader LectorRSS;

    public NoticiasController(ILogger<NoticiasController> logger, ContextoDb contexto) {
        _logger = logger;
        Contexto = contexto;
        LectorRSS = XmlReader.Create ("https://www.perfil.com/feed");
    }

    [AllowAnonymous]
    [HttpGet("Leer")]
    public IActionResult LeerRss () {
        SyndicationFeed Feed = SyndicationFeed.Load (LectorRSS);
        var Post = Feed.Items.FirstOrDefault ();
        return Ok (Post);
    }

    [AllowAnonymous]
    [HttpPost("Cargar")]
    public IActionResult CargarNoticias () {
        SyndicationFeed Feed = SyndicationFeed.Load (LectorRSS);

        //Ésto trae las noticias de hoy.
        var UltimasNoticias = Feed.Items.Where (item => item.PublishDate >= DateTime.Now.AddHours (-1)).ToArray ();
        int TotalArticulos = UltimasNoticias.Length;
        bool exito = true;
        int i = 0;

        try {
            do {
                var item = UltimasNoticias[i];
                Noticia NuevaNoticia = new Noticia ();

                List <SyndicationPerson> Autores = item.Authors.ToList ();
                List <string> NombresAutor = new List<string> ();

                foreach (var autor in Autores) {
                    //En la teoría debería ser así.
                    //NombresAutor.Add (autor.Name);
                    NombresAutor.Add (autor.Email); //Pero funciona así para el RSS de Perfil.
                }

                if (Autores.Count > 1) {
                    NuevaNoticia.Autor = string.Join (", ", NombresAutor);
                } else {
                    NuevaNoticia.Autor = NombresAutor.First ();
                }

                NuevaNoticia.Enlace = item.Links.First ().Uri.AbsoluteUri;
                NuevaNoticia.FechaPublicación = item.PublishDate.DateTime;
                NuevaNoticia.Título = item.Title.Text;
                NuevaNoticia.Contenido = item.Summary.Text;
                if (ModelState.IsValid) {
                    Contexto.Noticias.Add (NuevaNoticia);
                    Contexto.SaveChanges ();
                    i++;
                }
            } while (i < TotalArticulos);
        } catch (MySqlException ex) {
            exito = false;
            return StatusCode (500, ex);
        }

        if (exito == false) {
            return StatusCode (500, "Error interno.");
        } else {
            return Created ();
        }
    }
}