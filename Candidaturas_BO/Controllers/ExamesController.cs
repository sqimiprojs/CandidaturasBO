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
    public class ExamesController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Exames
        public ActionResult Index(string searchString, string codigo, string sortOrder, string edicao)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.CodeSortParm = sortOrder == "Código" ? "code_desc" : "Código";
                ViewBag.UserAdmin = ADAuthorization.ADAuthenticateAdmin();

                List<Exame> exames = db.Exame.ToList();

                if (!String.IsNullOrEmpty(edicao))
                {
                    exames = exames.Where(s => s.Edicao == edicao).ToList();
                }
                else
                {
                    Edicao edAux = db.Edicao.Where(e => e.DataInicio < System.DateTime.Now && e.DataFim > System.DateTime.Now).FirstOrDefault();
                    if(edAux != null)
                    {
                        exames = exames.Where(s => s.Edicao == edAux.Sigla).ToList();
                    }
                    else
                    {
                        Edicao ultimaEdicao = db.Edicao.OrderByDescending(e => e.DataFim).FirstOrDefault();
                        exames = exames.Where(s => s.Edicao == ultimaEdicao.Sigla).ToList();
                    }
                }

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    exames = exames.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }

                if(!String.IsNullOrEmpty(codigo))
                {
                    exames = exames.Where(s => s.Código.Equals(int.Parse(codigo))).ToList();
                }

                //sort
                switch (sortOrder)
                {
                    case "name_desc":
                        exames = exames.OrderByDescending(s => s.Nome).ToList();
                        break;
                    case "Código":
                        exames = exames.OrderBy(s => s.Código).ToList();
                        break;
                    case "code_desc":
                        exames = exames.OrderByDescending(s => s.Código).ToList();
                        break;
                    default:
                        exames = exames.OrderBy(s => s.Nome).ToList();
                        break;
                }

                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderBy(dp => dp.Sigla).Select(c => new SelectListItem
                {
                    Value = c.Sigla,
                    Text = c.Sigla
                });

                ViewBag.Edicao = edicaos.ToList();

                ViewBag.TotalExames = exames.Count();

                return View(exames);
            }
            else
            {
                return View("Error");
            }    
        }
        
        // GET: Exames/Create
        public ActionResult Create()
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderBy(dp => dp.Sigla).Select(c => new SelectListItem
                {
                    Value = c.Sigla,
                    Text = c.Sigla
                });

                ViewBag.Edicao = edicaos.ToList();

                return View();
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Exames/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nome,Código,Edicao")] Exame exame)
        {
            if (ModelState.IsValid)
            {
                db.Exame.Add(exame);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(exame);
        }

        // GET: Exames/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Exame exame = db.Exame.Find(id);
                if (exame == null)
                {
                    return HttpNotFound();
                }
                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderBy(dp => dp.Sigla).Select(c => new SelectListItem
                {
                    Value = c.Sigla,
                    Text = c.Sigla
                });

                ViewBag.Edicao = edicaos.ToList();

                return View(exame);
            }
            else
            {
                return View("Error");
            }            
        }

        // POST: Exames/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nome,Código,Edicao")] Exame exame)
        {
            if (ModelState.IsValid)
            {
                db.Entry(exame).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(exame);
        }

        // GET: Exames/Delete/5
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Exame exame = db.Exame.Find(id);
                if (exame == null)
                {
                    return HttpNotFound();
                }
                return View(exame);
            }
            else
            {
                return View("Error");
            }           
        }

        // POST: Exames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Exame exame = db.Exame.Find(id);
            db.Exame.Remove(exame);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: Exames/MassInsert
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
                            var edicao = workSheet.Cells[rowIterator, 3].Value.ToString();

                            if (!db.Exame.Any(e => e.Nome == nome && e.Edicao == edicao))
                            {
                                Exame exame = new Exame();

                                exame.Nome = nome;
                                exame.Código = codigo;
                                exame.Edicao = edicao;
                                

                                db.Exame.Add(exame);
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
