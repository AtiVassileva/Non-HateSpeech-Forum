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
            SeedModerator(serviceProvider);
            SeedAdministrator(serviceProvider);

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

        private static void SeedModerator(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider
                .GetRequiredService<ApplicationDbContext>();

            var moderatorRole = dbContext
                .Roles
                .FirstOrDefault(r => r.Name == "Moderator");

            if (moderatorRole == null)
            {
                dbContext.Roles.Add(new IdentityRole
                {
                    Name = "Moderator"
                });

                dbContext.SaveChanges();

                moderatorRole = dbContext
                    .Roles
                    .First(r => r.Name == "Moderator");
            }

            var moderatorUser = dbContext
                .Users
                .FirstOrDefault(u => u.Email == "moderator@abv.bg");

            var hasher = new PasswordHasher<object?>();

            if (moderatorUser == null)
            {
                const string email = "moderator@abv.bg";

                dbContext.Users.Add(new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    NormalizedEmail = email.ToUpper(),
                    NormalizedUserName = email.ToUpper(),
                    PasswordHash = hasher.HashPassword(null, "moderator1"),
                    EmailConfirmed = true
                });

                dbContext.SaveChanges();

                moderatorUser = dbContext
                    .Users
                    .First(u => u.Email == "moderator@abv.bg");
            }

            var isAlreadyModerator =
                dbContext.UserRoles.Any(ur => ur.RoleId == moderatorRole.Id && ur.UserId == moderatorUser.Id);

            if (isAlreadyModerator)
            {
                return;
            }

            dbContext.UserRoles.Add(new IdentityUserRole<string>()
            {
                UserId = moderatorUser.Id,
                RoleId = moderatorRole.Id
            });

            dbContext.SaveChanges();
        }

        private static void SeedAdministrator(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider
                .GetRequiredService<ApplicationDbContext>();

            var administratorRole = dbContext
                .Roles
                .FirstOrDefault(r => r.Name == "Administrator");

            if (administratorRole == null)
            {
                dbContext.Roles.Add(new IdentityRole
                {
                    Name = "Administrator"
                });

                dbContext.SaveChanges();

                administratorRole = dbContext
                    .Roles
                    .First(r => r.Name == "Administrator");
            }

            var administratorUser = dbContext
                .Users
                .FirstOrDefault(u => u.Email == "administrator@abv.bg");

            var hasher = new PasswordHasher<object?>();

            if (administratorUser == null)
            {
                const string email = "administrator@abv.bg";

                dbContext.Users.Add(new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    NormalizedEmail = email.ToUpper(),
                    NormalizedUserName = email.ToUpper(),
                    PasswordHash = hasher.HashPassword(null, "administrator1"),
                    EmailConfirmed = true
                });

                dbContext.SaveChanges();

                administratorUser = dbContext
                    .Users
                    .First(u => u.Email == "administrator@abv.bg");
            }

            var isAlreadyAdministrator =
                dbContext.UserRoles.Any(ur => ur.RoleId == administratorRole.Id && ur.UserId == administratorUser.Id);

            if (isAlreadyAdministrator)
            {
                return;
            }

            dbContext.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = administratorUser.Id,
                RoleId = administratorRole.Id
            });

            dbContext.SaveChanges();
        }
    }
}