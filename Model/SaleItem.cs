
namespace DataAnalysis.Model
{
    public class SaleItem
    {
        public int SaleItemId { get; set; }

        public int SaleId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal FinalPrice
        {
            get
            {
                return this.Quantity * this.Price;
            }
        }
    }
}
