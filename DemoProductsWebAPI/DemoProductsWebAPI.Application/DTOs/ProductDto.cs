namespace DemoProductsWebAPI.Application.DTOs
{
    public class ProductDto
    {
        /// <summary>
        /// Product identifier
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Product display name
        /// </summary>
        public string ProductName { get; set; } = string.Empty;
        /// <summary>
        /// Creator username or identifier
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;
        /// <summary>
        /// UTC datetime when product was created
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// Last modifier username or identifier
        /// </summary>
        public string? ModifiedBy { get; set; }
        /// <summary>
        /// UTC datetime when product was last modified
        /// </summary>
        public DateTime? ModifiedOn { get; set; }
    }
}
