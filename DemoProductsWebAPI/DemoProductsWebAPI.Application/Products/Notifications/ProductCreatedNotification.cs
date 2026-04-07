using DemoProductsWebAPI.Common.DTOs;
using MediatR;

namespace DemoProductsWebAPI.Application.Products.Notifications
{
    public record ProductCreatedNotification(ProductDto Product) : INotification;
}
