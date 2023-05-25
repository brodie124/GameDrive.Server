using GameDrive.Server.Extensions;
using GameDrive.Server.Models.Options;

var builder = WebApplication.CreateBuilder(args);
var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

// Add services to the container.
builder.Services.AddGameDriveConfigurationOptions(builder.Configuration);
builder.Services.AddGameDriveDbContext();
builder.Services.AddGameDriveServices();
builder.Services.AddGameDriveAuthentication(jwtOptions);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
