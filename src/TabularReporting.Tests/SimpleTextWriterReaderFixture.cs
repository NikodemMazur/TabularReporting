using System;
using System.IO;
using Xunit;

namespace TabularReporting.Tests
{
    public class SimpleTextWriterReaderFixture
    {
        [Fact]
        public void WriteReadWorksCorrectly()
        {
            var expectedContent = $"Report content start.{Environment.NewLine}Report content end.";
            var fileDir = Path.GetTempPath();
            var fileName = $"{nameof(WriteReadWorksCorrectly)}TestFile";

            var sutWriter = new SimpleTextWriter();
            var sutReader = new SimpleTextReader();

            var filePath = sutWriter.WriteReport(expectedContent, fileDir, fileName);
            var actualContent = sutReader.ReadReport(filePath);

            File.Delete(filePath);

            Assert.Equal(expectedContent, actualContent);
        }
    }
}