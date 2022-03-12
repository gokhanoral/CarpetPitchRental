using CarpetPitchRental_EL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetPitchRental_EL.Models
{
    [Table("Reservations")]
    public class Reservation : Base<int>
    {
        public ReservationStatus ReservationStatus { get; set; }

        public int FacilityId { get; set; }
        [ForeignKey("FacilityId")]
        public virtual Facility Facility { get; set; }

        public string MemberId { get; set; }
        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; }
        public virtual ReservationDate ReservationDate { get; set; }

    }
}
