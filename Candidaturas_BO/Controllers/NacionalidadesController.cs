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
    public class NacionalidadesController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Nacionalidades
        public ActionResult Index(string searchString, string sortOrder)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.SiglaSortParm = sortOrder == "Sigla" ? "sigla_desc" : "Sigla";
                ViewBag.UserAdmin = ADAuthorization.ADAuthenticateAdmin();

                List<Pais> pais = db.Pais.ToList();

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    pais = pais.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }

                //sort
                switch (sortOrder)
                {
                    case "name_desc":
                        pais = pais.OrderByDescending(s => s.Nome).ToList();
                        break;
                    case "Sigla":
                        pais = pais.OrderBy(s => s.Sigla).ToList();
                        break;
                    case "sigla_desc":
                        pais = pais.OrderByDescending(s => s.Sigla).ToList();
                        break;
                    default:
                        pais = pais.OrderBy(s => s.Nome).ToList();
                        break;
                }

                ViewBag.TotalNacionalidades = pais.Count();

                return View(pais);
            }
            else
            {
                return View("Error");
            }
        }
        
        // GET: Nacionalidades/Create
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

        // POST: Nacionalidades/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Nome,Sigla")] Pais pais)
        {
            if (ModelState.IsValid)
            {
                db.Pais.Add(pais);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pais);
        }

        // GET: Nacionalidades/Edit/5
        public ActionResult Edit(String sigla)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (sigla == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Pais pais = db.Pais.Find(sigla);

                if (pais == null)
                {
                    return HttpNotFound();
                }

                return View(pais);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Nacionalidades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Nome,Sigla")] Pais pais)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pais).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pais);
        }

        // GET: Nacionalidades/Delete/5
        public ActionResult Delete(String sigla)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (sigla == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Pais pais = db.Pais.Find(sigla);

                if (pais == null)
                {
                    return HttpNotFound();
                }

                return View(pais);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Nacionalidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(String sigla)
        {
            Pais pais = db.Pais.Find(sigla);
            db.Pais.Remove(pais);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: Nacionalidades/MassInsert
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
                            var sigla = workSheet.Cells[rowIterator, 2].Value.ToString();

                            if (!db.Pais.Any(n => n.Nome == nome || n.Sigla == sigla))
                            {
                                Pais pais = new Pais
                                {
                                    Nome = nome,
                                    Sigla = sigla
                                };

                                db.Pais.Add(pais);
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
