using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomNumbers.Utils;

namespace RandomNumbers.Tests {
    /// <summary>
    /// 9: Maurer’s “Universal Statistical” Test
    /// </summary>
    /// <remarks>
    /// The focus of this test is the number of bits between matching patterns (a measure that is related to the
    /// length of a compressed sequence). The purpose of the test is to detect whether or not the sequence can be
    /// significantly compressed without loss of information. A significantly compressible sequence is
    /// considered to be non-random.
    /// </remarks>
    public class Universal : Test {

        /// <summary>
        /// Decision Rule (at the 1% Level)
        /// </summary>
        private const double ALPHA = 0.01;

        /// <summary>
        /// The length of the bit string
        /// </summary>
        private int n { get; set; }
        /// <summary>
        /// The length of each block. 
        /// </summary>
        /// <remarks>
        /// The length of each block. Note: the use of L as the block size is not consistent with the
        /// block size notation (M) used for the other tests. However, the use of L as the block size
        /// was specified in the original source of Maurer's test.
        /// </remarks>
        private int L { get; set; }
        /// <summary>
        /// The number of blocks in the initialization sequence.
        /// </summary>
        private int Q { get; set; }

        /// <summary>
        /// Constructor of test, supplied with all neccessary values
        /// </summary>
        /// <param name="L">The Length of each block (different to other tests 'm')</param>
        /// <param name="Q">The number of blocks in initialization sequence</param>
        /// <param name="n">The length of the bit string to be analysed</param>
        /// <param name="model">Model containing the the binary string</param>
        /// <exception cref="ArgumentException"/>
        public Universal(int L, int Q, int n, ref Model model)
            : base(ref model) {
                if (n > model.epsilon.Count || n <= 0) {
                    throw new ArgumentException("The value of n must be smaller than the size of the input data, and be greater than 0", "Frequency n");
                }
                if (L > 16 || L < 6) {
                    throw new ArgumentException("The value of L must be between 6 and 16 inclusive", "Frequency L");
                }
                if (Q < 10 * (int)Math.Pow(2, L) || Q > 0.5 * n / L) {
                    throw new ArgumentException("The value of Q must be greater than 0.5*(n/L), and less than 10*(2^L)", "Frequency Q");
                }
                this.n = n;
                this.L = L;
                this.Q = Q;
        }

        /// <summary>
        /// Constructor of test, supplied with all neccessary values
        /// </summary>
        /// <param name="n">The length of the bit string to be analysed</param>
        /// <param name="model">Model containing the the binary string</param>
        public Universal(int n, ref Model model)
            : base(ref model) {
                if (n > model.epsilon.Count || n <= 0) {
                    throw new ArgumentException("The value of n must be smaller than the size of the input data, and be greater than 0", "Frequency n");
                }
                this.n = n;
                if (n >= 1059061760) L = 16;
                else if (n >= 496435200) L = 15;
                else if (n >= 231669760) L = 14;
                else if (n >= 107560960) L = 13;
                else if (n >= 49643520) L = 12;
                else if (n >= 22753280) L = 11;
                else if (n >= 10342400) L = 10;
                else if (n >= 4654080) L = 9;
                else if (n >= 2068480) L = 8;
                else if (n >= 904960) L = 7;
                else if (n >= 387840) L = 6;
                else L = 5;
                Q = 10 * (int)Math.Pow(2, L);
        }

        /// <summary>
        /// Runs the test
        /// </summary>
        /// <param name="printResults">If true text output will be added to a log, otherwise not</param>
        /// <returns>The p_value(s) of the test based upon the input data</returns>
        public override double[] run(bool printResults) {

	        double[] expected_value = { 0, 0, 0, 0, 0, 0, 5.2177052, 6.1962507, 7.1836656,
				                        8.1764248, 9.1723243, 10.170032, 11.168765,
				                        12.168070, 13.167693, 14.167488, 15.167379 };
	        double[] variance = { 0, 0, 0, 0, 0, 0, 2.954, 3.125, 3.238, 3.311, 3.356, 3.384,
				                  3.401, 3.410, 3.416, 3.419, 3.421 };
	        
	        int K = (int) (n/L - (double)Q); //number of test blocks
	
	        int p = (int)Math.Pow(2, L);
	        long[] T = new long[p];

            //initialization segment
            for (int i = 1; i <= Q; i++) {
                long decValue = 0;
                for (int j = 0; j < L; j++) {
                    decValue += model.epsilon[(i - 1) * L + j] * (long)Math.Pow(2, L - 1 - j);  //calculate decimal value of segment
                }
                T[decValue] = i;
            }

            //test segment
            double sum = 0;
	        for ( int i=Q+1; i<=Q+K; i++ ) {
		        long decValue = 0;
                for (int j = 0; j < L; j++) {
                    decValue += model.epsilon[(i - 1) * L + j] * (long)Math.Pow(2, L - 1 - j);  //calculate decimal value of segment
                }
		        sum += Math.Log(i - T[decValue])/Math.Log(2);
		        T[decValue] = i;
	        }
            double phi = (double)(sum / (double)K);

            //forumla from 2.9.4(5)
            double c = 0.7 - 0.8 / (double)L + (4 + 32 / (double)L) * Math.Pow(K, -3 / (double)L) / 15;
            double sigma = c * Math.Sqrt(variance[L] / (double)K);

            //calculate p_value
	        double arg = Math.Abs(phi-expected_value[L])/(Math.Sqrt(2) * sigma);
	        double p_value = Cephes.erfc(arg);
            
            if (printResults) {
                Report report = new Report("9: Maurer’s “Universal Statistical” Test");
                report.Write("\t\tUNIVERSAL STATISTICAL TEST");
                report.Write("\t\t--------------------------------------------");
                report.Write("\t\tCOMPUTATIONAL INFORMATION:");
                report.Write("\t\t--------------------------------------------");
                report.Write("\t\t(a) L         = "+ L);
                report.Write("\t\t(b) Q         = " + Q);
                report.Write("\t\t(c) K         = " + K);
                report.Write("\t\t(d) sum       = " + sum);
                report.Write("\t\t(f) variance  = " + variance[L]);
                report.Write("\t\t(g) exp_value = " + expected_value[L]);
                report.Write("\t\t(h) phi       = " + phi);
                report.Write("\t\t(i) WARNING:  "+(n - (Q + K) * L)+" bits were discarded.");
                report.Write("\t\t-----------------------------------------");
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
            return "9: Maurers Universal Statistical Test to run on " + n + " bits, with a block length of "+L+" and a "+Q+" block init sequence";
        }

    }
}
