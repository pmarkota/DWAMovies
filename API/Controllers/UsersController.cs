using AutoMapper;
using DAL.BLModels;
using DAL.DALModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace API.Controllers
{
    public class UserLoginViewModel
    {
        public string Username { get; set; } 
        public string Password { get; set; } 
    }
    public class UserRegistrationViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        public int CountryOfResidenceId { get; set; }
    }
    public class ChangePasswordViewModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    
    public class UsersController : ControllerBase
    {
        private readonly DwaMoviesContext _db;
        private readonly IMapper _mapper;

        public UsersController(DwaMoviesContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _db.Users.ToListAsync();
            var blUsers = _mapper.Map<List<BLUser>>(users);
            return Ok(blUsers);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password, salt);


            var newUser = new User
            {
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Phone = model.Phone,
                CountryOfResidenceId = model.CountryOfResidenceId,
                PwdHash = hashedPassword,
                PwdSalt = salt,
                Email = model.Email

            };

            newUser.CreatedAt = DateTime.UtcNow;
            newUser.IsConfirmed = false;
            newUser.SecurityToken = Guid.NewGuid().ToString();
            newUser.DeletedAt = null;

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            string validationUrl = $"http://localhost:5102/api/users/validate-email?token={newUser.SecurityToken}";
            string emailBody = $"Please click the following link to validate your email: {validationUrl}";

            var notification = new Notification
            {
                ReceiverEmail = newUser.Email,
                Subject = "Email Validation",
                Body = emailBody,
                CreatedAt = DateTime.UtcNow
            };

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();

            try
            {
                var apiKey = "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";
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("petar.markota@gmail.com", "Petar");
                var subject = notification.Subject;

                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == notification.ReceiverEmail);

                var toName = user != null ? user.FirstName : "User";
                var to = new EmailAddress(notification.ReceiverEmail, toName);
                var plainTextContent = emailBody;
                var htmlContent = emailBody;
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);
                notification.SentAt = DateTime.Now;
            }
            catch (Exception e)
            {
                await Console.Out.WriteLineAsync(e.Message);

                throw;
            }

            return Ok("User registered successfully");
        }
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var username = User.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return BadRequest("Invalid password");
            }

            if (!BCrypt.Net.BCrypt.Verify(model.OldPassword, user.PwdHash))
            {
                return BadRequest("Invalid password");
            }

            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword, salt);

            user.PwdHash = hashedPassword;
            user.PwdSalt = salt;

            await _db.SaveChangesAsync();

            return Ok("Password changed successfully");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }   
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (user == null)
            {
                  return BadRequest("Invalid username or password");
            }
            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PwdHash))
            {
                return BadRequest("Invalid username or password");
            }
            //Create JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("E5554ABDCE25D6551BD5DFF2D8A38231");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    //Email
                    new Claim(ClaimTypes.Email, user.Email),
                    //Username
                    new Claim(ClaimTypes.NameIdentifier, user.Username),
                    new Claim("IsAllowed","true"),
                    //Role
                    new Claim(ClaimTypes.Role, user.IsConfirmed ? "verified" : "notverified"),
                    //Expiration
                    new Claim(ClaimTypes.Expiration, DateTime.UtcNow.AddHours(11).ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(11), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var usernameClaim = ((System.IdentityModel.Tokens.Jwt.JwtSecurityToken)token).Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }

        [HttpGet("validate-email")]
        public async Task<IActionResult> ValidateEmail(string token)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.SecurityToken == token);
            if (user == null)
            {
                return BadRequest("Invalid token");
            }

            user.IsConfirmed = true;
            user.SecurityToken = null;
            await _db.SaveChangesAsync();

            return Ok("Email validated successfully");
        }

        [HttpGet("test-claims")]
        [Authorize(Roles ="verified")]
        public IActionResult TestClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(claims);
        }
    }
}
