using Microsoft.EntityFrameworkCore;
using Reference_Books.Data;
using Reference_Books.Middleware;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddControllers();

builder.Services.AddControllers().AddJsonOptions(sp => sp.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ReferenceBookDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("ReferenceBookDbContext"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ErrorLoggingMiddleware>();

app.MapControllers();

app.Run();
