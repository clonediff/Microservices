using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc;

public class PlatformDataClient : IPlatformDataClient
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public PlatformDataClient(IConfiguration configuration, IMapper mapper)
    {
        _configuration = configuration;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<Platform>?> ReturnAllPlatformsAsync()
    {
        Console.WriteLine($"--> Calling GRPC Service {_configuration[AppSettingsConsts.GrpcPlatform]}");
        var channel = GrpcChannel.ForAddress(_configuration[AppSettingsConsts.GrpcPlatform]!);
        var client = new GrpcPlatform.GrpcPlatformClient(channel);
        var request = new GetAllRequest();

        try
        {
            var reply = await client.GetAllPlatformsAsync(request);
            return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not call GRPC Server: {ex.Message}");
            return null;
        }
    }
}
