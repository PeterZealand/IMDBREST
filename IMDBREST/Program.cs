namespace IMDBFrontend{
    public class Program{
        // Add services to the container.
        public static string? connectionString = "";
        static void Main(string[] args){
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            connectionString = builder.Configuration.GetConnectionString("IMDB");

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
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseCors("AllowAll");

            app.MapControllers();

            app.Run();
        }
    }
}
