using Microsoft.EntityFrameworkCore;
using PlatformService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseInMemoryDatabase("InMem"));

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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

app.PrepPopulation();

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
