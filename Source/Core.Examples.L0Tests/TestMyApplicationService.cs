using Core.Examples.L0Tests.Application;
using Core.Examples.L0Tests.Domain;
using LeanTest.Core.ExecutionHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Examples.L0Tests
{
	/// <summary>Note that in a real-world example we must choose an IoC container - here we have implemented our own for
	/// this example.
	/// This container must be initialized in an AssemblyInitializer class - refer to this class to see how it is
	/// done with our example IoC container.
	/// For the example to be more complete, there should be two projects, one for test one for what is being tested.</summary>
	[TestClass]
	public class TestMyApplicationService
	{
		private ContextBuilder _contextBuilder;
		private MyApplicationService _target;

		[TestInitialize]
		public void TestInitialize()
		{
			_contextBuilder = ContextBuilderFactory.CreateContextBuilder()
				.WithData<MyData>()
				.Build();

			_target = _contextBuilder.GetInstance<MyApplicationService>();
		}
		#region Example of existing state
		[TestMethod]
		public void GetAgeMustReturn10WhenKeyMatchesNewedUpData()
		{
			#region Example of using a builder pattern
			_contextBuilder
				.WithData(new MyData { Age = 10, Key = "ac_32_576259321" })
				.WithData(new MyOtherData { OtherAge = 10, OtherKey = "ac_32_576259321" })
				.Build();
			#endregion

			int actual = _target.GetAge("FourtyTwo");

			Assert.AreEqual(10, actual);
		}
		#endregion
	}
}
