using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderService.Configuration;
using OrderService.Data;
using OrderService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<OrderManager>();
builder.Services.Configure<ServiceUrls>(builder.Configuration.GetSection("ServiceUrls"));
builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrdersDb")));

builder.Services.AddHttpClient<IProductApiClient, ProductApiClient>((serviceProvider, client) =>
{
    var serviceUrls = serviceProvider.GetRequiredService<IOptions<ServiceUrls>>().Value;
    client.BaseAddress = new Uri(serviceUrls.ProductService);
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



