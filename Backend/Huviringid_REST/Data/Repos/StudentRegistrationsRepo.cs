
using Huviringid_REST.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace Huviringid_REST.Data.Repos
{
    public class StudentRegistrationsRepo (DataContext context)
    {
        private readonly DataContext context = context;

        public async Task<StudentRegistration?> SaveRegistrationToDb(StudentRegistration registration)
        {
            bool registrationExists = await context.Registrations
                .AnyAsync(r => r.StudentId == registration.StudentId && r.ExtracurricularId == registration.ExtracurricularId);

            if (registrationExists)
            {
                return null;
            }
            context.Add(registration);
            await context.SaveChangesAsync();
            return registration;
        }

        public async Task<List<StudentRegistration>> GetAllRegistrations()
        {
            IQueryable<StudentRegistration> query = context.Registrations; 
            return await query.ToListAsync();
        }

        public async Task<StudentRegistration?> GetRegistration(int studentId, int extracurricularId)
        {
            var registration = await context.Registrations.FirstOrDefaultAsync(reg => reg.StudentId == studentId && reg.ExtracurricularId == extracurricularId);
            return registration;
        }

        public async Task<bool> UpdateRegistration(int id, StudentRegistration registration) {
            bool isIdsMatch = id == registration.Id;
            bool registrationExists = await RegistrationExistsInDb(id);

            if(!isIdsMatch || !registrationExists) {
                return false;
            }

            context.Update(registration);
            int updatedRecordsCount = await context.SaveChangesAsync();
            return updatedRecordsCount == 1;
        }

        public async Task<bool> DeleteRegistration(int id)
        {
            var registration = await context.Registrations.FindAsync(id);
            if (registration == null)
            {
                return false; // Registration not found
            }

            context.Registrations.Remove(registration);
           int deletedRecordsCount = await context.SaveChangesAsync();
            return deletedRecordsCount == 1;
        }

        public async Task<bool> RegistrationExistsInDb(int id) => await context.Registrations.AnyAsync(x => x.Id == id);
    }
}