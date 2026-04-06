using MediatR;
using DemoProductsWebAPI.Application.DTOs;

namespace DemoProductsWebAPI.Application.Products.Notifications
{
    public record ProductCreatedNotification(ProductDto Product) : INotification;
}
