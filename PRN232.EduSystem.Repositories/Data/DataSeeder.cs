using PRN232.EduSystem.Repositories.Entities;

namespace PRN232.EduSystem.Repositories.Data;

public static class DataSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Semesters.Any()) return;

        var semesters = new List<Semester>
        {
            new() { SemesterName = "Spring 2023", StartDate = new DateTime(2023, 1, 9),  EndDate = new DateTime(2023, 5, 26) },
            new() { SemesterName = "Fall 2023",   StartDate = new DateTime(2023, 8, 28), EndDate = new DateTime(2023, 12, 22) },
            new() { SemesterName = "Spring 2024", StartDate = new DateTime(2024, 1, 8),  EndDate = new DateTime(2024, 5, 24) },
            new() { SemesterName = "Fall 2024",   StartDate = new DateTime(2024, 8, 26), EndDate = new DateTime(2024, 12, 20) },
            new() { SemesterName = "Spring 2025", StartDate = new DateTime(2025, 1, 6),  EndDate = new DateTime(2025, 5, 23) },
        };
        context.Semesters.AddRange(semesters);
        context.SaveChanges();

        context.Subjects.AddRange(
            new Subject { SubjectCode = "CS101", SubjectName = "Introduction to Programming",   Credit = 3 },
            new Subject { SubjectCode = "CS102", SubjectName = "Data Structures & Algorithms",  Credit = 4 },
            new Subject { SubjectCode = "CS103", SubjectName = "Database Systems",              Credit = 3 },
            new Subject { SubjectCode = "CS104", SubjectName = "Web Development Fundamentals",  Credit = 3 },
            new Subject { SubjectCode = "CS105", SubjectName = "Object-Oriented Programming",   Credit = 3 },
            new Subject { SubjectCode = "CS106", SubjectName = "Computer Networks",             Credit = 3 },
            new Subject { SubjectCode = "CS107", SubjectName = "Software Engineering",          Credit = 4 },
            new Subject { SubjectCode = "CS108", SubjectName = "Operating Systems",             Credit = 3 },
            new Subject { SubjectCode = "CS109", SubjectName = "Artificial Intelligence",       Credit = 3 },
            new Subject { SubjectCode = "CS110", SubjectName = "Cloud Computing",               Credit = 3 }
        );
        context.SaveChanges();

        var s = semesters; // s[0]..s[4] now have auto-generated SemesterIds
        var courses = new List<Course>
        {
            new() { CourseName = "Intro to Programming – Spring 2023",  SemesterId = s[0].SemesterId },
            new() { CourseName = "Data Structures – Spring 2023",        SemesterId = s[0].SemesterId },
            new() { CourseName = "Database Systems – Spring 2023",       SemesterId = s[0].SemesterId },
            new() { CourseName = "Web Development – Spring 2023",        SemesterId = s[0].SemesterId },
            new() { CourseName = "OOP – Fall 2023",                     SemesterId = s[1].SemesterId },
            new() { CourseName = "Computer Networks – Fall 2023",        SemesterId = s[1].SemesterId },
            new() { CourseName = "Software Engineering – Fall 2023",     SemesterId = s[1].SemesterId },
            new() { CourseName = "Operating Systems – Fall 2023",        SemesterId = s[1].SemesterId },
            new() { CourseName = "AI Fundamentals – Spring 2024",        SemesterId = s[2].SemesterId },
            new() { CourseName = "Cloud Computing – Spring 2024",        SemesterId = s[2].SemesterId },
            new() { CourseName = "Intro to Programming – Spring 2024",  SemesterId = s[2].SemesterId },
            new() { CourseName = "Data Structures – Spring 2024",        SemesterId = s[2].SemesterId },
            new() { CourseName = "Database Systems – Fall 2024",         SemesterId = s[3].SemesterId },
            new() { CourseName = "Web Development – Fall 2024",          SemesterId = s[3].SemesterId },
            new() { CourseName = "OOP – Fall 2024",                     SemesterId = s[3].SemesterId },
            new() { CourseName = "Computer Networks – Fall 2024",        SemesterId = s[3].SemesterId },
            new() { CourseName = "Software Engineering – Spring 2025",   SemesterId = s[4].SemesterId },
            new() { CourseName = "Operating Systems – Spring 2025",      SemesterId = s[4].SemesterId },
            new() { CourseName = "AI Fundamentals – Spring 2025",        SemesterId = s[4].SemesterId },
            new() { CourseName = "Cloud Computing – Spring 2025",        SemesterId = s[4].SemesterId },
        };
        context.Courses.AddRange(courses);
        context.SaveChanges();

        var lastNames  = new[] { "Nguyen", "Tran", "Le", "Pham", "Hoang", "Phan", "Vu", "Dang", "Bui", "Do" };
        var firstNames = new[] { "Minh", "Anh", "Thu", "Lan", "Hoa", "Khanh", "Long", "Hung", "Tuan", "Linh",
                                  "Hieu", "Quang", "Duc", "Nam", "Bao", "Huy", "Viet", "Phuc", "Mai", "Thanh" };
        var students = new List<Student>();
        for (int i = 0; i < 50; i++)
        {
            students.Add(new Student
            {
                FullName    = $"{lastNames[i % lastNames.Length]} {firstNames[i % firstNames.Length]}",
                Email       = $"student{i + 1:D2}@edu.vn",
                DateOfBirth = new DateTime(2000 + (i % 5), (i % 12) + 1, (i % 28) + 1),
            });
        }
        context.Students.AddRange(students);
        context.SaveChanges();

        var statuses    = new[] { "Active", "Inactive", "Completed" };
        var enrollments = new List<Enrollment>();
        int idx         = 0;

        for (int si = 0; si < students.Count; si++)
        {
            var selectedCourses = Enumerable.Range(0, courses.Count)
                .OrderBy(c => (c * (si + 1) * 17 + (si + 1) * 7) % 100)
                .Take(10);

            foreach (var ci in selectedCourses)
            {
                enrollments.Add(new Enrollment
                {
                    StudentId  = students[si].StudentId,
                    CourseId   = courses[ci].CourseId,
                    EnrollDate = new DateTime(2023, 1, 1).AddDays(idx % 730),
                    Status     = statuses[idx % 3],
                });
                idx++;
            }
        }
        context.Enrollments.AddRange(enrollments);
        context.SaveChanges();
    }
}
