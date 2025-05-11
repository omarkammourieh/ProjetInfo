public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();

    services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
}
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();

    services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
}
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseSession();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    });
}
