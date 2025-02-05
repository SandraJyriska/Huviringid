using Huviringid_REST.Data.Repos;
using Huviringid_REST.Models.Classes;
using Microsoft.AspNetCore.Mvc;

namespace Huviringid_REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpcomingLessonsController(UpcomingLessonsRepo repo) : ControllerBase
    {
        private readonly UpcomingLessonsRepo repo = repo;

        /// <summary>Leiab kõik saabuvad tunnid</summary>
        /// <returns>Saabuvad tunnid</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            var result = await repo.GetAllUpcomingLessons();
            return Ok(result);
        }

        /// <summary>Leiab saabuvad tunnid õpilase id järgi</summary>
        /// <param name="studentId">Õpilase id</param>
        /// <returns>Saabuvad tunnid</returns>
        [HttpGet("Student/{studentId}")]
        public async Task<IActionResult> GetUpcomingLessonsForStudent(int studentId)
        {
            var upcomingLessons = await repo.GetUpcomingLessonsForStudentAsync(studentId);
            return Ok(upcomingLessons);
        }

        /// <summary>Kontrollib, kas antud õpilasel on antud tunnist puudumine</summary>
        /// <param name="studentId">Õpilase id</param>
        /// /// <param name="lessonId">Tunni id</param>
        /// <returns>True või false</returns>
        [HttpGet("Students/{studentId}/is-lesson-absent/{lessonId}")]
        public async Task<IActionResult> IsLessonAbsent(int studentId, int lessonId)
        {
            var isAbsent = await repo.IsLessonAbsentAsync(studentId, lessonId);
            return Ok(isAbsent);
        }

        /// <summary>Lisab saabuvate tundide andmed andmebaasi</summary>
        /// <returns>Kinnitatud andmed saabuvate tundide kohta</returns>
        [HttpPost("Seed")]
        public async Task<IActionResult> SeedUpcomingLessonsAsync()
        {
            await repo.SeedUpcomingLessonsAsync();
            return Ok();
        }

        /// <summary>Uuendab andmeid saabuvate tundide kohta</summary>
        /// <returns>Uuendatud saabuvad tunnid</returns>
        [HttpPost("Refresh-upcoming-lessons")]
        public async Task<IActionResult> RefreshUpcomingLessonsAsync()
        {
            await repo.RefreshUpcomingLessonsAsync();
            return Ok();
        }
    }
}