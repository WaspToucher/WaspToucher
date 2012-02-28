using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Microsoft.Practices.ServiceLocation;

namespace WaspToucher
{
    public class BootStrapper
    {
        public static void Initialise()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsImplementedInterfaces();

            IContainer serviceContainer = builder.Build(Autofac.Builder.ContainerBuildOptions.Default);
            IServiceLocator provider = new AutofacContrib.CommonServiceLocator.AutofacServiceLocator(serviceContainer);
            ServiceLocator.SetLocatorProvider(() => provider);
        }
    }
}