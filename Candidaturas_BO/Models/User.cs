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
            this.Inquerito = new HashSet<Inquerito>();
            this.UserExame = new HashSet<UserExame>();
            this.UserCurso = new HashSet<UserCurso>();
            this.UserDocumento = new HashSet<UserDocumento>();
        }
    
        public int ID { get; set; }
        public string NomeCompleto { get; set; }
        public string Password { get; set; }
        public string NDI { get; set; }
        public bool Militar { get; set; }
        public System.DateTime DataNascimento { get; set; }
        public string Email { get; set; }
        public string LoginErrorMessage { get; set; }
        public string TipoDocID { get; set; }
        public System.DateTime DataCriacao { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Inquerito> Inquerito { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserExame> UserExame { get; set; }
        public virtual DadosPessoais DadosPessoais { get; set; }
        public virtual TipoDocumentoID TipoDocumentoID { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserCurso> UserCurso { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserDocumento> UserDocumento { get; set; }
    }
}
