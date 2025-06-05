using IntegrationProject.DbHelpers;
using IntegrationProject.Helpers;
using IntegrationProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddHttpClient();

builder.Services.AddScoped<DataService>();
builder.Services.AddScoped<IntegrationService>();

builder.Services.AddScoped<IntegrationDbHelper>();
builder.Services.AddScoped<FileHelper>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
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
