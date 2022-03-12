using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetPitchRental_EL.Models
{
    [Table("Facilities")]
    public class Facility:Base<int>
    {
        [Required]
        [StringLength(400, MinimumLength = 2, ErrorMessage = "Tesis adı en az 2 en çok 400 karakter olabilir!")]
        public string FacilityName { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Adres bilgisi en fazla 500 karakter olmalıdır!")]
        public string Address { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Telefon Numarası başında 0 olmadan 10 haneli olacak şekilde girilmelidir!")] 
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int DistrictId { get; set; }
        [ForeignKey("DistrictId")]
        public virtual District FacilityDistrict { get; set; }
        public string EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
        public virtual List<Reservation> FacilityReservations { get; set; }
       


    }
}
