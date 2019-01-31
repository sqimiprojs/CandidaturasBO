﻿using Candidaturas_BO.Models;
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
                List<Candidato> candidatos = new List<Candidato>();

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
                CandidatoDTO candidato = new CandidatoDTO();

                candidato.user.email = db.User.Where(guy => guy.ID == id).Select(guy => guy.Email).FirstOrDefault();

                candidato.user.dadosDTO = db.DadosPessoais
                    .Where(guy => guy.UserId == id)
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
                        Nacionalidade = data.Nacionalidade,
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
                        Ramo = data.Ramo,
                        Categoria = data.Categoria,
                        Posto = db.Posto.Where(p => p.Código == data.Posto).Select(p => p.Nome).FirstOrDefault(),
                        Classe = data.Classe,
                        NIM = data.NIM
                    })
                    .FirstOrDefault();

                candidato.user.inqueritoDTO = db.Inquerito
                    .Where(guy => guy.UserId == id)
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

                candidato.user.cursosDTO = db.UserCurso
                    .Where(guy => guy.UserId == id)
                    .Select(data => new UserCursoDTO
                    {
                        Nome = db.Curso.Where(c => c.ID == data.CursoId).Select(c => c.Nome).FirstOrDefault(),
                        Prioridade = data.Prioridade
                    })
                    .ToList();

                candidato.user.examesDTO = db.UserExame
                    .Where(guy => guy.UserId == id)
                    .Select(data => new UserExameDTO
                    {
                        Codigo = db.Exame.Where(e => e.ID == data.ExameId).Select(e => e.Código).FirstOrDefault(),
                        Exame = db.Exame.Where(e => e.ID == data.ExameId).Select(e => e.Nome).FirstOrDefault()
                    })
                    .ToList();

                candidato.user.docsDTO = db.Documento
                    .Where(guy => guy.UserID == id)
                    .Select(data => new DocsDTO
                    {
                        Nome = data.Nome,
                        Descricao = data.Descricao,
                        Tipo = data.Tipo,
                        DocumentoBinario = db.DocumentoBinario.Where(doc => doc.DocID == data.ID).FirstOrDefault()
                    })
                    .ToList();

                candidato.candidato = db.Candidato
                    .Where(guy => guy.UserID == id)
                    .FirstOrDefault();

                return View(candidato);
            }
            else
            {
                return View("Error");
            }
        }
    }
}