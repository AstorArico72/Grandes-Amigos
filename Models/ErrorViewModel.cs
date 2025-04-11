namespace Grandes_Amigos.Models;

public class ErrorViewModel
{
    //Pendiente: Hacer vistas de error con Angular.
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
