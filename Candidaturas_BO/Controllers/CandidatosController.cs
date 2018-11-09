﻿using Candidaturas_BO.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Candidaturas_BO.Controllers
{
    public class CandidatosController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Candidatos
        public ActionResult Index(string distrito, string sortOrder)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                List<Candidato> candidatos = new List<Candidato>();

                List<User> users = db.User.ToList();

                foreach(var user in users)
                {
                    Candidato candidato = new Candidato();

                    List<CursoDisplay> cursoDisplays = new List<CursoDisplay>();
                    List<string> exames = new List<string>();

                    candidato.User = user;
                    candidato.DadosPessoais = db.DadosPessoais.Where(dp => dp.UserId == user.ID).FirstOrDefault();
                    candidato.Inquerito = db.Inquerito.Where(i => i.UserId == user.ID).FirstOrDefault();

                    foreach (var uc in db.UserCurso.Where(uc => uc.UserId == candidato.User.ID).ToList())
                    {
                        CursoDisplay cursoDisplay = new CursoDisplay
                        {
                            Prioridade = uc.Prioridade,
                            Nome = db.Curso.Where(c => c.ID == uc.CursoId).Select(c => c.Nome).FirstOrDefault()
                        };

                        cursoDisplays.Add(cursoDisplay);
                    }

                    candidato.UserCursos = cursoDisplays;

                    foreach (var ue in db.UserExame.Where(ue => ue.UserId == candidato.User.ID).ToList())
                    {
                        exames.Add(db.Exame.Where(e => e.ID == ue.ExameId).Select(e => e.Nome).FirstOrDefault());
                    }

                    candidato.UserExames = exames;

                    candidatos.Add(candidato);
                }

                ViewBag.TotalCandidatos = candidatos.Count();

                return View(candidatos);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Candidatos/Details/5
        public ActionResult Details(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                Candidato candidato = new Candidato();

                List<CursoDisplay> cursoDisplays = new List<CursoDisplay>();
                List<string> exames = new List<string>();

                candidato.User = db.User.Where(u => u.ID == id).FirstOrDefault();
                candidato.DadosPessoais = db.DadosPessoais.Where(dp => dp.UserId == id).FirstOrDefault();
                candidato.Inquerito = db.Inquerito.Where(i => i.UserId == id).FirstOrDefault();

                foreach(var uc in db.UserCurso.Where(uc => uc.UserId == id).ToList())
                {
                    CursoDisplay cursoDisplay = new CursoDisplay
                    {
                        Prioridade = uc.Prioridade,
                        Nome = db.Curso.Where(c => c.ID == uc.CursoId).Select(c => c.Nome).FirstOrDefault()
                    };

                    cursoDisplays.Add(cursoDisplay);
                }

                candidato.UserCursos = cursoDisplays;

                foreach(var ue in db.UserExame.Where(ue => ue.UserId == id).ToList())
                {
                    exames.Add(db.Exame.Where(e => e.ID == ue.ExameId).Select(e => e.Nome).FirstOrDefault());
                }

                candidato.UserExames = exames;

                return View(candidato);
            }
            else
            {
                return View("Error");
            }
        }
    }
}