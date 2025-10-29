using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Data.SqlClient;

namespace IMDBFrontend{
    public class Program{
        // Add services to the container.
        public static string? connectionString = "";
        static void Main(string[] args){
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            connectionString = builder.Configuration.GetConnectionString("IMDB");
            // Add services to the container.

            builder.Services.AddCors(options => {
                    options.AddPolicy("AllowAll",
                            builder =>
                            {
                            builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                            });
                    });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            // builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
                // app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors("AllowAll");

            app.MapControllers();

            app.Run();
        }
    }
}
