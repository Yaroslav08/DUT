using DUT.Infrastructure.IoC;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddDUTServices(builder.Configuration);

builder.Services.AddControllersWithViews();

var app = builder.Build();

#endregion


#region Pipeline

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

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "default",
      pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapRazorPages();

#endregion

app.Run();