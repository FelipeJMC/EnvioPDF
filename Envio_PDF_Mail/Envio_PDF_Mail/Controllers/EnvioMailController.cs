using Envio_PDF_Mail.Model;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Net.Mail;
using System.Net.Mime;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Envio_PDF_Mail.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvioMailController : ControllerBase
    {

        [HttpPost]
        public IActionResult SendPdfEmail([FromBody] PdfModel model)
        {
            try
            {
                
                byte[] pdfBytes = Convert.FromBase64String(model.Base64Pdf);

                // ESTO ES LO IMPORTANTE CREAR EL PDF EN MEMORIA!!!
                using var stream = new MemoryStream(pdfBytes);
                var attachment = new MimePart("application", "pdf")
                {
                    Content = new MimeContent(stream, ContentEncoding.Default),
                    ContentDisposition = new MimeKit.ContentDisposition(MimeKit.ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = "archivo.pdf"
                };

              
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Felipe Morales", "f.moralescandia@outlook.com"));
                email.To.Add(new MailboxAddress("Felipe Morales", "felipe.moralescandia@gmail.com"));
                email.Subject = "Documento PDF";
                var body = new TextPart("plain") { Text = @"Hola, te adjunto el PDF." };
                var multipart = new Multipart("mixed") { body, attachment };
                email.Body = multipart;

                // Enviar el correo
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                smtp.Connect("smtp.outlook.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("f.moralescandia@outlook.com", "no te voy a dar mi password!!!");
                smtp.Send(email);
                smtp.Disconnect(true);

                return Ok("Correo enviado exitosamente.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
