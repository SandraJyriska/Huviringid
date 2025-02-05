using Huviringid_REST.Data.Repos;
using Huviringid_REST.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace Huviringid_REST.Controllers
{
    [Route("api/[controller]")]
    public class UsersController(UsersRepo repo) : ControllerBase
    {
        private readonly UsersRepo repo = repo;

        /// <summary>Logib kasutaja sisse</summary>
        /// <param name="login">Kasutaja</param>
        /// <returns>Kasutaja tokeni, rolli ja user id</returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] User login)
        {
            var token = await repo.Login(login);
            var user = await repo.GetUserByUsername(login.Username);

            if (!string.IsNullOrEmpty(token) && user != null)
            {
                return Ok(new { Token = token, Role = user.Role, UserId = user.Id });
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>Kontrollib, kas kasutajanimi eksisteerib</summary>
        /// <param name="username">kasutajanimi</param>
        /// <returns>True või false</returns>
        [HttpGet("Username-Check/{username}")]
        public async Task<bool> UsernameExists(string username) 
        {
            var result = await repo.GetAllUsers();
            var usernameExists = result.Any(x => x.Username == username);
            return usernameExists;
        }

        /// <summary>Loob uue kasutaja</summary>
        /// <param name="user">kasutaja</param>
        /// <returns>Loodud kasutaja</returns>
        [HttpPost]
        public async Task<IActionResult> SaveUser([FromBody] User user) {
            var userExists = await repo.UserExistsInDb(user.Id);
            if (userExists) {
                return Conflict();
            }
            var result = await repo.SaveUserToDb(user);
            return CreatedAtAction(nameof(SaveUser), new {user.Id}, result);
        }

    }
}