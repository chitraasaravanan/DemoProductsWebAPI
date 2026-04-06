using MediatR;
using DemoProductsWebAPI.Application.DTOs;

namespace DemoProductsWebAPI.Application.Auth.Commands
{
    public record AuthenticateCommand(string Username, string Password) : IRequest<AuthResultDto>;
}
