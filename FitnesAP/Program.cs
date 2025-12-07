var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// omogoèi Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddSingleton<FitnesAP.Data.UserService>();
builder.Services.AddScoped<FitnesAP.Data.ExerciseService>();
var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// omogoèi Session
app.UseSession();

app.MapRazorPages();

app.Run();


