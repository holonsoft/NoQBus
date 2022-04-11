using FluentAssertions;
using holonsoft.NoQBus.Serialization;
using holonsoft.NoQBus.Tests.TestDtoClasses;
using Xunit;

namespace holonsoft.NoQBus.Tests;

public class TestPolymorphicSerialization
{
  [Fact]
  public void TestBaseClass()
  {
    var serializer = new MessageSerializer();

    var testObj = new TestClassWithBaseClassField() { BaseClassField = new TestInheritedA() { SomeInt = 12, SomeString = "NowWithA" } };

    var serialized = serializer.Serialize(testObj);
    var deserialized = (TestClassWithBaseClassField) serializer.Deserialize(typeof(TestClassWithBaseClassField), serialized);

    deserialized.BaseClassField.SomeString.Should().Be("NowWithA");
    deserialized.BaseClassField.Should().BeOfType<TestInheritedA>().Subject.SomeInt.Should().Be(12);

    testObj = new TestClassWithBaseClassField() { BaseClassField = new TestInheritedB() { SomeFloat = 12.5, SomeString = "NowWithB" } };

    serialized = serializer.Serialize(testObj);
    deserialized = (TestClassWithBaseClassField) serializer.Deserialize(typeof(TestClassWithBaseClassField), serialized);

    deserialized.BaseClassField.SomeString.Should().Be("NowWithB");
    deserialized.BaseClassField.Should().BeOfType<TestInheritedB>().Subject.SomeFloat.Should().Be(12.5);


    testObj = new TestClassWithBaseClassField() { BaseClassField = new TestInheritedB() { SomeFloat = 12.5, SomeString = "NowWithB" } };

    testObj.BaseClassField2 = testObj.BaseClassField;

    serialized = serializer.Serialize(testObj);
    deserialized = (TestClassWithBaseClassField) serializer.Deserialize(typeof(TestClassWithBaseClassField), serialized);

    deserialized.BaseClassField.SomeString.Should().Be("NowWithB");
    deserialized.BaseClassField.Should().BeOfType<TestInheritedB>().Subject.SomeFloat.Should().Be(12.5);
    Assert.True(object.ReferenceEquals(deserialized.BaseClassField, deserialized.BaseClassField2));
  }
}
