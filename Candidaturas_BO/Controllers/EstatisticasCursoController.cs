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
    public class EstatisticasCursoController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Cursos
        public ActionResult Index(string searchString, string sortOrder, string edicao)
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

                List<Curso> cursos = db.Curso.ToList();
                if (!String.IsNullOrEmpty(edicao))
                {
                    cursos = cursos.Where(s => s.Edicao == edicao).ToList();
                    ViewBag.TotalCandidatos = db.Candidatura.Where(c => c.Edicao == edicao && c.DadosPessoais != null && c.Certificado != null).Count();
                }         
                else
                {
                    ViewBag.TotalCandidatos = db.Candidatura.Where(c => c.DadosPessoais != null && c.Certificado != null).Count();
                }


                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    cursos = cursos.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }
                List<EstatisticaCursoDisplay> display = new List<EstatisticaCursoDisplay>();
                foreach (Curso curso in cursos)
                {
                        EstatisticaCursoDisplay displayCurso = new EstatisticaCursoDisplay();
                        displayCurso.Edicao = curso.Edicao;
                        displayCurso.Nome = curso.Nome;
                        displayCurso.Total = (db.Opcoes.Where(o => o.CursoId == curso.ID && o.Candidatura.Certificado != null).Count());
                        displayCurso.Percentagem = Math.Round(((double)(db.Opcoes.Where(o => o.CursoId == curso.ID && o.Candidatura.Certificado != null).Count() / (double)ViewBag.TotalCandidatos) * 100), 2);
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
