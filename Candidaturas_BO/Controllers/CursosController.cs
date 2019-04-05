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
    public class CursosController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Cursos
        public ActionResult Index(string searchString, string sortOrder, string edicao)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.UserAdmin = ADAuthorization.ADAuthenticateAdmin();

                List<Curso> cursos = db.Curso.ToList();

                if (!String.IsNullOrEmpty(edicao))
                {
                    cursos = cursos.Where(s => s.Edicao == edicao).ToList();
                    ViewBag.EdicaoEscolhida = edicao;

                }
                else
                {
                    Edicao edAux = db.Edicao.Where(e => e.DataInicio < System.DateTime.Now && e.DataFim > System.DateTime.Now).FirstOrDefault();
                    if (edAux != null)
                    {
                        cursos = cursos.Where(s => s.Edicao == edAux.Sigla).ToList();
                        ViewBag.EdicaoEscolhida = edAux.Sigla;

                    }
                    else
                    {
                        Edicao ultimaEdicao = db.Edicao.OrderByDescending(e => e.DataFim).FirstOrDefault();
                        cursos = cursos.Where(s => s.Edicao == ultimaEdicao.Sigla).ToList();
                        ViewBag.EdicaoEscolhida = ultimaEdicao.Sigla;

                    }

                }

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    cursos = cursos.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }

                //sort
                switch (sortOrder)
                {
                    case "name_desc":
                        cursos = cursos.OrderByDescending(s => s.Nome).ToList();
                        break;
                    default:
                        cursos = cursos.OrderBy(s => s.Nome).ToList();
                        break;
                }
                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderByDescending(dp => dp.DataFim).Select(c => new SelectListItem
                {
                    Value = c.Sigla,
                    Text = c.Sigla
                });

                ViewBag.Edicao = edicaos.ToList();

                ViewBag.TotalCursos = cursos.Count();

                return View(cursos);
            }
            else
            {
                return View("Error");
            }

            
        }

        // GET: Cursos/Create
        public ActionResult Create()
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderByDescending(dp => dp.DataFim).Select(c => new SelectListItem
                {
                    Value = c.Sigla,
                    Text = c.Sigla
                });

                Edicao edAux = db.Edicao.Where(e => e.DataInicio < System.DateTime.Now && e.DataFim > System.DateTime.Now).FirstOrDefault();
                if (edAux != null)
                {
                    ViewBag.EdicaoEscolhida = edAux.Sigla;

                }
                else
                {
                    Edicao ultimaEdicao = db.Edicao.OrderByDescending(e => e.DataFim).FirstOrDefault();
                    ViewBag.EdicaoEscolhida = ultimaEdicao.Sigla;

                }

                ViewBag.Edicao = edicaos.ToList();

                return View();
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Cursos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nome,Edicao")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                db.Curso.Add(curso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(curso);
        }

        // GET: Cursos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Curso curso = db.Curso.Find(id);
                if (curso == null)
                {
                    return HttpNotFound();
                }
                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderBy(dp => dp.Sigla).Select(c => new SelectListItem
                {
                    Value = c.Sigla,
                    Text = c.Sigla
                });

                ViewBag.Edicao = edicaos.ToList();
                return View(curso);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Cursos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nome,Edicao")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                db.Entry(curso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(curso);
        }

        // GET: Cursos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Curso curso = db.Curso.Find(id);
                if (curso == null)
                {
                    return HttpNotFound();
                }
                return View(curso);
            }
            else
            {
                return View("Error");
            }            
        }

        // POST: Cursos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Curso curso = db.Curso.Find(id);
            db.Curso.Remove(curso);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: Cursos/MassInsert
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
                            var edicao = workSheet.Cells[rowIterator, 2].Value.ToString();

                            if (!db.Curso.Any(c => c.Nome == nome && c.Edicao == edicao))
                            {
                                Curso curso = new Curso();

                                curso.Nome = nome;
                                curso.Edicao = edicao;
                                   

                                db.Curso.Add(curso);
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
