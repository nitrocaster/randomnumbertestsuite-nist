using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomNumbers.Utils;

namespace RandomNumbers.Tests {
    /// <summary>
    /// 8: Overlapping Template Matching Test
    /// </summary>
    /// <remarks>
    /// The focus of the Overlapping Template Matching test is the number of occurrences of pre-specified target
    /// strings. Both this test and the Non-overlapping Template Matching test of Section 2.7 use an m-bit
    /// window to search for a specific m-bit pattern. As with the test in Section 2.7, if the pattern is not found,
    /// the window slides one bit position. The difference between this test and the test in Section 2.7 is that
    /// when the pattern is found, the window slides only one bit before resuming the search.
    /// </remarks>
    public class OverlappingTemplateMatching : Test {

        /// <summary>
        /// Decision Rule (at the 1% Level)
        /// </summary>
        private const double ALPHA = 0.01;
        /// <summary>
        /// The length in bits of a substring to be tested.
        /// </summary>
        /// <remarks>
        /// M has been set to 1032 in the test code.
        /// </remarks>
        private static int M = 1032;
        /// <summary>
        /// The number of independent blocks of n.
        /// </summary>
        /// <remarks>
        /// N has been set to 968 in the test code.
        /// </remarks>
        private static int N;
        /// <summary>
        /// The number of degrees of freedom.
        /// </summary>
        /// <remarks>
        /// K has been fixed at 5 in the test code.
        /// </remarks>
        private static int K = 5;

        /// <summary>
        /// The length of the bit string
        /// </summary>
        private int n { get; set; }
        /// <summary>
        /// The m-bit template to be matched.
        /// </summary>
        private int[] B { get; set; }

        /// <summary>
        /// Constructor of test, supplied with all neccessary values
        /// </summary>
        /// <param name="B">The m bit template to match</param>
        /// <param name="n">The length of the bit string</param>
        /// <param name="model">Model containing the the binary string</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="OverflowException"/>
        public OverlappingTemplateMatching(String B, int n, ref Model model)
            : base(ref model) {
                if (n > model.epsilon.Count || n <= 0) {
                    throw new ArgumentException("The value of n must be smaller than the size of the input data, and greater than 0", "Block Frequency n");
                }
                this.n = n;
                this.B = new int[B.Length];
                for (int i=0;i<B.Length;i++) {
                    try {
                        this.B[i] = Convert.ToInt32(B.Substring(i, 1));
                    } catch (FormatException) {
                        throw new FormatException("The input data did not consist of a an optional " +
                                "sign followed by a sequence of digits (0 through 9):\r\n\r\n" + B);
                    } catch (OverflowException) {
                        throw new OverflowException("The input string was not of a number within the program's ranges:\r\n\r\n" + B);
                    }
                }
                N = n / M;
        }

        /// <summary>
        /// Runs the test
        /// </summary>
        /// <param name="printResults">If true text output will be added to a log, otherwise not</param>
        /// <returns>The p_value(s) of the test based upon the input data</returns>
        public override double[] run(bool printResults) {
	        
            double[] pi = new double[6];
            double lambda = (double)(M - B.Length + 1) / Math.Pow(2, B.Length);
            double eta = lambda / 2.0;
            double total = 0.0;
            for (int i = 0; i < K; i++) { //compute prior probabilities
                pi[i] = probability(i, eta);
                total += pi[i];
            }
            pi[K] = 1 - total;

            int[] v = new int[K+1];
            for (int i = 0; i < N; i++) {   //search for a match of the template in each block
                int count = 0;
                for (int j = 0; j < M - B.Length + 1; j++) {
                    bool match = true;
                    for (int k = 0; k < B.Length; k++) {
                        if (B[k] != model.epsilon[i * M + j + k]) {
                            match = false;
                        }
                    }
                    if (match) {
                        count++;
                    }
                }
                if (count < K) {   //record the matches found
                    v[count]++;
                } else {
                    v[K]++;
                }
            }

            //compute p_value
	        double sum = 0.0;
	        double chiSquared = 0.0;
            for (int i = 0; i < K + 1; i++) {
                chiSquared += Math.Pow((double)v[i] - (double)N * pi[i], 2) / ((double)N * pi[i]);
                sum += v[i];
            }

            double p_value = Cephes.igamc(5.0 / 2.0, chiSquared / 2.0);

            if (printResults) {
                Report report = new Report("8: Overlapping Template Matching Test");
                report.Write("\t\t    OVERLAPPING TEMPLATE OF ALL ONES TEST");
                report.Write("\t\t-----------------------------------------------");
                report.Write("\t\tCOMPUTATIONAL INFORMATION:");
                report.Write("\t\t-----------------------------------------------");
                report.Write("\t\t(a) n (sequence_length)      = " + n);
                report.Write("\t\t(b) m (block length of 1s)   = " + B.Length);
                report.Write("\t\t(c) M (length of substring)  = " + M);
                report.Write("\t\t(d) N (number of substrings) = " + N);
                report.Write("\t\t(e) lambda [(M-m+1)/2^m]     = " + lambda);
                report.Write("\t\t(f) eta                      = " + eta);
                report.Write("\t\t(g) Chi^2                    = " + chiSquared);
                report.Write("\t\t(h) P-value                  = " + p_value);
                report.Write("\t\t-----------------------------------------------");
                report.Write("\t\t   F R E Q U E N C Y");
                report.Write("\t\t", false);
                for (int i = 0; i < K; i++) {
                    report.Write("   " + i + " ");
                }
                report.Write("  >=" + K);
                report.Write("\t\t-----------------------------------------------");
                report.Write("\t\t", false);
                for (int i = 0; i < K + 1; i++) {
                    report.Write(" " + v[i] + " ");
                }
                if (p_value < 0 || p_value > 1) { 
                    report.Write("WARNING:  P_VALUE IS OUT OF RANGE.");
                }
                report.Write(p_value+" "+(p_value < ALPHA ? "FAILURE" : "SUCCESS"));
                model.reports.Add(report.title, report);
            }

            return new double[] { p_value };
        }

        /// <summary>
        /// Title of test and key data the object contains
        /// </summary>
        /// <returns>String with title and data</returns>
        public override string ToString() {
            return "8: Overrlapping Template Matching Test to run on " + n + " bits compared with a " + B.Length + " length template";
        }

        /// <summary>
        /// Returns a probability based on those given in 3.8
        /// </summary>
        /// <param name="u"></param>
        /// <param name="eta"></param>
        /// <returns></returns>
        private double probability(int u, double eta) {
            int l;
            double sum, p;

            if (u == 0)
                p = Math.Exp(-eta);
            else {
                sum = 0.0;
                for (l = 1; l <= u; l++)
                    sum += Math.Exp(-eta - u * Math.Log(2) + l * Math.Log(eta) - Cephes.lgam(l + 1) + Cephes.lgam(u) - Cephes.lgam(l) - Cephes.lgam(u - l + 1));
                p = sum;
            }
            return p;

        }
    }
}
