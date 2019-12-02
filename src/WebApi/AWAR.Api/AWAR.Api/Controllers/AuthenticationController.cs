namespace AWAR.Api.Controllers
{
    using AWAR.Data.Models.User;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;

        public AuthenticationController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Register(ApplicationUserRegisterModel model)
        {
            var applicationUser = new ApplicationUser()
            {
                UserName = model.Username,
                Email = model.Email,
            };

            try
            {
                var result = await this.userManager.CreateAsync(applicationUser, model.Password);
                if (result.Succeeded)
                {
                    await this.userManager.AddToRoleAsync(applicationUser, "User");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Login(ApplicationUserLoginModel model)
        {
            var user = await this.userManager.FindByNameAsync(model.UserName);

            IdentityOptions options = new IdentityOptions();

            if (user != null && await this.userManager.CheckPasswordAsync(user, model.Password))
            {
                var role = await this.userManager.GetRolesAsync(user);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID", user.Id.ToString()),
                        new Claim(options.ClaimsIdentity.RoleClaimType, role.FirstOrDefault())
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234567890123456")), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);

                return Ok(new { token });
            }
            else
            {
                return this.BadRequest(new { message = "Username or password is incorrect." });
            }
        }
    }
}