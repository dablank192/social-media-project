using System.Text;
using System.Text.Json.Serialization;
using Amazon.S3;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using vsa_w_controller_csharp.Feature.Auth;
using vsa_w_controller_csharp.Feature.Likes;
using vsa_w_controller_csharp.Infrastructure;
using vsa_w_controller_csharp.Share.CloudinaryImgUpload;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// MediatR config

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblyContaining<Program>();
});


// Add dependencies to the container

builder.Services.AddScoped<IAuthHelper, AuthHelper>();
builder.Services.AddScoped<ICldImageManagement, CldImageManagement>();


// Add database connection

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseNpgsql(connectionString);
});


// Add Cookie base authentication

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(option =>
{
    option.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Cookies["x-access-token"];

            if(!string.IsNullOrEmpty(accessToken))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };

    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidateAudience = false,
        ValidateIssuer = false,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
});


// Add Exception Handler

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


//Add Controller

builder.Services.AddControllers();


/// Add Blazor CORS Config

var blazorOrigin = "http://localhost:5256";

builder.Services.AddCors(option =>
{
    option.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins(blazorOrigin)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});


// // Supabase S3 Storage config 

// var accessKey = builder.Configuration["S3Storage:AccessKey"];
// var accessSecretKey = builder.Configuration["S3Storage:SecretAccessKey"];
// var s3Endpoint = builder.Configuration["S3Storage:Endpoint"];

// var s3config = new AmazonS3Config
// {
//     ServiceURL = s3Endpoint,
//     ForcePathStyle = true
// };

// var s3Client = new AmazonS3Client(
//     accessKey,
//     accessSecretKey,
//     s3config
// );

// // Add S3 Client to DI Container
// builder.Services.AddSingleton<IAmazonS3>(s3Client);


// Cloudinary Config
var cldApiKey = builder.Configuration["Cloudinary:APIKey"];
var cldApiSecret = builder.Configuration["Cloudinary:APISecret"];
var cldName = builder.Configuration["Cloudinary:CloudName"];

var cloudinaryConfig = new Account
{
    ApiKey = cldApiKey,
    ApiSecret = cldApiSecret,
    Cloud = cldName
};
// Cloudinary Client
var cloudinaryClient = new Cloudinary(cloudinaryConfig);
cloudinaryClient.Api.Secure = true;

// Add Cloudinary to DI Container
builder.Services.AddSingleton(cloudinaryClient);


// Add Like queue and worker
builder.Services.AddSingleton<LikeActionQueue>();
builder.Services.AddHostedService<LikeActionWorker>();


var app = builder.Build();


app.UseRouting();

app.UseCors("AllowBlazor");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        // Lệnh này tương đương với dotnet ef database update
        dbContext.Database.Migrate(); 
    }
}

app.UseHttpsRedirection();


app.Run();
