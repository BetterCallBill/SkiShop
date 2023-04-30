using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager) 
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "billy",
                    Email = "billy@test.com",
                    UserName = "billy@test.com",
                    Address = new Address
                    {
                        FirstName = "billy",
                        LastName = "wang",
                        Street = "NB5",
                        City = "New City",
                        State = "NY",
                        ZipCode = "12345",
                    }
                };

                await userManager.CreateAsync(user, "Pa$$w0rd");
            }
        }
    }
}
