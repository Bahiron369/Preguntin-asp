using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using Preguntin_ASP.NET.Models.DTO;
namespace Preguntin_ASP.NET.Services.confirmacion_usuario
{
    public interface IConfirmInformation
    {
        public Task SendMessageAsync(string CurrentEmail, string EmailDireccion, string asunto, string bodyMessage);
    }
    public class ConfirmInformation : IConfirmInformation
    {
        private readonly LoginAdmin _credential;
        public ConfirmInformation(IOptions<LoginAdmin> credential)
        {
            _credential = credential.Value;
        }

        public async Task SendMessageAsync(string CurrentEmail, string EmailDireccion, string asunto, string bodyMessage)
        {
            MimeMessage message = new MimeMessage(); //creamos el mensaje
            //correos electronicos
            message.From.Add(MailboxAddress.Parse(CurrentEmail)); //creamos el remitente
            message.To.Add(MailboxAddress.Parse(EmailDireccion)); //creamos el destinario
            //asunto
            message.Subject = asunto;
            //Cuerpo del mensaje
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = bodyMessage };

            //creamos el SMTP (Protocolo de envio de mensajes)
            using SmtpClient smpt = new SmtpClient();
            //abrimos una conexion con el host, el puerto (seguro) y la seguridad Tls
            await smpt.ConnectAsync(_credential.Smpt, 587, SecureSocketOptions.StartTls);//agregamos configuracion de puertos
            await smpt.AuthenticateAsync(_credential.Email, _credential.Token); //agregamos cuenta
            await smpt.SendAsync(message); //enviamos el email
            await smpt.DisconnectAsync(true); //*se desconecta*
        }
    }
}
