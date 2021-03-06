﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Candidaturas_BO.Models;
using OfficeOpenXml;
using PagedList;

namespace Candidaturas_BO.Controllers
{
    public class FreguesiasController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Freguesias
        public ActionResult Index(string searchString, string concelho, string currentFilter, string sortOrder, int? page)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.CodeSortParm = sortOrder == "Code" ? "code_desc" : "Code";
                ViewBag.ConcelhoSortParm = sortOrder == "Concelho" ? "concelho_desc" : "Concelho";
                ViewBag.DistritoSortParm = sortOrder == "Distrito" ? "distrito_desc" : "Distrito";
                ViewBag.UserAdmin = ADAuthorization.ADAuthenticateAdmin();

                List<Freguesia> freguesias = db.Freguesia.ToList();

                //search
                if (!String.IsNullOrEmpty(concelho))
                {
                    int codigoConcelho = db.Concelho.Where(s => s.Nome == concelho).Select(s => s.Codigo).FirstOrDefault();

                    int codigoDistrito = db.Concelho.Where(s => s.Nome == concelho).Select(s => s.CodigoDistrito).FirstOrDefault();

                    freguesias = freguesias.Where(s => s.CodigoConcelho == codigoConcelho && s.CodigoDistrito == codigoDistrito).ToList();

                    page = 1;
                }
                else
                {
                    concelho = currentFilter;
                }

                ViewBag.CurrentFilter = concelho;

                if (!String.IsNullOrEmpty(searchString))
                {
                    freguesias = freguesias.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }

                //sort
                switch (sortOrder)
                {
                    case "name_desc":
                        freguesias = freguesias.OrderByDescending(s => s.Nome).ToList();
                        break;
                    case "Code":
                        freguesias = freguesias.OrderBy(s => s.Codigo).ToList();
                        break;
                    case "code_desc":
                        freguesias = freguesias.OrderByDescending(s => s.Codigo).ToList();
                        break;
                    case "Concelho":
                        freguesias = freguesias.OrderBy(s => s.CodigoConcelho).ToList();
                        break;
                    case "concelho_desc":
                        freguesias = freguesias.OrderByDescending(s => s.CodigoConcelho).ToList();
                        break;
                    case "Distrito":
                        freguesias = freguesias.OrderBy(s => s.CodigoDistrito).ToList();
                        break;
                    case "distrito_desc":
                        freguesias = freguesias.OrderByDescending(s => s.CodigoDistrito).ToList();
                        break;
                    default:
                        freguesias = freguesias.OrderBy(s => s.Nome).ToList();
                        break;
                }

                IEnumerable<SelectListItem> concelhos = db.Concelho.OrderBy(dp => dp.Nome).Select(c => new SelectListItem
                {
                    Value = c.Nome,
                    Text = c.Nome + " - " + db.Distrito.Where(dp => dp.Codigo == c.CodigoDistrito).Select(d => d.Nome).FirstOrDefault()
                });

                ViewBag.Concelho = concelhos.ToList();

                ViewBag.TotalFreguesias = freguesias.Count();

                int pageSize = 50;
                int pageNumber = (page ?? 1);

                return View(freguesias.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return View("Error");
            }
        }
        
        // GET: Freguesias/Create
        public ActionResult Create()
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                IEnumerable<SelectListItem> concelhos = db.Concelho.OrderBy(dp => dp.Nome).Select(c => new SelectListItem
                {
                    Value = c.Codigo.ToString(),
                    Text = c.Nome
                });

                ViewBag.CodigoConcelho = concelhos.ToList();

                return View();
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Freguesias/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Nome,Codigo,CodigoConcelho,CodigoDistrito")] Freguesia freguesia)
        {
            if (ModelState.IsValid)
            {
                int codigoDistrito = db.Concelho.Where(d => d.Codigo == freguesia.CodigoConcelho).Select(d => d.CodigoDistrito).FirstOrDefault();
                freguesia.CodigoDistrito = codigoDistrito;
                db.Freguesia.Add(freguesia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(freguesia);
        }

        // GET: Freguesias/Edit/5
        public ActionResult Edit(int? codigo, int? codigoConcelho, int? codigoDistrito)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (codigo == null || codigoConcelho == null || codigoDistrito == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Freguesia freguesia = db.Freguesia.Find(codigoDistrito, codigoConcelho, codigo);
                if (freguesia == null)
                {
                    return HttpNotFound();
                }

                IEnumerable<SelectListItem> concelhos = db.Concelho.OrderBy(dp => dp.Nome).Select(c => new SelectListItem
                {
                    Value = c.Codigo.ToString(),
                    Text = c.Nome,
                    Selected = c.Codigo == freguesia.CodigoConcelho
                });

                ViewBag.CodigoConcelho = concelhos.ToList();

                return View(freguesia);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Freguesias/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Nome,Codigo,CodigoConcelho,CodigoDistrito")] Freguesia freguesia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(freguesia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(freguesia);
        }

        // GET: Freguesias/Delete/5
        public ActionResult Delete(int? codigo, int? codigoConcelho, int? codigoDistrito)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (codigo == null || codigoConcelho == null || codigoDistrito == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Freguesia freguesia = db.Freguesia.Find(codigoDistrito, codigoConcelho, codigo);

                if (freguesia == null)
                {
                    return HttpNotFound();
                }

                return View(freguesia);
            }
            else
            {
                return View("Error");
            }
        }

        //GET: Freguesias/MassInsert
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
                            var codigoConcelho = Convert.ToInt32(workSheet.Cells[rowIterator, 3].Value.ToString());
                            var codigoDistrito = Convert.ToInt32(workSheet.Cells[rowIterator, 4].Value.ToString());

                            if(!db.Freguesia.Any(f => f.Nome == nome || f.Codigo == codigo))
                            {
                                Freguesia freguesia = new Freguesia
                                {
                                    Nome = nome,
                                    Codigo = codigo,
                                    CodigoConcelho = codigoConcelho,
                                    CodigoDistrito = codigoDistrito
                                };

                                db.Freguesia.Add(freguesia);
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

        // POST: Freguesias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int codigo, int? codigoConcelho, int? codigoDistrito)
        {
            Freguesia freguesia = db.Freguesia.Find(codigoDistrito, codigoConcelho, codigo);
            db.Freguesia.Remove(freguesia);
            db.SaveChanges();
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
