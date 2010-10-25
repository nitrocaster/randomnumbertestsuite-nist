using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomNumbers.Utils;

namespace RandomNumbers.Tests {
    /// <summary>
    /// 3: Runs Test
    /// </summary>
    /// <remarks>
    /// The focus of this test is the total number of runs in the sequence, where a run is an uninterrupted sequence
    /// of identical bits. A run of length k consists of exactly k identical bits and is bounded before and after with
    /// a bit of the opposite value. The purpose of the runs test is to determine whether the number of runs of
    /// ones and zeros of various lengths is as expected for a random sequence. In particular, this test determines
    /// whether the oscillation between such zeros and ones is too fast or too slow.
    /// </remarks>
    public class Runs : Test {

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
        public Runs(int n, ref Model model)
            : base(ref model) {
                if (n > model.epsilon.Count || n <= 0) {
                    throw new ArgumentException("The value of n must be smaller than the size of the input data, and be greater than 0", "Runs n");
                }
                this.n = n;
        }

        /// <summary>
        /// Runs the test
        /// </summary>
        /// <param name="printResults">If true text output will be added to a log, otherwise not</param>
        /// <returns>The p_value(s) of the test based upon the input data</returns>
        public override double[] run(bool printResults) {
            
            double pi = (double)model.epsilon.GetRange(0, n).Sum() / (double)n;    //number of ones in the binary string / length

            //prerequisite for test, if not passed test is not applicable   
            if (Math.Abs(pi - 0.5) > (2.0 / Math.Sqrt(n))) {
                if (printResults) {
                    Report report = new Report("3: Runs Test");
                    report.Write("\t\t\t\tRUNS TEST");
                    report.Write("\t\t------------------------------------------");
                    report.Write("\t\tPI ESTIMATOR CRITERIA NOT MET! PI = " + pi);
                    model.reports.Add(report.title, report);
                }
                return new double[] { 0.0 };
            } else {
                //count number of runs in the binary string (where e(i) = e(i+1) is considered to be a run)
                int V_obs = 1;
                for (int i = 1; i < n; i++) {
                    if (model.epsilon[i] != model.epsilon[i - 1]) {
                        V_obs++;
                    }
                }

                //calculate p_value
                double p_value = Cephes.erfc(Math.Abs(V_obs - 2.0 * n * pi * (1 - pi)) / (2.0 * pi * (1 - pi) * Math.Sqrt(2 * n)));

                if (printResults) {
                    Report report = new Report("3: Runs Test");
                    report.Write("\t\t\t\tRUNS TEST");
                    report.Write("\t\t------------------------------------------");
                    report.Write("\t\tCOMPUTATIONAL INFORMATION:");
                    report.Write("\t\t------------------------------------------");
                    report.Write("\t\t(a) Pi                        = " + pi);
                    report.Write("\t\t(b) V_n_obs (Total # of runs) = " + (int)V_obs);
                    report.Write("\t\t(c) V_n_obs - 2 n pi (1-pi)", false);
                    report.Write("\t\t      2 sqrt(2n) pi (1-pi)");
                    report.Write("\t\t------------------------------------------");
                    if (p_value < 0 || p_value > 1) {
                        report.Write("WARNING:  P_VALUE IS OUT OF RANGE.");
                    }
                    report.Write(p_value < ALPHA ? "FAILURE" : "SUCCESS"+"\t\tp_value = "+p_value);
                    model.reports.Add(report.title, report);
                }
                return new double[] { p_value };
            }
        }

        /// <summary>
        /// Title of test and key data the object contains
        /// </summary>
        /// <returns>String with title and data</returns>
        public override string ToString() {
            return "3: Runs Test to run on " + n + " bits";
        }
    }
}
