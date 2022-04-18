using DataFirsttest.Data;
using DataFirsttest.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DataFirsttest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        CodeFirstContext obj = new CodeFirstContext();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginClass objUser)
        {
            var res =obj.LoginSts.Where(a => a.Email == objUser.Email).FirstOrDefault();

            if (res == null)
            {

                TempData["Invalid"] = "Email is not found";
            }

            else
            {
                if (res.Email == objUser.Email && res.Password == objUser.Password)
                {

                    var claims = new[] { new Claim(ClaimTypes.Name, res.Name),
                                        new Claim(ClaimTypes.Email, res.Email) };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };
                    HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    authProperties);


                    HttpContext.Session.SetString("Name", objUser.Email);

                    return RedirectToAction("Index", "Home");

                }

                else
                {

                    ViewBag.Inv = "Wrong Email Id or password";

                    return View("Login");
                }


            }


            return View("Index");
        }
        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registration(LoginClass abc)
        {
            LoginSt cbj = new LoginSt();
            cbj.Name = abc.Name;
            cbj.Email = abc.Email;
            cbj.Password = abc.Password;


            obj.LoginSts.Add(cbj);
            obj.SaveChanges();
            return RedirectToAction("Login");
        }


        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return View("Login");
        }


        [Authorize]
        public IActionResult Table()
        {
            List<StudentClass> modobj = new List<StudentClass>();
            var res = obj.StuDetails.ToList();

            foreach (var item in res)
            {
                modobj.Add(new StudentClass
                {
                    Id = item.Id,
                    Name = item.Name,
                    Email = item.Email,
                    Phone = item.Phone
                });
            }
            return View(modobj);
        }
        [Authorize]
        [HttpGet]
        public IActionResult Show()
        {

            return View();
        }
        [Authorize]
        [HttpPost]
        public IActionResult Show(StudentClass cbj)
        {
            StuDetail abj = new StuDetail();
            abj.Id = cbj.Id;
            abj.Name = cbj.Name;
            abj.Email = cbj.Email;
            abj.Phone = cbj.Phone;
            if (cbj.Id == 0)
            {
                obj.StuDetails.Add(abj);
                obj.SaveChanges();
            }
            else
            {
                obj.Entry(abj).State = EntityState.Modified;
                obj.SaveChanges();
            }
            return RedirectToAction("Table");
        }
        public IActionResult delete(int id)
        {
            var deleteitem = obj.StuDetails.Where(m => m.Id == id).First();
            obj.StuDetails.Remove(deleteitem);
           obj.SaveChanges();
            return RedirectToAction("Table");

        }
        public IActionResult edit(int id)
        {
            StudentClass abj = new StudentClass();
            var edititem = obj.StuDetails.Where(m => m.Id == id).First();

            abj.Id = edititem.Id;
            abj.Name = edititem.Name;
            abj.Email = edititem.Email;
            abj.Phone = edititem.Phone;

            return View("Show", abj);
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
