using Microsoft.AspNetCore.Identity;

namespace api.Data
{
    public static class RoleSeeder
    {
        // Riktig liste med alle gyldige roller i NextPlay
        private static readonly string[] Roles = new[]
        {
            "PlatformAdmin",
            "ClubAdmin",
            "Coach",
            "Player"
        };

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
