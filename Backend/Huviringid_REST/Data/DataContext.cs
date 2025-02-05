
using Huviringid_REST.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace Huviringid_REST.Data
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<Extracurricular> Extracurriculars { get; set; } = null!;
        public DbSet<StudentRegistration> Registrations { get; set; } = null!;
        public DbSet<StudentMessage> StudentMessages { get; set; } = null!;
        public DbSet<Note> Notes { get; set; } = null!;
        public DbSet<UpcomingLesson> UpcomingLessons { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder mb) 
        {
            base.OnModelCreating(mb);

            mb.Entity<User>().ToTable("Users").HasKey(x => x.Id);
            mb.Entity<User>().Property(x => x.Id).ValueGeneratedOnAdd();
            mb.Entity<User>().HasData(
                    new User
                    {
                        Id = 1,
                        Username = "test1",
                        // parool on test1
                        Password = "St9tpNN2zrinRGNUgKWCy4JjZRFEorSQ0Zg3a/8m7k4=",
                        Role = "Student"
                    },
                    new User
                    {
                        Id = 2,
                        Username = "test2",
                        Password = "zWoe4T9h2Hj9G4dyUtWwcKwV6zMR1Q0yr3Uch+xSze8=", // test2
                        Role = "Student"
                    },
                    new User
                    {
                        Id = 3,
                        Username = "test3",
                        Password = "6RwNz8ehCp0yZ0KkUE7i+Shy+2l7C1Eh9dT/RULwZN8=", // test3
                        Role = "Student"
                    },
                    new User
                    {
                        Id = 4,
                        Username = "test4",
                        Password = "z8+REyxR4JVWVzkW12nvnmEbFrUHO8dFlD15Go9qngY=", // test4
                        Role = "Student"
                    },
                    new User
                    {
                        Id = 5,
                        Username = "test5",
                        Password = "b/hfab7bwOEackK6s1ibDcD0xZW7z+WLKLYTO+5JwR4=", // test5
                        Role = "Student"
                    },
                    new User
                    {
                        Id = 6,
                        Username = "test6",
                        Password = "0oQ99Eg99r3Aeepx4L71dxT14UHSlR15XG/xeG5D0eY=", // test6
                        Role = "Teacher"
                    },
                    new User
                    {
                        Id = 7,
                        Username = "test7",
                        Password = "wQI3t54+4azBkbMBVBvc7f31MG4dn5DXauPHG4mrUy4=", // test7
                        Role = "Teacher"
                    },
                    new User
                    {
                        Id = 8,
                        Username = "test8",
                        Password = "JrbIxYjAs4PXZAbPzza6Y631uQMJu/WF3A9LERldyuw=", // test8
                        Role = "Teacher"
                    },
                    new User
                    {
                        Id = 9,
                        Username = "test9",
                        Password = "YWpLATRWNSCz9SX+nSnxbr7KJCIu/M7FViG3GWhe5Rs=", // test9
                        Role = "Teacher"
                    }
                );

            mb.Entity<Student>().ToTable("Students").HasKey(x => x.Id); 
            mb.Entity<Student>().Property(x => x.Id).ValueGeneratedOnAdd();
            mb.Entity<Student>().HasData(
                new Student {
                    Id = 1,
                    FirstName = "Mari",
                    LastName = "Maasikas",
                    PersonalId = "60401170889",
                    PhoneNr = "55582789",
                    Email = "mari.maasikas@gmail.com",
                    UserId = 1
                },
                new Student {
                    Id = 2,
                    FirstName = "Kati",
                    LastName = "Kuusik",
                    PersonalId = "60401179941",
                    PhoneNr = "55599789",
                    Email = "kati.kuusik@gmail.com",
                    UserId = 2
                },
                new Student {
                    Id = 3,
                    FirstName = "Tiina",
                    LastName = "Riisik",
                    PersonalId = "60311188891",
                    PhoneNr = "54482789",
                    Email = "tiina.riisik@gmail.com",
                    UserId = 3
                },
                new Student {
                    Id = 4,
                    FirstName = "Kai",
                    LastName = "Kivi",
                    PersonalId = "60311189931",
                    PhoneNr = "54482123",
                    Email = "kai.kivi@gmail.com",
                    UserId = 4
                },
                new Student {
                    Id = 5,
                    FirstName = "Emma",
                    LastName = "Mustikas",
                    PersonalId = "60511189931",
                    PhoneNr = "54482124",
                    Email = "emma.mustikas@gmail.com",
                    UserId = 5
                }
            );
            
            mb.Entity<Teacher>().ToTable("Teachers").HasKey(x => x.Id);
            mb.Entity<Teacher>().Property(x => x.Id).ValueGeneratedOnAdd();
            mb.Entity<Teacher>().HasData(
                new Teacher {
                    Id = 1,
                    Name = "Anna Kask",
                    Email ="anna.kask@gmail.com",
                    UserId = 6
                },
                new Teacher {
                    Id = 2,
                    Name = "Mart Tamm",
                    Email ="mart.tamm@gmail.com",
                    UserId = 7
                },
                new Teacher {
                    Id = 3,
                    Name = "Evely Saaremaa",
                    Email ="evely.saaremaa@gmail.com",
                    UserId = 8
                },
                new Teacher {
                    Id = 4,
                    Name = "Liis Rohi",
                    Email ="liis.rohi@gmail.com",
                    UserId = 9
                }
            );
            mb.Entity<Extracurricular>().ToTable("Extracurriculars").HasKey(x => x.Id);
            mb.Entity<Extracurricular>().Property(x => x.Id).ValueGeneratedOnAdd();
            mb.Entity<Extracurricular>().HasData(
                new Extracurricular {
                    Id = 1,
                    Name = "Koor Ellerhein",
                    TeacherIds = new List<int> { 1, 4 },
                    Days = [DayOfWeek.Monday],
                    StartTime = "15:00",
                    EndTime = "17:00",
                    Location = "Vilde tee 69",
                    Description = "Koor Ellerhein on muusikaline huviring, mis pakub laulmise ja koorimuusika õppimise " +
                    "võimalust. Kooris osalemine aitab arendada laulutehnikat, kuulamisoskust ja ansamblitunnetust. " +
                    "Lisaks toimub regulaarseid esinemisi ja kontserte nii Eestis kui ka välismaal."
                },
                new Extracurricular {
                    Id = 2,
                    Name = "Robootika Klubi",
                    TeacherIds = new List<int> { 2 },
                    Days = [DayOfWeek.Thursday],
                    StartTime = "10:00",
                    EndTime = "12:00",
                    Location = "GAG klass 221",
                    Description = "Robootika Klubi on mõeldud neile, kes soovivad tutvuda robootika ja programmeerimise alustega. " +
                    "Klubi liikmed saavad õppida robotite ehitamise ja programmeerimise põhitõdesid ning osaleda praktilistes projektides. " +
                    "Kursuse lõpuks on võimalik oma loodud roboteid näidata ning osaleda kohalikel robootikavõistlustel.",
                },
                new Extracurricular
                {
                    Id = 3,
                    Name = "Kunstistuudio",
                    TeacherIds = new List<int> { 3 },
                    Days = [DayOfWeek.Friday],
                    StartTime = "13:00",
                    EndTime = "15:00",
                    Location = "EKA klass 332",
                    Description = "Kunstistuudio on suunatud noortele, kes soovivad arendada oma loovust ja kunstioskusi. " +
                    "Tundides katsetatakse erinevaid joonistamise, maalimise ja skulptuuritehnikaid. " +
                    "Stuudio lõpus korraldatakse ka näitus, kus kõik osalejad saavad oma töid esitleda."
                },
                new Extracurricular
                {
                    Id = 4,
                    Name = "Tantsuklubi",
                    TeacherIds = new List<int> { 4 },
                    Days = [DayOfWeek.Monday],
                    StartTime = "16:00",
                    EndTime = "18:00",
                    Location = "Vabaduse 17",
                    Description = "Tantsuklubi pakub noortele võimalust õppida erinevaid tantsustiile, alates kaasaegsest tantsust " +
                    "kuni hip-hopini. Klubi sobib nii algajatele kui edasijõudnutele, kes soovivad arendada oma liikumist, rütmitunnetust " +
                    "ja esinemisoskust. Treeningute käigus on võimalik osaleda koreograafia loomises ning esineda kooli üritustel."
                }
            );

            mb.Entity<StudentRegistration>().ToTable("StudentRegistrations").HasKey(x => x.Id);
            // voibolla vaja lisada HasIndex...
            mb.Entity<StudentRegistration>().Property(x => x.Id).ValueGeneratedOnAdd();
            mb.Entity<StudentRegistration>().HasData(
                new StudentRegistration {
                    Id = 1,
                    StudentId = 1,
                    ExtracurricularId = 1,
                    Status = "Approved"
                },
                new StudentRegistration {
                    Id = 2,
                    StudentId = 2,
                    ExtracurricularId = 1,
                    Status = "Approved"
                },
                new StudentRegistration {
                    Id = 3,
                    StudentId = 3,
                    ExtracurricularId = 1,
                    Status = "Approved"
                },
                new StudentRegistration {
                    Id = 4,
                    StudentId = 4,
                    ExtracurricularId = 1,
                    Status = "Approved"
                },
                new StudentRegistration {
                    Id = 5,
                    StudentId = 5,
                    ExtracurricularId = 3,
                    Status = "Pending"
                },
                new StudentRegistration {
                    Id = 6,
                    StudentId = 1,
                    ExtracurricularId = 3,
                    Status = "Approved"
                },
                new StudentRegistration {
                    Id = 7,
                    StudentId = 2,
                    ExtracurricularId = 2,
                    Status = "Approved"
                },
                new StudentRegistration {
                    Id = 8,
                    StudentId = 2,
                    ExtracurricularId = 3,
                    Status = "Pending"
                }
            );

            mb.Entity<Note>().ToTable("Notes").HasKey(x => x.Id);
            mb.Entity<Note>().Property(x => x.Id).ValueGeneratedOnAdd();
            mb.Entity<Note>().HasData(
                new Note {
                    Id = 1,
                    NoteType = "Puudumine",
                    Message = "Puudun, haige",
                    StudentId = 1,
                    ExtracurricularId = 1,
                    LessonDate = GetNextMonday(DateTime.UtcNow),
                    SendingDateTime = DateTime.UtcNow,
                },
                new Note {
                    Id = 2,
                    NoteType = "Puudumine",
                    Message = "Reisil",
                    StudentId = 2,
                    ExtracurricularId = 1,
                    LessonDate = GetNextMonday(DateTime.UtcNow).AddDays(7),
                    SendingDateTime = DateTime.UtcNow,
                },
                new Note {
                    Id = 3,
                    NoteType = "Hilinemine",
                    Message = "Hilinen 10min",
                    StudentId = 3,
                    ExtracurricularId = 1,
                    LessonDate = GetNextMonday(DateTime.UtcNow).AddDays(14),
                    SendingDateTime = DateTime.UtcNow,
                },
                new Note {
                    Id = 4,
                    NoteType = "Hilinemine",
                    Message = "Hilinen 20min",
                    StudentId = 4,
                    ExtracurricularId = 1,
                    LessonDate = GetNextMonday(DateTime.UtcNow).AddDays(14),
                    SendingDateTime = DateTime.UtcNow,
                }
            );
            
            mb.Entity<StudentMessage>().ToTable("StudentMessages").HasKey(x => x.Id);    
            mb.Entity<StudentMessage>().Property(x => x.Id).ValueGeneratedOnAdd();
            mb.Entity<StudentMessage>().HasData(
            new StudentMessage
            {
                Id = 1,
                Title = "Puudumine",
                Sender = "Anna Kask",
                Content = "Tund jääb täna ära.",
                Time = "2024-10-01T10:00:00", //chat andis
                TeacherId = 1,
                ExtracurricularId = 1,
                StudentIds = new List<int> { 1, 2, 3, 4 },
            }
            );

            mb.Entity<UpcomingLesson>().ToTable("UpcomingLessons").HasKey(x => x.Id);
            mb.Entity<UpcomingLesson>().Property(x => x.Id).ValueGeneratedOnAdd();

        }

        private DateTime GetNextMonday(DateTime startDate)
        {
            // Get the current day of the week (0=Sunday, 1=Monday, ..., 6=Saturday) ..
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)startDate.DayOfWeek + 7) % 7;
            return startDate.AddDays(daysUntilMonday);
        }
    }
}