using System.Threading.Tasks;
using DataAccess.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models;
using Services;
using University.MVC.Authorization;

namespace University.MVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.Configure<RepositoryOptions>(Configuration);
            services.AddDbContext<UniversityContext>();
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("demo.site"));

            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
            services.Configure<RepositoryOptions>(Configuration);
            services.AddScoped<StudentService>();
            services.AddScoped<CourseService>();
            services.AddScoped<HomeTaskService>();
            services.AddDbContext<UniversityContext>();
            services.Add(ServiceDescriptor.Scoped(typeof(IRepository<>), typeof(UniversityRepository<>)));
            services.AddScoped<IAuthorizationHandler, SameStudentRequirementAuthorizationHandler>();
            services.ConfigureApplicationCookie(p =>
            {
                p.LoginPath = "/Security/Login";
                p.Cookie.Name = "University";
                p.AccessDeniedPath = "/Security/AccessDenied";
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("SameUserPolicy", builder =>
                {
                    builder.Requirements.Add(new SameStudentRequirement());
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            CreateAdminUser(userManager, roleManager).Wait();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    "{controller=Course}/{action=Courses}/{id?}");
            });
        }

        private async Task CreateAdminUser(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            IdentityUser identityUser = new IdentityUser
            {
                Email = "Admin@test.com",
                UserName = "Admin@test.com"
            };
            var userRes = await userManager.CreateAsync(identityUser, "SuperAdmin#1234");
            var rol = await roleManager.CreateAsync(new IdentityRole("Admin"));
            var res = await userManager.AddToRoleAsync(identityUser, "Admin");
            var isInRole = await userManager.IsInRoleAsync(identityUser, "Admin");
            var roles =await userManager.GetRolesAsync(identityUser);
            var allRoles =await roleManager.Roles.ToListAsync();
            var x = userManager.Options.SignIn.RequireConfirmedAccount;
        }
    }
}
