using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace WClipboard.Core.DI
{
    public static class Extensions
    {
        public static void AddSingleton(this IServiceCollection serviceCollection, Type implementation, Type service1, Type service2, params Type[] moreServices)
        {
            serviceCollection.AddSingleton(implementation);
            serviceCollection.AddSingleton(service1, sp => sp.GetRequiredService(implementation));
            serviceCollection.AddSingleton(service2, sp => sp.GetRequiredService(implementation));
            foreach (var service in moreServices)
            {
                serviceCollection.AddSingleton(service, sp => sp.GetRequiredService(implementation));
            }
        }

        public static void AddSingleton<TImplementation>(this IServiceCollection serviceCollection, Type service1, Type service2, params Type[] moreServices) => 
            AddSingleton(serviceCollection, typeof(TImplementation), service1, service2, moreServices);

        public static void AddSingleton<TService1, TService2, TImplementation>(this IServiceCollection serviceCollection)
            where TImplementation : class, TService1, TService2
            where TService1 : class
            where TService2 : class =>
            AddSingleton<TImplementation>(serviceCollection, typeof(TService1), typeof(TService2));
        public static void AddSingleton<TService1, TService2, TService3, TImplementation>(this IServiceCollection serviceCollection)
            where TImplementation : class, TService1, TService2, TService3
            where TService1 : class
            where TService2 : class
            where TService3 : class =>
            AddSingleton<TImplementation>(serviceCollection, typeof(TService1), typeof(TService2), typeof(TService3));
        public static void AddSingleton<TService1, TService2, TService3, TService4, TImplementation>(this IServiceCollection serviceCollection)
            where TImplementation : class, TService1, TService2, TService3, TService4
            where TService1 : class
            where TService2 : class
            where TService3 : class
            where TService4 : class =>
            AddSingleton<TImplementation>(serviceCollection, typeof(TService1), typeof(TService2), typeof(TService3), typeof(TService4));
        public static void AddSingleton<TService1, TService2, TService3, TService4, TService5, TImplementation>(this IServiceCollection serviceCollection)
            where TImplementation : class, TService1, TService2, TService3, TService4, TService5
            where TService1 : class
            where TService2 : class
            where TService3 : class
            where TService4 : class
            where TService5 : class =>
            AddSingleton<TImplementation>(serviceCollection, typeof(TService1), typeof(TService2), typeof(TService3), typeof(TService4), typeof(TService5));

        public static void AddSingleton<TImplementation>(this IServiceCollection serviceCollection, TImplementation instance, Type service1, Type service2, params Type[] moreServices) where TImplementation : notnull
        {
            serviceCollection.AddSingleton(service1, instance);
            serviceCollection.AddSingleton(service2, instance);
            foreach (var service in moreServices)
            {
                serviceCollection.AddSingleton(service, instance);
            }
        }

        public static void AddSingleton<TService1, TService2, TImplementation>(this IServiceCollection serviceCollection, TImplementation instance)
            where TImplementation : class, TService1, TService2
            where TService1 : class
            where TService2 : class =>
            AddSingleton(serviceCollection, instance, typeof(TService1), typeof(TService2));
        public static void AddSingleton<TService1, TService2, TService3, TImplementation>(this IServiceCollection serviceCollection, TImplementation instance)
            where TImplementation : class, TService1, TService2, TService3
            where TService1 : class
            where TService2 : class
            where TService3 : class =>
            AddSingleton(serviceCollection, instance, typeof(TService1), typeof(TService2), typeof(TService3));
        public static void AddSingleton<TService1, TService2, TService3, TService4, TImplementation>(this IServiceCollection serviceCollection, TImplementation instance)
            where TImplementation : class, TService1, TService2, TService3, TService4
            where TService1 : class
            where TService2 : class
            where TService3 : class
            where TService4 : class =>
            AddSingleton(serviceCollection, instance, typeof(TService1), typeof(TService2), typeof(TService3), typeof(TService4));
        public static void AddSingleton<TService1, TService2, TService3, TService4, TService5, TImplementation>(this IServiceCollection serviceCollection, TImplementation instance)
            where TImplementation : class, TService1, TService2, TService3, TService4, TService5
            where TService1 : class
            where TService2 : class
            where TService3 : class
            where TService4 : class
            where TService5 : class =>
            AddSingleton(serviceCollection, instance, typeof(TService1), typeof(TService2), typeof(TService3), typeof(TService4), typeof(TService5));

        public static void AddSingletonWithAutoInject<TService, TImplementation>(this IServiceCollection serviceCollection) where TService : class where TImplementation : class, TService
        {
            serviceCollection.AddSingleton<TImplementation>();
            serviceCollection.AddSingleton<TService>(sp => sp.GetRequiredService<TImplementation>());
            foreach (var @interface in typeof(TImplementation).GetInterfaces())
            {
                if (@interface.GetCustomAttribute<AutoInjectAttribute>() != null)
                {
                    serviceCollection.AddSingleton(@interface, sp => sp.GetRequiredService<TImplementation>());
                }
            }
        }

        public static void AddSingletonWithAutoInject<TService, TImplementation>(this IServiceCollection serviceCollection, TImplementation instance) where TService : class where TImplementation : class, TService
        {
            serviceCollection.AddSingleton<TService>(instance);
            foreach (var @interface in typeof(TImplementation).GetInterfaces())
            {
                if (@interface.GetCustomAttribute<AutoInjectAttribute>() != null)
                {
                    serviceCollection.AddSingleton(@interface, instance);
                }
            }
        }

        public static void AddSingletonWithAutoInject<TImplementation>(this IServiceCollection serviceCollection) where TImplementation : class
        {
            serviceCollection.AddSingleton<TImplementation>();
            foreach (var @interface in typeof(TImplementation).GetInterfaces())
            {
                if (@interface.GetCustomAttribute<AutoInjectAttribute>() != null)
                {
                    serviceCollection.AddSingleton(@interface, sp => sp.GetRequiredService<TImplementation>());
                }
            }
        }

        public static void AddSingletonWithAutoInject<TImplementation>(this IServiceCollection serviceCollection, TImplementation instance) where TImplementation : class
        {
            serviceCollection.AddSingleton(instance);
            foreach (var @interface in typeof(TImplementation).GetInterfaces())
            {
                if (@interface.GetCustomAttribute<AutoInjectAttribute>() != null)
                {
                    serviceCollection.AddSingleton(@interface, instance);
                }
            }
        }

        /// <summary>
        /// Registering auto injects of <paramref name="instance"/> without registering <paramref name="instance"/> itself.
        /// 
        /// </summary>
        /// <remarks>
        /// The auto injects are registered as singletons.
        /// </remarks>
        /// <param name="serviceCollection"></param>
        /// <param name="instance">Object wherefrom the auto injects should be registered</param>
        public static void AddAutoInject(this IServiceCollection serviceCollection, object instance)
        {
            foreach (var @interface in instance.GetType().GetInterfaces())
            {
                if (@interface.GetCustomAttribute<AutoInjectAttribute>() != null)
                {
                    serviceCollection.AddSingleton(@interface, instance);
                }
            }
        }

        public static void AddTransientWithAutoInject<TService, TImplementation>(this IServiceCollection serviceCollection) where TService : class where TImplementation : class, TService
        {
            serviceCollection.AddTransient<TService, TImplementation>();
            foreach (var @interface in typeof(TImplementation).GetInterfaces())
            {
                if (@interface.GetCustomAttribute<AutoInjectAttribute>() != null)
                {
                    serviceCollection.AddTransient(@interface, typeof(TImplementation));
                }
            }
        }

        public static void AddTransientWithAutoInject<TImplementation>(this IServiceCollection serviceCollection) where TImplementation : class
        {
            foreach (var @interface in typeof(TImplementation).GetInterfaces())
            {
                if (@interface.GetCustomAttribute<AutoInjectAttribute>() != null)
                {
                    serviceCollection.AddTransient(@interface, typeof(TImplementation));
                }
            }
        }

        public static Lazy<T> GetLazy<T>(this IServiceProvider serviceProvider)
        {
            return new Lazy<T>(() => serviceProvider.GetService<T>()!, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public static T Create<T>(this IServiceProvider serviceProvider, params object[] parameters)
        {
            return ActivatorUtilities.CreateInstance<T>(serviceProvider, parameters);
        }
    }
}
