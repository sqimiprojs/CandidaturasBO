using System;
using System.Collections.Generic;

namespace Candidaturas_BO.Models
{  
    [Serializable]
    public class UserFull
    {
        public string email { get; set; }
        public DadosPessoaisDTO dadosDTO;
        public InqueritoDTO inqueritoDTO;
        public List<UserCursoDTO> cursosDTO;
        public List<UserExameDTO> examesDTO;
        public List<DocsDTO> docsDTO;
    }
    [Serializable]
    public class DadosPessoaisDTO
    {
        public string NomeColoquial { get; set; }
        public string Nomes { get; set; }
        public string Apelidos { get; set; }
        public string NomePai { get; set; }
        public string NomeMae { get; set; }
        public string NDI { get; set; }
        public string TipoDocID { get; set; }
        public System.DateTime DocumentoValidade { get; set; }
        public string Genero { get; set; }
        public string EstadoCivil { get; set; }
        public string Nacionalidade { get; set; }
        public string DistritoNatural { get; set; }
        public string ConcelhoNatural { get; set; }
        public string FreguesiaNatural { get; set; }
        public string Morada { get; set; }
        public string Localidade { get; set; }
        public string RepFinNIF { get; set; }
        public string CCDigitosControlo { get; set; }
        public string NSegSoc { get; set; }
        public string NIF { get; set; }
        public string DistritoMorada { get; set; }
        public string ConcelhoMorada { get; set; }
        public string FreguesiaMorada { get; set; }
        public string Telefone { get; set; }
        public Nullable<short> CodigoPostal4Dig { get; set; }
        public Nullable<short> CodigoPostal3Dig { get; set; }
        public System.DateTime DataCriacao { get; set; }
        public System.DateTime DataUltimaAtualizacao { get; set; }
        public System.DateTime DataNascimento { get; set; }
        public bool Militar { get; set; }
        public string Ramo { get; set; }
        public string Categoria { get; set; }
        public string Posto { get; set; }
        public string Classe { get; set; }
        public string NIM { get; set; }
    }
    [Serializable]
    public class InqueritoDTO
    {
        public string SituacaoPai { get; set; }
        public string OutraPai { get; set; }
        public string SituacaoMae { get; set; }
        public string OutraMae { get; set; }
        public string ConhecimentoEscola { get; set; }
        public string Outro { get; set; }
        public bool CandidatarOutros { get; set; }
    }
    [Serializable]
    public class UserCursoDTO
    {
        public string Nome { get; set; }
        public int Prioridade { get; set; }
    }
    [Serializable]
    public class UserExameDTO
    {
        public int Codigo { get; set; }
        public string Exame { get; set; }
    }
    [Serializable]
    public class DocsDTO
    {
        public String Nome { get; set; }
        public String Descricao { get; set; }
        public String Tipo { get; set; }
        public DocumentoBinario DocumentoBinario { get; set; }
    }
}