using System.Security.Claims;
using Auth0.AspNetCore.Authentication;
using Auth0_RazorPages_WebApp;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.WebEncoders.Testing;

var builder = WebApplication.CreateBuilder(args);

// Add custom authorization handler class
builder.Services.AddSingleton<
    IAuthorizationMiddlewareResultHandler, CustomAuthorizationMiddlewareResultHandler>();

builder.Services
    .AddAuth0WebAppAuthentication(options =>
    {
        options.Domain = builder.Configuration["Auth0:Domain"];
        options.ClientId = builder.Configuration["Auth0:ClientId"];
        options.Scope = "openid profile email";
        options.CallbackPath = "/callback";
        options.OpenIdConnectEvents = new OpenIdConnectEvents
        {
            OnTokenValidated = (ctx) =>
            {
                // Get the user's email 
                var email = ctx.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                //// Query the database to get the role - just an example
                //using (var db = ctx.HttpContext.RequestServices.GetRequiredService<DbContext>())
                //{
                //    // Get the Users from the database, with the logged in email address (from Azure)
                //    var user = db.Users.FirstOrDefault(u => u.UPN.Equals(email));

                //    if (user != null)
                //    {
                //        user.LastLogin = DateTime.Now;
                //        db.SaveChanges();

                //        // Add claims
                //        var claims = new List<Claim>
                //        {
                //            new Claim(ClaimTypes.Role, user.Role.ToString()),
                //            new Claim(ClaimTypes.Expired, (!user.IsActivated || user.IsBlocked).ToString())
                //        };

                //        // Save the claim
                //        var appIdentity = new ClaimsIdentity(claims);
                //        ctx.Principal.AddIdentity(appIdentity);
                //    }
                //    else
                //    {
                //        // Send to Login Page?
                //        ctx.HandleResponse();
                //        ctx.Response.Redirect("/path/to/login");
                //    }
                //}
                return Task.CompletedTask;
            }
        };
    });

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
