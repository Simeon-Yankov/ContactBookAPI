using ContactBookAPI.Infrastructure.Data;
using ContactBookAPI.Web.Endpoints;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();

builder.Host.UseSerilog((context, configuration) =>
   configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contact Book API V1");
        c.RoutePrefix = "api";
    });
}
else
{
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.UseExceptionHandler(options => { });

app.MapPeopleEndpoints();

app.UseMiddleware<PersonRequestLoggingMiddleware>();

app.Run();

public partial class Program { }
