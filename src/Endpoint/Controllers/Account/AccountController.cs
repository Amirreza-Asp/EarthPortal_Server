using Application;
using Application.Contracts.Persistence.Repositories;
using Application.CQRS.Account;
using Application.Models;
using Domain;
using Domain.Entities.Account;
using Endpoint.CustomeAttributes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Endpoint.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Role> _roleRepo;

        public AccountController(IMediator mediator, IRepository<User> userRepo, IRepository<Role> roleRepo)
        {
            _mediator = mediator;
            _userRepo = userRepo;
            _roleRepo = roleRepo;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<CommandResponse> Login([FromBody] LoginCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);

        [HttpGet]
        [Route("Logout")]
        public CommandResponse Logout()
        {
            HttpContext.Response.Cookies.Delete(SD.AuthToken);
            return CommandResponse.Success();
        }

        [HttpGet]
        [Route("Profile")]
        [Authorize]
        public async Task<CommandResponse> Profile(CancellationToken cancellationToken)
        {
            var userName = User.Claims.First(b => b.Type == AppClaims.UserName)?.Value;

            if (userName == null)
                return CommandResponse.Failure(400, "کاربر به سیستم وارد نشده");

            var user = await _userRepo.FirstOrDefaultAsync(b => b.UserName == userName, include: source => source.Include(b => b.Role), cancellationToken);

            if (userName == null)
                return CommandResponse.Failure(400, "کاربر در سیستم وجود ندارد");

            return CommandResponse.Success(new { userName = user.UserName, name = user.Name + " " + user.Family, role = user.Role?.Title, roleDisplay = user.Role?.Display, contentEdit = user.EnableContentEdit });
        }

        [HttpPut]
        [Authorize]
        [Route("[action]")]
        public async Task<CommandResponse> ToggleEditContent(CancellationToken cancellationToken) =>
            await _mediator.Send(new ToggleEditContentCommand(), cancellationToken);


        [HttpGet]
        [Route("[action]")]
        [AccessControl("Admin")]
        public async Task<List<Role>> Roles(CancellationToken cancellationToken) =>
            await _roleRepo.GetAllAsync<Role>(cancellationToken: cancellationToken);
    }
}
