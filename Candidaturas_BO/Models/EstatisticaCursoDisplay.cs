using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Candidaturas_BO.Models
{
    public class EstatisticaCursoDisplay
    {
        public string Edicao { get; set; }
        public string Nome { get; set; }
        public int Total { get; set; }
        public double Percentagem { get; set; }
    }
}