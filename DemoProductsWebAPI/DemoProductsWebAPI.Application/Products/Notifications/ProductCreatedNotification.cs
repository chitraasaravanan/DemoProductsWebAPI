using MediatR;
using DemoProductsWebAPI.Common.DTOs;

namespace DemoProductsWebAPI.Application.Products.Notifications
{
    public record ProductCreatedNotification(ProductDto Product) : INotification;
}
