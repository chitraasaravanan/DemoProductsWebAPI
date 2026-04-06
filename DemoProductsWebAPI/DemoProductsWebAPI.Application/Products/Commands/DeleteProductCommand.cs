using MediatR;

namespace DemoProductsWebAPI.Application.Products.Commands
{
    public record DeleteProductCommand(int Id) : IRequest<bool>;
}
