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
    public class SituacaosController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Situacaos
        public ActionResult Index(string searchString)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                List<Situacao> situacoes = db.Situacao.ToList();

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    situacoes = situacoes.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }

                return View(situacoes);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Situacaos/Details/5
        public ActionResult Details(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Situacao situacao = db.Situacao.Find(id);

                if (situacao == null)
                {
                    return HttpNotFound();
                }

                return View(situacao);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Situacaos/Create
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

        // POST: Situacaos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nome")] Situacao situacao)
        {
            if (ModelState.IsValid)
            {
                db.Situacao.Add(situacao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(situacao);
        }

        // GET: Situacaos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Situacao situacao = db.Situacao.Find(id);
                if (situacao == null)
                {
                    return HttpNotFound();
                }
                return View(situacao);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Situacaos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nome")] Situacao situacao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(situacao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(situacao);
        }

        // GET: Situacaos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Situacao situacao = db.Situacao.Find(id);
                if (situacao == null)
                {
                    return HttpNotFound();
                }
                return View(situacao);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Situacaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Situacao situacao = db.Situacao.Find(id);
            db.Situacao.Remove(situacao);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: Situacaos/MassInsert
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
                        Situacao situacao = new Situacao();
                        situacao.Nome = workSheet.Cells[rowIterator, 1].Value.ToString();
                        db.Situacao.Add(situacao);
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
