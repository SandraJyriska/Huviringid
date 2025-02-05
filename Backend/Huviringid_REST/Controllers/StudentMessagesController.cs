using Huviringid_REST.Data.Repos;
using Huviringid_REST.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace Huviringid_REST.Controllers
{
    [Route("api/[controller]")]
    public class StudentMessagesController(StudentMessagesRepo repo, StudentRegistrationsRepo regRepo) : ControllerBase
    {
        private readonly StudentMessagesRepo repo = repo;
        private readonly StudentRegistrationsRepo regRepo = regRepo;

        /// <summary>Leiab kõik sõnumid</summary>
        /// <returns>Nimekiri sõnumistest</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            var result = await repo.GetAllStudentMessages();
            return Ok(result);
        }

        /// <summary>Leiab sõnumi id järgi</summary>
        /// <param name="id">Sõnumi id</param>
        /// <returns>Sõnum</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentMessage(int id) {
            var studentMessage = await repo.GetStudentMessageById(id);
            if (studentMessage == null) {
                return NotFound();
            }
            return Ok(studentMessage);
        }

        /// <summary>Leiab konkreetse õpilase kõik sõnumid</summary>
        /// <param name="studentId">Õpilase id</param>
        /// <returns>Nimekiri sõnumitest</returns>
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetMessagesByStudentId(int studentId)
        {
            var messages = await repo.GetMessagesByStudentId(studentId);
            return Ok(messages);
        }

        /// <summary>Loob uue sõnumi</summary>
        /// <param name="studentMessage">Sõnum</param>
        /// <returns>Loodud sõnum</returns>
        [HttpPost]
        public async Task<IActionResult> SaveStudentMessage([FromBody] StudentMessage studentMessage)
        {
            var result = await repo.SaveStudentMessageToDb(studentMessage);
            return CreatedAtAction(nameof(SaveStudentMessage), new { studentMessage.Id }, result);
       }
    }
}