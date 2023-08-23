using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing;

public enum EventType
{
    PlatformPublished,
    Undetermined
}

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }
    
    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message);
                break;
            default:
                break;
        }
    }

    private void AddPlatform(string platformPublishMessage)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishMessage);
        try
        {
            var plat = _mapper.Map<Platform>(platformPublishedDto);
            if (!repo.ExternalPlatformExists(plat.ExternalId))
            {
                repo.CreatePlatform(plat);
                repo.SaveChanges();
                Console.WriteLine("--> Platform added!");
            }
            else
                Console.WriteLine("--> Platform already exists");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not add Platform to DB: {ex.Message}");
        }
    }
    
    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determine Event");
        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        var result = eventType.Event switch
        {
            "Platform_Published" => EventType.PlatformPublished,
            _ => EventType.Undetermined
        };

        Console.WriteLine(result switch
        {
            EventType.PlatformPublished => "--> Platform Published Event Detected",
            EventType.Undetermined => "--> Could no determine the event type",
            _ => $"--> {result} Event Detected"
        });
        
        if (result == EventType.Undetermined)
            Console.WriteLine("--> Could no determine the event type");

        return result;
    }
}
