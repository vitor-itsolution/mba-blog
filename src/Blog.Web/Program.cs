using Blog.Core.Configurations;
using Blog.Data.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddDatabaseSelector();

builder.Services.AddServicesConfigurations();

builder.Services.AddControllersWithViews()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Posts}/{action=Index}/{id?}");
app.MapRazorPages();

app.AddWebApplicationConfigurations();

app.Run();
