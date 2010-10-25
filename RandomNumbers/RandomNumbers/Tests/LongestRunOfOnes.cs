using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomNumbers.Utils;

namespace RandomNumbers.Tests {
    /// <summary>
    /// 4: Test for the Longest Run of Ones in a Block
    /// </summary>
    /// <remarks>
    /// The focus of the test is the longest run of ones within M-bit blocks. The purpose of this test is to
    /// determine whether the length of the longest run of ones within the tested sequence is consistent with the
    /// length of the longest run of ones that would be expected in a random sequence. Note that an irregularity in
    /// the expected length of the longest run of ones implies that there is also an irregularity in the expected
    /// length of the longest run of zeroes. Therefore, only a test for ones is necessary. See Section 4.4.
    /// </remarks>
    public class LongestRunOfOnes : Test {

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
        public LongestRunOfOnes(int n, ref Model model)
            : base(ref model) {
                if (n > model.epsilon.Count || n < 128) {
                    throw new ArgumentException("The value of n must be smaller than the size of the input data, and be at least 128", "Longest Run of Ones n");
                }
                this.n = n;
        }

        /// <summary>
        /// Runs the test
        /// </summary>
        /// <param name="printResults">If true text output will be added to a log, otherwise not</param>
        /// <returns>The p_value(s) of the test based upon the input data</returns>
        public override double[] run(bool printResults) {

            double[] pi;
            int K, M, V0;
            init(out K, out M, out V0, out pi);  //initialize variables to sensible values

            int[] V = new int[pi.Length];
            int N = n / M;    //number of blocks that can be made from the binary string
            for (int i = 0; i < N; i++) {
                //count the longest run of ones in each block
                int longestRun = 0;
                int currentRun = 0;
                for (int j = 0; j < M; j++) {
                    if (model.epsilon[i * M + j] == 1) {
                        longestRun = Math.Max(longestRun, ++currentRun);
                    } else {
                        currentRun = 0;
                    }
                }
                //record the longest run of ones found in the correct counter
                if (longestRun < V0) {
                    V[0]++;
                } else if (longestRun > V0 + K) {
                    V[K]++;
                } else {
                    V[longestRun - V0]++;
                }
            }

            //calculate p_value
            double chi_squared = 0.0;
            for (int i = 0; i <= K; i++) {
                chi_squared += ((V[i] - N * pi[i]) * (V[i] - N * pi[i])) / (N * pi[i]);
            }
            double p_value = Cephes.igamc((double)(K / 2.0), chi_squared / 2.0);

            if (printResults) {
                Report report = new Report("4: Test for the Longest Run of Ones in a Block");
                report.Write("\t\t\t  LONGEST RUNS OF ONES TEST\n");
                report.Write("\t\t---------------------------------------------\n");
                report.Write("\t\tCOMPUTATIONAL INFORMATION:\n");
                report.Write("\t\t---------------------------------------------\n");
                report.Write("\t\t(a) N (# of substrings)  = "+ N);
                report.Write("\t\t(b) M (Substring Length) = " + M);
                report.Write("\t\t(c) Chi^2                = " + chi_squared);
                report.Write("\t\t---------------------------------------------\n");
                report.Write("\t\t      F R E Q U E N C Y\n");
                report.Write("\t\t---------------------------------------------\n");
                if (K == 3) {
                    report.Write("\t\t  <=1     2     3    >=4   P-value  Assignment");
                    report.Write("\t\t "+V[0]+" "+V[1]+" "+V[2]+" "+V[3]);
                } else if (K == 5) {
                    report.Write("\t\t<=4  5  6  7  8  >=9 P-value  Assignment");
                    report.Write("\t\t "+V[0]+" "+V[1]+" "+V[2]+" "+V[3]+" "+V[4]+" "+V[5]);
                } else {
                    report.Write("\t\t<=10  11  12  13  14  15 >=16 P-value  Assignment");
                    report.Write("\t\t "+V[0]+" "+V[1]+" "+V[2]+" "+V[3]+" "+V[4]+" "+V[5]+" "+V[6]);
                }
                if (p_value < 0 || p_value > 1) {
                    report.Write("WARNING:  P_VALUE IS OUT OF RANGE.\n");
                }
                report.Write(p_value < ALPHA ? "FAILURE" : "SUCCESS"+"{0}\t\tp_value = "+ p_value);
                model.reports.Add(report.title, report);
            }

            return new double[] { p_value };
        }

        /// <summary>
        /// Title of test and key data the object contains
        /// </summary>
        /// <returns>String with title and data</returns>
        public override string ToString() {
            return "4: Longest Run Of Ones in a Block Test to run on " + n + " bits";
        }

        /// <summary>
        /// Initialization based upon the length of the binary string
        /// </summary>
        /// <param name="K">Out param for K based on table in 2.4.4(3)</param>
        /// <param name="M">Out param for M based on table in 2.4.4(3)</param>
        /// <param name="V0">Out param for start point in array based on table in 2.4.4(2)</param>
        /// <param name="pi">Out param for pi array based on table in 3.4</param>
        private void init(out int K, out int M, out int V0, out double[] pi) {
            if (n < 6272) {
                K = 3;
                M = 8;
                V0 = 1;
                pi = new double[4];
                pi[0] = 0.21484375;
                pi[1] = 0.3671875;
                pi[2] = 0.23046875;
                pi[3] = 0.1875;
            } else if (n < 750000) {
                K = 5;
                M = 128;
                V0 = 4;
                pi = new double[6];
                pi[0] = 0.1174035788;
                pi[1] = 0.242955959;
                pi[2] = 0.249363483;
                pi[3] = 0.17517706;
                pi[4] = 0.102701071;
                pi[5] = 0.112398847;
            } else {
                K = 6;
                M = 10000;
                V0 = 10;
                pi = new double[7];
                pi[0] = 0.0882;
                pi[1] = 0.2092;
                pi[2] = 0.2483;
                pi[3] = 0.1933;
                pi[4] = 0.1208;
                pi[5] = 0.0675;
                pi[6] = 0.0727;
            }
        }

    }
}
