//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Candidaturas_BO.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Documento
    {
        public int ID { get; set; }
        public int CandidaturaID { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Tipo { get; set; }
        public System.DateTime UploadTime { get; set; }
        public Nullable<System.DateTime> DataAualizacao { get; set; }
    
        public virtual Candidatura Candidatura { get; set; }
        public virtual DocumentoBinario DocumentoBinario { get; set; }
    }
}
