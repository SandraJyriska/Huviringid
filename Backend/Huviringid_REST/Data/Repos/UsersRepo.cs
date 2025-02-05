using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Huviringid_REST.Models.Classes;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Huviringid_REST.Data.Repos
{
    public class UsersRepo
    {
        private readonly DataContext _context;
        private IConfiguration _config;

        public UsersRepo(IConfiguration config, DataContext context)
        {
            _config = config;
            _context = context;
        }

        public async Task<string> Login([FromBody] User login)
        {
            var dbUser = await _context.Users!.FirstOrDefaultAsync(user => user.Username == login.Username);

            if (dbUser == null || dbUser.Password != HashPassword(login.Password)){
                return "";
            }
            login.Role = dbUser.Role;
            return GenerateJSONWebToken(login);

        }

        public async Task<User?> GetUserByUsername(string userName)
        {
            var dbUser = await _context.Users!.FirstOrDefaultAsync(user => user.Username == userName);
            return dbUser;
        }

        private string HashPassword(string password)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: [],
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashed;
        }

        private string GenerateJSONWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                new List<Claim> 
                { 
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("exp", DateTime.UtcNow.AddMinutes(10).ToString()) 
                },
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<List<User>> GetAllUsers() {
            IQueryable<User> query = _context.Users.AsQueryable();
            return await query.ToListAsync();
        }

        public async Task<User> SaveUserToDb(User user) {

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(s => s.Username == user.Username);

            if (existingUser != null)
            {
                return existingUser;
            }

            user.Password = HashPassword(user.Password);

            _context.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UserExistsInDb(int id) => await _context.Users.AnyAsync(x => x.Id == id);

    }

}
