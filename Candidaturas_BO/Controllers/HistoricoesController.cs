using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Candidaturas_BO.Models;

namespace Candidaturas_BO.Controllers
{
    public class HistoricoesController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Historicoes
        public ActionResult Index(string edicao, string startDate, string endDate, string searchString, string sortOrder, string currentFilter)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "";
                
                List<Historico> historico = db.Historico.ToList();
                
                if (!String.IsNullOrEmpty(edicao))
                {
                    historico = historico.Where(s => s.Candidatura.Edicao == edicao).ToList();
                }
                else
                {
                    edicao = currentFilter;
                }

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    historico = historico.Where(s => s.Candidatura.User.Email.Contains(searchString) || s.Candidatura.User.Email.ToLower().Contains(searchString)).ToList();
                }

                if (startDate != null || endDate != null)
                {
                    if (startDate != "" && endDate != "")
                    {
                        DateTime? start = Convert.ToDateTime(startDate);
                        DateTime? end = Convert.ToDateTime(endDate);

                        historico = historico.Where(u => u.timestamp >= start && u.timestamp <= end).ToList();
                    }
                    else if (startDate != "")
                    {
                        DateTime? start = Convert.ToDateTime(startDate);

                        historico = historico.Where(u => u.timestamp >= start).ToList();
                    }
                    else if (endDate != "")
                    {
                        DateTime? end = Convert.ToDateTime(endDate);

                        historico = historico.Where(u => u.timestamp <= end).ToList();
                    }
                }

                //sort
                switch (sortOrder)
                {
                    case "email_desc":
                        historico = historico.OrderByDescending(s => s.Candidatura.User.Email).ToList();
                        break;
                    default:
                        historico = historico.OrderBy(s => s.Candidatura.User.Email).ToList();
                        break;
                }

                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderBy(dp => dp.Sigla).Select(c => new SelectListItem
                {
                    Value = c.Sigla,
                    Text = c.Sigla
                });

                ViewBag.Edicao = edicaos.ToList();

                ViewBag.TotalHistoricos = historico.Count();


                return View(historico);
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
