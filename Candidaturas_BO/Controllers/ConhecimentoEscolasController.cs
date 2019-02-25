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
    public class ConhecimentoEscolasController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: ConhecimentoEscolas
        public ActionResult Index(string searchString, string sortOrder, string filtroEdicao)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

                List<ConhecimentoEscola> conhecimentosEscola = db.ConhecimentoEscola.ToList();

                if (!String.IsNullOrEmpty(filtroEdicao))
                {
                    conhecimentosEscola = conhecimentosEscola.Where(s => s.Edicao == filtroEdicao).ToList();
                }

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    conhecimentosEscola = conhecimentosEscola.Where(s => s.Nome.ToLower().Contains(searchString) || s.Nome.Contains(searchString)).ToList();
                }

                //sort
                switch (sortOrder)
                {
                    case "name_desc":
                        conhecimentosEscola = conhecimentosEscola.OrderByDescending(s => s.Nome).ToList();
                        break;
                    default:
                        conhecimentosEscola = conhecimentosEscola.OrderBy(s => s.Nome).ToList();
                        break;
                }

                ViewBag.TotalConhecimentosEscola = conhecimentosEscola.Count();

                return View(conhecimentosEscola);
            }
            else
            {
                return View("Error");
            }
            
        }
        
        // GET: ConhecimentoEscolas/Create
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

        // POST: ConhecimentoEscolas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nome")] ConhecimentoEscola conhecimentoEscola)
        {
            if (ModelState.IsValid)
            {
                db.ConhecimentoEscola.Add(conhecimentoEscola);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(conhecimentoEscola);
        }

        // GET: ConhecimentoEscolas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ConhecimentoEscola conhecimentoEscola = db.ConhecimentoEscola.Find(id);
                if (conhecimentoEscola == null)
                {
                    return HttpNotFound();
                }
                return View(conhecimentoEscola);
            }
            else
            {
                return View("Error");
            }            
        }

        // POST: ConhecimentoEscolas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nome")] ConhecimentoEscola conhecimentoEscola)
        {
            if (ModelState.IsValid)
            {
                db.Entry(conhecimentoEscola).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(conhecimentoEscola);
        }

        // GET: ConhecimentoEscolas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ConhecimentoEscola conhecimentoEscola = db.ConhecimentoEscola.Find(id);
                if (conhecimentoEscola == null)
                {
                    return HttpNotFound();
                }
                return View(conhecimentoEscola);
            }
            else
            {
                return View("Error");
            }          
        }

        // POST: ConhecimentoEscolas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ConhecimentoEscola conhecimentoEscola = db.ConhecimentoEscola.Find(id);
            db.ConhecimentoEscola.Remove(conhecimentoEscola);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: ConhecimentoEscolas/MassInsert
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

                            if(!db.ConhecimentoEscola.Any(ce => ce.Nome == nome))
                            {
                                ConhecimentoEscola conhecimentoEscola = new ConhecimentoEscola();
                                conhecimentoEscola.Nome = nome;
                                db.ConhecimentoEscola.Add(conhecimentoEscola);
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
