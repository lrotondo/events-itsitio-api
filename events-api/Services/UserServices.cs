using AutoMapper;
using events_api.DTOs;
using events_api.Entities;
using events_api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static events_api.Utils.Constants;

namespace events_api.Services
{
    public class UserServices : ControllerBase, IUserServices
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;

        public UserServices(UserManager<ApplicationUser> userManager, IMapper mapper, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<ActionResult<AuthResponse>> SignIn(SignInDTO signInDTO)
        {
            var user = await userManager.FindByEmailAsync(signInDTO.Email);
            if (user == null) return BadRequest(BuildErrorResponse("EMAIL_NOT_REGISTERED", "Email no registrado"));

            var validCredentials = await userManager.CheckPasswordAsync(user, signInDTO.Password);
            if (!validCredentials) return BadRequest(BuildErrorResponse("INVALID_PASSWORD", "Credenciales no validas"));

            return await BuildToken(user);
        }

        public async Task<ActionResult<AuthResponse>> SignUp(SignUpDTO signUpDTO)
        {
            var user = mapper.Map<ApplicationUser>(signUpDTO);
            var result = await userManager.CreateAsync(user, signUpDTO.Password);
            if (!result.Succeeded) return BadRequest(BuildErrorResponse("SIGNUP_FAILED", String.Join(" ", result.Errors.Select(e => e.Description))));
            return await BuildToken(user);
        }

        public async Task<ApplicationUser> GetCurrentUser(ClaimsPrincipal principal)
        {
            var id = principal.Claims.FirstOrDefault(c => c.Type == CustomClaims.ID_CLAIM_TYPE)?.Value;
            if (id == null) return null;
            return await userManager.FindByIdAsync(id);
        }

        public string GetCurrentUserId(ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(c => c.Type == CustomClaims.ID_CLAIM_TYPE)?.Value;
        }

        private async Task<AuthResponse> BuildToken(ApplicationUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(CustomClaims.ID_CLAIM_TYPE, user.Id),
                new Claim(CustomClaims.EMAIL_CLAIM_TYPE, user.Email)
            };

            var claimsDB = await userManager.GetClaimsAsync(user);
            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("EVENTS_JWT_KEY")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var validDays = 3;
            var validHoursRefresh = 24;
            var expiration = DateTime.Now.AddDays(validDays);
            var refreshExpiration = DateTime.Now.AddHours(validHoursRefresh);

            var authToken = new JwtSecurityToken(issuer: null, audience: null, claims, expires: expiration, signingCredentials: credentials);
            var refreshToken = new JwtSecurityToken(issuer: null, audience: null, claims, expires: refreshExpiration, signingCredentials: credentials);

            var response = new AuthResponse();
            response.AuthToken.Token = new JwtSecurityTokenHandler().WriteToken(authToken);
            response.AuthToken.ExpiresIn = validDays * 24 * 60;
            response.RefreshToken.Token = new JwtSecurityTokenHandler().WriteToken(refreshToken);
            response.RefreshToken.ExpiresIn = response.AuthToken.ExpiresIn + validHoursRefresh * 60;
            response.TokenType = "Bearer";
            response.AuthState = mapper.Map<ApplicationUserPublic>(user);

            return response;
        }

        private AuthResponse BuildErrorResponse(string errorCode, string errorDescription)
        {
            return new AuthResponse()
            {
                Error = new AuthError()
                {
                    Code = errorCode,
                    Description = errorDescription
                }
            };
        }
    }
}
