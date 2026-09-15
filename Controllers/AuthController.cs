using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.Models.DTO;
using NZWalks.Repositories;

namespace NZWalks.Controllers
{
    [Route("api/[controller]")]                
    [ApiController]
    public class AuthController : ControllerBase  
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }

        // POST: /api/Auth/Register                      
        [HttpPost]
        [Route("Register")]                                                                                                                                                                                                                                                     

        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)

        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
                
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);  // create a new user in database


            if (identityResult.Succeeded)                                     
            {
                // Add roles this user

                if(registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                {
                  identityResult =   await userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles); // asssign a role to that user

                    if (identityResult.Succeeded)
                    {
                        return Ok("User is registered successfully please proceed to Login");
                    }
                }
            }
            return BadRequest ("Something went wrong");
        }
    
    
        // POST: /api/Auth/Login

        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login ([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Username);

            if (user != null)
            {
                var checkPasswordResult = await userManager.CheckPasswordAsync(user,loginRequestDto.Password);

                if (checkPasswordResult)
                {
                    // Get the roles for this user

                    var roles = await userManager.GetRolesAsync(user);

                    if (roles != null)
                    {
                        // Create Token

                        var jwtToken = tokenRepository.CreateJwtToken(user,roles.ToList()); 

                        // creating a var so that we dont have to send in Ok() huge data
                        // we will just send return Ok(response) everthing other data like Email n all will come under response variable for which i have created separate file as LoginResponseDto

                        var response = new LoginResponseDto
                        {
                            JwtToken = jwtToken
                        }; 

                        return Ok(response);
                    }
              
                }
            }

            return BadRequest("Username or Password is incorrect");
        }


    
    
    }
}
