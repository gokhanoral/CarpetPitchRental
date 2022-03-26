using CarpetPitchRental_BLL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetPitchRental_BLL.EmailService
{
    public interface IEmailSender
    {
        Task SendAsync(EmailMessage message);

        void SendAppointmentPdf(EmailMessage message, ReservationViewModel data);
        Task SendAppointmentPdfAsync(EmailMessage message, ReservationViewModel data);
    }
}
