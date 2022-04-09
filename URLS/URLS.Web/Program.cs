using URLS.Application;
using URLS.Application.Seeder;
using URLS.Constants;
using URLS.Infrastructure.Data.Context;
using URLS.Infrastructure.IoC;
using URLS.Web.Filters;
using URLS.Web.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddResponseCompression(options => options.EnableForHttps = true);

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

builder.Services.AddLogging(x =>
{
    x.AddConsole();
});

builder.Services.AddURLSServices(builder.Configuration);

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ModelStateValidatorAttribute());
});

builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "URLS API",
    });


    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Put your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    s.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    s.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

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

app.UseResponseCompression();

app.UseAccessTokenHandler();

app.UseGlobalErrorHandler();

app.UseApiVersioning();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSessionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseStatistics();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

#endregion

#region DataSeeder

using (var scope = app.Services.CreateScope())
{
    var seederService = scope.ServiceProvider.GetService<ISeederService>();
    await seederService.SeedSystemAsync();
}

#endregion

app.Run();