namespace Preguntin_ASP.NET.Models
{
    public class JwtModel
    {
        public required string Secret { get; set; }      
        public required string Issuer {  get; set; }
        public required string Audience { get; set; }    
        public double ExpirationMinute { get; set; }
    }
}
