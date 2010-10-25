using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomNumbers.Utils;

namespace RandomNumbers.Tests {
    /// <summary>
    /// 15: Random Excursions Variant Test
    /// </summary>
    /// <remarks>
    /// The focus of this test is the total number of times that a particular state is visited (i.e., occurs) in a
    /// cumulative sum random walk. The purpose of this test is to detect deviations from the expected number
    /// of visits to various states in the random walk. This test is actually a series of eighteen tests (and
    /// conclusions), one test and conclusion for each of the states: -9, -8, …, -1 and +1, +2, …, +9.
    /// </remarks>
    public class RandomExcursionsVariant : Test {

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
        public RandomExcursionsVariant(int n, ref Model model)
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

            int[] stateX = { -9, -8, -7, -6, -5, -4, -3, -2, -1, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            int J = 0;
            int[] S = new int[n];
            //compute partial sums
            S[0] = 2 * (int)model.epsilon[0] - 1;
            for (int i = 1; i < n; i++) {
                S[i] = S[i - 1] + 2 * model.epsilon[i] - 1;
                if (S[i] == 0) {
                    J++;
                }
            }
            if (S[n - 1] != 0) {
                J++;
            }

            Report report = new Report("15: Random Excursions Variant Test");
            if (printResults) {
                report.Write("\t\t\tRANDOM EXCURSIONS VARIANT TEST");
                report.Write("\t\t--------------------------------------------");
                report.Write("\t\tCOMPUTATIONAL INFORMATION:");
                report.Write("\t\t--------------------------------------------");
                report.Write("\t\t(a) Number Of Cycles (J) = " + J);
                report.Write("\t\t(b) Sequence Length (n)  = " + n);
            }

            double[] p_values = new double[18];
            int constraint = (int)Math.Max(0.005 * Math.Pow(n, 0.5), 500);
            if (J < constraint) {
                if (printResults) {
                    report.Write("\t\tWARNING:  TEST NOT APPLICABLE.  THERE ARE AN");
                    report.Write("\t\t\t  INSUFFICIENT NUMBER OF CYCLES.");
                    report.Write("\t\t---------------------------------------------");
                }
                for (int i = 0; i < 18; i++) {
                    report.Write(0.0.ToString());
                }
            } else {
                for (int p = 0; p < 18; p++) {
                    int x = stateX[p];
                    int count = 0;
                    //count occurences of state x
                    for (int i = 0; i < n; i++) {
                        if (S[i] == x) {
                            count++;
                        }
                    }
                    //compute p_value
                    p_values[p] = Cephes.erfc(Math.Abs(count - J) / (Math.Sqrt(2.0 * J * (4.0 * Math.Abs(x) - 2))));

                    if (printResults) {
                        if (p_values[p] < 0 || p_values[p] > 1) {
                            report.Write("\t\t(b) WARNING: P_VALUE IS OUT OF RANGE.");
                        }
                        report.Write(p_values[p] < ALPHA ? "FAILURE" : "SUCCESS" + "\t\t");
                        report.Write("(x = " + x + ") Total visits = " + count + "; p-value = " + p_values[p]);
                        report.Write(p_values[p].ToString());
                    }
                }
            }
            if (printResults) {
                model.reports.Add(report.title, report);
            }
            return p_values;
        }

        /// <summary>
        /// Title of test and key data the object contains
        /// </summary>
        /// <returns>String with title and data</returns>
        public override string ToString() {
            return "15: Random Excursions Variant Test to run on " + n + " bits";
        }
    }
}
