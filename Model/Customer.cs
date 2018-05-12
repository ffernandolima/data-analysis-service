
using System;
using System.Collections.Generic;

namespace DataAnalysis.Model
{
    public class Customer : IEqualityComparer<Customer>
    {
        public int CustomerId { get; set; }

        public string Cnpj { get; set; }

        public string Name { get; set; }

        public string BusinessArea { get; set; }

        public bool Equals(Customer x, Customer y)
        {
            return x.Cnpj.Equals(y.Cnpj, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(Customer obj)
        {
            return obj.Cnpj.GetHashCode();
        }
    }
}
