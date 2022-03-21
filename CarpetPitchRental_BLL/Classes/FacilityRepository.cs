﻿using CarpetPitchRental_BLL.Interfaces;
using CarpetPitchRental_DAL;
using CarpetPitchRental_EL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarpetPitchRental_BLL.Classes
{
    public class FacilityRepository:Repository<Facility>,IFacilityRepository
    {
        public FacilityRepository(MyContext myContext)
            :base(myContext)
        {

        }
    }
}
