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
    using System.ComponentModel;

    public partial class Curso
    {
        public int ID { get; set; }
        public string Nome { get; set; }

        [DisplayName("C�digo Curso")]
        public string CodigoCurso { get; set; }

        [DisplayName("C�digo Ramo")]
        public string CodigoRamo { get; set; }
    }
}
