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
    public class TiposDocumentoIDController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: TiposDocumentoID
        public ActionResult Index(string searchString, string sortOrder)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

                List<TipoDocumentoID> tiposDocID = db.TipoDocumentoID.ToList();

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    tiposDocID = tiposDocID.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }

                //sort
                switch (sortOrder)
                {
                    case "name_desc":
                        tiposDocID = tiposDocID.OrderByDescending(s => s.Nome).ToList();
                        break;
                    default:
                        tiposDocID = tiposDocID.OrderBy(s => s.Nome).ToList();
                        break;
                }

                ViewBag.TotalTiposDocID = tiposDocID.Count();

                return View(tiposDocID);
            }
            else
            {
                return View("Error");
            }
        }
        
        // GET: TiposDocumentoID/Create
        public ActionResult Create()
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

        // POST: TiposDocumentoID/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nome")] TipoDocumentoID tipoDocumentoID)
        {
            if (ModelState.IsValid)
            {
                db.TipoDocumentoID.Add(tipoDocumentoID);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoDocumentoID);
        }

        // GET: TiposDocumentoID/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                TipoDocumentoID tipoDocumentoID = db.TipoDocumentoID.Find(id);

                if (tipoDocumentoID == null)
                {
                    return HttpNotFound();
                }

                return View(tipoDocumentoID);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: TiposDocumentoID/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nome")] TipoDocumentoID tipoDocumentoID)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoDocumentoID).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoDocumentoID);
        }

        // GET: TiposDocumentoID/Delete/5
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                TipoDocumentoID tipoDocumentoID = db.TipoDocumentoID.Find(id);

                if (tipoDocumentoID == null)
                {
                    return HttpNotFound();
                }

                return View(tipoDocumentoID);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: TiposDocumentoID/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoDocumentoID tipoDocumentoID = db.TipoDocumentoID.Find(id);
            db.TipoDocumentoID.Remove(tipoDocumentoID);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: TiposDocumentoID/MassInsert
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

                            if(!db.TipoDocumentoID.Any(td => td.Nome == nome))
                            {
                                TipoDocumentoID tipoDocID = new TipoDocumentoID();
                                tipoDocID.Nome = nome;
                                db.TipoDocumentoID.Add(tipoDocID);
                                db.SaveChanges();
                            }
                        }
                    }
                    catch(Exception e)
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
