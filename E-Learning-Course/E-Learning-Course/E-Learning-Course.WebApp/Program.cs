using E_Learning_Course.Data.Entities;
using E_Learning_Course.Data;
using Microsoft.AspNetCore.Identity;
using E_Learning_Course.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http.Features;
using E_Learning_Course.Service;
using E_Learning_Course.WebApp.Pages.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 512 * 1024 * 1024;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 512 * 1024 * 1024;
});
builder.Services.AddControllers();
// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddSession();
builder.Services.AddRazorPages();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/ErrorAuthorization";
});
builder.Services.Configure<SendEmail>(builder.Configuration.GetSection("SendEmail"));
builder.Services.AddScoped<ISendEmail, SendEmailServices>();
builder.Services.AddScoped<ICommentService, CommentService> ();
builder.Services.AddHostedService<EnrollmentStatusUpdater>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin() // Allow any origin
               .AllowAnyMethod() // Allow any HTTP method
               .AllowAnyHeader(); // Allow any header
    });
});
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
app.UseCors("AllowAllOrigins");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapHub<CommentHub> ("/commentHub");


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();

    // Redirect root URL ("/") to either the admin dashboard or index page based on user role
    endpoints.MapGet("/", async context =>
    {
        if (context.User.Identity?.IsAuthenticated == true && context.User.IsInRole("Administrator"))
        {
            context.Response.Redirect("/Admin/Dashboard");
        }
        else if (context.User.Identity?.IsAuthenticated == true && context.User.IsInRole("Instructor"))
        {
            context.Response.Redirect("/Admin/Courses/List");
        }
        else
        {
            context.Response.Redirect("/Homepage/Index");
        }
    });
});

app.Run();
