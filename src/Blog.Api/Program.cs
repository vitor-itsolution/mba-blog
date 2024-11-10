using Blog.Core.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.AddDatabaseSelector();
builder.Services.AddServicesConfigurations();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.AddWebApplicationConfigurations();

app.Run();
