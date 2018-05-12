using System;
using System.Collections.Generic;

namespace DataAnalysis.Model
{
    public class Salesman : IEqualityComparer<Salesman>
    {
        public Salesman()
        {
            this.Sales = new List<Sale>();
        }

        public int SalesmanId { get; set; }

        public string Cpf { get; set; }

        public string Name { get; set; }

        public decimal Salary { get; set; }

        public ICollection<Sale> Sales { get; set; }

        public bool Equals(Salesman x, Salesman y)
        {
            return x.Cpf.Equals(y.Cpf, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(Salesman obj)
        {
            return obj.Cpf.GetHashCode();
        }
    }
}
