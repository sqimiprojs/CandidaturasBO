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
    public class EstatisticasDiaController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Cursos
        public ActionResult Index(string date, string sortOrder)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.PercSortParm = sortOrder == "Perc" ? "Perc_desc" : "Perc";

                List<Certificado> certificados = db.Certificado.ToList();
                    ViewBag.TotalCandidatos = db.Candidatura.Where(c => c.DadosPessoais != null && c.Certificado != null).Count();



                List<DateTime> datas = new List<DateTime>();
                foreach(Certificado certificado in certificados)
                {
                    if(!datas.Contains(certificado.DataCriação.Date))
                    {
                        datas.Add(certificado.DataCriação.Date);
                    }
                }
           

                //search
                if (!String.IsNullOrEmpty(date))
                {
                    datas = datas.Where(s => s.ToString().Contains(date) || s.ToString().ToLower().Contains(date)).ToList();
                }
                List<EstatisticaCursoDisplay> display = new List<EstatisticaCursoDisplay>();
                foreach (DateTime data in datas)
                {
                    EstatisticaCursoDisplay displayCurso = new EstatisticaCursoDisplay();
                    displayCurso.Nome = data.ToString().Split(' ')[0];
                    displayCurso.Total = db.Certificado.Where(c => c.DiaCriação == data).Count();
                    displayCurso.Percentagem = Math.Round(((double)(db.Certificado.Where(c => c.DiaCriação == data).Count() / (double)ViewBag.TotalCandidatos) * 100),2);
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
                List<DataPoint> dataPoints = new List<DataPoint>();
                foreach (EstatisticaCursoDisplay chart in display)
                {
                    dataPoints.Add(new DataPoint(chart.Nome, chart.Percentagem));
                }
                ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);


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
