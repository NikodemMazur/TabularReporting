using System.Linq;
using Xunit;

namespace TabularReporting.Tests
{
    public class Union2Fixture
    {
        [Fact]
        public void ExtractWorksCorrectly()
        {
            Union2<int, string>[] unions = new Union2<int, string>[]
                {
                    new Union2<int, string>.Case1(7),
                    new Union2<int, string>.Case2("7")
                };

            var strs = unions.Select(u => u.Extract(num => num.ToString(), str => str));

            Assert.Equal(new[] { "7", "7" }, strs);
        }

        [Fact]
        public void ProcessWorksCorrectly()
        {
            Union2<int, string>[] unions = new Union2<int, string>[]
                {
                    new Union2<int, string>.Case1(7),
                    new Union2<int, string>.Case2("7")
                };

            int sum = 0;
            foreach (var union in unions)
            {
                union.Act(i => sum += i,
                              str => sum += int.Parse(str));
            }

            Assert.True(14 == sum);
        }
    }
}
