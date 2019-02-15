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
    public class ReparticoesController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Reparticoes
        public ActionResult Index(string searchString, string sortOrder)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.CodeSortParm = sortOrder == "Code" ? "code_desc" : "Code";

                var reparticoes = db.Reparticoes.Include(r => r.Distrito).Include(r => r.Reparticoes1).Include(r => r.Reparticoes2).ToList();

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    reparticoes = reparticoes.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }

                //sort
                switch (sortOrder)
                {
                    case "name_desc":
                        reparticoes = reparticoes.OrderByDescending(s => s.Nome).ToList();
                        break;
                    case "Code":
                        reparticoes = reparticoes.OrderBy(s => s.Codigo).ToList();
                        break;
                    case "code_desc":
                        reparticoes = reparticoes.OrderByDescending(s => s.Codigo).ToList();
                        break;
                    default:
                        reparticoes = reparticoes.OrderBy(s => s.Nome).ToList();
                        break;
                }

                ViewBag.TotalReparticoes = reparticoes.Count();

                return View(reparticoes);

            }
            else
            {
                return View("Error");
            }
        }

        // GET: Reparticoes/Create
        public ActionResult Create()
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.CodigoDistrito = new SelectList(db.Distrito, "Codigo", "Nome");
                ViewBag.CodigoDistrito = new SelectList(db.Reparticoes, "CodigoDistrito", "Nome");
                ViewBag.CodigoDistrito = new SelectList(db.Reparticoes, "CodigoDistrito", "Nome");
                return View();
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Reparticoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CodigoDistrito,CodigoConcelho,CodigoFreguesia,Codigo,Nome")] Reparticoes reparticoes)
        {
            if (ModelState.IsValid)
            {
                db.Reparticoes.Add(reparticoes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CodigoDistrito = new SelectList(db.Distrito, "Codigo", "Nome", reparticoes.CodigoDistrito);
            ViewBag.CodigoDistrito = new SelectList(db.Reparticoes, "CodigoDistrito", "Nome", reparticoes.CodigoDistrito);
            ViewBag.CodigoDistrito = new SelectList(db.Reparticoes, "CodigoDistrito", "Nome", reparticoes.CodigoDistrito);
            return View(reparticoes);
        }

        // GET: Reparticoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Reparticoes reparticoes = db.Reparticoes.Find(id);
                if (reparticoes == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CodigoDistrito = new SelectList(db.Distrito, "Codigo", "Nome", reparticoes.CodigoDistrito);
                ViewBag.CodigoDistrito = new SelectList(db.Reparticoes, "CodigoDistrito", "Nome", reparticoes.CodigoDistrito);
                ViewBag.CodigoDistrito = new SelectList(db.Reparticoes, "CodigoDistrito", "Nome", reparticoes.CodigoDistrito);
                return View(reparticoes);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Reparticoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CodigoDistrito,CodigoConcelho,CodigoFreguesia,Codigo,Nome")] Reparticoes reparticoes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reparticoes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CodigoDistrito = new SelectList(db.Distrito, "Codigo", "Nome", reparticoes.CodigoDistrito);
            ViewBag.CodigoDistrito = new SelectList(db.Reparticoes, "CodigoDistrito", "Nome", reparticoes.CodigoDistrito);
            ViewBag.CodigoDistrito = new SelectList(db.Reparticoes, "CodigoDistrito", "Nome", reparticoes.CodigoDistrito);
            return View(reparticoes);
        }

        // GET: Reparticoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Reparticoes reparticoes = db.Reparticoes.Find(id);
                if (reparticoes == null)
                {
                    return HttpNotFound();
                }
                return View(reparticoes);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Reparticoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reparticoes reparticoes = db.Reparticoes.Find(id);
            db.Reparticoes.Remove(reparticoes);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: Reparticoes/MassInsert
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
                            var codigoDistrito = Convert.ToInt32(workSheet.Cells[rowIterator, 1].Value.ToString());
                            var codigoConcelho = Convert.ToInt32(workSheet.Cells[rowIterator, 2].Value.ToString());
                            var codigoFreguesia = Convert.ToInt32(workSheet.Cells[rowIterator, 3].Value.ToString());
                            var codigo = Convert.ToInt32(workSheet.Cells[rowIterator, 4].Value.ToString());
                            var nome = workSheet.Cells[rowIterator, 5].Value.ToString();
                            


                            if (!db.Reparticoes.Any(r => r.Nome == nome || r.Codigo == codigo))
                            {
                                Reparticoes reparticao = new Reparticoes
                                {
                                    CodigoDistrito = codigoDistrito,
                                    CodigoConcelho = codigoConcelho,
                                    CodigoFreguesia = codigoFreguesia,
                                    Codigo = codigo,
                                    Nome = nome

                                };

                                db.Reparticoes.Add(reparticao);
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
