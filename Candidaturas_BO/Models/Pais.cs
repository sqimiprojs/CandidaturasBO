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
    using System.ComponentModel.DataAnnotations;

    public partial class Pais
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Campo Obrigat�rio")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo Obrigat�rio")]
        public string Sigla { get; set; }
    }
}