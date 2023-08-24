using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data;

public static class PrepDb
{
    public static async Task PrepPopulation(this WebApplication app)
    {
        using var serviceScope = app.Services.CreateScope();
        var grpcClient = serviceScope.ServiceProvider.GetRequiredService<IPlatformDataClient>();

        var platforms = await grpcClient.ReturnAllPlatformsAsync();
        
        SeedData(serviceScope.ServiceProvider.GetRequiredService<ICommandRepo>(), platforms);
    }

    private static void SeedData(ICommandRepo repo, IEnumerable<Platform>? platforms)
    {
        Console.WriteLine("--> Seeding new Platforms...");
        if (platforms is null) return;
        
        foreach (var platform in platforms)
        {
            if (!repo.ExternalPlatformExists(platform.ExternalId))
                repo.CreatePlatform(platform);

            repo.SaveChanges();
        }
    }
}
