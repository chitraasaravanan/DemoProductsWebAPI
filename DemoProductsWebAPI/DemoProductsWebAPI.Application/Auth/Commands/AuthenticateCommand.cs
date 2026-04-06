using MediatR;
using DemoWebAPI.Core.DTOs;

namespace DemoProductsWebAPI.Application.Auth.Commands
{
    public record AuthenticateCommand(string Username, string Password) : IRequest<AuthResultDto>;
}
