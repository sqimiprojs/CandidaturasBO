using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Candidaturas_BO.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
                TempData.Remove("ErrorMessage");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Authorize(string userName, string password)
        {
            if (ADAuthorization.ADAuthenticate(userName, password))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "Username ou password inválido!";
                return RedirectToAction("Login", "Login");
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Login");
        }
    }
}