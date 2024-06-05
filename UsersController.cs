[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly ForumContext _context;

    public UsersController(ForumContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok(user);
    }

    [HttpPut("{id}/activate")]
    [Authorize(Roles = "Administrator")]
    public IActionResult ActivateUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null)
        {
            return NotFound();
        }

        user.IsActive = true;
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPut("{id}/deactivate")]
    [Authorize(Roles = "Administrator")]
    public IActionResult DeactivateUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null)
        {
            return NotFound();
        }

        user.IsActive = false;
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPut("{id}/role")]
    [Authorize(Roles = "Administrator")]
    public IActionResult UpdateUserRole(int id, [FromBody] Role role)
    {
        var user = _context.Users.Find(id);
        if (user == null)
        {
            return NotFound();
        }

        user.Role = role;
        _context.SaveChanges();

        return NoContent();
    }
}
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ForumContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<ForumContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication();
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdministratorRole",
                 policy => policy.RequireRole("Administrator"));
            options.AddPolicy("RequireModeratorRole",
                 policy => policy.RequireRole("Moderator"));
        });

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}