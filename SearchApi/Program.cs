using Microsoft.EntityFrameworkCore;
using SearchApi.Data;
using SearchApi.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//CORS 
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:3000");
        });
});

//Log 
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

//My classes
builder.Services.AddTransient<ISearchService, SearchService>();
builder.Services.AddTransient<ICacheService, CacheService>();

//Cache
builder.Services.AddMemoryCache();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Entity framework
builder.Services.AddDbContext<AppDBContext>(options=> 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
app.UseSerilogRequestLogging();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//CORS 
app.UseCors(builder =>
  builder.WithOrigins("http://localhost:3000")
 .AllowAnyHeader()
 .AllowCredentials()
 .AllowAnyMethod());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
