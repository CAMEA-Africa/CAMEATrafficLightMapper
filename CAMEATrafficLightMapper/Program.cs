using CAMEATrafficLightMapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "CAMEALightMapper";
});

builder.Services.AddControllers();
builder.Services.AddDataProtection();

builder.Services.AddSingleton<SettingsStore>();
builder.Services.AddSingleton<SyncStateStore>();
builder.Services.AddHttpClient<FeedClient>();
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

app.Run();
