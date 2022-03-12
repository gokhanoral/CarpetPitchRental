using CarpetPitchRental_EL.IdentityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetPitchRental_EL.Models
{
    [Table("Members")]
    public class Member:PersonBase
    {
        public string UserId { get; set; }// Identity Model'in ID değeri burada Foreign Key olacaktır.
        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }

        public virtual List<Reservation> MemberReservations { get; set; }
    }
}
