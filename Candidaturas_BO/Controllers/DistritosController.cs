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
    public class DistritosController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Distritos
        public ActionResult Index(string searchString, string sortOrder)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.CodeSortParm = sortOrder == "Code" ? "code_desc" : "Code";
                ViewBag.UserAdmin = ADAuthorization.ADAuthenticateAdmin();

                var distritos = db.Distrito.ToList();

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    distritos = distritos.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }

                //sort
                switch (sortOrder)
                {
                    case "name_desc":
                        distritos = distritos.OrderByDescending(s => s.Nome).ToList();
                        break;
                    case "Code":
                        distritos = distritos.OrderBy(s => s.Codigo).ToList();
                        break;
                    case "code_desc":
                        distritos = distritos.OrderByDescending(s => s.Codigo).ToList();
                        break;
                    default:
                        distritos = distritos.OrderBy(s => s.Nome).ToList();
                        break;
                }

                ViewBag.TotalDistritos = distritos.Count();

                return View(distritos);
            }
            else
            {
                return View("Error");
            }
        }
        
        // GET: Distritos/Create
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

        // POST: Distritos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Nome,Codigo")] Distrito distrito)
        {
            if (ModelState.IsValid)
            {
                db.Distrito.Add(distrito);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(distrito);
        }

        // GET: Distritos/Edit/5
        public ActionResult Edit(int? codigo)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (codigo == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Distrito distrito = db.Distrito.Find(codigo);
                if (distrito == null)
                {
                    return HttpNotFound();
                }
                return View(distrito);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Distritos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Nome,Codigo")] Distrito distrito)
        {
            if (ModelState.IsValid)
            {
                db.Entry(distrito).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(distrito);
        }

        // GET: Distritos/Delete/5
        public ActionResult Delete(int? codigo)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (codigo == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Distrito distrito = db.Distrito.Find(codigo);
                if (distrito == null)
                {
                    return HttpNotFound();
                }
                return View(distrito);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Distritos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int codigo)
        {
            Distrito distrito = db.Distrito.Find(codigo);
            db.Distrito.Remove(distrito);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: Distritos/MassInsert
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
                            var codigo = Convert.ToInt32(workSheet.Cells[rowIterator, 2].Value.ToString());

                            if (!db.Distrito.Any(d => d.Nome == nome || d.Codigo == codigo))
                            {
                                Distrito distrito = new Distrito
                                {
                                    Nome = nome,
                                    Codigo = codigo
                                };

                                db.Distrito.Add(distrito);
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
