using Huviringid_REST.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace Huviringid_REST.Data.Repos
{
    public class UpcomingLessonsRepo(DataContext context)
    {
        private readonly DataContext context = context;

        public async Task<List<UpcomingLesson>> GetAllUpcomingLessons() {
            IQueryable<UpcomingLesson> query = context.UpcomingLessons.AsQueryable();
            return await query.ToListAsync(); 
        }
        public async Task<List<UpcomingLesson>> GetUpcomingLessonsForStudentAsync(int studentId)
        {
            var approvedRegistrations = context.Registrations
                .Where(x => x.StudentId == studentId && x.Status == "Approved")
                .Select(x => x.ExtracurricularId);

            if (!approvedRegistrations.Any())
                return new List<UpcomingLesson>();

            var upcomingLessons = await context.UpcomingLessons
                .Where(lesson => approvedRegistrations.Contains(lesson.ExtracurricularId))
                .OrderBy(lesson => lesson.Date)
                .Take(4)
                .ToListAsync();

            return upcomingLessons;                 
        }

        public async Task<bool> IsLessonAbsentAsync(int studentId, int lessonId)
        {
            var lesson = await context.UpcomingLessons.FindAsync(lessonId);
            if (lesson == null)
                return false;
                
            var isAbsent = await context.Notes.AnyAsync(note =>
                note.StudentId == studentId &&
                note.LessonDate.Date == lesson.Date.Date &&
                note.ExtracurricularId == lesson.ExtracurricularId &&
                note.NoteType == "Puudumine"
            );

            return isAbsent;
        }

        public async Task SeedUpcomingLessonsAsync()
        {
            if (context.UpcomingLessons.Any()) return;

            var extracurriculars = context.Extracurriculars.ToList();
            var newLessons = new List<UpcomingLesson>();

            foreach (var extracurricular in extracurriculars)
            {
                newLessons.AddRange(GetUpcomingLessons(extracurricular));
            }

            await context.UpcomingLessons.AddRangeAsync(newLessons);
            await context.SaveChangesAsync();
        }
        public async Task RefreshUpcomingLessonsAsync()
        {
            await CleanUpUpcomingLessons();

            var extracurriculars = await context.Extracurriculars.ToListAsync();

            foreach (var extracurricular in extracurriculars)
            {
                var existingLessons = await context.UpcomingLessons
                    .Where(lesson => lesson.ExtracurricularId == extracurricular.Id)
                    .ToListAsync();

                if (existingLessons.Count < 4)
                {
                    var lessonsToAdd = GetUpcomingLessons(extracurricular);

                    var futureLessonsToAdd = lessonsToAdd
                    .Where(lesson => !existingLessons.Any(x => x.Date.Date == lesson.Date.Date))
                    .ToList();

                    await context.UpcomingLessons.AddRangeAsync(futureLessonsToAdd);
                    await context.SaveChangesAsync();
                }
            }
        }

        private async Task CleanUpUpcomingLessons()
        {
            var today = DateTime.UtcNow.Date;

            var outdatedLessons = await context.UpcomingLessons
                .Where(lesson => lesson.Date.Date < today)
                .ToListAsync();

            if (outdatedLessons.Any())
            {
                context.UpcomingLessons.RemoveRange(outdatedLessons);
                await context.SaveChangesAsync();
            }
        }

        private List<UpcomingLesson> GetUpcomingLessons(Extracurricular extracurricular){
            var newLessons = new List<UpcomingLesson>(); 

            var teacherNames = context.Teachers
                        .Where(t => extracurricular.TeacherIds.Contains(t.Id))
                        .Select(t => t.Name)  
                        .ToList();

            foreach (var lessonDay in extracurricular.Days){

                DateTime nextLessonDate = GetNextLessonDate(lessonDay);

                for (int i = 0; i < 4; i++) 
                {
                    var lessonDate = nextLessonDate.AddDays(i * 7);

                    newLessons.Add(new UpcomingLesson
                    {
                        ExtracurricularId = extracurricular.Id,
                        Date = lessonDate,
                        LessonName = extracurricular.Name,
                        TeacherNames = teacherNames,
                        StartTime = extracurricular.StartTime, 
                        EndTime = extracurricular.EndTime,   
                    });
                }
            }
            return newLessons.OrderBy(lesson => lesson.Date).ToList();
        }

        private DateTime GetNextLessonDate(DayOfWeek lessonDay)
        {
            DateTime currentDate = DateTime.UtcNow; // Use UTC time
            int daysUntilNextLesson = ((int)lessonDay - (int)currentDate.DayOfWeek + 7) % 7;
            DateTime nextLessonDate = daysUntilNextLesson == 0 ? currentDate : currentDate.AddDays(daysUntilNextLesson);
            
            return DateTime.SpecifyKind(nextLessonDate, DateTimeKind.Utc);
        }
    }
}