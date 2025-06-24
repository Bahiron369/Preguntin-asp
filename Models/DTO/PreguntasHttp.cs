namespace Preguntin_ASP.NET.Models.DTO
{
    public class PreguntasHttp
    {
        public string type { get; set; }    
        public string difficulty { get; set; }
        public string category { get; set; }
        public string question { get; set; }
        public string correct_answer { get; set; }
        public ICollection<string> incorrect_answers { get; set; }
    }
}
