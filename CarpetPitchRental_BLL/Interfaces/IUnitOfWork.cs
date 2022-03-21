using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetPitchRental_BLL.Interfaces
{
    public interface IUnitOfWork:IDisposable
    {
        ICityRepository CityRepository { get; }
        IDistrictRepository DistrictRepository { get; }
        IFacilityRepository FacilityRepository { get; }
        IEmployeeRepository EmployeeRepository { get; }
        IMemberRepository MemberRepository { get; }
        IReservationRepository ReservationRepository { get; }
        IReservationDateRepository ReservationDateRepository { get; }
    }
}
