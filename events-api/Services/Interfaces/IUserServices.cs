using events_api.DTOs;
using events_api.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace events_api.Services.Interfaces
{
    public interface IUserServices
    {
        Task<ActionResult<AuthResponse>> SignIn(SignInDTO signInDTO);
        Task<ActionResult<AuthResponse>> SignUp(SignUpDTO signUpDTO);
        Task<ApplicationUser> GetCurrentUser(ClaimsPrincipal principal);
    }
}
