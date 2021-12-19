
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using System.Security.Cryptography.X509Certificates;
using DDTV_Core.SystemAssembly.ConfigModule;

namespace DDTV_WEB_API//DDTVLiveRecWebServer
{
    public class Program
    {
        private static bool IsSSL = bool.Parse(CoreConfig.GetValue(CoreConfigClass.Key.WEB_API_SSL, "false", CoreConfigClass.Group.WEB_API));
        private static string pfxFileName = CoreConfig.GetValue(CoreConfigClass.Key.pfxFileName, "pfxFileName", CoreConfigClass.Group.WEB_API);
        private static string pfxPasswordFileName = CoreConfig.GetValue(CoreConfigClass.Key.pfxPasswordFileName, "pfxPasswordFileName", CoreConfigClass.Group.WEB_API);
        public static void Main(string[] args)
        {
            {
                DDTV_Core.InitDDTV_Core.Core_Init(DDTV_Core.InitDDTV_Core.SatrtType.DDTV_WEB);
                WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
                builder.Host.ConfigureServices(Services =>
                {
                    Services.AddControllers();
                    Services.AddEndpointsApiExplorer();
                    Services.AddSwaggerGen();
                    Services.AddMvc();
                    Services.AddControllers();
                    //注册Cookie认证服务
                    Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, option =>
                        {
                            option.AccessDeniedPath = "/LoginErrer"; //当用户尝试访问资源但没有通过任何授权策略时，这是请求会重定向的相对路径资源
                            option.LoginPath = "/login/";
                            option.Cookie.Name = "User";//设置存储用户登录信息（用户Token信息）的Cookie名称
                            option.Cookie.HttpOnly = true;//设置存储用户登录信息（用户Token信息）的Cookie，无法通过客户端浏览器脚本(如JavaScript等)访问到
                                                          //option.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                                                          //设置存储用户登录信息（用户Token信息）的Cookie，只会通过HTTPS协议传递，如果是HTTP协议，Cookie不会被发送。注意，option.Cookie.SecurePolicy属性的默认值是Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest
                        });

                });
                //builder.Host.ConfigureWebHost(webBuilder =>
                //{
                //    SetAPP(webBuilder);
                //});
                builder.Services.AddSwaggerGen();
                if (IsSSL)
                {
                    builder.WebHost.ConfigureKestrel(options =>
                    {
                        options.ConfigureHttpsDefaults(httpsOptions =>
                        {                         
                            var certPath = Path.Combine(builder.Environment.ContentRootPath, pfxFileName);
                            var keyPath = Path.Combine(builder.Environment.ContentRootPath, pfxPasswordFileName);
                            httpsOptions.ServerCertificate = X509Certificate2.CreateFromPemFile(certPath,keyPath);
                        });
                    });
                }
                var app = builder.Build();
                
                //用于检测是否为开发环境
                //if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }
                
                //app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();
                app.UseFileServer(new FileServerOptions()
                {
                    EnableDirectoryBrowsing = false,//关闭目录结构树访问权限
                    FileProvider = new PhysicalFileProvider(DDTV_Core.Tool.FileOperation.CreateAll(Environment.CurrentDirectory + @"/static")),
                    RequestPath = new PathString("/static")
                });
                app.Urls.Add("http://0.0.0.0:30086");
                app.Urls.Add("https://0.0.0.0:30087");
                app.Run();

            }
        }
    }
}