using TabularReporting.Abstractions;
using NationalInstruments.TestStand.Interop.API;

namespace TabularReporting.TestStand
{
    public class UnrunnableDemo
    {
        /// <summary>
        /// This method has been never run.
        /// </summary>
        public void Show()
        {
            PropertyObject mainSequenceResult = null;

            // 1. Define column descriptions.
            IRowQuery columnNamesRow =
                FluentRowBuilder.CreateRowBuilder().AddColWithStr("No.")
                                                   .AddColWithStr("Name")
                                                   .AddColWithStr("Result")
                                                   .AddColWithStr("Time")
                                                   .BuildOneTimeRow();

            // 2. Define rows made from NumericLimitTest steps.
            IRowQuery numericLimitTestRow =
                FluentRowBuilder.CreateRowBuilder().AddColCounter()
                                                   .AddColWithFormattedValue("TS.StepName")
                                                   .AddColWithFormattedValue("Value: ", "Numeric", string.Empty)
                                                   .AddColNumericDiff(mainSequenceResult.GetValNumber("TS.StartTime", 0x0), "TS.StartTime")
                                                   .BuildRowByStepType("NumericLimitTest");

            // 3. Define rows made from MultipleNumericLimitTest steps.
            IRowQuery multipleNumericLimitTestRow =
                FluentRowBuilder.CreateRowBuilder().AddColCounter()
                                                   .AddColWithFormattedValue("TS.StepName")
                                                   .AddColWithRowsFromPropertyObject("Measurement",
                                                        FluentRowBuilder.CreateRowBuilder().AddColWithFormattedValue("Data", "%.3f").BuildEveryRow())
                                                   .AddColNumericDiff(mainSequenceResult.GetValNumber("TS.StartTime", 0x0), "TS.StartTime")
                                                   .BuildRowByStepType("NI_MultipleNumericLimitTest");

            // 4. Report (the projection happens here).
            IColumn reportColumn = new Reporter().Report(mainSequenceResult, columnNamesRow, numericLimitTestRow, multipleNumericLimitTestRow);

            // 5. Format to textual table.
            string reportStr = new SimpleTextFormatter().Format(reportColumn);
        }
    }
}