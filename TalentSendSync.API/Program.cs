using System.Text.Json.Serialization;
using TalentSendSync.CrossCutting;
using TalentSendSync.CrossCutting.IoC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Api
builder.Services.AddInfrastructureAPI(builder.Configuration);
// Swagger
builder.Services.AddInfrastructureSwagger(builder.Configuration);





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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
