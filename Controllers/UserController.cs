using AuthenticationPlugin;
using CinemaApi.Data;
using CinemaApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CinemaApi.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly CinemaDbContext _context;
		private IConfiguration _configuration;
		private readonly AuthService _auth;

		public UserController(CinemaDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
			_auth = new AuthService(_configuration);
		}

		[HttpPost]
		public IActionResult Register([FromBody]User user) //kayıt ol
		{
			var userwithsameEmail = _context.Users.Where(x => x.Email == user.Email).SingleOrDefault();
			if (userwithsameEmail != null)
			{
				return BadRequest("User with same email already exist");
			}

			var userObj = new User
			{
				Name = user.Name,
				Email = user.Email,
				Password = SecurePasswordHasherHelper.Hash(user.Password),
				Reservations = user.Reservations,
				Role = "Users"

			};
			_context.Users.Add(userObj);
			_context.SaveChanges();
			return StatusCode(StatusCodes.Status201Created);

		}
		[HttpPost]
		public IActionResult Login([FromBody] User user)
		{
			var userEmail = _context.Users.FirstOrDefault(x => x.Email == user.Email);
			if (userEmail == null)
			{
				return NotFound();
			}

			if (!SecurePasswordHasherHelper.Verify(user.Password, userEmail.Password))
			{
				return Unauthorized();
			}

			var claims = new[]
            {
                  new Claim(JwtRegisteredClaimNames.Email, user.Email),
                  new Claim(ClaimTypes.Email, user.Email),
				  new Claim(ClaimTypes.Role, userEmail.Role) //dstabaseden rol alımını yapacağız dedik.
            };

			var token = _auth.GenerateAccessToken(claims);

			return new ObjectResult(new
			{
				access_token = token.AccessToken,
				expires_in = token.ExpiresIn,
				token_type = token.TokenType,
				creation_Time = token.ValidFrom,
				expiration_Time = token.ValidTo,
				user_id = userEmail.Id
			});

		}


	}
}
