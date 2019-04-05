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
        public ActionResult Index(string searchString, string sortOrder, string edicao)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.UserAdmin = ADAuthorization.ADAuthenticateAdmin();

                List<ConhecimentoEscola> conhecimentosEscola = db.ConhecimentoEscola.ToList();

                if (!String.IsNullOrEmpty(edicao))
                {
                    conhecimentosEscola = conhecimentosEscola.Where(s => s.Edicao == edicao).ToList();
                    ViewBag.EdicaoEscolhida = edicao;

                }
                else
                {
                    Edicao edAux = db.Edicao.Where(e => e.DataInicio < System.DateTime.Now && e.DataFim > System.DateTime.Now).FirstOrDefault();
                    if (edAux != null)
                    {
                        conhecimentosEscola = conhecimentosEscola.Where(s => s.Edicao == edAux.Sigla).ToList();
                        ViewBag.EdicaoEscolhida = edAux.Sigla;

                    }
                    else
                    {
                        Edicao ultimaEdicao = db.Edicao.OrderByDescending(e => e.DataFim).FirstOrDefault();
                        conhecimentosEscola = conhecimentosEscola.Where(s => s.Edicao == ultimaEdicao.Sigla).ToList();
                        ViewBag.EdicaoEscolhida = ultimaEdicao.Sigla;

                    }

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

                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderByDescending(dp => dp.DataFim).Select(c => new SelectListItem
                {
                    Value = c.Sigla,
                    Text = c.Sigla
                });

                ViewBag.Edicao = edicaos.ToList();

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

        // POST: ConhecimentoEscolas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nome,Edicao")] ConhecimentoEscola conhecimentoEscola)
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
            if (ADAuthorization.ADAuthenticateAdmin())
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

                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderBy(dp => dp.Sigla).Select(c => new SelectListItem
                {
                    Value = c.Sigla,
                    Text = c.Sigla
                });

                ViewBag.Edicao = edicaos.ToList();

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
        public ActionResult Edit([Bind(Include = "ID,Nome,Edicao")] ConhecimentoEscola conhecimentoEscola)
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
            if (ADAuthorization.ADAuthenticateAdmin())
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

                            if (!db.ConhecimentoEscola.Any(ce => ce.Nome == nome && ce.Edicao == edicao))
                            {
                                ConhecimentoEscola conhecimentoEscola = new ConhecimentoEscola();
                                conhecimentoEscola.Nome = nome;
                                conhecimentoEscola.Edicao = edicao;
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
