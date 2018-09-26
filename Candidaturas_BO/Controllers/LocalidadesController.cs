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
    public class LocalidadesController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Localidades
        public ActionResult Index(string searchString)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                List<Localidade> localidades = db.Localidade.ToList();

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    localidades = localidades.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }

                return View(localidades);
            }
            else
            {
                return View("Error");
            }            
        }

        // GET: Localidades/Details/5
        public ActionResult Details(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Localidade localidade = db.Localidade.Find(id);
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

        // GET: Localidades/Create
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

        // POST: Localidades/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nome")] Localidade localidade)
        {
            if (ModelState.IsValid)
            {
                db.Localidade.Add(localidade);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(localidade);
        }

        // GET: Localidades/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Localidade localidade = db.Localidade.Find(id);

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

        // POST: Localidades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nome")] Localidade localidade)
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
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Localidade localidade = db.Localidade.Find(id);

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
        public ActionResult DeleteConfirmed(int id)
        {
            Localidade localidade = db.Localidade.Find(id);
            db.Localidade.Remove(localidade);
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
