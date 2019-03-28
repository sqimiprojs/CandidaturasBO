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
using PagedList;

namespace Candidaturas_BO.Controllers
{
    public class LocalidadesController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Localidades
        public ActionResult Index(string searchString, string currentFilter, string sortOrder, int? page)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.CodeSortParm = sortOrder == "Code" ? "code_desc" : "Code";
                ViewBag.ConcelhoSortParm = sortOrder == "Concelho" ? "concelho_desc" : "Concelho";
                ViewBag.DistritoSortParm = sortOrder == "Distrito" ? "distrito_desc" : "Distrito";
                ViewBag.UserAdmin = ADAuthorization.ADAuthenticateAdmin();

                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;

                List<Localidade> localidades = db.Localidade.ToList();

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    localidades = localidades.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }

                //sort
                switch (sortOrder)
                {
                    case "name_desc":
                        localidades = localidades.OrderByDescending(s => s.Nome).ToList();
                        break;
                    case "Code":
                        localidades = localidades.OrderBy(s => s.Codigo).ToList();
                        break;
                    case "code_desc":
                        localidades = localidades.OrderByDescending(s => s.Codigo).ToList();
                        break;
                    case "Concelho":
                        localidades = localidades.OrderBy(s => s.CodigoConcelho).ToList();
                        break;
                    case "concelho_desc":
                        localidades = localidades.OrderByDescending(s => s.CodigoConcelho).ToList();
                        break;
                    case "Distrito":
                        localidades = localidades.OrderBy(s => s.CodigoDistrito).ToList();
                        break;
                    case "distrito_desc":
                        localidades = localidades.OrderByDescending(s => s.CodigoDistrito).ToList();
                        break;
                    default:
                        localidades = localidades.OrderBy(s => s.Nome).ToList();
                        break;
                }

                ViewBag.TotalLocalidades = localidades.Count();

                int pageSize = 50;
                int pageNumber = (page ?? 1);

                return View(localidades.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return View("Error");
            }            
        }
        
        // GET: Localidades/Create
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

        // POST: Localidades/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Nome,Codigo,CodigoConcelho,CodigoDistrito")] Localidade localidade)
        {
            if (ModelState.IsValid)
            {
                int codigoDistrito = db.Concelho.Where(d => d.Codigo == localidade.CodigoConcelho).Select(d => d.CodigoDistrito).FirstOrDefault();
                localidade.CodigoDistrito = codigoDistrito;
                db.Localidade.Add(localidade);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(localidade);
        }

        // GET: Localidades/Edit/5
        public ActionResult Edit(int? codigo, int? codigoConcelho, int? codigoDistrito)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (codigo == null || codigoConcelho == null || codigoDistrito == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Localidade localidade = db.Localidade.Find(codigoDistrito, codigoConcelho, codigo);

                if (localidade == null)
                {
                    return HttpNotFound();
                }

                IEnumerable<SelectListItem> concelhos = db.Concelho.OrderBy(dp => dp.Nome).Select(c => new SelectListItem
                {
                    Value = c.Codigo.ToString(),
                    Text = c.Nome,
                    Selected = c.Codigo == localidade.CodigoConcelho
                });

                ViewBag.CodigoConcelho = concelhos.ToList();

                return View(localidade);
            }
            else
            {
                return View("Error");
            }        
        }

        // POST: Localidades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Nome,Codigo,CodigoConcelho,CodigoDistrito")] Localidade localidade)
        {
            if (ModelState.IsValid)
            {
                db.Entry(localidade).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(localidade);
        }

        // GET: Localidades/Delete/5
        public ActionResult Delete(int? codigo, int? codigoConcelho, int? codigoDistrito)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (codigo == null || codigoConcelho == null || codigoDistrito == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Localidade localidade = db.Localidade.Find(codigoDistrito, codigoConcelho, codigo);

                if (localidade == null)
                {
                    return HttpNotFound();
                }

                return View(localidade);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Localidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int codigo, int? codigoConcelho, int? codigoDistrito)
        {
            Localidade localidade = db.Localidade.Find(codigoDistrito, codigoConcelho, codigo);
            db.Localidade.Remove(localidade);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: Localidades/MassInsert
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

                            if (!db.Localidade.Any(l => l.Nome == nome || l.Codigo == codigo))
                            {
                                Localidade localidade = new Localidade
                                {
                                    Nome = nome,
                                    Codigo = codigo,
                                    CodigoConcelho = codigoConcelho,
                                    CodigoDistrito = codigoDistrito
                                };

                                db.Localidade.Add(localidade);
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
