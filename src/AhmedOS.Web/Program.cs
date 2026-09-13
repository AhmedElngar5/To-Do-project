using AhmedOS.Application.Interfaces;
using AhmedOS.Infrastructure.Data;
using AhmedOS.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AhmedOSDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Server=(localdb)\\mssqllocaldb;Database=AhmedOS;Trusted_Connection=True;MultipleActiveResultSets=true"));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<AhmedOSDbContext>()
.AddDefaultTokenProviders()
.AddSignInManager<SignInManager<ApplicationUser>>();

// Cookie configuration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});

// Services
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAiService, GeminiAiService>();
builder.Services.AddScoped<IPriorityEngine, PriorityEngine>();

// Razor Pages + Controllers (for API endpoints)
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Antiforgery
builder.Services.AddAntiforgery();

var app = builder.Build();

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

// Health endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// Seed database & safe schema updates
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AhmedOSDbContext>();
    await context.Database.EnsureCreatedAsync();

    // Conditional schema safeguards for added columns and tables
    try
    {
        await context.Database.ExecuteSqlRawAsync(@"
            IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'GithubUsername' AND Object_ID = Object_ID(N'UserSettings'))
                ALTER TABLE UserSettings ADD GithubUsername NVARCHAR(200) NULL;

            IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'GeminiApiKey' AND Object_ID = Object_ID(N'UserSettings'))
                ALTER TABLE UserSettings ADD GeminiApiKey NVARCHAR(MAX) NULL;

            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Flashcards') AND type in (N'U'))
            BEGIN
                CREATE TABLE [Flashcards] (
                    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [Front] NVARCHAR(MAX) NOT NULL,
                    [Back] NVARCHAR(MAX) NOT NULL,
                    [Box] INT NOT NULL DEFAULT 1,
                    [NextReviewDate] DATETIME2 NULL,
                    [LastReviewedAt] DATETIME2 NULL,
                    [ReviewCount] INT NOT NULL DEFAULT 0,
                    [StudyTopicId] INT NOT NULL,
                    [UserId] NVARCHAR(450) NOT NULL,
                    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                    [UpdatedAt] DATETIME2 NULL,
                    [IsDeleted] BIT NOT NULL DEFAULT 0
                );
            END
        ");
    }
    catch { /* Safe fallback for diverse runtimes */ }
    
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await DatabaseSeeder.SeedAsync(context, userManager);
}

app.Run();
