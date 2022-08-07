using Shpec.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddShpecValidator();

var app = builder.Build();

app.MapControllers();

app.Run();

public partial class Program { }