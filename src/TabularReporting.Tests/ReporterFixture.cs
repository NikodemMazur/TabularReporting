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

    public class FakeType
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
            _stepType = "Container";
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
    }

    public class ReporterFixture
    {
        readonly IEnumerable<FakeType> _sources;

        public ReporterFixture()
        {
            _sources = 
                new[] { new FakeType(false),
                        new FakeType(7),
                        new FakeType("A", 1, 2, 3) };
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
            var colQueryMeasurement = new ColumnWithRowsBranchedQuery<FakeType>(ft => ft.InnerElements,
                new EveryRowQuery(colQueryNumeric, colQueryUnit));

            // MultiNumericTest rows
            var rowQueryMultiNumericStep = MockSrcRowQueryFiltByStepType("MultiNumericTest", new[] { colQueryMeasurement }).Object;

            // Body description
            var rowBodyDesc = new OneTimeRowQuery(new[] { new ColumnWithStrQuery("Value"),
                new ColumnWithStrQuery("Unit")});

            // Body
            var colQueryBody = new ColumnWithRowsQuery(
                    rowBodyDesc,
                    rowQueryNumericStep,
                    rowQueryPassFailStep,
                    rowQueryMultiNumericStep);

            // Header field name 0
            var colQueryStation = new ColumnWithStrQuery("Test station");

            // Header field value 0
            var colQueryStationValue = new ColumnWithStrQuery("Universal Test Station no 5.");

            // Header field name 1
            var colQuerySerialNumber = new ColumnWithStrQuery("Serial number");

            // Header field value 1
            var colQuerySerialNumberValue = new ColumnWithStrQuery("XYZ123");

            // Header
            var colQueryHeader =
                new ColumnWithRowsQuery(new OneTimeRowQuery(colQueryStation, colQueryStationValue),
                                        new OneTimeRowQuery(colQuerySerialNumber, colQuerySerialNumberValue));

            // Report
            var reportQueries = new[] { new OneTimeRowQuery(colQueryHeader),
                                        new OneTimeRowQuery(colQueryBody) };

            XNode colXmlNode = new Reporter<FakeType>().Report(_sources, reportQueries).ToXml();
            string actualXmlStr = colXmlNode.ToString();

            Assert.Equal(ExpectedTestResults.ExpectedReportAsXml, actualXmlStr);
        }
    }
}