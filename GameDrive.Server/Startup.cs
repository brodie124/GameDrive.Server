using GameDrive.Server.Extensions;
using GameDrive.Server.Models.Options;

namespace GameDrive.Server;

public class Startup
{
    public IConfiguration Configuration { get; }
    
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    public virtual void ConfigureServices(IServiceCollection services)
    {
        var jwtOptions = Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
        var databaseOptions = Configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>();

        // Add services to the container.
        services.AddGameDriveConfigurationOptions(Configuration);
        services.AddGameDriveDbContext(Configuration, databaseOptions);
        services.AddGameDriveServices();
        services.AddGameDriveAuthentication(jwtOptions);
        services.AddControllers();
        services
            .AddEndpointsApiExplorer(); // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddSwaggerGen();
        
        services
            .AddMvc()
            .AddApplicationPart(typeof(Startup).Assembly);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();
        app.UseHttpsRedirection();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}