using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomNumbers.Utils;

namespace RandomNumbers.Tests {
    /// <summary>
    /// 13: Cumulative Sums (Cusum) Test
    /// </summary>
    /// <remarks>
    /// The focus of this test is the maximal excursion (from zero) of the random walk defined by the cumulative
    /// sum of adjusted (-1, +1) digits in the sequence. The purpose of the test is to determine whether the
    /// cumulative sum of the partial sequences occurring in the tested sequence is too large or too small relative
    /// to the expected behavior of that cumulative sum for random sequences. This cumulative sum may be
    /// considered as a random walk. For a random sequence, the excursions of the random walk should be near
    /// zero. For certain types of non-random sequences, the excursions of this random walk from zero will be
    /// large.
    /// </remarks>
    public class CumulativeSums : Test {

        /// <summary>
        /// Decision Rule (at the 1% Level)
        /// </summary>
        private const double ALPHA = 0.01;

        /// <summary>
        /// The length of the bit string
        /// </summary>
        private int n { get; set; }
        /// <summary>
        /// Mode to run the test in (forward/backward)
        /// </summary>
        /// <remarks>
        /// Forward mode when true, backwards otherwise
        /// </remarks>
        private bool mode { get; set; }

        /// <summary>
        /// Constructor of test, supplied with all neccessary values
        /// </summary>
        /// <param name="forward">If true the test will be performed forwards through the string, backwards otherwise</param>
        /// <param name="n">The length of the bit string</param>
        /// <param name="model">Model containing the the binary string</param>
        /// <exception cref="ArgumentException"/>
        public CumulativeSums(bool forward, int n, ref Model model)
            : base(ref model) {
            if (n > model.epsilon.Count || n <= 0) {
                throw new ArgumentException("The value of n must be smaller than the size of the input data, and be greater than 0", "Frequency n");
            }
            this.mode = forward;
            this.n = n;
        }

        /// <summary>
        /// Runs the test
        /// </summary>
        /// <param name="printResults">If true text output will be added to a log, otherwise not</param>
        /// <returns>The p_value(s) of the test based upon the input data</returns>
        public override double[] run(bool printResults) {
            int S = 0;
            int sup = 0;
            int inf = 0;
            int z = 0;
            
            for (int k = 0; k < n; k++) {
                //calculate the partial sum
                if (model.epsilon[k] == 1) {
                    S++;
                } else {
                    S--;
                }
                if (S > sup) {
                    sup++;
                }
                if (S < inf) {
                    inf--;
                }
                //compute the test statistic
                if (mode) {
                    z = (sup > -inf) ? sup : -inf;
                } else {
                    z = (sup - S > S - inf) ? sup - S : S - inf;
                }
            }

            //compute p_value
            double sum1 = 0.0;
            for (int k = (-n / z + 1) / 4; k <= (n / z - 1) / 4; k++) {
                sum1 += Cephes.normal(((4 * k + 1) * z) / Math.Sqrt(n));
                sum1 -= Cephes.normal(((4 * k - 1) * z) / Math.Sqrt(n));
            }
            double sum2 = 0.0;
            for (int k = (-n / z - 3) / 4; k <= (n / z - 1) / 4; k++) {
                sum2 += Cephes.normal(((4 * k + 3) * z) / Math.Sqrt(n));
                sum2 -= Cephes.normal(((4 * k + 1) * z) / Math.Sqrt(n));
            }
            double p_value = 1.0 - sum1 + sum2;

            if (printResults) {
                Report report = new Report("13: Cumulative Sums (Cusum) Test");
                report.Write("\t\t      CUMULATIVE SUMS (" + (mode ? "FORWARD" : "REVERSE") + ") TEST");
                report.Write("\t\t-------------------------------------------");
                report.Write("\t\tCOMPUTATIONAL INFORMATION:");
                report.Write("\t\t-------------------------------------------");
                report.Write("\t\t(a) The maximum partial sum = "+ z);
                report.Write("\t\t-------------------------------------------");

                if (p_value < 0 || p_value > 1) {
                    report.Write("\t\tWARNING:  P_VALUE IS OUT OF RANGE");
                }

                report.Write((p_value < ALPHA ? "FAILURE" : "SUCCESS")+"\t\tp_value = "+p_value);
                model.reports.Add(report.title, report);
            }

            return new double[] { p_value };
        }

        /// <summary>
        /// Title of test and key data the object contains
        /// </summary>
        /// <returns>String with title and data</returns>
        public override string ToString() {
            return "13: Cumulative Sums (Cusum) Test to run " + (mode ? "forwards" : "backwards") + " on " + n + " bits";
        }



    }
}
