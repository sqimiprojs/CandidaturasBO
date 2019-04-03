using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Candidaturas_BO.Models;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace Candidaturas_BO.Controllers
{
    public class EstatisticasDistritoController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Cursos
        public ActionResult Index(string searchString, string sortOrder, string edicao, bool finalizado = false)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.PercSortParm = sortOrder == "Perc" ? "Perc_desc" : "Perc";
                if (String.IsNullOrEmpty(edicao))
                {
                    edicao = db.Edicao.Where(e => e.DataInicio < System.DateTime.Now && e.DataFim > System.DateTime.Now).Select(e => e.Sigla).FirstOrDefault();
                    if (String.IsNullOrEmpty(edicao))
                    {
                        edicao = db.Edicao.OrderByDescending(e => e.DataFim).Select(e => e.Sigla).FirstOrDefault();
                    }
                }

                List<Distrito> distritos = db.Distrito.ToList();
                if (!String.IsNullOrEmpty(edicao) && finalizado)
                {
                    ViewBag.TotalCandidatos = db.Candidatura.Where(c => c.Edicao == edicao && c.DadosPessoais != null && c.Certificado != null).Count();
                }
                else if (!String.IsNullOrEmpty(edicao))
                {
                    ViewBag.TotalCandidatos = db.Candidatura.Where(c => c.Edicao == edicao && c.DadosPessoais != null).Count();
                }
                else if (finalizado)
                {
                    ViewBag.TotalCandidatos = db.Candidatura.Where(c => c.DadosPessoais != null && c.Certificado != null).Count();
                }
                else
                {
                    ViewBag.TotalCandidatos = db.Candidatura.Where(c => c.DadosPessoais != null).Count();
                }


                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    distritos = distritos.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }
                List<EstatisticaCursoDisplay> display = new List<EstatisticaCursoDisplay>();
                foreach (Distrito distrito in distritos)
                {
                    EstatisticaCursoDisplay displayCurso = new EstatisticaCursoDisplay();
                    displayCurso.Nome = distrito.Nome;
                    if (!String.IsNullOrEmpty(edicao) && finalizado)
                    {
                        displayCurso.Total = db.Candidatura.Where(c => c.DadosPessoais.DistritoMorada == distrito.Codigo && c.Edicao == edicao && c.Certificado != null).Count();
                        displayCurso.Percentagem = Math.Round(((double)(db.Candidatura.Where(c => c.DadosPessoais.DistritoMorada == distrito.Codigo && c.Edicao == edicao && c.Certificado != null).Count() / (double)ViewBag.TotalCandidatos) * 100), 2);
                    }
                    else if (finalizado)
                    {
                        displayCurso.Total = db.Candidatura.Where(c => c.DadosPessoais.DistritoMorada == distrito.Codigo && c.Certificado != null).Count();
                        displayCurso.Percentagem = Math.Round(((double)(db.Candidatura.Where(c => c.DadosPessoais.DistritoMorada == distrito.Codigo && c.Certificado != null).Count() / (double)ViewBag.TotalCandidatos) * 100), 2);
                    }
                    else if (!String.IsNullOrEmpty(edicao))
                    {
                        displayCurso.Total = db.Candidatura.Where(c => c.DadosPessoais.DistritoMorada == distrito.Codigo && c.Edicao == edicao).Count();
                        displayCurso.Percentagem = Math.Round(((double)(db.Candidatura.Where(c => c.DadosPessoais.DistritoMorada == distrito.Codigo && c.Edicao == edicao).Count() / (double)ViewBag.TotalCandidatos) * 100),2);
                    }
                    else
                    {
                        displayCurso.Total = db.Candidatura.Where(c => c.DadosPessoais.DistritoMorada == distrito.Codigo).Count();
                        displayCurso.Percentagem = Math.Round(((double)(db.Candidatura.Where(c => c.DadosPessoais.DistritoMorada == distrito.Codigo).Count() / (double)ViewBag.TotalCandidatos) * 100),2);
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
                    case "Perc_desc":
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
