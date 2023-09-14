using Microsoft.AspNetCore.Mvc;
using MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddMvc(options =>
{
    options.EnableEndpointRouting = false; 
}).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

builder.Services.AddHttpClient<ApiService>(httpClient =>
{
    httpClient.BaseAddress = new Uri("http://localhost:5102");
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// ...
app.UseHttpsRedirection();
app.UseStaticFiles();

// UseSession should come before UseRouting
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

