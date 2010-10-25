using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomNumbers.Utils;


namespace RandomNumbers.Tests {
    /// <summary>
    /// 1: Frequency (Monobit) Test
    /// </summary>
    /// <remarks>
    /// The focus of the test is the proportion of zeroes and ones for the entire sequence. The purpose of this test
    /// to determine whether the number of ones and zeros in a sequence are approximately the same as would
    /// be expected for a truly random sequence. The test assesses the closeness of the fraction of ones to ½, that
    /// is, the number of ones and zeroes in a sequence should be about the same. All subsequent tests depend on
    /// the passing of this test.
    /// </remarks>
    public class Frequency : Test {

        /// <summary>
        /// Decision Rule (at the 1% Level)
        /// </summary>
        private const double ALPHA = 0.01;

        /// <summary>
        /// The length of the bit string
        /// </summary>
        private int n { get; set; }

        /// <summary>
        /// Constructor of test, supplied with all neccessary values
        /// </summary>
        /// <param name="n">The length of the bit string</param>
        /// <param name="model">Model containing the the binary string</param>
        /// <exception cref="ArgumentException"/>
        public Frequency(int n, ref Model model)
            : base(ref model) {
                if (n > model.epsilon.Count || n <= 0) {
                    throw new ArgumentException("The value of n must be smaller than the size of the input data, and be greater than 0", "Frequency n");
                }
                this.n = n;
        }

        /// <summary>
        /// Runs the test
        /// </summary>
        /// <param name="printResults">If true text output will be added to a log, otherwise not</param>
        /// <returns>The p_value(s) of the test based upon the input data</returns>
        public override double[] run(bool printResults) {

            //calculate the sum of all bits in the string after mapping (0->-1, 1->1)
            double S_n = model.epsilon.GetRange(0, n).Sum(delegate(int i) { return 2 * i - 1; });

            //calculate p_value
            double S_obs = Math.Abs(S_n) / Math.Sqrt(n);
            double p_value = Cephes.erfc(S_obs / Math.Sqrt(2));

            if (printResults) {
                Report report = new Report("1: Frequency (Monobit) Test");
                report.Write("\t\t\t      FREQUENCY TEST");
                report.Write("\t\t---------------------------------------------");
                report.Write("\t\tCOMPUTATIONAL INFORMATION:");
                report.Write("\t\t---------------------------------------------");
                report.Write("\t\t(a) The nth partial sum = " + (int)S_n);
                report.Write("\t\t(b) S_n/n               = " + S_n / n);
                report.Write("\t\t---------------------------------------------");
                report.Write(p_value < ALPHA ? "FAILURE" : "SUCCESS" + "\t\tp_value = " + p_value);
                model.reports.Add(report.title, report);
            }

            return new double[] { p_value };
        }

        /// <summary>
        /// Title of test and key data the object contains
        /// </summary>
        /// <returns>String with title and data</returns>
        public override string ToString() {
            return "1: Frequency Monobit Test to run on "+n+" bits";
        }
    }
}