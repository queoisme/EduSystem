using Microsoft.EntityFrameworkCore;
using PRN232.EduSystem.Repositories.Data;
using PRN232.EduSystem.Repositories.Implementations;
using PRN232.EduSystem.Repositories.Interfaces;
using PRN232.EduSystem.Services.Implementations;
using PRN232.EduSystem.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStudentRepository,    StudentRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<ICourseRepository,     CourseRepository>();
builder.Services.AddScoped<ISemesterRepository,   SemesterRepository>();
builder.Services.AddScoped<ISubjectRepository,    SubjectRepository>();

builder.Services.AddScoped<IStudentService,    StudentService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<ICourseService,     CourseService>();
builder.Services.AddScoped<ISemesterService,   SemesterService>();
builder.Services.AddScoped<ISubjectService,    SubjectService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "PRN232 EduSystem API", Version = "v1" });
});

var app = builder.Build();

var retries = 10;
while (retries-- > 0)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        DataSeeder.Seed(db);
        break;
    }
    catch when (retries > 0)
    {
        Thread.Sleep(5000);
    }
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PRN232 EduSystem API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
