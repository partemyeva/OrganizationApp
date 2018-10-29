using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Reflection;
using Autofac;
using Autofac.Integration.WebApi;
using OrganizationApp.Models.Repository;


namespace OrganizationApp.Configuration
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            var config = GlobalConfiguration.Configuration;

            // Регистрируем контекст данных
            builder.RegisterType<EmployeeContext>();

            // Регистрируем репозиторий
            builder.RegisterType<EmployeeRepository>().InstancePerRequest().As<IRepository>();

            // Регистрируем контроллеры в текущей сборке
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            
            // Создаем новый контейнер с теми зависимостями, которые определены выше
            var container = builder.Build();

            // Установливаем сопоставитель зависимостей
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}