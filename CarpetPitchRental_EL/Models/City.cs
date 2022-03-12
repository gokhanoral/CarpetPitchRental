using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetPitchRental_EL.Models
{
    [Table("Cities")]
    public class City:Base<int>
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "İl adı en az 2 en çok 50 karakter olmalıdır!")]
        public string CityName { get; set; }

        public virtual List<District> Districts { get; set; } //İlçeler ile 1'e çok olacak şekilde bir ilişki kurdum.
    }
}
