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
    
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.Candidatura = new HashSet<Candidatura>();
        }
    
        public int ID { get; set; }
        public byte[] Password { get; set; }
        public string Email { get; set; }
        public System.DateTime DataCriacao { get; set; }
        public string NomeColoquial { get; set; }
        public System.DateTime DataNascimento { get; set; }
        public int TipoDocID { get; set; }
        public string NDI { get; set; }
        public System.DateTime DocumentoValidade { get; set; }
        public bool Militar { get; set; }
        public string Edicao { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Candidatura> Candidatura { get; set; }
        public virtual Edicao Edicao1 { get; set; }
        public virtual Militar Militar1 { get; set; }
    }
}
