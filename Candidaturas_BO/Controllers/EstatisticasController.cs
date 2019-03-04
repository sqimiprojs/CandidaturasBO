using Candidaturas_BO.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Candidaturas_BO.Controllers
{
    public class EstatisticasController : Controller
    {
        public ActionResult Index()
        {
            if (ADAuthorization.ADAuthenticate())
            {
                return View();
            }
            else
            {
                return View("Error");
            }
        }
    }
}