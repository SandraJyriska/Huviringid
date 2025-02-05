
using Huviringid_REST.Data.Repos;
using Huviringid_REST.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace Huviringid_REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentRegistrationsController(StudentRegistrationsRepo repo): ControllerBase
    {
        private readonly StudentRegistrationsRepo repo = repo;

        /// <summary>Leiab kõik registreeringud (õpilase ja huviringi seosed)</summary>
        /// <returns>Registreeringud</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            var result = await repo.GetAllRegistrations();
            return Ok(result);
        }

        /// <summary>Leiab registreeringu õpilase id ja huviringi id järgi</summary>
        /// <param name="studentId">õpilase id</param>
        /// <param name="extracurricularId">Huviringi id</param>
        /// <returns>Registreering</returns>
        [HttpGet("registration/{studentId}/{extracurricularId}")]
        public async Task<IActionResult> GetRegistration(int studentId, int extracurricularId)
        {
            var registration = await repo.GetRegistration(studentId, extracurricularId);
            return Ok(registration);
        }

        /// <summary>Loob uue registreeringu (õpilase ja huviringi seos)</summary>
        /// <param name="registration">Registreeringu objekt</param>
        /// <returns>Loodud registreering</returns>
        [HttpPost]
        public async Task<IActionResult> SaveRegistration([FromBody] StudentRegistration registration) {
            var registrationExists = await repo.RegistrationExistsInDb(registration.Id);
            if (registrationExists) {
                return Conflict();
            }
            var result = await repo.SaveRegistrationToDb(registration);

            if (result == null)
            {
                return Conflict(new { message = "This student is already registered for the selected extracurricular activity." });
            }
            return CreatedAtAction(nameof(SaveRegistration), new {registration.Id}, result);
        }

        /// <summary>Uuendab olemasolevat registreeringut</summary>
        /// <param name="id">Registreeringu id</param>
        /// <param name="registration">Registreeringu objekt</param>
        /// <returns>NoContent või NotFound</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StudentRegistration registration) {
            bool result = await repo.UpdateRegistration(id, registration);
            return result ? NoContent() : NotFound();
        }

        /// <summary>Kustutab registreeringu id järgi</summary>
        /// <param name="id">Registreeringu id</param>
        /// <returns>NoContent või NotFound</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await repo.DeleteRegistration(id);
            return result ? NoContent() : NotFound();
        }
    }
}