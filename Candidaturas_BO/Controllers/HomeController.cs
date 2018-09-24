using System.Web.Mvc;

namespace Candidaturas_BO.Controllers
{
    public class HomeController : Controller
    {

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