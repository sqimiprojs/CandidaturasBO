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
    public class CursoExamesController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: CursoExames
        public ActionResult Index(string sortOrder, string edicao)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

                List<CursoExame> cursosExames = db.CursoExame.ToList();

                if (!String.IsNullOrEmpty(edicao))
                {
                    cursosExames = cursosExames.Where(s => s.Edicao == edicao).ToList();
                }



                //sort
                switch (sortOrder)
                {
                    case "name_desc":
                        cursosExames = cursosExames.OrderByDescending(s => s.CursoID).ToList();
                        break;
                    default:
                        cursosExames = cursosExames.OrderBy(s => s.CursoID).ToList();
                        break;
                }
                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderBy(dp => dp.Sigla).Select(c => new SelectListItem
                {
                    Value = c.Sigla,
                    Text = c.Sigla
                });

                ViewBag.Edicao = edicaos.ToList();

                ViewBag.TotalCursosExames = cursosExames.Count();

                return View(cursosExames);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: CursoExames/Create
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

        // POST: CursoExames/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CursoID,ExameID,Edicao")] CursoExame cursoExame)
        {
            if (ModelState.IsValid)
            {
                db.CursoExame.Add(cursoExame);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CursoID = new SelectList(db.Curso, "ID", "Nome", cursoExame.CursoID);
            ViewBag.Edicao = new SelectList(db.Edicao, "Sigla", "AnoLectivo", cursoExame.Edicao);
            ViewBag.ExameID = new SelectList(db.Exame, "ID", "Nome", cursoExame.ExameID);
            return View(cursoExame);
        }

        // GET: CursoExames/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CursoExame cursoExame = db.CursoExame.Find(id);
                if (cursoExame == null)
                {
                    return HttpNotFound();
                }
                ViewBag.CursoID = new SelectList(db.Curso, "ID", "Nome", cursoExame.CursoID);
                ViewBag.Edicao = new SelectList(db.Edicao, "Sigla", "AnoLectivo", cursoExame.Edicao);
                ViewBag.ExameID = new SelectList(db.Exame, "ID", "Nome", cursoExame.ExameID);
                return View(cursoExame);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: CursoExames/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CursoID,ExameID,Edicao")] CursoExame cursoExame)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cursoExame).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CursoID = new SelectList(db.Curso, "ID", "Nome", cursoExame.CursoID);
            ViewBag.Edicao = new SelectList(db.Edicao, "Sigla", "AnoLectivo", cursoExame.Edicao);
            ViewBag.ExameID = new SelectList(db.Exame, "ID", "Nome", cursoExame.ExameID);
            return View(cursoExame);
        }

        // GET: CursoExames/Delete/5
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CursoExame cursoExame = db.CursoExame.Find(id);
            if (cursoExame == null)
            {
                return HttpNotFound();
            }
            return View(cursoExame);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: CursoExames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CursoExame cursoExame = db.CursoExame.Find(id);
            db.CursoExame.Remove(cursoExame);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //GET: Cursos/MassInsert
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
                            var codigoCurso = Convert.ToInt32(workSheet.Cells[rowIterator, 1].Value.ToString());
                            var codigoExame = Convert.ToInt32(workSheet.Cells[rowIterator, 2].Value.ToString());
                            var edicao = workSheet.Cells[rowIterator, 3].Value.ToString();

                            if (!db.CursoExame.Any(c => c.CursoID == codigoCurso && c.ExameID == codigoExame && c.Edicao == edicao))
                            {
                                CursoExame curso = new CursoExame();
                                curso.CursoID = codigoCurso;
                                curso.ExameID = codigoExame;
                                curso.Edicao = edicao;

                                db.CursoExame.Add(curso);
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
