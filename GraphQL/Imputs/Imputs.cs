using Preguntin_ASP.NET.Models.DTO;
using Preguntin_ASP.NET.Models.Preguntas;
using Preguntin_ASP.NET.Models.Usuarios;

namespace Preguntin_ASP.NET.GraphQL.Imputs
{
    /// <summary>
    /// Actualiza la informacion basica del jugador
    /// </summary>
    /// <param name="nombre"></param>
    /// <param name="Email"></param>
    /// <param name="passworHash"></param>
    /// <param name="telefono"></param>
   public record InputUpdateInfJugador(
       string? nombre = null,
       string? Email = null,
       string? newPassword = null,
       string? OldPassword = null,
       string? telefono = null
    );

    public record InputValidEmail(string id,string newEmail, string tokenEmail); //para confirmar el token enviado al gmail
    public record InputValidEmailForget(string email); //para validar el email y enviar el token a este mismo correo
    public record InputUpdatePasswordForget(string email,string token, string password); //para cuando se resiva el token se cambie la contraseña

    /// <summary>
    /// Crea un CRUD para las preguntas del juego 
    /// </summary>
    public record InputAddcategoria(string Nombre, int? puntos = 0); //input añadir categoria
    public record InputDeleteCategoria(int id); //input para eleminar una categoria (No Default)
    public record InputPutNombreCategoria(int nombre); //Se puede cambiar el nombre de la categoria 

    //Input agregar nueva pregunta
    public record InputAddPreguntas(
        int idCategoria,
        string nombre,
        string dificultad,
        string tipo,
        string preguntaCorrecta,
        int puntosPreguntas,
        TimeSpan? Time
    );

    public record InputDeletePregunta(int idPregunta); //input eliminar pregunta
    public record InputAddRespuesta(int idPregunta, string texto, bool isCorrect);//input para agregar respuesta
    public record InputPutRespuesta(int idPregunta, string? texto, bool? isCorrect);//input para modificar respuesta

    /// <summary>
    /// Estos campos se devulven al usuario despues de hacer una mutacion, devulven el objecto, mensaje de error o estados
    /// </summary>
    public record JugadorPayLoad(string? token, string mensajes,bool valido);
    public record CategoriaPayLoad(ModelCategoria ModelCategoria, string mensaje);
    public record PreguntaPayLoad(PreguntasResponse Pregunta, string mensaje);
    public record RespuestaUpdatePayLoad(ModelRespuesta respuesta, string mensaje);

}
