using Candidaturas_BO.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web.Mvc;

namespace Candidaturas_BO.Controllers
{
    public class CandidatosController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Candidatos
        public ActionResult Index(string startDate, string endDate, string email)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                List<UserFull> candidatos = new List<UserFull>();

                List<User> users = db.User.ToList();
                
                //search
                if(startDate != null || endDate != null)
                {
                    if (startDate != "" && endDate != "")
                    {
                        DateTime? start = Convert.ToDateTime(startDate);
                        DateTime? end = Convert.ToDateTime(endDate);

                        users = users.Where(u => u.DataCriacao >= start && u.DataCriacao <= end).ToList();
                    }
                    else if (startDate != "")
                    {
                        DateTime? start = Convert.ToDateTime(startDate);

                        users = users.Where(u => u.DataCriacao >= start).ToList();
                    }
                    else if (endDate != "")
                    {
                        DateTime? end = Convert.ToDateTime(endDate);

                        users = users.Where(u => u.DataCriacao <= end).ToList();
                    }
                }

                if (email != null)
                {
                    if (email != "")
                    {
                        users = users.Where(u => u.Email == email).ToList();
                    }
                }

                foreach (var user in users)
                {
                    UserFull candidato = new UserFull();

                    candidato.User = user;
                    candidato.DadosPessoais = db.DadosPessoais.Where(dp => dp.UserId == user.ID).FirstOrDefault();

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
                UserFull candidato = new UserFull();

                List<CursoDisplay> cursoDisplays = new List<CursoDisplay>();
                List<string> exames = new List<string>();
                List<DocModel> documentos = new List<DocModel>();

                candidato.User = db.User.Where(dp => dp.ID == id).FirstOrDefault();
                candidato.DadosPessoais = db.DadosPessoais.Where(dp => dp.UserId == id).FirstOrDefault();
                candidato.Inquerito = db.Inquerito.Where(i => i.UserId == id).FirstOrDefault();
                candidato.Candidato = db.Candidato.Where(i => i.UserID == id).FirstOrDefault();

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

                foreach (var doc in db.Documento.Where(d => d.UserID == candidato.User.ID).ToList())
                {
                    DocModel document = new DocModel();
                    document.DocumentoInfo = doc;
                    document.DocumentoBinario = db.DocumentoBinario.Where(db => db.DocID == doc.ID).FirstOrDefault();
                    documentos.Add(document);
                }

                candidato.UserDocs = documentos;

                return View(candidato);
            }
            else
            {
                return View("Error");
            }
        }
    }
}