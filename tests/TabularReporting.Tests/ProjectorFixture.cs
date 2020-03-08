using TabularReporting.Abstractions;
using Moq;
using System.Collections.Generic;
using Xunit;
using System;
using System.Collections;
using System.Xml.Linq;
using System.Linq;

namespace TabularReporting.Tests
{
    // for convenience
    using ColQueryUnion = Union2<IEnumerable<IRowQuery>, object>;

    public class FakeType : IEnumerable<FakeType>
    {
        readonly IEnumerable<FakeType> _innerElements;
        readonly string _stepType;
        readonly double _numeric;
        readonly bool _passFail;
        readonly string _unit;

        public virtual IEnumerable<FakeType> InnerElements => _innerElements;
        public virtual string StepType => _stepType;
        public virtual double Numeric => _numeric;
        public virtual bool PassFail => _passFail;
        public virtual string Unit => _unit;

        public FakeType(IEnumerable<FakeType> innerElements)
        {
            _innerElements = innerElements ?? throw new ArgumentNullException(nameof(innerElements));
        }

        public FakeType(params FakeType[] innerElements) : this(innerElements.AsEnumerable()) { }

        public FakeType(double numeric, string unit = "V")
        {
            _stepType = "NumericTest";
            _numeric = numeric;
            _unit = unit;
        }

        public FakeType(bool passFail)
        {
            _stepType = "PassFailTest";
            _passFail = passFail;
            _unit = "1";
        }

        public FakeType(string unit, params double[] measurements)
        {
            _stepType = "MultiNumericTest";
            var measList = new List<FakeType>();
            foreach (var meas in measurements)
            {
                measList.Add(new FakeType(meas, unit));
            }
            _innerElements = measList;
        }

        public IEnumerator<FakeType> GetEnumerator()
        {
            return _innerElements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ProjectorFixture
    {
        readonly FakeType _type;

        public ProjectorFixture()
        {
            _type = new FakeType(new FakeType(false),
                                 new FakeType(7),
                                 new FakeType("A", 1, 2, 3));
        }

        Mock<ISourcedColumnQuery<FakeType>> MockSrcColQueryRetNum()
        {
            var mock = new Mock<ISourcedColumnQuery<FakeType>>(MockBehavior.Strict);
            mock.SetupAllProperties();
            mock.Setup(scq => scq.Content).Returns(() =>
                new ColQueryUnion.Case2(mock.Object.Source.Numeric.ToString()));
            return mock;
        }

        Mock<ISourcedColumnQuery<FakeType>> MockSrcColQueryRetPassFail()
        {
            var mock = new Mock<ISourcedColumnQuery<FakeType>>(MockBehavior.Strict);
            mock.SetupAllProperties();
            mock.Setup(scq => scq.Content).Returns(() =>
                new ColQueryUnion.Case2(mock.Object.Source.PassFail.ToString()));
            return mock;
        }

        Mock<ISourcedColumnQuery<FakeType>> MockSrcColQueryRetUnit()
        {
            var mock = new Mock<ISourcedColumnQuery<FakeType>>(MockBehavior.Strict);
            mock.SetupAllProperties();
            mock.Setup(scq => scq.Content).Returns(() =>
                new ColQueryUnion.Case2(mock.Object.Source.Unit.ToString()));
            return mock;
        }

        Mock<ISourcedRowQuery<FakeType>> MockSrcRowQueryFiltByStepType(string stepType, IColumnQuery[] columnQueries)
        {
            var mock = new Mock<ISourcedRowQuery<FakeType>>(MockBehavior.Strict);
            mock.SetupAllProperties();
            mock.Setup(scq => scq.Predicate).Returns(() => mock.Object.Source.StepType == stepType);
            mock.Setup(scq => scq.ColumnQueries).Returns(columnQueries);
            return mock;
        }

        Mock<ISourcedRowQuery<FakeType>> MockSrcRowQueryAllRows(IColumnQuery[] columnQueries)
        {
            var mock = new Mock<ISourcedRowQuery<FakeType>>(MockBehavior.Strict);
            mock.SetupAllProperties();
            mock.Setup(scq => scq.Predicate).Returns(() => true);
            mock.Setup(scq => scq.ColumnQueries).Returns(columnQueries);
            return mock;
        }


        [Fact]
        public void ProjectsNumToColumn()
        {
            string expected = "7";
            FakeType source = new FakeType(7);
            var mockColQuery = MockSrcColQueryRetNum();

            var actual = new Reporter<FakeType>().Report(source, mockColQuery.Object).Content.Extract(rq => null, obj => obj.ToString());

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreatesReportOfThreeTestStepTypes()
        {
            // Column from Numeric property
            var colQueryNumeric = MockSrcColQueryRetNum().Object;

            // Column from Unit property
            var colQueryUnit = MockSrcColQueryRetUnit().Object;

            // Column from PassFail property
            var colQueryPassFail = MockSrcColQueryRetPassFail().Object;

            // NumericTest rows
            var rowQueryNumericStep = MockSrcRowQueryFiltByStepType("NumericTest", new[] { colQueryNumeric, colQueryUnit }).Object;

            // PassFailTest rows
            var rowQueryPassFailStep = MockSrcRowQueryFiltByStepType("PassFailTest", new[] { colQueryPassFail, colQueryUnit }).Object;

            // Column from Measurement[] Property
            var colQueryMeasurement = new ColumnQueryWithRows(new[] { MockSrcRowQueryAllRows(new[] { colQueryNumeric,
                colQueryUnit }).Object });

            // MultiNumericTest rows
            var rowQueryMultiNumericStep = MockSrcRowQueryFiltByStepType("MultiNumericTest", new[] { colQueryMeasurement }).Object;

            // Body description
            var rowBodyDesc = new OneTimeRowQuery(new[] { new ColumnQueryWithStr("Value"),
                new ColumnQueryWithStr("Unit")});

            // Body
            var colQueryBody = new ColumnQueryWithRows(new IRowQuery[] { rowBodyDesc,
                                                                         rowQueryNumericStep,
                                                                         rowQueryPassFailStep,
                                                                         rowQueryMultiNumericStep});

            // Header field name 0
            var colQueryStation = new ColumnQueryWithStr("Test station");

            // Header field value 0
            var colQueryStationValue = new ColumnQueryWithStr("Universal Test Station no 5.");

            // Header field name 1
            var colQuerySerialNumber = new ColumnQueryWithStr("Serial number");

            // Header field value 1
            var colQuerySerialNumberValue = new ColumnQueryWithStr("XYZ123");

            // Header
            var colQueryHeader =
                new ColumnQueryWithRows(new[] {new OneTimeRowQuery(new[] { colQueryStation,
                                                                           colQueryStationValue }),
                                               new OneTimeRowQuery(new[] { colQuerySerialNumber,
                                               colQuerySerialNumberValue }) });

            // Report
            var colQueryReport = new ColumnQueryWithRows(new[] { new OneTimeRowQuery(new[] { colQueryHeader }),
                                                                 new OneTimeRowQuery(new[] { colQueryBody }) });

            XNode colXmlNode = new Reporter<FakeType>().Report(_type, colQueryReport).ToXml();
            string actualXmlStr = colXmlNode.ToString();

            Assert.Equal(ExpectedTestResults.ExpectedReportAsXml, actualXmlStr);
        }
    }
}