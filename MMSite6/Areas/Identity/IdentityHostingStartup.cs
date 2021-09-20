using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MMSite6.Areas.Identity.Data;
using MMSite6.Models;

[assembly: HostingStartup(typeof(MMSite6.Areas.Identity.IdentityHostingStartup))]
namespace MMSite6.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<MMSite6Context>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("MMSite6ContextConnection")));

                services.AddDefaultIdentity<MMSite6User>()
                    .AddEntityFrameworkStores<MMSite6Context>();
            });
        }
    }
}