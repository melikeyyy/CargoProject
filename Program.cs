using Core.Repositories;
using Core.Services;
using Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Repositories;
using Repository.UnitOfWorks;
using Service.Mapping;
using Service.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddAutoMapper(typeof(MapProfile));

builder.Services.AddScoped<ICarrierRepository, CarrierRepository>();
builder.Services.AddScoped<ICarrierService, CarrierService>();


builder.Services.AddScoped<ICarrierConfigurationRepository, CarrierCanfigurationRepository>();
builder.Services.AddScoped<ICarrierConfigurationService, CarrierConfigurationService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();


builder.Services.AddDbContext<AppDBContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"), option =>
    {
        option.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDBContext)).GetName().Name);
    });
});

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

app.Run();
