using System;
using Xunit;
using FluentAssertions;
using DemoProductsWebAPI.Infrastructure.Data.Repositories;
using DemoProductsWebAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DemoProductsWebAPI.Domain.Entities;
using System.Threading;
using System.Linq;

namespace DemoProductsWebAPI.Tests.UnitTestcase
{
    public class ProductRepositoryUnitTestcase
    {
        private static ApplicationDbContext CreateDb(string name)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(name)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async System.Threading.Tasks.Task AddAndGetById_Works()
        {
            using var db = CreateDb("repo_add_get");
            var repo = new ProductRepository(db);
            var p = new Product { ProductName = "r1", CreatedBy = "t", CreatedOn = DateTime.UtcNow };
            await repo.AddAsync(p, CancellationToken.None);
            await db.SaveChangesAsync();

            var fetched = await repo.GetByIdAsync(p.Id);
            fetched.Should().NotBeNull();
            fetched!.ProductName.Should().Be("r1");
        }

        [Fact]
        public async System.Threading.Tasks.Task GetAll_Returns_List()
        {
            using var db = CreateDb("repo_get_all");
            db.Products.Add(new Product { ProductName = "p1", CreatedBy = "t", CreatedOn = DateTime.UtcNow });
            await db.SaveChangesAsync();
            var repo = new ProductRepository(db);
            var list = await repo.GetAllAsync();
            list.Should().NotBeNull();
            list.Should().HaveCountGreaterOrEqualTo(1);
        }

        [Fact]
        public async System.Threading.Tasks.Task AddRange_Works()
        {
            using var db = CreateDb("repo_add_range");
            var repo = new ProductRepository(db);
            var items = Enumerable.Range(1, 5).Select(i => new Product { ProductName = "b" + i, CreatedBy = "t", CreatedOn = DateTime.UtcNow }).ToList();
            await repo.AddRangeAsync(items, CancellationToken.None);
            await db.SaveChangesAsync();
            var list = await repo.GetAllAsync();
            list.Should().HaveCount(5);
        }
    }
}
