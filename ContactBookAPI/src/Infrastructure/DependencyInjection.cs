﻿using System.Data;
using ContactBookAPI.Application.Common.Interfaces;
using ContactBookAPI.Application.People;
using ContactBookAPI.Infrastructure.Data;
using ContactBookAPI.Infrastructure.Data.Interceptors;
using ContactBookAPI.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddScoped<ISaveChangesInterceptor, OverrideEntitiesInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddTransient<IDbConnection>(sp =>
            new NpgsqlConnection(connectionString));

        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IPeopleQueryRepository, PeopleQueryRepository>();

        return services;
    }
}
