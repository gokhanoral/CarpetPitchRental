using CarpetPitchRental_BLL.Interfaces;
using CarpetPitchRental_DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetPitchRental_BLL.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyContext _myContext;

        public UnitOfWork(MyContext myContext)
        {
            _myContext = myContext;

            CityRepository = new CityRepository(_myContext);
            DistrictRepository = new DistrictRepository(_myContext);
            FacilityRepository = new FacilityRepository(_myContext);
            EmployeeRepository = new EmployeeRepository(_myContext);
            MemberRepository = new MemberRepository(_myContext);
            ReservationRepository = new ReservationRepository(_myContext);
            ReservationDateRepository = new ReservationDateRepository(_myContext);
            //Constructor'da inşa ettim
        }

        public ICityRepository CityRepository { get; private set; }

        public IDistrictRepository DistrictRepository { get; private set; }

        public IFacilityRepository FacilityRepository { get; private set; }

        public IEmployeeRepository EmployeeRepository { get; private set; }

        public IMemberRepository MemberRepository { get; private set; }

        public IReservationRepository ReservationRepository { get; private set; }

        public IReservationDateRepository ReservationDateRepository { get; private set; }

        public void Dispose()
        {
            _myContext.Dispose();
        }
    }
}
