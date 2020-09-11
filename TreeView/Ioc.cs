using Microsoft.Extensions.DependencyInjection;
using System;
using TreeView.ViewModels;

namespace TreeView
{
    public class Ioc
    {
        private static readonly IServiceProvider _provider;

        static Ioc()
        {
            var services = new ServiceCollection();
            services.AddTransient<MainViewModel>();

            _provider = services.BuildServiceProvider();
        }

        public static T Resolve<T>() => _provider.GetRequiredService<T>();
    }
}
