using System.Text.Json.Serialization;
using Microsoft.AspNetCore.RateLimiting;
using TalentSendSync.CrossCutting;
using TalentSendSync.CrossCutting.IoC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Api
builder.Services.AddInfrastructureAPI(builder.Configuration);
// jwt
builder.Services.AddJwtConfiguration(builder.Configuration);
// RateLimit
builder.Services.AddInfrastructureRateLimiter(builder.Configuration);
// Swagger
builder.Services.AddInfrastructureSwagger(builder.Configuration);
// CORS
builder.Services.AddInfrastructureCors(builder.Configuration);




builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();

    app.UseSwaggerUi(options =>
    {
        options.Path = "";
    });
}

app.UseMiddleware<TalentSendSync.API.Middleware.ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();


app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
