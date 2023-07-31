using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Questao5.Domain.Entities
{
    public class ContaCorrente
    {
        public string IdContaCorrente { get; set; }
        public int Numero { get; set; }
        public string Nome { get; set; }
        public Boolean Ativo { get; set; }

        public List<string> GetTipoMovimento()
        {
            var list = new Dictionary<string, string>();
            list.Add("C", "Crédito");
            list.Add("D", "Débito");
            return list.Keys.ToList();
        }
    }
}
