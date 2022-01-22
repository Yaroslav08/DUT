using DUT.Application;
using DUT.Constants;
using DUT.Infrastructure.Data.Context;
using DUT.Infrastructure.IoC;
using DUT.Web.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = TokenOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = TokenOptions.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = TokenOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
    });

builder.Services.AddDUTServices(builder.Configuration);

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddSwaggerGen();

#endregion

var app = builder.Build();

#region Pipeline

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseAccessTokenHandler();

app.UseGlobalErrorHandler();

app.UseApiVersioning();

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

#region DataSeeder

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetService<DUTDbContext>();
    DataSeeder.SeedSystem(context);
}

#endregion

app.Run();