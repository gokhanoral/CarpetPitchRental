using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetPitchRental_EL.IdentityModels
{
    public class AppRole : IdentityRole
    {
        [StringLength(400, ErrorMessage = "Rol açıklamasına en fazla 400 karakter girilebilir.")]
        public string Description { get; set; }
    }
}
