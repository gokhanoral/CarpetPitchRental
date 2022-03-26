using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetPitchRental_EL.Enums
{
    public class AllEnums
    {
    }
    public enum ReservationStatus : byte
    {
        Past = 0,
        Active = 1,
    }
    public enum RoleNames : byte
    {
        Passive,
        Admin,
        Member,
        PassiveEmployee,
        ActiveEmployee
    }
}
