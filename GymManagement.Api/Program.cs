using GymManagement.Application;
using GymManagement.Infrastructure;
using GymManagement.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

builder.Services
    .AddApplication()
    .AddInfrastructure();


var app = builder.Build();
// Global Exception Handler. Works hand in hand with builder.Services.AddProblemDetails()
app.UseExceptionHandler();  

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection()
app.UseAuthorization();
app.MapControllers();

// DB Migrations
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<GymManagementDbContext>();
dbContext.Database.Migrate();
await Seed.SeedData(dbContext);

app.Run();
