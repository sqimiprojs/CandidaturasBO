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
    
    public partial class DocumentoBinario
    {
        public int DocID { get; set; }
        public byte[] DocBinario { get; set; }
    
        public virtual Documento Documento { get; set; }
    }
}
