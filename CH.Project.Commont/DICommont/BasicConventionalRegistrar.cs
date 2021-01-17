using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CH.Project.Commont.DICommont
{
    public class BasicConventionalRegistrar : SingleCommont<BasicConventionalRegistrar>
    {
        private static WindsorContainer _container = new WindsorContainer();

        /// <summary>
        /// 注册程序集中满足约定的类
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public WindsorContainer RegisterAssembly(List<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                //Transient
                _container.Register(
                    Classes.FromAssembly(assembly)
                           .IncludeNonPublicTypes()
                           .InNamespace("")//待修改
                           //.BasedOn<ITransientDependency>()    //ITransientDependency 自己定义接口
                           .If(type => !type.GetTypeInfo().IsGenericTypeDefinition)
                           .WithService.Self()
                           .WithService.DefaultInterfaces()
                           .LifestyleTransient()
                );

                //Singleton
                _container.Register(
                    Classes.FromAssembly(assembly)
                           .IncludeNonPublicTypes()
                            .InNamespace("")//待修改
                                            //.BasedOn<ISingletonDependency>()      //ISingletonDependency自己定义接口
                           .If(type => !type.GetTypeInfo().IsGenericTypeDefinition)
                           .WithService.Self()
                           .WithService.DefaultInterfaces()
                           .LifestyleSingleton()
                );
            }
            return _container;
        }

        public WindsorContainer GetWindsorContainer() => _container;
    }
}
