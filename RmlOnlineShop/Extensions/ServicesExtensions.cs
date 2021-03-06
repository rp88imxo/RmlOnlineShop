﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using RmlOnlineShop.Database.DatabaseContext;
using RmlOnlineShop.Data.Models;
using RmlOnlineShop.Application.DataServices.Interfaces;
using RmlOnlineShop.Application.DataServices;
using RmlOnlineShop.Application.LogicServices.Interfaces;
using RmlOnlineShop.Application.LogicServices;

namespace RmlOnlineShop.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddDefaultServiceSetup(this IServiceCollection services, IConfiguration config)
        {
            services.AddControllersWithViews();

            // MSSQL Db service setup
            services.AddDbContext<ApplicationDbContext>(
                o => o.UseSqlServer(config.GetConnectionString("DefaultConnection"))
                );

            // Add Identity
            services.AddIdentity<User, IdentityRole>(o=> 
            {
                o.User.RequireUniqueEmail = true;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireDigit = false;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddAuthorization(opt => {
                opt.AddPolicy("Admin", policy => policy.RequireClaim("Role", "Admin"));
                opt.AddPolicy("User", policy => policy.RequireClaim("Role", "User"));
            });
            
            services.AddRazorPages().AddRazorRuntimeCompilation();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60.0);
                options.Cookie.Name = "UserSession";
                options.Cookie.MaxAge = TimeSpan.FromMinutes(60.0);
            });

        }

        public static void AddCustomServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IProductManager, ProductManager>();
            services.AddScoped<IStockManager, StockManager>();


            services.AddScoped<IAdminDashboardLogic, AdminDashboardLogic>();
            services.AddScoped<ICartLogic, CartLogic>();
            services.AddScoped<IClientLogic, ClientLogic>();

        }
    }
}
