using ALHAMD.Models;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddHttpClient("ContainerInquiry", client =>
{
    client.BaseAddress = new Uri("http://3.64.191.248:8083");
});

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}









app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
