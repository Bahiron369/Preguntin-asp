namespace Preguntin_ASP.NET.Models.DTO
{
    public class ResponseHttp
    {
        public int response_code { get; set; }  
        public ICollection<PreguntasHttp>? results { get; set; }
    }
}
