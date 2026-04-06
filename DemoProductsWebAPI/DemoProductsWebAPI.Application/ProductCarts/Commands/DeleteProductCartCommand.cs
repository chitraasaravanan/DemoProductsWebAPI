using MediatR;

namespace DemoProductsWebAPI.Application.ProductCarts.Commands
{
    public record DeleteProductCartCommand(int Id) : IRequest<bool>;
}
