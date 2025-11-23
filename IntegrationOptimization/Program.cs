using IntegrationOptimization.ApiClients;
using IntegrationOptimization.Commands;

namespace IntegrationOptimization;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<IProductsApiClient, ProductsApiClient>();
        builder.Services.AddScoped<IProcessProdcutsUseCase, ProcessProdcutsUseCase>();
        builder.Services.AddHttpClient("DummyJsonApi", client =>
        {
            client.BaseAddress = new Uri("https://dummyjson.com/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        
        builder.Services.AddControllers();        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
