using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NonHateSpeechForum.Data;

namespace NonHateSpeechForum.Infrastructure
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder PrepareDatabase(this IApplicationBuilder app)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();
            var serviceProvider = scopedServices.ServiceProvider;

            MigrateDatabase(serviceProvider);
            SeedRoles(serviceProvider);

            return app;
        }

        private static void MigrateDatabase(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider
                .GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }

        private static void SeedRoles(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider
                .GetRequiredService<ApplicationDbContext>();

            if (dbContext.Roles.Any())
            {
                return;
            }

            dbContext.Roles.AddRange(new[]
            {
                new IdentityRole { Name = "Regular User" },
                new IdentityRole { Name = "Moderator" },
                new IdentityRole { Name = "Administrator" }
            });

            dbContext.SaveChanges();
        }
    }
}
