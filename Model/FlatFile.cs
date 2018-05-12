
using System.Collections.Generic;
using System.Linq;

namespace DataAnalysis.Model
{
    public class FlatFile
    {
        public FlatFile()
        {
            this.Salesmans = new List<Salesman>();
            this.Customers = new List<Customer>();
            this.Sales = new List<Sale>();
        }

        public ICollection<Salesman> Salesmans { get; set; }
        public ICollection<Customer> Customers { get; set; }
        public ICollection<Sale> Sales { get; set; }

        public int GetAmountOfclients()
        {
            return this.Customers.Count;
        }

        public int GetAmountOfSalesman()
        {
            return this.Salesmans.Count;
        }

        public int GetMostExpensiveSaleId()
        {

            var summary = from sale in this.Sales
                          group sale by sale.SaleId
                              into grouping
                              select new
                              {
                                  SaleId = grouping.Key,
                                  SalePrice = grouping.SelectMany(x => x.SaleItems).Sum(s => s.FinalPrice)
                              };

            var expensiveSale = summary.OrderByDescending(x => x.SalePrice).FirstOrDefault();

            return expensiveSale != null ? expensiveSale.SaleId : 0;
        }

        public string GetWorstSalesmanName()
        {
            var summary = from saleman in this.Salesmans
                          from sale in saleman.Sales
                          group sale by new { saleman.Cpf, saleman.Name, sale.SaleId }
                              into grouping
                              select new
                              {
                                  SalesmanName = grouping.Key.Name,
                                  SaleId = grouping.Key.SaleId,
                                  SalePrice = grouping.SelectMany(x => x.SaleItems).Sum(s => s.FinalPrice)
                              };

            var cheapSale = summary.OrderBy(x => x.SalePrice).FirstOrDefault();

            return cheapSale != null ? cheapSale.SalesmanName : string.Empty;
        }

        public bool HasData()
        {
            return this.Salesmans.Any() || this.Customers.Any() || this.Sales.Any();
        }
    }
}
