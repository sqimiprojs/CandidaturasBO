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
    public class RamosController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Ramos
        public ActionResult Index(string searchString, string sortOrder)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.UserAdmin = ADAuthorization.ADAuthenticateAdmin();

                var ramos = db.Ramo.ToList();

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    ramos = ramos.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }

                //sort
                switch (sortOrder)
                {
                    case "name_desc":
                        ramos = ramos.OrderByDescending(s => s.Nome).ToList();
                        break;
                    default:
                        ramos = ramos.OrderBy(s => s.Nome).ToList();
                        break;
                }

                ViewBag.TotalRamos = ramos.Count();

                return View(ramos);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Ramos/Create
        public ActionResult Create()
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                return View();

            }
            else
            {
                return View("Error");
            }
        }

        // POST: Ramos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Sigla,Nome")] Ramo ramo)
        {
            if (ModelState.IsValid)
            {
                db.Ramo.Add(ramo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ramo);
        }

        // GET: Ramos/Edit/5
        public ActionResult Edit(string id)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Ramo ramo = db.Ramo.Find(id);
                if (ramo == null)
                {
                    return HttpNotFound();
                }
                return View(ramo);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Ramos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Sigla,Nome")] Ramo ramo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ramo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ramo);
        }

        // GET: Ramos/Delete/5
        public ActionResult Delete(string id)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Ramo ramo = db.Ramo.Find(id);
                if (ramo == null)
                {
                    return HttpNotFound();
                }
                return View(ramo);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Ramos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Ramo ramo = db.Ramo.Find(id);
            db.Ramo.Remove(ramo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: Ramos/MassInsert
        public ActionResult MassInsert()
        {
            if (ADAuthorization.ADAuthenticateAdmin())
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
                if (ExcelValidator.HasExcelExtension(uploadFile) && ExcelValidator.HasExcelMimeType(uploadFile))
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
                            var sigla = workSheet.Cells[rowIterator, 1].Value.ToString();
                            var nome = workSheet.Cells[rowIterator, 2].Value.ToString();

                            if (!db.Ramo.Any(r => r.Sigla == sigla || r.Nome == nome))
                            {
                                Ramo ramo = new Ramo
                                {
                                    Sigla = sigla,
                                    Nome = nome
                                };

                                db.Ramo.Add(ramo);
                                db.SaveChanges();
                            }
                        }
                    }
                    catch (Exception)
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
