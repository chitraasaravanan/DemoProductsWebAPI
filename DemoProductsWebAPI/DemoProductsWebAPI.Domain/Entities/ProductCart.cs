using System;

namespace DemoProductsWebAPI.Domain.Entities
{
    public class ProductCart
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        // Navigation
        public Product? Product { get; set; }
    }
}
