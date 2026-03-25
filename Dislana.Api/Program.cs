// Per-file using directives were moved to GlobalUsings.cs to reduce repetition and
// keep Program.cs focused on application bootstrap.

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.File(
        Path.Combine(AppContext.BaseDirectory, "logs", "log-.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dislana API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token en el formato: Bearer {token}"
    });

    options.AddSecurityRequirement((document) => new OpenApiSecurityRequirement()
    {
        [new OpenApiSecuritySchemeReference("Authorization", document)] = []
    }); 
});

// Application
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
// Bind WompiOptions and register the bound instance so it can be injected directly
var wompiOptions = builder.Configuration.GetSection("Payments:Wompi").Get<Dislana.Application.Payment.Options.WompiOptions>() ?? new Dislana.Application.Payment.Options.WompiOptions();
builder.Services.AddSingleton(wompiOptions);
// Secret providers: configuration first, then environment
// Secret providers: register configuration and environment providers, then expose a composite provider as the ISecretProvider
builder.Services.AddSingleton<ConfigurationSecretProvider>(sp =>
    new ConfigurationSecretProvider(key => builder.Configuration[key] ?? builder.Configuration.GetConnectionString(key)));
builder.Services.AddSingleton<EnvironmentSecretProvider>();
builder.Services.AddSingleton<ISecretProvider>(sp =>
    new CompositeSecretProvider(new ISecretProvider[] {
        sp.GetRequiredService<ConfigurationSecretProvider>(),
        sp.GetRequiredService<EnvironmentSecretProvider>()
    }));

builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IQuoteService, QuoteService>();

// Infrastructure
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCredentialRepository, UserCredentialRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IProductRepository,ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IDbExecutor, DbExecutor>();
builder.Services.AddTransient<DbConnectionFactory>();

// Bind JwtSettings from configuration but allow explicit environment variable fallbacks
var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>()
    ?? new JwtSettings();

// Helper to resolve a configuration value preferring bound value, then configuration keys, then environment variables
string ResolveConfig(string? current, params string[] keys)
{
    if (!string.IsNullOrWhiteSpace(current))
        return current!;

    foreach (var key in keys)
    {
        // First try configuration (supports appsettings and other providers)
        var cfg = builder.Configuration[key];
        if (!string.IsNullOrWhiteSpace(cfg)) return cfg;

        // Then try environment variables (both hierarchical and flat conventions)
        var env = Environment.GetEnvironmentVariable(key)
                  ?? Environment.GetEnvironmentVariable(key.Replace(':', '_'))
                  ?? Environment.GetEnvironmentVariable(key.ToUpperInvariant().Replace(':', '_'));

        if (!string.IsNullOrWhiteSpace(env)) return env;
    }

    return string.Empty;
}

jwtSettings.Key = ResolveConfig(jwtSettings.Key, "Jwt:Key", "Jwt__Key", "JWT_KEY");
jwtSettings.Issuer = ResolveConfig(jwtSettings.Issuer, "Jwt:Issuer", "Jwt__Issuer", "JWT_ISSUER");
jwtSettings.Audience = ResolveConfig(jwtSettings.Audience, "Jwt:Audience", "Jwt__Audience", "JWT_AUDIENCE");

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
    throw new InvalidOperationException("JWT Key is not configured.");

if (string.IsNullOrWhiteSpace(jwtSettings.Issuer))
    throw new InvalidOperationException("JWT Issuer is not configured.");

if (string.IsNullOrWhiteSpace(jwtSettings.Audience))
    throw new InvalidOperationException("JWT Audience is not configured.");

// Validate connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key)
            ),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.UseCors("AllowAngularDev");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();
app.Run();
