using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Domain.Dto;
using MinimalApi.Domain.Entity;
using MinimalApi.Domain.Enum;
using MinimalApi.Domain.Interface;
using MinimalApi.Domain.Service;
using MinimalApi.Domain.View;
using MinimalApi.Infrastructure.Db;

namespace MinimalApi;

public class Startup(IConfiguration configuration)
{
    private readonly string _key = configuration["Jwt"] ?? "";

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication
        (
            option =>
            {
                option.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            }
        ).AddJwtBearer
        (
            option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key))
                };
            }
        );

        services.AddAuthorization();
        
        services.AddScoped<IAdministratorService, AdministratorService>();
        services.AddScoped<IVehicleService, VehicleService>();
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen
        (
            options =>
            {
                options.AddSecurityDefinition
                (
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Insert JWT token like: Bearer {token}."
                    }
                );

                options.AddSecurityRequirement
                (
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] { }
                        }
                    }
                );
            }
        );
        
        services.AddDbContext<Context>
        (
            options => options.UseMySql
            (
                configuration.GetConnectionString("MySql"),
                ServerVersion.AutoDetect
                (
                    configuration.GetConnectionString("MySql")
                )
            )
        );
        
        services.AddCors
        (
            options =>
            {
                options.AddDefaultPolicy
                (
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                );
            }
        );
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors();

        app.UseEndpoints
        (
            endpoints =>
            {
                #region Home

                endpoints.MapGet("/", () => Results.Json(new Home()))
                    .AllowAnonymous()
                    .WithTags("Home");

                #endregion

                #region Administrator

                string GenerateTokenJwt(Administrator administrator)
                {
                    if (string.IsNullOrEmpty(_key)) return string.Empty;

                    var securityKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
                    var credentials = new SigningCredentials(securityKey,
                        SecurityAlgorithms.HmacSha256);

                    var claims = new List<Claim>()
                    {
                        new Claim("Email", administrator.Email!),
                        new Claim("Profile", administrator.Profile!),
                        new Claim(ClaimTypes.Role, administrator.Profile!)
                    };

                    var token = new JwtSecurityToken
                    (
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: credentials
                    );

                    return new JwtSecurityTokenHandler().WriteToken(token);
                }

                endpoints.MapPost
                    (
                        "/administrator/login",
                        (
                            [FromBody] LoginDto loginDto,
                            IAdministratorService administratorService
                        ) =>
                        {
                            var administrator =
                                administratorService.Login(loginDto);

                            if (administrator == null)
                                return Results.Unauthorized();

                            var token = GenerateTokenJwt(administrator);
                            return Results.Ok
                            (
                                new AdminLoggedIn
                                {
                                    Email = administrator.Email!,
                                    Profile = administrator.Profile!,
                                    Token = token
                                }
                            );

                        }
                    ).AllowAnonymous()
                    .WithTags("Administrator");

                endpoints.MapGet
                    (
                        "/administrator",
                        (
                            [FromQuery] int? page,
                            IAdministratorService administratorService
                        ) =>
                        {
                            var adms = new List<AdminLogin>();
                            var administrators =
                                administratorService.GetAll(page);

                            foreach (var adm in administrators)
                            {
                                adms.Add
                                (
                                    new AdminLogin
                                    {
                                        Id = adm.Id,
                                        Email = adm.Email,
                                        Profile = adm.Profile
                                    }
                                );
                            }

                            return Results.Ok(adms);
                        }
                    ).RequireAuthorization()
                    .RequireAuthorization(new AuthorizeAttribute
                        { Roles = "Adm" })
                    .WithTags("Administrator");

                endpoints.MapPost
                    (
                        "/administrator",
                        (
                            [FromBody] AdministratorDto administratorDto,
                            IAdministratorService administratorService
                        ) =>
                        {
                            var response = new ErrorRequest
                            {
                                Messages = new List<string>()
                            };

                            if (string.IsNullOrEmpty(administratorDto.Email))
                                response.Messages.Add(
                                    "Email não pode ser vazio");
                            if (string.IsNullOrEmpty(administratorDto.Password))
                                response.Messages.Add(
                                    "Password não pode ser vazia");
                            if (administratorDto.Profile == null)
                                response.Messages.Add(
                                    "Perfil não pode ser vazio");

                            if (response.Messages.Count > 0)
                                return Results.BadRequest(response);

                            var administrator = new Administrator
                            {
                                Email = administratorDto.Email,
                                Password = administratorDto.Password,
                                Profile = administratorDto.Profile.ToString() ??
                                          Profile.Editor.ToString()
                            };

                            administratorService.Add(administrator);

                            return Results.Created
                            (
                                $"/administrator/{administrator.Id}",
                                new AdminLogin
                                {
                                    Id = administrator.Id,
                                    Email = administrator.Email,
                                    Profile = administrator.Profile
                                }
                            );

                        }
                    ).RequireAuthorization()
                    .RequireAuthorization(new AuthorizeAttribute
                        { Roles = "Adm" })
                    .WithTags("Administrator");

                #endregion

                #region Vehicle

                endpoints.MapPost
                    (
                        "/vehicle",
                        (
                            [FromBody] VehicleDto vehicleDto,
                            IVehicleService vehicleService
                        ) =>
                        {
                            var newVehicle = new Vehicle
                            {
                                Name = vehicleDto.Name,
                                Brand = vehicleDto.Brand,
                                Year = vehicleDto.Year
                            };
                            vehicleService.Add(newVehicle);

                            return Results.Created($"/vehicle/{newVehicle.Id}",
                                newVehicle);
                        }
                    ).RequireAuthorization()
                    .RequireAuthorization(new AuthorizeAttribute
                        { Roles = "Adm,Editor" })
                    .WithTags("Vehicle");

                endpoints.MapGet
                    (
                        "/vehicle",
                        ([FromQuery] int? page,
                            IVehicleService vehicleService) =>
                        {
                            var veiculos = vehicleService.GetAll(page);

                            return Results.Ok(veiculos);
                        }
                    ).RequireAuthorization()
                    .WithTags("Vehicle");

                endpoints.MapGet
                    (
                        "/vehicle/{id}",
                        ([FromRoute] int id, IVehicleService vehicleService) =>
                        {
                            var vehicle = vehicleService.GetById(id);
                            if (vehicle == null) return Results.NotFound();

                            return Results.Ok(vehicle);
                        }
                    ).RequireAuthorization()
                    .RequireAuthorization(new AuthorizeAttribute
                        { Roles = "Adm,Editor" })
                    .WithTags("Vehicle");

                endpoints.MapPut
                    (
                        "/vehicle/{id}",
                        (
                            [FromRoute] int id,
                            Vehicle newVehicle,
                            IVehicleService vehicleService
                        ) =>
                        {
                            var vehicle = vehicleService.GetById(id);
                            if (vehicle == null) return Results.NotFound();

                            vehicle.Name = newVehicle.Name;
                            vehicle.Brand = newVehicle.Brand;
                            vehicle.Year = newVehicle.Year;

                            vehicleService.Update(vehicle);

                            return Results.Ok(vehicle);
                        }
                    ).RequireAuthorization()
                    .RequireAuthorization(new AuthorizeAttribute
                        { Roles = "Adm" })
                    .WithTags("Vehicle");

                endpoints.MapDelete
                    (
                        "/vehicle/{id}",
                        ([FromRoute] int id, IVehicleService vehicleService) =>
                        {
                            var vehicle = vehicleService.GetById(id);
                            if (vehicle == null) return Results.NotFound();

                            vehicleService.Delete(vehicle);

                            return Results.NoContent();
                        }
                    ).RequireAuthorization()
                    .RequireAuthorization(new AuthorizeAttribute
                        { Roles = "Adm" })
                    .WithTags("Vehicle");

                #endregion
            }
        );
    }

}