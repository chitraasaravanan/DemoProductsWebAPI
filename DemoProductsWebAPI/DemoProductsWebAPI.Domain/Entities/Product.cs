using System;

namespace DemoProductsWebAPI.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public Product() { }

        public Product(int id, string productName, string createdBy, DateTime createdOn)
        {
            Id = id;
            ProductName = productName;
            CreatedBy = createdBy;
            CreatedOn = createdOn;
        }
    }
}
