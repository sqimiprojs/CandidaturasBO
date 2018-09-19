using System.Web.Mvc;

namespace Candidaturas_BO.Controllers
{
    public class HomeController : Controller
    {

        /*public ActionResult Index()
        {
            return View();
        }*/
        
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