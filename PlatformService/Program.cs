using Microsoft.EntityFrameworkCore;
using PlatformService;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsProduction())
{
    Console.WriteLine("--> using SqlServer Db");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString(AppSettingsConsts.PlatformConnectionString)));
}
else
{
    Console.WriteLine("--> using InMem Db");
    builder.Services.AddDbContext<AppDbContext>(options => 
        options.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

Console.WriteLine($"--> Command Service Endpoint: {builder.Configuration[AppSettingsConsts.CommandService]}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.PrepPopulation(app.Environment.IsProduction());

app.Run();

// docker build -t clonediff/platformservice .              (Run from ../Microservices/PlatformService to build image)
// docker run -p 8080:80 -d clonediff/platformservice       (Run from ../Microservices/PlatformService to launch image as new container)
// docker ps                                                (Run to show already launched containers)
// docker stop <container_id>                               (Run to stop launched container)
// docker start <container_id>                              (Run to start existing container)
// docker push clonediff/platformservice                    (Run to push image to DockerHub)

// kubectl apply -f platforms-depl.yaml                     (Run from ../Microservices/K8S to create deployment)
// kubectl get deployments                                  (Run to get all deployments)
// kubectl get pods                                         (Run to get all pods)
// kubectl get services                                     (Run to get all services)
// kubectl delete deployment <deployment>                   (Run to delete deployment)
// kubectl apply -f platforms-np-srv.yaml                   (Run from ../Microservices/K8S to create service)
// kubectl rollout restart deployment platforms-depl        (Run to restart deployment and pull updates)

// kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.1/deploy/static/provider/cloud/deploy.yaml
// (Run to apply ingress-nginx)
// kubectl get namespace                                    (Run to get all namespaces)
// kubectl get pods --namespace=<namespace_name>            (Run to get all pods in namespace_name) 

// kubectl apply -f ingress-srv.yaml                        (Run to apply Ingress)
// Чтобы в ingress-srv.yaml работал acme.com надо в C:\Windows\System32\drivers\etc\hosts прописать "127.0.0.1 acme.com" (на локалке)

// kubectl get storageclass                                                         (Run to get storages)
// kubectl apply -f local-pvc.yaml                                                  (Run to apply PersistentVolumeClaim)
// kubectl get pvc                                                                  (Run to get PersistentVolumeClaims)
// kubectl create secret generic mssql --from-literal=SA_PASSWORD="pa55w0rd!"       (Run to create secret)
// kubectl apply -f mssql-plat-depl.yaml                                            (Run to mssql deployment)
// (Иногда может падать таймаут во время пула изображения, сначала спулил вручную -> заработало)

// dotnet ef migrations add InitialCreate                   (Run to create migration)
// rebuild platformservice image and push it to docker hub
// apply new deployment by rollout restart or delete + apply
