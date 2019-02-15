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
    public class PostosController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Postos
        public ActionResult Index(string searchString, string sortOrder)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.OrdemSortParm = sortOrder == "Ordem" ? "Ordem_desc" : "Ordem";

                var postos = db.Posto.ToList();

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    postos = postos.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }

                //sort
                switch (sortOrder)
                {
                    case "name_desc":
                        postos = postos.OrderByDescending(s => s.Nome).ToList();
                        break;
                    case "Ordem":
                        postos = postos.OrderBy(s => s.Ordem).ToList();
                        break;
                    case "Ordem_desc":
                        postos = postos.OrderByDescending(s => s.Ordem).ToList();
                        break;
                    default:
                        postos = postos.OrderBy(s => s.Nome).ToList();
                        break;
                }

                ViewBag.TotalPostos = postos.Count();

                return View(postos);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Postos/Create
        public ActionResult Create()
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.CategoriaMilitar = new SelectList(db.Categoria, "Sigla", "Nome");
                ViewBag.RamoMilitar = new SelectList(db.Ramo, "Sigla", "Nome");
                return View();
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Postos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Código,Nome,Abreviatura,RamoMilitar,CategoriaMilitar,Ordem")] Posto posto)
        {
            if (ModelState.IsValid)
            {
                db.Posto.Add(posto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoriaMilitar = new SelectList(db.Categoria, "Sigla", "Nome", posto.CategoriaMilitar);
            ViewBag.RamoMilitar = new SelectList(db.Ramo, "Sigla", "Nome", posto.RamoMilitar);
            return View(posto);
        }

        // GET: Postos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Posto posto = db.Posto.Find(id);
                if (posto == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CategoriaMilitar = new SelectList(db.Categoria, "Sigla", "Nome", posto.CategoriaMilitar);
                ViewBag.RamoMilitar = new SelectList(db.Ramo, "Sigla", "Nome", posto.RamoMilitar);
                return View(posto);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Postos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Código,Nome,Abreviatura,RamoMilitar,CategoriaMilitar,Ordem")] Posto posto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(posto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoriaMilitar = new SelectList(db.Categoria, "Sigla", "Nome", posto.CategoriaMilitar);
            ViewBag.RamoMilitar = new SelectList(db.Ramo, "Sigla", "Nome", posto.RamoMilitar);
            return View(posto);
        }

        // GET: Postos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Posto posto = db.Posto.Find(id);
                if (posto == null)
                {
                    return HttpNotFound();
                }
                return View(posto);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Postos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Posto posto = db.Posto.Find(id);
            db.Posto.Remove(posto);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: Postos/MassInsert
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
                            var codigo = Convert.ToInt32(workSheet.Cells[rowIterator, 1].Value.ToString());
                            var nome = workSheet.Cells[rowIterator, 2].Value.ToString();
                            var abreviatura = workSheet.Cells[rowIterator, 3].Value.ToString();
                            var ramoMilitar = workSheet.Cells[rowIterator, 4].Value.ToString();
                            var categoriaMilitar = workSheet.Cells[rowIterator, 5].Value.ToString();
                            var ordem = Convert.ToInt32(workSheet.Cells[rowIterator, 6].Value.ToString());


                            if (!db.Posto.Any(p => p.Nome == nome || p.Código == codigo || p.Abreviatura == abreviatura || p.Ordem == ordem))
                            {
                                Posto posto = new Posto
                                {
                                    Código = codigo,
                                    Nome = nome,
                                    Abreviatura = abreviatura,
                                    RamoMilitar = ramoMilitar,
                                    CategoriaMilitar = categoriaMilitar,
                                    Ordem = ordem

                                };

                                db.Posto.Add(posto);
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
