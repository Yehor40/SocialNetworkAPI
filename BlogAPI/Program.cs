using System.Text;
using BlogAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Load .env file
DotNetEnv.Env.TraversePath().Load();

// Construct connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? builder.Configuration["DefaultConnection"];

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<BlogAPI.Services.IAuthService, BlogAPI.Services.AuthService>();
builder.Services.AddScoped<BlogAPI.Services.IArticleService, BlogAPI.Services.ArticleService>();
builder.Services.AddScoped<BlogAPI.Services.ICommentService, BlogAPI.Services.CommentService>();
builder.Services.AddScoped<BlogAPI.Services.IVoteService, BlogAPI.Services.VoteService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlogAPI", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Configure PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure JWT Authentication
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
             ?? throw new InvalidOperationException("JWT_KEY is not configured");

var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                ?? throw new InvalidOperationException("JWT_ISSUER is not configured");

var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                  ?? throw new InvalidOperationException("JWT_AUDIENCE is not configured");

Console.WriteLine($"JWT_KEY: {jwtKey}");
Console.WriteLine($"JWT_ISSUER: {jwtIssuer}");
Console.WriteLine($"JWT_AUDIENCE: {jwtAudience}");

var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        ValidateLifetime = true
    };
});

// Configure GraphQL (HotChocolate)
builder.Services
    .AddGraphQLServer()
    .AddQueryType<BlogAPI.GraphQL.Query>()
    .AddMutationType<BlogAPI.GraphQL.Mutation>()
    .AddSubscriptionType<BlogAPI.GraphQL.Subscription>()
    .AddInMemorySubscriptions()
    .AddAuthorization();

var app = builder.Build();

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseMiddleware<BlogAPI.Middleware.ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGraphQL();

app.Run();