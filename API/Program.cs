using API.Extensions;
using Infrastructure;
using Serilog;


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

builder.Services.AddOptionSettings(builder.Configuration);

builder.Services.AddAuthConfiguration(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSwaggerInfrastructure();
builder.Services.AddInfrastructures(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", 
    builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tài liệu API");
        c.RoutePrefix = string.Empty;
    });    
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
