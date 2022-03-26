using CarpetPitchRental_EL.Enums;
using CarpetPitchRental_EL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetPitchRental_BLL.ViewModels
{
    public class ReservationViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }

        [Required]
        public string MemberId { get; set; }

        public int FacilityId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public ReservationStatus ReservationStatus { get; set; }

        public Member Member { get; set; }
        public Facility Facility { get; set; }
    }
}
