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

namespace Candidaturas_BO.Controllers
{
    public class GenerosController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Generos
        public ActionResult Index(string searchString)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                List<Genero> generos = db.Genero.ToList();

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    generos = generos.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }

                return View(generos);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Generos/Details/5
        public ActionResult Details(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Genero genero = db.Genero.Find(id);
                if (genero == null)
                {
                    return HttpNotFound();
                }
                return View(genero);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Generos/Create
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

        // POST: Generos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nome")] Genero genero)
        {
            if (ModelState.IsValid)
            {
                db.Genero.Add(genero);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(genero);
        }

        // GET: Generos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Genero genero = db.Genero.Find(id);
                if (genero == null)
                {
                    return HttpNotFound();
                }
                return View(genero);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Generos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nome")] Genero genero)
        {
            if (ModelState.IsValid)
            {
                db.Entry(genero).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(genero);
        }

        // GET: Generos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Genero genero = db.Genero.Find(id);
                if (genero == null)
                {
                    return HttpNotFound();
                }
                return View(genero);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Generos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Genero genero = db.Genero.Find(id);
            db.Genero.Remove(genero);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: Generos/MassInsert
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
                string fileName = uploadFile.FileName;
                string fileContentType = uploadFile.ContentType;
                byte[] fileBytes = new byte[uploadFile.ContentLength];
                var data = uploadFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(uploadFile.ContentLength));

                using (var package = new ExcelPackage(uploadFile.InputStream))
                {
                    var currentSheet = package.Workbook.Worksheets;
                    var workSheet = currentSheet.First();
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;

                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        Genero genero = new Genero();
                        genero.Nome = workSheet.Cells[rowIterator, 1].Value.ToString();
                        db.Genero.Add(genero);
                        db.SaveChanges();
                    }
                }
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
