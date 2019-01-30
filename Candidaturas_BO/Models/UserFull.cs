using System.Collections.Generic;

namespace Candidaturas_BO.Models
{
    public class UserFull
    {
        public User User { get; set; }
        public DadosPessoais DadosPessoais { get; set; }
        public Inquerito Inquerito { get; set; }
        public List<CursoDisplay> UserCursos { get; set; }
        public List<string> UserExames { get; set; }
        public Candidato Candidato { get; set; }
        public List<DocModel> UserDocs { get; set; }
    }
}