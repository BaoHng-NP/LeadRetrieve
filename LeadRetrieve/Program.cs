
using DotNetEnv;
using LeadRetrieve.Controllers;
using LeadRetrieve.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;



namespace LeadRetrieve
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Logging.AddConsole();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<LeadAdContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));



            builder.Services.AddHttpClient();
            // Register PageTokenService for dependency injection
            builder.Services.AddScoped<PageTokenService>();


            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<LeadAdContext>();
                    var created = context.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

    }
}
