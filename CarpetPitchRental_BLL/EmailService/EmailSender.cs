using CarpetPitchRental_BLL.ViewModels;
using Microsoft.Extensions.Configuration;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CarpetPitchRental_BLL.EmailService
{
    public class EmailSender : IEmailSender
    {
        //injection
        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string SenderMail => _configuration.GetSection("EmailOptions:SenderMail").Value;
        public string Password => _configuration.GetSection("EmailOptions:Password").Value;
        public string Smtp => _configuration.GetSection("EmailOptions:Smtp").Value;
        public int SmtpPort => Convert.ToInt32(_configuration.GetSection("EmailOptions:SmtpPort").Value);
        public string CC => _configuration.GetSection("ManagerEmails:EmailToCC").Value;

        public void SendAppointmentPdf(EmailMessage message, ReservationViewModel data)
        {
            try
            {
                string htmlString =
                     "<html><head></head><body><h3>Hasta Adı:"
                     + data.Member.AppUser.Name + " " + data.Member.AppUser.Surname
                     + "</h3><br/>"
                     + "<h4>Aşağıda rezervasyon bilgileriniz bulunmaktadır.</h4><br/>"
                     + "<h4>İl - İlçe: "
                     + data.Facility.FacilityDistrict.City.CityName
                     + " - "
                     + data.Facility.FacilityDistrict.DistrictName
                     + "</h4><br/><h4>Tesis: " + data.Facility.FacilityName
                     + "</h4><br/>"
                     + "</h4><br/><h4>Doktor :" + data.Facility.Employee.AppUser.Name
                     + " "
                     + data.Facility.Employee.AppUser.Surname
                     + "</h4><br/>"
                     + "<h4>Rezervasyon Tarihi ve Saati:"
                     + data.Date
                     + "</h4></body></html>";

                // instantiate a html to pdf converter object
                HtmlToPdf converter = new HtmlToPdf();
                // create a new pdf document converting an url
                PdfDocument doc = converter.ConvertHtmlString(htmlString);

                // create memory stream to save PDF
                MemoryStream pdfStream = new MemoryStream();

                // save pdf document into a MemoryStream
                doc.Save(pdfStream);

                // reset stream position
                pdfStream.Position = 0;
                var mail = new MailMessage()
                {
                    From = new MailAddress(this.SenderMail)
                };

                //contacts 
                foreach (var item in message.Contacts)
                {
                    mail.To.Add(item);
                }
                //cc
                if (message.CC != null)
                {
                    foreach (var item in message.CC)
                    {
                        mail.CC.Add(new MailAddress(item));
                    }
                }

                if (CC != null)
                {
                    var ccData = CC.Split(',');
                    foreach (var item in ccData)
                    {
                        mail.CC.Add(new MailAddress(item));
                    }
                }
                //bcc
                if (message.BCC != null)
                {
                    foreach (var item in message.BCC)
                    {
                        mail.Bcc.Add(new MailAddress(item));
                    }
                }
                mail.Attachments.Add(new Attachment(pdfStream, "Document.pdf"));

                mail.Subject = message.Subject;
                mail.Body = message.Body;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = Encoding.UTF8;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.HeadersEncoding = Encoding.UTF8;

                var smtpClient = new SmtpClient(Smtp, SmtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(SenderMail, Password)
                };
                smtpClient.Send(mail);

                // close pdf document
                doc.Close();


            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task SendAppointmentPdfAsync(EmailMessage message, ReservationViewModel data)
        {
            try
            {
                string htmlString =
                     "<html><head></head><body><h3>Hasta Adı:"
                     + data.Member.AppUser.Name + " " + data.Member.AppUser.Surname
                     + "</h3><br/>"
                     + "<h4>Aşağıda rezervasyon bilgileriniz bulunmaktadır.</h4><br/>"
                     + "<h4>İl - İlçe: "
                     + data.Facility.FacilityDistrict.City.CityName
                     + " - "
                     + data.Facility.FacilityDistrict.DistrictName
                     + "</h4><br/><h4>Tesis: " + data.Facility.FacilityName
                     + "</h4><br/>"
                     + "</h4><br/><h4>Doktor :" + data.Facility.Employee.AppUser.Name
                     + " "
                     + data.Facility.Employee.AppUser.Surname
                     + "</h4><br/>"
                     + "<h4>Rezervasyon Tarihi ve Saati:"
                     + data.Date
                     + "</h4></body></html>";

                // instantiate a html to pdf converter object
                HtmlToPdf converter = new HtmlToPdf();
                // create a new pdf document converting an url
                PdfDocument doc = converter.ConvertHtmlString(htmlString);

                // create memory stream to save PDF
                MemoryStream pdfStream = new MemoryStream();

                // save pdf document into a MemoryStream
                doc.Save(pdfStream);

                // reset stream position
                pdfStream.Position = 0;
                var mail = new MailMessage()
                {
                    From = new MailAddress(this.SenderMail)
                };

                //contacts 
                foreach (var item in message.Contacts)
                {
                    mail.To.Add(item);
                }
                //cc
                if (message.CC != null)
                {
                    foreach (var item in message.CC)
                    {
                        mail.CC.Add(new MailAddress(item));
                    }
                }

                if (CC != null)
                {
                    var ccData = CC.Split(',');
                    foreach (var item in ccData)
                    {
                        mail.CC.Add(new MailAddress(item));
                    }
                }
                //bcc
                if (message.BCC != null)
                {
                    foreach (var item in message.BCC)
                    {
                        mail.Bcc.Add(new MailAddress(item));
                    }
                }
                mail.Attachments.Add(new Attachment(pdfStream, "Document.pdf"));

                mail.Subject = message.Subject;
                mail.Body = message.Body;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = Encoding.UTF8;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.HeadersEncoding = Encoding.UTF8;

                var smtpClient = new SmtpClient(Smtp, SmtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(SenderMail, Password)
                };
                await smtpClient.SendMailAsync(mail);

                // close pdf document
                doc.Close();


            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task SendAsync(EmailMessage message)
        {
            var mail = new MailMessage()
            {
                From = new MailAddress(this.SenderMail)
            };
            //Contacts
            foreach (var item in message.Contacts)
            {
                mail.To.Add(item);
            }
            //CC
            if (message.CC != null)
            {
                foreach (var item in message.CC)
                {
                    mail.CC.Add(new MailAddress(item));
                }
            }
            if (CC != null)
            {
                var ccData = CC.Split(',');
                foreach (var item in ccData)
                {
                    mail.CC.Add(new MailAddress(item));
                }
            }
            //BCC
            if (message.BCC != null)
            {
                foreach (var item in message.BCC)
                {
                    mail.Bcc.Add(new MailAddress(item));
                }
            }

            mail.Subject = message.Subject;
            mail.Body = message.Body;
            mail.IsBodyHtml = true;
            mail.BodyEncoding = Encoding.UTF8;
            mail.SubjectEncoding = Encoding.UTF8;
            mail.HeadersEncoding = Encoding.UTF8;

            var smtpClient = new SmtpClient(Smtp, SmtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(SenderMail, Password)
            };
            await smtpClient.SendMailAsync(mail);
        }
    }
}
