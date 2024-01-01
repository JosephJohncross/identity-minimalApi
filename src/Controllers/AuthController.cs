using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace src.Controllers
{
    public static class Authentication
    {

        public static  void MapAuthenticate(this WebApplication app){
            app.MapPost("/authenticate",  (IConfiguration config, [FromBody]Credential credential)=> {
                if (credential.Email == "testuser@567.com" && credential.Password == "tesuser@56789")
                {
                    // Claims
                    List<Claim> claims = [
                        new Claim(ClaimTypes.Email, credential.Email),
                        new Claim(ClaimTypes.Name, credential.Name),
                        new Claim("Department", "HR"),
                        new Claim("Admin", "true"),
                        new Claim("EmployeeDate", "2023-05-12")
                    ];

                    var expiresAt = DateTime.UtcNow.AddMinutes(10);

                    return Results.Ok(new
                    {
                        access_token = CreateToken(claims, expiresAt, config),
                        expires_at = expiresAt
                    });
                }
                return Results.Unauthorized();
            }).WithTags("Auth-JWT");

        }
        private static string CreateToken(IEnumerable<Claim> claims, DateTime expiresAt, IConfiguration config)
        {
            var secretKey = Encoding.ASCII.GetBytes(config.GetValue<string>("SecretKey") ?? "");

            // generate the JWT 
            var jwt = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
            );
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }

    public class Credential
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}