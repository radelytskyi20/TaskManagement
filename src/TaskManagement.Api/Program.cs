using Microsoft.AspNetCore.CookiePolicy;
using TaskManagement.Api;
using TaskManagement.Persistence;
using TaskManagement.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiAuthentication(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddService();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    //Secure = CookieSecurePolicy.Always //use http for dev => disable secure
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
