using GymManagement.Domain.Admins;

namespace GymManagement.Infrastructure.Common.Persistence;

public static class Seed
{
    public static async Task SeedData(GymManagementDbContext dbContext)
    {
        if (!dbContext.Admins.Any())
        {
            var seedAdmin = new Admin(
                userId: Guid.NewGuid(),
                subscriptionId: null,
                id:  Guid.Parse("2150e333-8fdc-42a3-9474-1a3956d46de8")  // Test data
            );

            await dbContext.Admins.AddAsync(seedAdmin);
            await dbContext.SaveChangesAsync();
        }
    }
}
