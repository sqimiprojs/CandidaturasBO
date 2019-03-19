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
    public class SituacaosController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Situacaos
        public ActionResult Index(string edicao, string searchString, string sortOrder)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

                List<Situacao> situacoes = db.Situacao.ToList();

                if (!String.IsNullOrEmpty(edicao))
                {
                    situacoes = situacoes.Where(s => s.Edicao == edicao).ToList();
                }
                else
                {
                    Edicao edAux = db.Edicao.Where(e => e.DataInicio < System.DateTime.Now && e.DataFim > System.DateTime.Now).FirstOrDefault();
                    if (edAux != null)
                    {
                        situacoes = situacoes.Where(s => s.Edicao == edAux.Sigla).ToList();
                    }
                    else
                    {
                        Edicao ultimaEdicao = db.Edicao.OrderByDescending(e => e.DataFim).FirstOrDefault();
                        situacoes = situacoes.Where(s => s.Edicao == ultimaEdicao.Sigla).ToList();
                    }

                }


                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    situacoes = situacoes.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }

                //sort
                switch (sortOrder)
                {
                    case "name_desc":
                        situacoes = situacoes.OrderByDescending(s => s.Nome).ToList();
                        break;
                    default:
                        situacoes = situacoes.OrderBy(s => s.Nome).ToList();
                        break;
                }

                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderBy(dp => dp.Sigla).Select(c => new SelectListItem
                {
                    Value = c.Sigla,
                    Text = c.Sigla
                });

                ViewBag.Edicao = edicaos.ToList();

                ViewBag.TotalSituacaos = situacoes.Count();

                return View(situacoes);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Situacaos/Create
        public ActionResult Create()
        {
            if (ADAuthorization.ADAuthenticate())
            {
                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderBy(dp => dp.Sigla).Select(c => new SelectListItem
                {
                    Value = c.Sigla,
                    Text = c.Sigla
                });

                ViewBag.Edicao = edicaos.ToList();

                return View();
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Situacaos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nome,Edicao")] Situacao situacao)
        {
            if (ModelState.IsValid)
            {
                db.Situacao.Add(situacao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(situacao);
        }

        // GET: Situacaos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Situacao situacao = db.Situacao.Find(id);
                if (situacao == null)
                {
                    return HttpNotFound();
                }
                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderBy(dp => dp.Sigla).Select(c => new SelectListItem
                {
                    Value = c.Sigla,
                    Text = c.Sigla
                });

                ViewBag.Edicao = edicaos.ToList();

                return View(situacao);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Situacaos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nome,Edicao")] Situacao situacao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(situacao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(situacao);
        }

        // GET: Situacaos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Situacao situacao = db.Situacao.Find(id);
                if (situacao == null)
                {
                    return HttpNotFound();
                }
                return View(situacao);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Situacaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Situacao situacao = db.Situacao.Find(id);
            db.Situacao.Remove(situacao);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: Situacaos/MassInsert
        public ActionResult MassInsert()
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

        [HttpPost]
        public ActionResult MassInsert(HttpPostedFileBase uploadFile)
        {
            if (uploadFile != null && uploadFile.ContentLength > 0)
            {
                if(ExcelValidator.HasExcelExtension(uploadFile) && ExcelValidator.HasExcelMimeType(uploadFile))
                {
                    string fileName = uploadFile.FileName;
                    string fileContentType = uploadFile.ContentType;
                    byte[] fileBytes = new byte[uploadFile.ContentLength];
                    var data = uploadFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(uploadFile.ContentLength));

                    try
                    {
                        var package = new ExcelPackage(uploadFile.InputStream);
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;

                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var nome = workSheet.Cells[rowIterator, 1].Value.ToString();
                            var edicao = workSheet.Cells[rowIterator, 2].Value.ToString();

                            if (!db.Situacao.Any(s => s.Nome == nome && s.Edicao == edicao))
                            {
                                Situacao situacao = new Situacao();

                                situacao.Nome = nome;
                                situacao.Edicao = edicao;
                                

                                db.Situacao.Add(situacao);
                                db.SaveChanges();
                            }
                        }
                    }
                    catch(Exception)
                    {
                        ViewBag.ErrorMessage = "Tipo de ficheiro inválido. Por favor seleccione um ficheiro Excel válido.";
                        return View();
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Tipo de ficheiro inválido. Por favor seleccione um ficheiro Excel válido.";
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Não foi seleccionado nenhum ficheiro. Por favor seleccione um ficheiro Excel válido.";
                return View();
            }
            return RedirectToAction("Index");
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
