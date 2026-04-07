using DemoWebAPI.Core.DTOs;
using MediatR;

namespace DemoProductsWebAPI.Application.Auth.Commands
{
    public record AuthenticateCommand(string Username, string Password) : IRequest<AuthResultDto>;
}
