using CarpetPitchRental_BLL.EmailService;
using CarpetPitchRental_BLL.Interfaces;
using CarpetPitchRental_EL.IdentityModels;
using CarpetPitchRental_EL.Models;
using CarpetPitchRental_UI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarpetPitchRental_UI.Controllers
{
    public class FacilityController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        //Dependency Injection

        public FacilityController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, IEmailSender emailSender, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        public JsonResult GetFacilityByDistrictId(int districtId)
        {
            //Amacım Ataşehir'deki tüm tesisleri getirmek.
            try
            {
                var data = new List<Facility>();
                if (districtId > 0)
                {
                    //İl
                    //İlçe
                    //Tesis

                    data = _unitOfWork.FacilityRepository
                    .GetAll(x => x.DistrictId == districtId, orderBy: x => x.OrderBy(y => y.FacilityName)).ToList();
                }
                return Json(new { isSuccess = true, data });
            }
            catch (Exception ex)
            {

                return Json(new { isSuccess = false });
            }
        }
        public IActionResult GetAllFacilities()
        {
            //Model'i ViewModel'e çevirip önyüze ViewModel'i yolladım.
            var list = _unitOfWork.FacilityRepository.GetAll().ToList();
            List<FacilityViewModel> facilityViews = new List<FacilityViewModel>();
            foreach(Facility facility in list){
                FacilityViewModel facilityView = new FacilityViewModel();
                facilityView.Id = facility.Id;
                facilityView.Address = facility.Address;
                facilityView.DistrictId = facility.DistrictId;
                facilityView.FacilityName = facility.FacilityName;
                facilityView.PhoneNumber = facility.PhoneNumber;
                facilityView.Email = facility.Email;
                facilityView.Address = facility.Address;

                facilityViews.Add(facilityView);
            }


            return View(facilityViews);
        }

        [HttpGet]
        public IActionResult FacilityAdd()
        {
            try
            {
                ViewBag.Cities = _unitOfWork.CityRepository.GetAll(orderBy: x => x.OrderBy(a => a.CityName));

                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public IActionResult FacilityAdd(FacilityViewModel model)
        {
            try
            {

                Facility newFacility = new Facility()
                {
                    FacilityName = model.FacilityName,
                    Address = model.Address,
                    Email = model.Email,
                    DistrictId = model.DistrictId,
                    PhoneNumber=model.PhoneNumber

                };

                bool result = _unitOfWork.FacilityRepository.Add(newFacility);
                if (result)
                {
                    return RedirectToAction("Index", "Home");
                    //Redirect Ana sayfa
                }
                throw new Exception("HATA : Beklenmedik bir sorun oluştu!"); //else
            }
            catch (Exception)
            {
                ViewBag.Cities = _unitOfWork.CityRepository.GetAll(orderBy: x => x.OrderBy(a => a.CityName));
                throw; // TODO : Buraya return View() yapılıp hata mesajı ekrana getirilecek.
            }
        }
        [HttpGet]
        public IActionResult FacilityEdit(int id)
        {
            ViewBag.Cities = _unitOfWork.CityRepository.GetAll(orderBy: x => x.OrderBy(a => a.CityName));
            
            //Id'si seçilen tesisi aldım.
            Facility facility = _unitOfWork.FacilityRepository.GetFirstOrDefault(x => x.Id.Equals(id));
            District selectedDistrict = _unitOfWork.DistrictRepository.GetFirstOrDefault(x => x.Id.Equals(facility.DistrictId));
            ViewBag.Districts = _unitOfWork.DistrictRepository.GetAll(x => x.CityId.Equals(selectedDistrict.CityId), orderBy: x => x.OrderBy(a => a.DistrictName));
            //District'teki CityId'den seçilen tesisin CityId'sini view'a yollıcam.
            ViewBag.SelectedCityId = selectedDistrict.CityId;


            FacilityViewModel model = new FacilityViewModel() 
            {
                FacilityName= facility.FacilityName,
                DistrictId=facility.DistrictId,
                PhoneNumber=facility.PhoneNumber,
                Email=facility.Email,
                Address=facility.Address
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult FacilityEdit(FacilityViewModel model)
        {

        }
    }


}
