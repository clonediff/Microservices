using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandsService.SyncDataServices.Grpc;

public class PlatformDataClient : IPlatformDataClient
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly GrpcPlatform.GrpcPlatformClient _client;

    public PlatformDataClient(IConfiguration configuration, IMapper mapper, GrpcPlatform.GrpcPlatformClient client)
    {
        _configuration = configuration;
        _mapper = mapper;
        _client = client;
    }
    
    public async Task<IEnumerable<Platform>?> ReturnAllPlatformsAsync()
    {
        Console.WriteLine($"--> Calling GRPC Service {_configuration[AppSettingsConsts.GrpcPlatform]}");
        // var handler = new HttpClientHandler();
        // handler.ServerCertificateCustomValidationCallback =
        //     HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        // var channel = GrpcChannel.ForAddress(_configuration[AppSettingsConsts.GrpcPlatform]!, new GrpcChannelOptions { HttpHandler = handler });
        // var client = new GrpcPlatform.GrpcPlatformClient(channel);
        var request = new GetAllRequest();

        try
        {
            var reply = await _client.GetAllPlatformsAsync(request);
            return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not call GRPC Server: {ex.Message}");
            return null;
        }
    }
}
