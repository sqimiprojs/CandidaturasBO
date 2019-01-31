using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Candidaturas_BO.Models
{
    public class CandidatoDTO
    {
        public UserFull user { get; set; }
        public Candidato candidato { get; set; }
    }
}