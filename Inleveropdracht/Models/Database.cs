using Inleveropdracht.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inleveropdracht.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPassword { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public bool RememberMe { get; set; }
        public bool IsAdminClass { get; set; }
        public int Points { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
    }

    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public bool IsFinished { get; set; }
        public DateTime FinishedAt { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Add a CustomerId property to establish the relationship
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }


    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string ProductDescription { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int SalePercentage { get; set; }
        public int Quantity { get; set; }
    }
}


public class OrderItem
{
    public int OrderItemId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public Product Product { get; set; }
}
