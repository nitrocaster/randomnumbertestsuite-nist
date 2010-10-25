using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomNumbers.Utils;

namespace RandomNumbers.Tests {
    /// <summary>
    /// 14: Random Excursions Test
    /// </summary>
    /// <remarks>
    /// The focus of this test is the number of cycles having exactly K visits in a cumulative sum random walk.
    /// The cumulative sum random walk is derived from partial sums after the (0,1) sequence is transferred to
    /// the appropriate (-1, +1) sequence. A cycle of a random walk consists of a sequence of steps of unit length
    /// taken at random that begin at and return to the origin. The purpose of this test is to determine if the
    /// number of visits to a particular state within a cycle deviates from what one would expect for a random
    /// sequence. This test is actually a series of eight tests (and conclusions), one test and conclusion for each of
    /// the states: -4, -3, -2, -1 and +1, +2, +3, +4.
    /// </remarks>
    public class RandomExcursions : Test {

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
        public RandomExcursions(int n, ref Model model)
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

            int[] stateX = { -4, -3, -2, -1, 1, 2, 3, 4 };
  
            double[,] pi = { {0.0000000000, 0.00000000000, 0.00000000000, 0.00000000000, 0.00000000000, 0.0000000000}, 
						     {0.5000000000, 0.25000000000, 0.12500000000, 0.06250000000, 0.03125000000, 0.0312500000},
						     {0.7500000000, 0.06250000000, 0.04687500000, 0.03515625000, 0.02636718750, 0.0791015625},
						     {0.8333333333, 0.02777777778, 0.02314814815, 0.01929012346, 0.01607510288, 0.0803755143},
						     {0.8750000000, 0.01562500000, 0.01367187500, 0.01196289063, 0.01046752930, 0.0732727051} };

            Report report = new Report("14: Random Excursions Test");   

            //determine cycles
            int J = 0;
            int[] S_k = new int[n];
            S_k[0] = 2 * (int)model.epsilon[0] - 1;
            int[] cycle = new int[Math.Max(1000, n / 100)];
            for (int i = 1; i < n; i++) {
                S_k[i] = S_k[i - 1] + 2 * model.epsilon[i] - 1;
                if (S_k[i] == 0) {
                    J++;
                    if (J > Math.Max(1000, n / 100)) {
                        if (printResults) {
                            report.Write("ERROR IN FUNCTION randomExcursions:  EXCEEDING THE MAX NUMBER OF CYCLES EXPECTED.");
                        }
                        return null;
                    }
                    cycle[J] = i;
                }
            }
            if (S_k[n - 1] != 0) {
                J++;
            }
            cycle[J] = n;

            if (printResults) {
                report.Write("\t\t\t  RANDOM EXCURSIONS TEST");
                report.Write("\t\t--------------------------------------------");
                report.Write("\t\tCOMPUTATIONAL INFORMATION:");
                report.Write("\t\t--------------------------------------------");
                report.Write("\t\t(a) Number Of Cycles (J) = " + J);
                report.Write("\t\t(b) Sequence Length (n)  = " + n);
            }

            double constraint = Math.Max(0.005 * Math.Pow(n, 0.5), 500);
            double[] p_values = new double[8];
            if (J < constraint) {
                if (printResults) {
                    report.Write("\t\t---------------------------------------------");
                    report.Write("\t\tWARNING:  TEST NOT APPLICABLE.  THERE ARE AN");
                    report.Write("\t\t\t  INSUFFICIENT NUMBER OF CYCLES.");
                    report.Write("\t\t---------------------------------------------");
                    for (int i = 0; i < 8; i++) {
                        report.Write(0.0.ToString());
                    }
                }
            } else {
                if (printResults) {
                    report.Write("\t\t(c) Rejection Constraint = " + constraint);
                    report.Write("\t\t-------------------------------------------");
                }

                double[,] v = new double[6, 8];
                int cycleStart = 0;
                int cycleStop = cycle[1];
                for (int k = 0; k < 6; k++) {
                    for (int i = 0; i < 8; i++) {
                        v[k, i] = 0.0;
                    }
                }
                int[] counter = new int[8];
                //for each cycle compute frequency of x
                for (int j = 1; j <= J; j++) {
                    for (int i = 0; i < 8; i++) {
                        counter[i] = 0;
                    }
                    for (int i = cycleStart; i < cycleStop; i++) {
                        if ((S_k[i] >= 1 && S_k[i] <= 4) || (S_k[i] >= -4 && S_k[i] <= -1)) {
                            int b = S_k[i]<0 ? 4 : 3;
                            counter[S_k[i] + b]++;
                        }
                    }
                    cycleStart = cycle[j] + 1;
                    if (j < J) {
                        cycleStop = cycle[j + 1];
                    }

                    for (int i = 0; i < 8; i++) {
                        if ((counter[i] >= 0) && (counter[i] <= 4)) {
                            v[counter[i], i]++;
                        } else if (counter[i] >= 5) {
                            v[5, i]++;
                        }
                    }
                }

                //calculate p_values
                for (int i = 0; i < 8; i++) {
                    int x = stateX[i];
                    double sum = 0;
                    for (int k = 0; k < 6; k++) {
                        sum += Math.Pow(v[k, i] - J * pi[(int)Math.Abs(x), k], 2) / (J * pi[(int)Math.Abs(x), k]);
                    }
                    p_values[i] = Cephes.igamc(2.5, sum / 2.0);

                    if (printResults) {
                        if (p_values[i] < 0 || p_values[i] > 1) {
                            report.Write("WARNING:  P_VALUE IS OUT OF RANGE.");
                        }

                        report.Write(p_values[i] < ALPHA ? "FAILURE" : "SUCCESS"+"\t\tx = "+x+" chi^2 = "+sum+" p_value = "+p_values[i]);
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
            return "14: Random Excursions Test to run on " + n + " bits";
        }
    }
}
