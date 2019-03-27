using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASPNET_MVC_ChartsDemo.Models;
using Candidaturas_BO.Models;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace Candidaturas_BO.Controllers
{
    public class EstatisticasConcelhoController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Cursos
        public ActionResult Index(string searchString, string sortOrder, string distrito, string edicao, bool finalizado = false)
        {
            if (ADAuthorization.ADAuthenticate())
            {

                IEnumerable<SelectListItem> distritos = db.Distrito.OrderBy(dp => dp.Nome).Select(c => new SelectListItem
                {
                    Value = c.Codigo.ToString(),
                    Text = c.Nome
                });
                ViewBag.Distrito = distritos.ToList();

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

                List<Concelho> concelhos = db.Concelho.ToList();
                if (!String.IsNullOrEmpty(distrito))
                {
                    concelhos = concelhos.Where(s => s.CodigoDistrito == Convert.ToInt32(distrito)).ToList();
                }
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
                    concelhos = concelhos.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }
                List<EstatisticaCursoDisplay> display = new List<EstatisticaCursoDisplay>();
                foreach (Concelho concelho in concelhos)
                {
                    EstatisticaCursoDisplay displayCurso = new EstatisticaCursoDisplay();
                    displayCurso.Nome = concelho.Nome;
                    if (!String.IsNullOrEmpty(edicao) && finalizado)
                    {
                        displayCurso.Total = db.Candidatura.Where(c => c.DadosPessoais.ConcelhoMorada == concelho.Codigo && c.DadosPessoais.DistritoMorada == concelho.CodigoDistrito && c.Edicao == edicao && c.Certificado != null).Count();
                        displayCurso.Percentagem = Math.Round(((double)(db.Candidatura.Where(c => c.DadosPessoais.ConcelhoMorada == concelho.Codigo && c.DadosPessoais.DistritoMorada == concelho.CodigoDistrito && c.Edicao == edicao && c.Certificado != null).Count() / (double)ViewBag.TotalCandidatos) * 100), 2);
                    }
                    else if (finalizado)
                    {
                        displayCurso.Total = db.Candidatura.Where(c => c.DadosPessoais.ConcelhoMorada == concelho.Codigo && c.DadosPessoais.DistritoMorada == concelho.CodigoDistrito && c.Certificado != null).Count();
                        displayCurso.Percentagem = Math.Round(((double)(db.Candidatura.Where(c => c.DadosPessoais.ConcelhoMorada == concelho.Codigo && c.DadosPessoais.DistritoMorada == concelho.CodigoDistrito && c.Certificado != null).Count() / (double)ViewBag.TotalCandidatos) * 100), 2);
                    }
                    else if (!String.IsNullOrEmpty(edicao))
                    {
                        displayCurso.Total = db.Candidatura.Where(c => c.DadosPessoais.ConcelhoMorada == concelho.Codigo && c.DadosPessoais.DistritoMorada == concelho.CodigoDistrito && c.Edicao == edicao).Count();
                        displayCurso.Percentagem = Math.Round(((double)(db.Candidatura.Where(c => c.DadosPessoais.ConcelhoMorada == concelho.Codigo && c.DadosPessoais.DistritoMorada == concelho.CodigoDistrito && c.Edicao == edicao).Count() / (double)ViewBag.TotalCandidatos) * 100),2);
                    }
                    else
                    {
                        displayCurso.Total = db.Candidatura.Where(c => c.DadosPessoais.ConcelhoMorada == concelho.Codigo && c.DadosPessoais.DistritoMorada == concelho.CodigoDistrito).Count();
                        displayCurso.Percentagem = Math.Round(((double)(db.Candidatura.Where(c => c.DadosPessoais.ConcelhoMorada == concelho.Codigo && c.DadosPessoais.DistritoMorada == concelho.CodigoDistrito).Count() / (double)ViewBag.TotalCandidatos) * 100),2);
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
                List<DataPoint> dataPointsPer = new List<DataPoint>();
                List<DataPoint> dataPointsTot = new List<DataPoint>();
                foreach (EstatisticaCursoDisplay chart in display)
                {
                    dataPointsPer.Add(new DataPoint(chart.Nome, chart.Percentagem));
                    dataPointsTot.Add(new DataPoint(chart.Nome, chart.Total));
                }


                ViewBag.DataPointsPer = JsonConvert.SerializeObject(dataPointsPer);
                ViewBag.DataPointsTot = JsonConvert.SerializeObject(dataPointsTot);

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
