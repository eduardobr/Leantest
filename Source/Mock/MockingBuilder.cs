using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LeanTest.Core.ExecutionHandling;

namespace LeanTest.Mock
{
    internal class MockingBuilder : IBuilder
    {
        private readonly IIocContainer _container;
        private readonly IDataStore _dataStore;
        private readonly IDictionary<Type, Func<IEnumerable<object>>> _typedMockEnumsDelegates = new Dictionary<Type, Func<IEnumerable<object>>>();

        public MockingBuilder(IIocContainer container, IDataStore dataStore)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (dataStore == null) throw new ArgumentNullException(nameof(dataStore));

            _container = container;
            _dataStore = dataStore;
        }

        public void Build()
        {
            foreach (KeyValuePair<Type, Func<IEnumerable<object>>> mockDelegatesForType in _typedMockEnumsDelegates)
            {
                IEnumerable<object> mocks = mockDelegatesForType.Value().ToArray();
                foreach (object mock in mocks)
                {
                    Type theClass = typeof(IMockForData<>).MakeGenericType(mockDelegatesForType.Key);
                    
                    MethodInfo clearMethod = theClass.GetTypeInfo().GetDeclaredMethod("Clear");
                    clearMethod.Invoke(mock, new object[] { mockDelegatesForType.Key });

                    if (Enumerable.All<KeyValuePair<Type, List<object>>>(_dataStore.TypedData, t => t.Key != mockDelegatesForType.Key))
                        continue;
                    MethodInfo withDataMethod = theClass.GetTypeInfo().GetDeclaredMethod("WithData");
                    foreach (object data in _dataStore.TypedData[mockDelegatesForType.Key])
                        withDataMethod.Invoke(mock, new[] { data });

                    MethodInfo buildMethod = theClass.GetTypeInfo().GetDeclaredMethod("Build");
                    buildMethod.Invoke(mock, null/*, new object[] { mockDelegatesForType.Key }*/); // TODO: Add a Type parameter to Build?
                }
            }
        }

        public void WithBuilderForData<T>()
        {
            _typedMockEnumsDelegates[typeof(T)] = () => from mock in _container.TryResolveAll<IMockForData<T>>() select mock as object;
        }
    }
}