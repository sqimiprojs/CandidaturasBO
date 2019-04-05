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
    public class DocumentosNecessariosController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: DocumentosNecessarios
        public ActionResult Index(string edicao, string searchString, string sortOrder)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.UserAdmin = ADAuthorization.ADAuthenticateAdmin();

                List<DocumentosNecessarios> docNecessarios = db.DocumentosNecessarios.ToList();

                if (!String.IsNullOrEmpty(edicao))
                {
                    docNecessarios = docNecessarios.Where(s => s.Edicao == edicao).ToList();
                    ViewBag.EdicaoEscolhida = edicao;

                }
                else
                {
                    Edicao edAux = db.Edicao.Where(e => e.DataInicio < System.DateTime.Now && e.DataFim > System.DateTime.Now).FirstOrDefault();
                    if (edAux != null)
                    {
                        docNecessarios = docNecessarios.Where(s => s.Edicao == edAux.Sigla).ToList();
                        ViewBag.EdicaoEscolhida = edAux.Sigla;

                    }
                    else
                    {
                        Edicao ultimaEdicao = db.Edicao.OrderByDescending(e => e.DataFim).FirstOrDefault();
                        docNecessarios = docNecessarios.Where(s => s.Edicao == ultimaEdicao.Sigla).ToList();
                        ViewBag.EdicaoEscolhida = ultimaEdicao.Sigla;

                    }

                }


                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    docNecessarios = docNecessarios.Where(s => s.Documento.Contains(searchString) || s.Documento.ToLower().Contains(searchString)).ToList();
                }

                //sort
                switch (sortOrder)
                {
                    case "name_desc":
                        docNecessarios = docNecessarios.OrderByDescending(s => s.Documento).ToList();
                        break;
                    default:
                        docNecessarios = docNecessarios.OrderBy(s => s.Documento).ToList();
                        break;
                }

                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderBy(dp => dp.Sigla).Select(c => new SelectListItem
                {
                    Value = c.Sigla,
                    Text = c.Sigla
                });

                ViewBag.Edicao = edicaos.ToList();

                ViewBag.TotalDocumentos = docNecessarios.Count();

                return View(docNecessarios);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: DocumentosNecessarios/Create
        public ActionResult Create()
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderBy(dp => dp.Sigla).Select(c => new SelectListItem
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

        // POST: DocumentosNecessarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Documento,Edicao")] DocumentosNecessarios docNecessario)
        {
            if (ModelState.IsValid)
            {
                db.DocumentosNecessarios.Add(docNecessario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(docNecessario);
        }

        // GET: DocumentosNecessarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DocumentosNecessarios docNecessario = db.DocumentosNecessarios.Find(id);
                if (docNecessario == null)
                {
                    return HttpNotFound();
                }
                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderBy(dp => dp.Sigla).Select(c => new SelectListItem
                {
                    Value = c.Sigla,
                    Text = c.Sigla
                });

                ViewBag.Edicao = edicaos.ToList();

                return View(docNecessario);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: DocumentosNecessarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Documento,Edicao")] DocumentosNecessarios docNecessario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(docNecessario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(docNecessario);
        }

        // GET: DocumentosNecessarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                DocumentosNecessarios docNecessario = db.DocumentosNecessarios.Find(id);
                if (docNecessario == null)
                {
                    return HttpNotFound();
                }
                return View(docNecessario);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: DocumentosNEcessarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DocumentosNecessarios docNecessario = db.DocumentosNecessarios.Find(id);
            db.DocumentosNecessarios.Remove(docNecessario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: Situacaos/MassInsert
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
                            var documento = workSheet.Cells[rowIterator, 1].Value.ToString();
                            var edicao = workSheet.Cells[rowIterator, 2].Value.ToString();

                            if (!db.DocumentosNecessarios.Any(s => s.Documento == documento && s.Edicao == edicao))
                            {
                                DocumentosNecessarios docNecessario = new DocumentosNecessarios();

                                docNecessario.Documento = documento;
                                docNecessario.Edicao = edicao;


                                db.DocumentosNecessarios.Add(docNecessario);
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
