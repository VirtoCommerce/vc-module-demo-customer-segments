using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VirtoCommerce.CoreModule.Core.Conditions;
using VirtoCommerce.CustomerModule.Data.Search.Indexing;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Events;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Models;
using VirtoCommerce.DemoCustomerSegmentsModule.Core.Services;
using VirtoCommerce.DemoCustomerSegmentsModule.Data.Handlers;
using VirtoCommerce.DemoCustomerSegmentsModule.Data.Repositories;
using VirtoCommerce.DemoCustomerSegmentsModule.Data.Search.Indexing;
using VirtoCommerce.DemoCustomerSegmentsModule.Data.Services;
using VirtoCommerce.DemoCustomerSegmentsModule.Web.JsonConverters;
using VirtoCommerce.Platform.Core.Bus;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.DemoCustomerSegmentsModule.Web
{
    public class Module : IModule
    {
        public ManifestModuleInfo ModuleInfo { get; set; }

        public void Initialize(IServiceCollection serviceCollection)
        {
            // database initialization
            serviceCollection.AddDbContext<DemoCustomerSegmentDbContext>((provider, options) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                options.UseSqlServer(configuration.GetConnectionString(ModuleInfo.Id) ?? configuration.GetConnectionString("VirtoCommerce"));
            });

            serviceCollection.AddTransient<IDemoCustomerSegmentRepository, DemoCustomerSegmentRepository>();
            serviceCollection.AddTransient<Func<IDemoCustomerSegmentRepository>>(provider => () => provider.CreateScope().ServiceProvider.GetRequiredService<IDemoCustomerSegmentRepository>());
            serviceCollection.AddTransient<IDemoCustomerSegmentService, DemoCustomerSegmentService>();
            serviceCollection.AddTransient<IDemoCustomerSegmentSearchService, DemoCustomerSegmentSearchService>();
            serviceCollection.AddTransient<IUserGroupEvaluator, UserGroupEvaluator>();
            serviceCollection.AddTransient<LogChangesEventHandler>();
            serviceCollection.AddTransient<CacheCustomerSegmentChangedEventHandler>();
            serviceCollection.AddTransient<IndexCustomerSegmentChangedEventHandler>();
            serviceCollection.AddSingleton<MemberDocumentChangesProvider, DemoMemberDocumentChangesProvider>();
            serviceCollection.AddSingleton<MemberDocumentBuilder, DemoMemberDocumentBuilder>();
        }

        public void PostInitialize(IApplicationBuilder appBuilder)
        {
            var mvcJsonOptions = appBuilder.ApplicationServices.GetService<IOptions<MvcNewtonsoftJsonOptions>>();
            mvcJsonOptions.Value.SerializerSettings.Converters.Add(new PolymorphicCustomerSegmentJsonConverter());

            AbstractTypeFactory<IConditionTree>.RegisterType<DemoBlockCustomerSegmentRule>();
            AbstractTypeFactory<IConditionTree>.RegisterType<DemoConditionPropertyValues>();

            var inProcessBus = appBuilder.ApplicationServices.GetService<IHandlerRegistrar>();
            inProcessBus.RegisterHandler<DemoCustomerSegmentChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<LogChangesEventHandler>().Handle(message));
            inProcessBus.RegisterHandler<DemoCustomerSegmentChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<CacheCustomerSegmentChangedEventHandler>().Handle(message));

            var settingsManager = appBuilder.ApplicationServices.GetService<ISettingsManager>();
            if (settingsManager.GetValue(CustomerModule.Core.ModuleConstants.Settings.General.EventBasedIndexation.Name, false))
            {
                inProcessBus.RegisterHandler<DemoCustomerSegmentChangedEvent>(async (message, token) => await appBuilder.ApplicationServices.GetService<IndexCustomerSegmentChangedEventHandler>().Handle(message));
            }

            var memberIndexingConfigurations = appBuilder.ApplicationServices.GetServices<IndexDocumentConfiguration>();
            if (memberIndexingConfigurations != null)
            {
                var changesProvider = appBuilder.ApplicationServices.GetService<MemberDocumentChangesProvider>();
                var documentBuilder = appBuilder.ApplicationServices.GetService<MemberDocumentBuilder>();

                foreach (var configuration in memberIndexingConfigurations.Where(c => c.DocumentType == KnownDocumentTypes.Member))
                {
                    configuration.DocumentSource.ChangesProvider = changesProvider;
                    configuration.DocumentSource.DocumentBuilder = documentBuilder;
                }
            }

            // Ensure that any pending migrations are applied
            using var serviceScope = appBuilder.ApplicationServices.CreateScope();
            using var dbContext = serviceScope.ServiceProvider.GetRequiredService<DemoCustomerSegmentDbContext>();
            dbContext.Database.EnsureCreated();
            dbContext.Database.Migrate();
        }

        public void Uninstall()
        {
            // do nothing in here
        }
    }
}
