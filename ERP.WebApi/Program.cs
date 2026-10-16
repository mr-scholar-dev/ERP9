using ERP.Application.Implementation;
using ERP.Application.Interfaces;
using ERP.Application.Mapper;
using ERP.Infraestructure.Data;
using ERP.Infraestructure.Data.Seed; // ← AGREGAR ESTA LÍNEA
using ERP.Infraestructure.Implementation;
using ERP.Infraestructure.Interfaces;
using ERP.Domain.DTO.PatternResult;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(entry => entry.Value!.Errors.Count > 0)
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

            var result = Result<Dictionary<string, string[]>>.Failure(
                400, "Bad Request", "Uno o más datos no son válidos.");
            result.Data = errors;

            return new BadRequestObjectResult(result);
        };
    });
builder.Services.AddOpenApi();

// Pattern Dependency Injection
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ISupplierService, SupplierService>();

builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();


// Registrar configuración del Seeder
var adminPassword = builder.Configuration["Seed:AdminPassword"]
    ?? throw new InvalidOperationException(
        "Falta la configuración Seed:AdminPassword (User Secrets).");

var isDevelopment = builder.Environment.IsDevelopment();

// Register DbContext with the Connection String
builder.Services.AddDbContext<AppDbContext>(options => options
    .UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))

    // Las herramientas de EF usan la versión síncrona
    .UseSeeding((context, _) =>
    {
        MasterSeeder.Seed(context, adminPassword);

        if (isDevelopment)
            DevelopmentSeeder.Seed(context);
    })

    // La aplicación usa la versión asíncrona
    .UseAsyncSeeding(async (context, _, ct) =>
    {
        await MasterSeeder.SeedAsync(context, adminPassword, ct);

        if (isDevelopment)
            await DevelopmentSeeder.SeedAsync(context, ct);
    })
);

// Mapster: Pattern Mapper
// https://code-maze.com/mapster-aspnetcore-introduction/
// 1. Trigger custom configurations 
MapsterConfig.RegisterMaps();
// 2. Grab global settings or create a new config instance
var config = TypeAdapterConfig.GlobalSettings;
// 3. Register Mapster into the Service Collection. It can use it with D.I.
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();
builder.Services.AddMapster();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
