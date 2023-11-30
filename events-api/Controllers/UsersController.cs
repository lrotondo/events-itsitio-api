using events_api.DTOs;
using events_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace events_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices userServices;

        public UsersController(IUserServices userServices)
        {
            this.userServices = userServices;
        }

        [HttpPost("auth/sign-in")]
        public async Task<ActionResult<AuthResponse>> SignIn([FromBody] SignInDTO signInDTO)
        {
            return await userServices.SignIn(signInDTO);
        }

        [HttpPost("auth/sign-up")]
        public async Task<ActionResult<AuthResponse>> SignUp([FromBody] SignUpDTO signUpDTO)
        {
            return await userServices.SignUp(signUpDTO);
        }
    }
}
