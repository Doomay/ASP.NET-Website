using System.ComponentModel.DataAnnotations.Schema;

namespace Inleveropdracht.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPassword { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemViewModel>? OrderItems { get; set; }
        public List<Product>? Products { get; set; }
        public List<int> SelectedItemIds { get; set; }
        public decimal TotalPrice { get;set; }
        public Dictionary<int, int> IdCounts { get; set; }
    }

    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

    }

}
