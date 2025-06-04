using Microsoft.EntityFrameworkCore;
using SMInternship.Application.Interfaces;
using SMInternship.Application.Services;
using SMInternship.Domain.Interfaces;
using SMInternship.Infrastructure;
using SMInternship.Infrastructure.Repositories;

namespace SMInternship.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<Context>(
                opt => opt.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                    )
                );

            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IProductRepository, ProductRepository>();
            builder.Services.AddTransient<INegotiationRepository, NegotiationRepository>();

            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IProductService, ProductService>();
            builder.Services.AddTransient<INegotiationService, NegotiationService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
