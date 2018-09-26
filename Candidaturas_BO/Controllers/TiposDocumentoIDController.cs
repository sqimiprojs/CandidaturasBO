﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Candidaturas_BO.Models;

namespace Candidaturas_BO.Controllers
{
    public class TiposDocumentoIDController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: TiposDocumentoID
        public ActionResult Index(string searchString)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                List<TipoDocumentoID> tiposDocID = db.TipoDocumentoID.ToList();

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    tiposDocID = tiposDocID.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }

                return View(tiposDocID);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: TiposDocumentoID/Details/5
        public ActionResult Details(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                TipoDocumentoID tipoDocumentoID = db.TipoDocumentoID.Find(id);

                if (tipoDocumentoID == null)
                {
                    return HttpNotFound();
                }

                return View(tipoDocumentoID);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: TiposDocumentoID/Create
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

        // POST: TiposDocumentoID/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nome")] TipoDocumentoID tipoDocumentoID)
        {
            if (ModelState.IsValid)
            {
                db.TipoDocumentoID.Add(tipoDocumentoID);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoDocumentoID);
        }

        // GET: TiposDocumentoID/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                TipoDocumentoID tipoDocumentoID = db.TipoDocumentoID.Find(id);

                if (tipoDocumentoID == null)
                {
                    return HttpNotFound();
                }

                return View(tipoDocumentoID);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: TiposDocumentoID/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nome")] TipoDocumentoID tipoDocumentoID)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoDocumentoID).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoDocumentoID);
        }

        // GET: TiposDocumentoID/Delete/5
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                TipoDocumentoID tipoDocumentoID = db.TipoDocumentoID.Find(id);

                if (tipoDocumentoID == null)
                {
                    return HttpNotFound();
                }

                return View(tipoDocumentoID);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: TiposDocumentoID/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoDocumentoID tipoDocumentoID = db.TipoDocumentoID.Find(id);
            db.TipoDocumentoID.Remove(tipoDocumentoID);
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
