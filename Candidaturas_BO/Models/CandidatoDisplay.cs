using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Candidaturas_BO.Models
{
    public class CandidatoDisplay
    {
        public int userId { get; set; }
        public int numCand { get; set; }
        public String nome { get; set; }
        public String email { get; set; }
        public System.DateTime dataCandidatura { get; set; }
        public String edicao { get; set; }
    }
}