using Autofac;
using MiniCloudServer.Controllers;
using MiniCloudServer.Core;
using MiniCloudServer.Persistence;
using MiniCloudServer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniCloudServer.IoC
{
    public static class Bootstraper
    {
        public static IContainer Container { get; private set;  }
        static Bootstraper()
        {
            var builder = new ContainerBuilder();
            var asmTypes=typeof(Bootstraper).Assembly.GetTypes();

            var controllers = asmTypes.Where(x => typeof(IController).IsAssignableFrom(x) && x.IsInterface == false);
            foreach (var controller in controllers)
            {
                builder.RegisterType(controller).Named<IController>(controller.Name)
                    .InstancePerLifetimeScope();
            }

            var services= asmTypes.Where(x => typeof(IService).IsAssignableFrom(x) && x.IsInterface==false);
            foreach(var service in services)
            {
                builder.RegisterType(service).AsImplementedInterfaces().InstancePerLifetimeScope();
            }

            builder.RegisterType<MiniCloudContext>().AsSelf();

            Container = builder.Build();
        }
        
    }
}
