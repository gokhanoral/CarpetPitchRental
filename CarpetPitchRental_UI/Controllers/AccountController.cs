using CarpetPitchRental_BLL;
using CarpetPitchRental_BLL.EmailService;
using CarpetPitchRental_BLL.Interfaces;
using CarpetPitchRental_EL.Enums;
using CarpetPitchRental_EL.IdentityModels;
using CarpetPitchRental_EL.Models;
using CarpetPitchRental_UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CarpetPitchRental_UI.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager,IUnitOfWork unitOfWork, IEmailSender emailSender,IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                var checkUserForUserName = await _userManager.FindByNameAsync(model.TCNumber);
                if (checkUserForUserName != null)
                {
                    ModelState.AddModelError(nameof(model.TCNumber), "Bu TC kimlik numarası ile sisteme daha önce kayıt olunmuştur!");
                    return View(model);
                }
                var checkUserForEmail = await _userManager.FindByEmailAsync(model.Email);
                if (checkUserForEmail != null)
                {
                    ModelState.AddModelError(nameof(model.Email), "Bu email ile sisteme daha önce kayıt olunmuştur.");
                    return View(model);
                }
                //Yeni kullanıcı oluşturalım.
                AppUser newUser = new AppUser()
                {
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname,
                    UserName = model.TCNumber,
                    //TO DO : Birthdate?
                    //TO DO : PhoneNumber
                };
                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded)
                {
                    var roleResult = _userManager.AddToRoleAsync(newUser, RoleNames.Passive.ToString());
                    //email gönderilmelidir.
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callBackUrl = Url.Action("ConfirmEmail", "Account", new { userId = newUser.Id, code = code }, protocol: Request.Scheme);

                    var emailMessage = new EmailMessage()
                    {
                        Contacts = new string[] { newUser.Email },
                        Subject = "CPR - Email Aktivasyonu",
                        Body = $"Merhaba {newUser.Name} {newUser.Surname},<br> Hesabınızı aktifleştirmek için <a href='{HtmlEncoder.Default.Encode(callBackUrl)}'>buraya<a/> tıklayınız."
                    };
                    await _emailSender.SendAsync(emailMessage);
                    //member tablosuna ekleme yapılmalıdır.
                    Member newMember = new Member()
                    {
                        TCNumber = model.TCNumber,
                        UserId = newUser.Id
                    };
                    if (_unitOfWork.MemberRepository.Add(newMember) == false)
                    {
                        //sistem yöneticisine email gitsin.
                        var emailMessageToAdmin = new EmailMessage()
                        {
                            Contacts = new string[] { _configuration.GetSection("ManagerEmails:Email").Value },
                            CC = new string[] { _configuration.GetSection("ManagerEmails:EmailToCC").Value },
                            Subject = "CPR - HATA! Member Tablosu",
                            Body = $"Aşağıdaki bilgilere sahip kişi eklenirken hata olmuş.Member tablosuna bilgileri ekleyiniz. <br/> Bilgiler: TCNumber:{model.TCNumber} <br/> UserId{newUser.Id}"
                        };
                        await _emailSender.SendAsync(emailMessageToAdmin);
                    }
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Beklenmedik bir hata oluştu!");
                    return View(model);
                }
            }
            catch (Exception ex)
            {

                return View();
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Veri girişleri düzgün olmalıdır!");
                    return View(model);
                }

                //user'ı bulup EmailConfirmed kontrol edilsin.
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    //if(user.EmailConfirmed==false)
                    if (!user.EmailConfirmed)
                    {
                        ModelState.AddModelError("", "Sistemi kullanabilmeniz için üyeliğinizi aktifleştirmeniz gerekmektedir. Emailinize gönderilen aktivasyon linkine tıklayarak aktifleştirme işlemini yapabilirsiniz!");
                        return View(model);
                    }
                }

                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, true);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalıdır!");
                    return View(model);
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu!");
                return View(model);
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    ViewBag.ResetPasswordMessage = "Girdiğiniz email bulunamadı!";
                }
                else
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callBackUrl = Url.Action("ConfirmResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Scheme);

                    var emailMessage = new EmailMessage()
                    {
                        Contacts = new string[] { user.Email },
                        Subject = "CPR - Şifremi unuttum ",
                        Body = $"Merhaba {user.Name} {user.Surname}," +
                        $" <br>Yeni parola belirlemek için" +
                        $"<a href='{HtmlEncoder.Default.Encode(callBackUrl)}'>buraya<a/> tıklayınız."
                    };
                    await _emailSender.SendAsync(emailMessage);
                    ViewBag.ResetPasswordMessage = "Emailinize şifre güncelleme yönergesi gönderilmiştir.";
                    //return 
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ResetPasswordMessage = "Beklenmedik bir hata oluştu!";
                return View();
            }
        }
        [HttpGet]
        public IActionResult ConfirmResetPassword(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                //return BadRequest("deneme");
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı!");
                return View();

            }
            ViewBag.UserId = userId;
            ViewBag.Code = code;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı");
                    return View(model);
                }
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
                var result = await _userManager.ResetPasswordAsync(user, code, model.NewPassword);
                if (result.Succeeded)
                {
                    TempData["ConfirmResetPasswordMessage"] = "Şifreniz başarılı bir şekilde değiştirildi.";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "HATA : Şifreniz değiştirilemedi!");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu!");
                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            try
            {
                if (userId == null || code == null)
                {
                    return NotFound("Sayfa Bulunamadı!");
                }
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("Kullanıcı Bulunamadı!");
                }
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
                var result = await _userManager.ConfirmEmailAsync(user, code);

                if (result.Succeeded)
                {
                    //User pasif rolde mi?
                    if (_userManager.IsInRoleAsync(user, RoleNames.Passive.ToString()).Result)
                    {
                        await _userManager.RemoveFromRoleAsync(user, RoleNames.Passive.ToString());
                        await _userManager.AddToRoleAsync(user, RoleNames.Member.ToString());
                    }

                    TempData["EmailConfirmedMessage"] = "Hesabınız aktifleştirilmiştir...";
                    return RedirectToAction("Login", "Account");
                }

                ViewBag.EmailConfirmedMessage = "Hesap aktifleştirme başarısızdır!";
                //Aynı sayfada kalsın istersek ViewBag,başka bir sayfaya yönlendirmesini istersek TempData kullanıyoruz.
                return View();
            }
            catch (Exception)
            {
                ViewBag.EmailConfirmedMessage = "Beklenmedik bir hata oluştu! Tekrar deneyiniz.";
                return View();
            }
        }
    }
}
