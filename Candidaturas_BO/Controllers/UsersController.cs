using Candidaturas_BO.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web.Mvc;

namespace Candidaturas_BO.Controllers
{
    public class UsersController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Candidatos
        public ActionResult Index(string edicao, string startDate, string endDate, string searchString, string sortOrder, string currentFilter, int? page, bool finalizado = false)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "";
                ViewBag.IDSortParm = sortOrder == "ID" ? "id_desc" : "ID";
                if (String.IsNullOrEmpty(edicao))
                {
                    edicao = db.Edicao.Where(e => e.DataInicio < System.DateTime.Now && e.DataFim > System.DateTime.Now).Select(e => e.Sigla).FirstOrDefault();
                    if (String.IsNullOrEmpty(edicao))
                    {
                        edicao = db.Edicao.OrderByDescending(e => e.DataFim).Select(e => e.Sigla).FirstOrDefault();
                    }
                }

                List<User> usersDB = db.User.ToList();
                
                if(finalizado)
                {
                    usersDB = db.Certificado.Select(c => c.Candidatura.User).ToList();
                    
                }

                if (!String.IsNullOrEmpty(edicao))
                {
                    usersDB = usersDB.Where(s => s.Edicao ==edicao).ToList();
                    page = 1;
                }
                else
                {
                    edicao = currentFilter;
                }

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    usersDB = usersDB.Where(s => s.Email.Contains(searchString) || s.Email.ToLower().Contains(searchString)).ToList();
                    page = 1;
                }

                if (startDate != null || endDate != null)
                {
                    if (startDate != "" && endDate != "")
                    {
                        DateTime? start = Convert.ToDateTime(startDate);
                        DateTime? end = Convert.ToDateTime(endDate);

                        usersDB = usersDB.Where(u => u.DataCriacao >= start && u.DataCriacao <= end).ToList();
                    }
                    else if (startDate != "")
                    {
                        DateTime? start = Convert.ToDateTime(startDate);

                        usersDB = usersDB.Where(u => u.DataCriacao >= start).ToList();
                    }
                    else if (endDate != "")
                    {
                        DateTime? end = Convert.ToDateTime(endDate);

                        usersDB = usersDB.Where(u => u.DataCriacao <= end).ToList();
                    }
                }

                //sort
                switch (sortOrder)
                {
                    case "email_desc":
                        usersDB = usersDB.OrderByDescending(s => s.Email).ToList();
                        break;
                    case "ID":
                        usersDB = usersDB.OrderBy(s => s.ID).ToList();
                        break;
                    case "id_desc":
                        usersDB = usersDB.OrderByDescending(s => s.ID).ToList();
                        break;
                    default:
                        usersDB = usersDB.OrderBy(s => s.Email).ToList();
                        break;
                }

                IEnumerable<SelectListItem> edicaos = db.Edicao.OrderBy(dp => dp.Sigla).Select(c => new SelectListItem
                {
                    Value = c.Sigla,
                    Text = c.Sigla
                });

                ViewBag.Edicao = edicaos.ToList();

                ViewBag.TotalCandidatos = usersDB.Count();

                //change pageSize to be global variable
                int pageSize = 50;
                int pageNumber = (page ?? 1);

                return View(usersDB.ToPagedList(pageNumber, pageSize));
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
                UserFull user = new UserFull();
                Candidatura candidatura = new Candidatura();
                candidatura = db.Candidatura.Where(c => c.UserId == id).FirstOrDefault();

                user.email = db.User.Where(guy => guy.ID == id).Select(guy => guy.Email).FirstOrDefault();

                user.dadosDTO = db.DadosPessoais
                    .Where(guy => guy.CandidaturaId == candidatura.id)
                    .Select(data => new DadosPessoaisDTO
                    {
                        NomeColoquial = data.NomeColoquial,
                        Nomes = data.Nomes,
                        Apelidos = data.Apelidos,
                        NomePai = data.NomePai,
                        NomeMae = data.NomeMae,
                        NDI = data.NDI,
                        TipoDocID = db.TipoDocumentoID.Where(td => td.ID == data.TipoDocID).Select(td => td.Nome).FirstOrDefault(),
                        DocumentoValidade = data.DocumentoValidade,
                        Genero = db.Genero.Where(g => g.ID == data.Genero).Select(g => g.Nome).FirstOrDefault(),
                        EstadoCivil = db.EstadoCivil.Where(ec => ec.ID == data.EstadoCivil).Select(ec => ec.Nome).FirstOrDefault(),
                        Nacionalidade = db.Pais.Where(p => p.Sigla == data.Nacionalidade).Select(p => p.Nome).FirstOrDefault(),
                        DistritoNatural = db.Distrito.Where(d => d.Codigo == data.DistritoNatural).Select(d => d.Nome).FirstOrDefault(),
                        ConcelhoNatural = db.Concelho.Where(c => c.Codigo == data.ConcelhoNatural).Select(c => c.Nome).FirstOrDefault(),
                        FreguesiaNatural = db.Freguesia.Where(f => f.Codigo == data.FreguesiaNatural).Select(f => f.Nome).FirstOrDefault(),
                        Morada = data.Morada,
                        Localidade = db.Localidade.Where(l => l.Codigo == data.Localidade).Select(l => l.Nome).FirstOrDefault(),
                        RepFinNIF = db.Reparticoes.Where(r => r.Codigo == data.RepFinNIF).Select(r => r.Nome).FirstOrDefault(),
                        CCDigitosControlo = data.CCDigitosControlo,
                        NSegSoc = data.NSegSoc,
                        NIF = data.NIF,
                        DistritoMorada = db.Distrito.Where(d => d.Codigo == data.DistritoMorada).Select(d => d.Nome).FirstOrDefault(),
                        ConcelhoMorada = db.Concelho.Where(c => c.Codigo == data.ConcelhoMorada).Select(c => c.Nome).FirstOrDefault(),
                        FreguesiaMorada = db.Freguesia.Where(f => f.Codigo == data.FreguesiaMorada).Select(f => f.Nome).FirstOrDefault(),
                        Telefone = data.Telefone,
                        CodigoPostal4Dig = data.CodigoPostal4Dig,
                        CodigoPostal3Dig = data.CodigoPostal3Dig,
                        DataCriacao = data.DataCriacao,
                        DataUltimaAtualizacao = data.DataUltimaAtualizacao,
                        DataNascimento = data.DataNascimento,
                        Militar = data.Militar,
                        Ramo = db.Ramo.Where(r => r.Sigla == data.Ramo).Select(r => r.Nome).FirstOrDefault(),
                        Categoria = db.Categoria.Where(c => c.Sigla == data.Categoria).Select(c => c.Nome).FirstOrDefault(),
                        Posto = db.Posto.Where(p => p.Código == data.Posto).Select(p => p.Nome).FirstOrDefault(),
                        Classe = data.Classe,
                        NIM = data.NIM
                    })
                    .FirstOrDefault();

                user.inqueritoDTO = db.Inquerito
                    .Where(guy => guy.CandidaturaID == candidatura.id)
                    .Select(data => new InqueritoDTO
                    {
                        SituacaoPai = db.Situacao.Where(s => s.ID == data.SituacaoPai).Select(s => s.Nome).FirstOrDefault(),
                        OutraPai = data.OutraPai,
                        SituacaoMae = db.Situacao.Where(s => s.ID == data.SituacaoMae).Select(s => s.Nome).FirstOrDefault(),
                        OutraMae = data.OutraMae,
                        ConhecimentoEscola = db.ConhecimentoEscola.Where(ce => ce.ID == data.ConhecimentoEscola).Select(ce => ce.Nome).FirstOrDefault(),
                        CandidatarOutros = data.CandidatarOutros,
                        Outro = data.Outro
                    })
                    .FirstOrDefault();

                user.certificado = db.Certificado.Where(c => c.CandidaturaID == candidatura.id).FirstOrDefault();

                user.cursosDTO = db.Opcoes
                    .Where(guy => guy.CandidaturaId == candidatura.id)
                    .Select(data => new UserCursoDTO
                    {
                        Nome = db.Curso.Where(c => c.ID == data.CursoId).Select(c => c.Nome).FirstOrDefault(),
                        Prioridade = data.Prioridade
                    })
                    .ToList();

                user.examesDTO = db.UserExame
                    .Where(guy => guy.CandidaturaId == candidatura.id)
                    .Select(data => new UserExameDTO
                    {
                        Codigo = db.Exame.Where(e => e.ID == data.ExameId).Select(e => e.Código).FirstOrDefault(),
                        Exame = db.Exame.Where(e => e.ID == data.ExameId).Select(e => e.Nome).FirstOrDefault()
                    })
                    .ToList();

                user.docsDTO = db.Documento
                    .Where(guy => guy.CandidaturaID == candidatura.id)
                    .Select(data => new DocsDTO
                    {
                        Nome = data.Nome,
                        Descricao = data.Descricao,
                        Tipo = data.Tipo,
                        UploadTime = data.UploadTime,
                        DocumentoBinario = db.DocumentoBinario.Where(doc => doc.DocID == data.ID).FirstOrDefault()
                    })
                    .ToList();

                ViewData["candidato"] = true;

                if (candidatura.Certificado == null)
                {
                    ViewData["candidato"] = false;
                }

                ViewData["mil"] = user.dadosDTO.Militar;

                return View(user);
            }
            else
            {
                return View("Error");
            }
        }

        public ActionResult DownloadDocumento(int id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                using (CandidaturasBOEntities dbModel = new CandidaturasBOEntities())
                {
                    try
                    {
                        Documento doc = dbModel.Documento.Where(dp => dp.ID == id).FirstOrDefault();
                        DocumentoBinario docbin = dbModel.DocumentoBinario.Where(dp => dp.DocID == id).FirstOrDefault();

                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.Cache.SetCacheability(System.Web.HttpCacheability.Private);
                        Response.ContentType = doc.Tipo;
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" + doc.Nome);
                        Response.BinaryWrite(docbin.DocBinario);
                        Response.Flush();
                        Response.End();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        throw raise;
                    }
                }
                return View("~/Views/Home/Welcome.cshtml");
            }
            else
            {
                return View("Error");
            }
        }

        public ActionResult DownloadFormulario(int id)
        {
            CandidaturasBOEntities db = new CandidaturasBOEntities();
            

            byte[] dForm = db.Certificado.Where(dp => dp.CandidaturaID == id).Select(dp => dp.FormBin).FirstOrDefault();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(System.Web.HttpCacheability.Private);
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + "ComprovativoCandidatura.pdf");
            Response.BinaryWrite(dForm);
            Response.Flush();
            Response.End();

            return View("~/Views/Home/Welcome.cshtml");
        }
    }
}