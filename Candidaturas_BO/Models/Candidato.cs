using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Candidaturas_BO.Models
{
    public class Candidato
    {
        public User User { get; set; }
        public DadosPessoais DadosPessoais {get; set;}
        public Inquerito Inquerito { get; set; }
        public List<CursoDisplay> UserCursos { get; set; }
        public List<string> UserExames { get; set; }
    }
}