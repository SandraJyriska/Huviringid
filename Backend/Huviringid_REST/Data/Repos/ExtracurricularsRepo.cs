using Huviringid_REST.Models.Classes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Huviringid_REST.Data.Repos
{
    public class ExtracurricularsRepo(DataContext context)
    {
        private readonly DataContext context = context;

        public async Task<List<Extracurricular>> GetAllExtracurriculars() {
            IQueryable<Extracurricular> query = context.Extracurriculars.AsQueryable();
            return await query.ToListAsync();
        }

        public async Task<List<Extracurricular>> GetExtracurricularsByStudentId(int studentId)
        {
            return await context.Registrations
                .Where(x => x.StudentId == studentId && x.Status == "Approved")
                .Join(
                    context.Extracurriculars,
                    x => x.ExtracurricularId,
                    e => e.Id,
                    (x, e) => e
                )
                .ToListAsync();
        }

        public async Task<List<Extracurricular>> GetAvailableExtracurricularsForStudent(int studentId){
            return await context.Extracurriculars
            .Where(ex => !context.Registrations
                .Any(reg => reg.StudentId == studentId && reg.ExtracurricularId == ex.Id))
            .ToListAsync(); 
        }

        public async Task<List<DateTime>> GetExtracurricularDates(int extracurricularId)
        {
            var extracurricular = await context.Extracurriculars
                .FirstOrDefaultAsync(e => e.Id == extracurricularId);

            if (extracurricular == null)
                return new List<DateTime>();
        
            var today = DateTime.Today;
            var dates = new List<DateTime>(); 

            foreach (var targetDay in extracurricular.Days){
                // Hiljutisem kuupäev enne tänast või täna
                var startDay = today.AddDays(-(int)(today.DayOfWeek - targetDay + 7) % 7);

                // 3 mineviku kuupäeva
                for (int i = 0; i < 3; i++)
                {
                    dates.Add(startDay.AddDays(-7 * i));
                }

                // 7 tuleviku kuupäeva
                for (int i = 1; i <= 7; i++)
                {
                    dates.Add(startDay.AddDays(7 * i));
                }
            }
            var orderedDates = dates.OrderBy(date => date).ToList();
            // Selleks, et saada täpselt 10 kuupäeva
            var pastDates = orderedDates.Where(d => d <= today).OrderByDescending(d => d).Take(3).ToList();
            var futureDates = orderedDates.Where(d => d > today).Take(7).ToList(); 

            // Liidame need kokku
            var finalDates = pastDates.Concat(futureDates).ToList();

            return finalDates.OrderBy(date => date).ToList(); //sorteeritud
        }

        public async Task<Extracurricular?> GetExtracurricularById(int id) => await context.Extracurriculars.FindAsync(id);

        public async Task<List<Extracurricular>> GetExtracurricularsByTeacherId(int teacherId)
        { 
            var extracurriculars = await context.Extracurriculars
                .Where(e => e.TeacherIds != null && e.TeacherIds.Contains(teacherId))
                .ToListAsync();

            return extracurriculars;
        }

        public async Task<Extracurricular?> SaveExtracurricularToDb(Extracurricular extracurricular)
        {
            var existingExtracurricular = await context.Extracurriculars
            .FirstOrDefaultAsync(e => e.Name == extracurricular.Name && e.TeacherIds == extracurricular.TeacherIds);

            if (existingExtracurricular != null)
            {
                return existingExtracurricular;  
            }

            context.Add(extracurricular);
            await context.SaveChangesAsync();
            return extracurricular;
        }

        public async Task<bool> ExtracurricularExistsInDb(int id) => await context.Extracurriculars.AnyAsync(x => x.Id == id);
            // MIINUS: laeb esmalt KÕIK huviringid mällu, mistõttu paljude huviringide kooral jõudlus madal, kuid saab kasutada teacherIds listi. 
            // ALTERNATIIV: kaotada teacherIds list ja luua seostabel TeacherExtracurricular, kus defineeritakse õpetaja ja huviringi vahelised seosed 
                        // ning siis on võimalik teha tavapärast filtreerimist, kuid sel juhul tuleb koodis kõik kohad, mis kasutavad TeacherIds property't
                        // ära muuta ja panna kasutama seostabelit. 
                        // UPDATE: Enam vist mitte :)
        }
}