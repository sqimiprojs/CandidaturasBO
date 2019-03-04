using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Candidaturas_BO.Models;
using OfficeOpenXml;

namespace Candidaturas_BO.Controllers
{
    public class EstatisticasConcelhoController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Cursos
        public ActionResult Index(string searchString, string sortOrder, string edicao)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.PercSortParm = sortOrder == "Perc" ? "Perc_desc" : "Perc";


                List<Concelho> concelhos = db.Concelho.ToList();
                ViewBag.TotalCandidatos = db.Candidatura.Count();
                if (!String.IsNullOrEmpty(edicao))
                {
                    ViewBag.TotalCandidatos = db.Candidatura.Where(c => c.Edicao == edicao).Count();
                }

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    concelhos = concelhos.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }
                List<EstatisticaCursoDisplay> display = new List<EstatisticaCursoDisplay>();
                foreach (Concelho concelho in concelhos)
                {
                    EstatisticaCursoDisplay displayCurso = new EstatisticaCursoDisplay();
                    displayCurso.Nome = concelho.Nome;
                    if (!String.IsNullOrEmpty(edicao))
                    {
                        displayCurso.Total = db.Candidatura.Where(c => c.DadosPessoais.ConcelhoNatural == concelho.Codigo && c.Edicao == edicao).Count();
                        displayCurso.Percentagem = (db.Candidatura.Where(c => c.DadosPessoais.ConcelhoNatural == concelho.Codigo && c.Edicao == edicao).Count() / ViewBag.TotalCandidatos) * 100;
                    }
                    else
                    {
                        displayCurso.Total = db.Candidatura.Where(c => c.DadosPessoais.ConcelhoNatural == concelho.Codigo).Count();
                        displayCurso.Percentagem = (db.Candidatura.Where(c => c.DadosPessoais.ConcelhoNatural == concelho.Codigo).Count() / ViewBag.TotalCandidatos) * 100;
                    }
                    display.Add(displayCurso);
                }

                //sort
                switch (sortOrder)
                {
                    case "name_desc":
                        display = display.OrderByDescending(s => s.Nome).ToList();
                        break;
                    case "Perc":
                        display = display.OrderBy(s => s.Percentagem).ToList();
                        break;
                    case "perc_desc":
                        display = display.OrderByDescending(s => s.Percentagem).ToList();
                        break;
                    default:
                        display = display.OrderBy(s => s.Nome).ToList();
                        break;
                }
                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderBy(dp => dp.Sigla).Select(c => new SelectListItem
                {
                    Value = c.Sigla,
                    Text = c.Sigla
                });

                ViewBag.Edicao = edicaos.ToList();

                return View(display);
            }
            else
            {
                return View("Error");
            }

            
        }

      
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
