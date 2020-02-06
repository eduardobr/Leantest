using System;
using System.Collections.Generic;
using Core.Examples.L0Tests.Application;
using Core.Examples.L0Tests.Domain;
using Core.Examples.L0Tests.Mocks;
using LeanTest.Core.ExecutionHandling;
using LeanTest.Mock;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Examples.L0Tests.TestSetup.IoC
{
	public class IocContainer : IIocContainer
	{
		private readonly IServiceProvider _serviceProvider;
		public IocContainer(IServiceCollection serviceCollection) => _serviceProvider = serviceCollection.BuildServiceProvider();
 
		public T Resolve<T>() where T : class => _serviceProvider.GetRequiredService<T>();
		public T TryResolve<T>() where T : class => _serviceProvider.GetService<T>();
		public IEnumerable<T> TryResolveAll<T>() where T : class => _serviceProvider.GetServices<T>();
	}
	public static class L0CompositionRootForTest
	{
		public static IServiceCollection Initialize(IServiceCollection serviceCollection)
		{
			// Mocks (not mock-for-data):
			// TODO
 
			// Mock-for-data:
			serviceCollection.RegisterMockForData<IMyExternalService, MockMyExternalService, MyData, MyOtherData>();
 
			return serviceCollection;
		}

		private static void RegisterMockForData<TInterface, TImplementation, TData1, TData2>(this IServiceCollection container)
			where TImplementation: class, TInterface, IMockForData<TData1>, IMockForData<TData2>
			where TInterface: class
		{
			container.AddSingleton<TImplementation>();
			container.AddSingleton<TInterface>(x => x.GetRequiredService<TImplementation>());
			container.AddSingleton<IMockForData<TData1>>(x => x.GetRequiredService<TImplementation>());
			container.AddSingleton<IMockForData<TData2>>(x => x.GetRequiredService<TImplementation>());
		}
	}
}