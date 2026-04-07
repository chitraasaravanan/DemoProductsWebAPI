using DemoProductsWebAPI.Domain.Entities;
using DemoProductsWebAPI.Infrastructure.Data;
using DemoProductsWebAPI.Infrastructure.Data.EFCoreRepositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace DemoProductsWebAPI.Tests.UnitTestcase
{
    /// <summary>
    /// Unit tests for <see cref="ProductRepository"/> to verify Entity Framework Core data access operations.
    /// Tests cover all CRUD operations (Create, Read, Update, Delete) on the Product entity.
    /// Uses in-memory database for isolated, fast test execution without external dependencies.
    /// </summary>
    public class ProductRepositoryTests
    {
        /// <summary>
        /// Helper method to create an isolated in-memory database context for each test.
        /// </summary>
        /// <param name="name">Unique database name to ensure complete test isolation</param>
        /// <returns>ApplicationDbContext configured with InMemoryDatabase provider</returns>
        private static ApplicationDbContext CreateInMemoryDb(string name)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(name)
                .Options;
            return new ApplicationDbContext(options);
        }

        /// <summary>
        /// Verifies that AddAsync successfully persists a new product to the database.
        /// Flow: Create product entity → Add to repository → Save changes → Retrieve and verify.
        /// </summary>
        [Fact]
        public async Task AddAsync_AddsProductToDatabase()
        {
            // Arrange: Create in-memory database and initialize repository
            using var db = CreateInMemoryDb(nameof(AddAsync_AddsProductToDatabase));
            var repo = new ProductRepository(db);
            var product = new Product { ProductName = "Test Product", CreatedBy = "test", CreatedOn = DateTime.UtcNow };

            // Act: Add product to repository and persist to database
            await repo.AddAsync(product, CancellationToken.None);
            await db.SaveChangesAsync();

            // Assert: Fetch product and verify all fields persisted correctly
            var fetched = await repo.GetByIdAsync(product.Id, CancellationToken.None);
            fetched.Should().NotBeNull();
            fetched!.ProductName.Should().Be("Test Product");
            fetched.CreatedBy.Should().Be("test");
        }

        /// <summary>
        /// Verifies that GetByIdAsync retrieves an existing product by ID.
        /// Flow: Insert product into database → Call GetByIdAsync → Assert product returned with correct data.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ReturnsProduct_WhenExists()
        {
            // Arrange: Create database with a seeded product entity
            using var db = CreateInMemoryDb(nameof(GetByIdAsync_ReturnsProduct_WhenExists));
            var product = new Product { ProductName = "Existing", CreatedBy = "test", CreatedOn = DateTime.UtcNow };
            db.Products.Add(product);
            await db.SaveChangesAsync();
            var repo = new ProductRepository(db);

            // Act: Retrieve product by ID
            var result = await repo.GetByIdAsync(product.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(product.Id);
            result.ProductName.Should().Be("Existing");
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            using var db = CreateInMemoryDb(nameof(GetByIdAsync_ReturnsNull_WhenNotExists));
            var repo = new ProductRepository(db);

            // Act
            var result = await repo.GetByIdAsync(999, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllProducts()
        {
            // Arrange
            using var db = CreateInMemoryDb(nameof(GetAllAsync_ReturnsAllProducts));
            db.Products.AddRange(
                new Product { ProductName = "Product1", CreatedBy = "test", CreatedOn = DateTime.UtcNow },
                new Product { ProductName = "Product2", CreatedBy = "test", CreatedOn = DateTime.UtcNow }
            );
            await db.SaveChangesAsync();
            var repo = new ProductRepository(db);

            // Act
            var result = await repo.GetAllAsync(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoProducts()
        {
            // Arrange
            using var db = CreateInMemoryDb(nameof(GetAllAsync_ReturnsEmptyList_WhenNoProducts));
            var repo = new ProductRepository(db);

            // Act
            var result = await repo.GetAllAsync(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task AddRangeAsync_AddsMultipleProducts()
        {
            // Arrange
            using var db = CreateInMemoryDb(nameof(AddRangeAsync_AddsMultipleProducts));
            var repo = new ProductRepository(db);
            var products = Enumerable.Range(1, 5)
                .Select(i => new Product { ProductName = $"Product{i}", CreatedBy = "test", CreatedOn = DateTime.UtcNow })
                .ToList();

            // Act
            await repo.AddRangeAsync(products, CancellationToken.None);
            await db.SaveChangesAsync();

            // Assert
            var result = await repo.GetAllAsync(CancellationToken.None);
            result.Should().HaveCount(5);
        }

        [Fact]
        public void Update_UpdatesExistingProduct()
        {
            // Arrange
            using var db = CreateInMemoryDb(nameof(Update_UpdatesExistingProduct));
            var product = new Product { ProductName = "Original", CreatedBy = "test", CreatedOn = DateTime.UtcNow };
            db.Products.Add(product);
            db.SaveChanges();
            var repo = new ProductRepository(db);

            // Act
            product.ProductName = "Updated";
            repo.Update(product);
            db.SaveChanges();

            // Assert
            var updated = db.Products.Find(product.Id);
            updated.Should().NotBeNull();
            updated!.ProductName.Should().Be("Updated");
        }

        [Fact]
        public void Remove_DeletesProduct()
        {
            // Arrange
            using var db = CreateInMemoryDb(nameof(Remove_DeletesProduct));
            var product = new Product { ProductName = "ToDelete", CreatedBy = "test", CreatedOn = DateTime.UtcNow };
            db.Products.Add(product);
            db.SaveChanges();
            var repo = new ProductRepository(db);

            // Act
            repo.Remove(product);
            db.SaveChanges();

            // Assert
            var deleted = db.Products.Find(product.Id);
            deleted.Should().BeNull();
        }
    }
}
