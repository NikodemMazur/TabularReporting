using Moq;
using NationalInstruments.TestStand.Interop.API;
using Xunit;

namespace TabularReporting.TestStand.Tests
{
    public class PropertyObjectExtensionsFixture
    {
        [Fact]
        public void MatchesLocationWorksAsExpected()
        {
            var parentParent = Mock.Of<PropertyObject>(po => po.Name == "TS");
            var parent = Mock.Of<PropertyObject>(po => po.Parent == parentParent && po.Name == "Error");
            var propObj = Mock.Of<PropertyObject>(po => po.Parent == parent && po.Name == "Msg");

            var shouldBeTrue = PropertyObjectExtensions.MatchesLocation(propObj, "TS.Error.Msg");
            var shouldBeFalse = PropertyObjectExtensions.MatchesLocation(propObj, "Error.Msg");

            Assert.True(shouldBeTrue);
            Assert.False(shouldBeFalse);
        }

        [Fact]
        public void MatchesLocationReturnsFalseAtEmptyName()
        {
            var propObjWithEmptyName = Mock.Of<PropertyObject>(po => po.Name == string.Empty);

            var shouldBeFalse = PropertyObjectExtensions.MatchesLocation(propObjWithEmptyName, "Any.Any.Any");

            Assert.False(shouldBeFalse);
        }
    }
}