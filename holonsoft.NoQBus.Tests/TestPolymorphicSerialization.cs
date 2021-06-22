using FluentAssertions;
using holonsoft.NoQBus.Tests.TestDtoClasses;
using System.Text.Json;
using Xunit;

namespace holonsoft.NoQBus.Tests
{
	public class TestPolymorphicSerialization
	{
		[Fact]
		public void TestBaseClass()
		{
			var options = MessageBusSinkBase.CreateSerializerOptions();

			var testObj = new TestClassWithBaseClassField() { BaseClassField = new TestInheritedA() { SomeInt = 12, SomeString = "NowWithA" } };

			string serialized = JsonSerializer.Serialize(testObj, typeof(TestClassWithBaseClassField), options);
			TestClassWithBaseClassField deserialized = (TestClassWithBaseClassField) JsonSerializer.Deserialize(serialized, typeof(TestClassWithBaseClassField), options);

			deserialized.BaseClassField.SomeString.Should().Be("NowWithA");
			deserialized.BaseClassField.Should().BeOfType<TestInheritedA>().Subject.SomeInt.Should().Be(12);

			testObj = new TestClassWithBaseClassField() { BaseClassField = new TestInheritedB() { SomeFloat = 12.5, SomeString = "NowWithB" } };

			serialized = JsonSerializer.Serialize(testObj, typeof(TestClassWithBaseClassField), options);
			deserialized = (TestClassWithBaseClassField) JsonSerializer.Deserialize(serialized, typeof(TestClassWithBaseClassField), options);

			deserialized.BaseClassField.SomeString.Should().Be("NowWithB");
			deserialized.BaseClassField.Should().BeOfType<TestInheritedB>().Subject.SomeFloat.Should().Be(12.5);


			testObj = new TestClassWithBaseClassField() { BaseClassField = new TestInheritedB() { SomeFloat = 12.5, SomeString = "NowWithB" } };

			testObj.BaseClassField2 = testObj.BaseClassField;

			serialized = JsonSerializer.Serialize(testObj, typeof(TestClassWithBaseClassField), options);
			deserialized = (TestClassWithBaseClassField) JsonSerializer.Deserialize(serialized, typeof(TestClassWithBaseClassField), options);

			deserialized.BaseClassField.SomeString.Should().Be("NowWithB");
			deserialized.BaseClassField.Should().BeOfType<TestInheritedB>().Subject.SomeFloat.Should().Be(12.5);
			Assert.True(object.ReferenceEquals(deserialized.BaseClassField, deserialized.BaseClassField2));
		}
	}
}
