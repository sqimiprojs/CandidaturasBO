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
    public class ConcelhosController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Concelhos
        public ActionResult Index(string searchString, string distrito, string sortOrder, string currentFilter, int? page)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.CodeSortParm = sortOrder == "Code" ? "code_desc" : "Code";
                ViewBag.DistritoSortParm = sortOrder == "Distrito" ? "distrito_desc" : "Distrito";
                ViewBag.UserAdmin = ADAuthorization.ADAuthenticateAdmin();

                List<Concelho> concelhos = db.Concelho.ToList();

                //search
                if (!String.IsNullOrEmpty(distrito))
                {
                    concelhos = concelhos.Where(s => s.CodigoDistrito == Convert.ToInt32(distrito)).ToList();
                    page = 1;
                }
                else
                {
                    distrito = currentFilter;
                }

                ViewBag.CurrentFilter = distrito;

                if (!String.IsNullOrEmpty(searchString))
                {
                    concelhos = concelhos.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }

                //sort
                switch (sortOrder)
                {
                    case "name_desc":
                        concelhos = concelhos.OrderByDescending(s => s.Nome).ToList();
                        break;
                    case "Code":
                        concelhos = concelhos.OrderBy(s => s.Codigo).ToList();
                        break;
                    case "code_desc":
                        concelhos = concelhos.OrderByDescending(s => s.Codigo).ToList();
                        break;
                    case "Distrito":
                        concelhos = concelhos.OrderBy(s => s.CodigoDistrito).ToList();
                        break;
                    case "distrito_desc":
                        concelhos = concelhos.OrderByDescending(s => s.CodigoDistrito).ToList();
                        break;
                    default:
                        concelhos = concelhos.OrderBy(s => s.Nome).ToList();
                        break;
                }

                IEnumerable<SelectListItem> distritos = db.Distrito.OrderBy(dp => dp.Nome).Select(c => new SelectListItem
                {
                    Value = c.Codigo.ToString(),
                    Text = c.Nome
                });

                ViewBag.Distrito = distritos.ToList();

                ViewBag.TotalConcelhos = concelhos.Count();

                int pageSize = 50;
                int pageNumber = (page ?? 1);

                return View(concelhos.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Concelhos/Create
        public ActionResult Create()
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                IEnumerable<SelectListItem> distritos = db.Distrito.OrderBy(dp => dp.Nome).Select(c => new SelectListItem
                {
                    Value = c.Codigo.ToString(),
                    Text = c.Nome
                });

                ViewBag.CodigoDistrito = distritos.ToList();

                return View();
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Concelhos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Nome,Codigo,CodigoDistrito")] Concelho concelho)
        {
            if (ModelState.IsValid)
            {
                db.Concelho.Add(concelho);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(concelho);
        }

        // GET: Concelhos/Edit/5
        public ActionResult Edit(int? codigo, int? codigoDistrito)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (codigo == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Concelho concelho = db.Concelho.Find(codigo, codigoDistrito);
                if (concelho == null)
                {
                    return HttpNotFound();
                }

                IEnumerable<SelectListItem> distritos = db.Distrito.OrderBy(dp => dp.Nome).Select(c => new SelectListItem
                {
                    Value = c.Codigo.ToString(),
                    Text = c.Nome,
                    Selected = c.Codigo == concelho.CodigoDistrito
                });

                ViewBag.CodigoDistrito = distritos.ToList();

                return View(concelho);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Concelhos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Nome,Codigo,CodigoDistrito")] Concelho concelho)
        {
            if (ModelState.IsValid)
            {
                db.Entry(concelho).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(concelho);
        }

        // GET: Concelhos/Delete/5
        public ActionResult Delete(int? codigo, int? codigoDistrito)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (codigo == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Concelho concelho = db.Concelho.Find(codigo, codigoDistrito);
                if (concelho == null)
                {
                    return HttpNotFound();
                }

                return View(concelho);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Concelhos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int codigo, int? codigoDistrito)
        {
            Concelho concelho = db.Concelho.Find(codigo, codigoDistrito);
            db.Concelho.Remove(concelho);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: Concelhos/MassInsert
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
                            var codigoDistrito = Convert.ToInt32(workSheet.Cells[rowIterator, 3].Value.ToString());

                            if(!db.Concelho.Any(c => c.Nome == nome || c.Codigo == codigo))
                            {
                                Concelho concelho = new Concelho
                                {
                                    Nome = nome,
                                    Codigo = codigo,
                                    CodigoDistrito = codigoDistrito
                                };

                                db.Concelho.Add(concelho);
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
