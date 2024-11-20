using E_Learning_Course.Data.Entities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private User? _user;

        public AuthenticateService(
            UserManager<User> userManager,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> RegisterUser(UserForRegistration userForRegistration)
        {
            var user = new User
            {
                UserName = userForRegistration.UserName,
                FirstName = userForRegistration.UserName,
                Email = userForRegistration.Email,
                PhoneNumber = userForRegistration.PhoneNumber,
                DateOfBirth = userForRegistration.DateOfBirth,
                CreatedAt = DateTime.UtcNow,   
                UpdatedAt = DateTime.UtcNow   
            };

            // Create the user in the system
            var creationResult = await _userManager.CreateAsync(user, userForRegistration.Password);
            if (!creationResult.Succeeded)
                return creationResult;

            // Find the creator (CreatedBy) by email
            var creatorUser = await _userManager.FindByEmailAsync(userForRegistration.Email);
            if (creatorUser == null)
                return IdentityResult.Failed(new IdentityError { Description = "Creator user not found." });

            // Set the CreatedBy and UpdatedBy fields using the creator's ID
            if (!Guid.TryParse(creatorUser.Id, out Guid creatorId))
                return IdentityResult.Failed(new IdentityError { Description = "Invalid creator user ID format." });

            user.CreatedBy = creatorId;
            user.UpdatedBy = creatorId;

            // Update the user with the new fields
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return updateResult;

            // Ensure the "Customer" role exists, and if not, create it
            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                var roleCreationResult = await _roleManager.CreateAsync(new IdentityRole("Customer"));
                if (!roleCreationResult.Succeeded)
                    return roleCreationResult;
            }

            // Assign the user to the "Customer" role
            var roleAssignmentResult = await _userManager.AddToRoleAsync(user, "Customer");
            if (!roleAssignmentResult.Succeeded)
                return roleAssignmentResult;

            return IdentityResult.Success;
        }
        public async Task<bool> ValidateUser(UserForAuthentication userForAuth)
        {
            _user = await _userManager.FindByEmailAsync(userForAuth.Email);
            var result = (_user != null && await _userManager.CheckPasswordAsync(_user,
           userForAuth.Password));
            return result;
        }
        public async Task<string> CreateToken()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:secretKey"]);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
             {
             new Claim(ClaimTypes.Name, _user.UserName),
             new Claim(ClaimTypes.NameIdentifier, _user.Id),
             new Claim("FirstName", _user.FirstName ?? ""),         
             new Claim("LastName", _user.LastName ?? ""),
             new Claim("Avatar", _user.Avatar ?? "")
             };
            var roles = await _userManager.GetRolesAsync(_user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }


        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials,
        List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var tokenOptions = new JwtSecurityToken
            (
            issuer: jwtSettings["validIssuer"],
            audience: jwtSettings["validAudience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
            signingCredentials: signingCredentials
            );
            return tokenOptions;
        }

        public async Task<bool> IsEmailConfirmed(string email)
        {
            var _user = await _userManager.FindByEmailAsync(email);
            return await _userManager.IsEmailConfirmedAsync(_user);
        }

        public async Task<IList<string>> GetUserRoles(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                return await _userManager.GetRolesAsync(user); // Return the list of roles
            }

            return new List<string>(); // Return an empty list if the user is not found
        }
    }
}
