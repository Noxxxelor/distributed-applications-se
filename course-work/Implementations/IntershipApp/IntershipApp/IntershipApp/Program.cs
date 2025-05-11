var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Используем маршруты
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Включаем статические файлы
app.UseStaticFiles();

// Маршруты
app.UseRouting();

// Указание маршрута по умолчанию
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Student}/{action=Index}/{id?}"); // Изменяем на Student по умолчанию

app.Run();

