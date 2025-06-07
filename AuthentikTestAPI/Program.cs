
using Microsoft.IdentityModel.Logging;

namespace AuthentikTestAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpLogging(opt =>
            {
                
            });

            builder.Services.AddCors(opt =>
            {
                opt.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.WithOrigins("http://localhost:5173");
                });
            });

            builder.Services
                .AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    // Discovery will pull issuer, signing keys and token endpoints from
                    // https://auth.hyperc.tr/.well-known/openid-configuration
                    // Access tokens must be signed JWTs (JWS). Encrypted JWE tokens will fail to validate.
                    options.MetadataAddress = "https://auth.hyperc.tr/application/o/testapp/.well-known/openid-configuration";
                    options.Authority = "https://auth.hyperc.tr";
                    options.Audience = "b2fS6rmY8JzD80iVplmaBq6ylM6xzKi73nEh9TVd";
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        // Validate the JWT issuer and audience
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = "https://auth.hyperc.tr/application/o/testapp/",
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        RoleClaimType = "groups"
                    };

                    // Custom error response for unauthorized requests
                    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsync("{\"error\":\"Unauthorized\"}");
                        }
                    };
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseCors();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Process authentication/authorization for incoming requests
            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
