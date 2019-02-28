using Candidaturas_BO.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Candidaturas_BO.Controllers
{
    public class HomeController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        public ActionResult Index()
        {
            
            if (Session["userName"] != null)
            {

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
    }
}