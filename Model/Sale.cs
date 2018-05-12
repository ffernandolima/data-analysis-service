using System.Collections.Generic;

namespace DataAnalysis.Model
{
    public class Sale
    {
        public Sale()
        {
            this.SaleItems = new List<SaleItem>();
        }

        public int SaleId { get; set; }

        public string SalesmanName { get; set; }

        public ICollection<SaleItem> SaleItems { get; set; }
    }
}
