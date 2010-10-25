using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomNumbers.Utils;

namespace RandomNumbers.Tests {
    /// <summary>
    /// 11: Serial Test
    /// </summary>
    /// <remarks>
    /// The focus of this test is the frequency of all possible overlapping m-bit patterns across the entire
    /// sequence. The purpose of this test is to determine whether the number of occurrences of the 2m m-bit
    /// overlapping patterns is approximately the same as would be expected for a random sequence. Random
    /// sequences have uniformity; that is, every m-bit pattern has the same chance of appearing as every other
    /// m-bit pattern. Note that for m = 1, the Serial test is equivalent to the Frequency test of Section 2.1.
    /// </remarks>
    public class Serial : Test {

        /// <summary>
        /// Decision Rule (at the 1% Level)
        /// </summary>
        private const double ALPHA = 0.01;

        /// <summary>
        /// The length of the bit string
        /// </summary>
        private int n { get; set; }
        /// <summary>
        /// The length in bits of each block
        /// </summary>
        private int m { get; set; }

        /// <summary>
        /// Constructor of test, supplied with all neccessary values
        /// </summary>
        /// <param name="m">The length in bits of each block</param>
        /// <param name="n">The length of the bit string</param>
        /// <param name="model">Model containing the the binary string</param>
        /// <exception cref="ArgumentException"/>
        public Serial(int m, int n, ref Model model)
            : base(ref model) {
                if (m > Math.Floor(Math.Log(n, 2)) - 2 || m <= 0) {
                    throw new ArgumentException("The value of m must be strictly less than floor(log(2,n)-1, and be greater than 0", "Serial m");
                }
                if (n > model.epsilon.Count || n <= 0) {
                    throw new ArgumentException("The value of n must be smaller than the size of the input data, and be greater than 0", "Serial n");
                }
                this.m = m;
                this.n = n;
        }

        /// <summary>
        /// Runs the test
        /// </summary>
        /// <param name="printResults">If true text output will be added to a log, otherwise not</param>
        /// <returns>The p_value(s) of the test based upon the input data</returns>
        public override double[] run(bool printResults) {

            //calculate all psi_squares
            double psim0 = psi2(m, n);
            double psim1 = psi2(m - 1, n);
            double psim2 = psi2(m - 2, n);
            double del1 = psim0 - psim1;
            double del2 = psim0 - 2.0 * psim1 + psim2;

            //calculate p_values
            double p_value1 = Cephes.igamc(Math.Pow(2, m - 1) / 2, del1 / 2.0);
            double p_value2 = Cephes.igamc(Math.Pow(2, m - 2) / 2, del2 / 2.0);

            if (printResults) {
                Report report = new Report("11: Serial Test");
                report.Write("\t\t\t       SERIAL TEST");
                report.Write("\t\t---------------------------------------------");
                report.Write("\t\t COMPUTATIONAL INFORMATION:		  ");
                report.Write("\t\t---------------------------------------------");
                report.Write("\t\t(a) Block length    (m) = " + m);
                report.Write("\t\t(b) Sequence length (n) = " + n);
                report.Write("\t\t(c) Psi_m               = " + psim0);
                report.Write("\t\t(d) Psi_m-1             = " + psim1);
                report.Write("\t\t(e) Psi_m-2             = " + psim2);
                report.Write("\t\t(f) Del_1               = " + del1);
                report.Write("\t\t(g) Del_2               = " + del2);
                report.Write("\t\t---------------------------------------------\n");
                report.Write(p_value1 < ALPHA ? "FAILURE" : "SUCCESS" + "\t\tp_value1 = " + p_value1);
                report.Write(p_value2 < ALPHA ? "FAILURE" : "SUCCESS" + "\t\tp_value2 = " + p_value2);
                model.reports.Add(report.title, report);
            }

            return new double[] { p_value1, p_value2 };
        }

        /// <summary>
        /// Title of test and key data the object contains
        /// </summary>
        /// <returns>String with title and data</returns>
        public override string ToString() {
            return "11: Linear Complexity Test to run on " + n + " bits, and a block length of "+m;
        }

        double psi2(int m, int n) {
            if ((m == 0) || (m == -1)) {
                return 0.0;
            }

            int[] P = new int[(int)Math.Pow(2, m+1)-1];
            //calculate frequency of all overlapping m-bit blocks
            for (int i = 0; i < n; i++) {
                int k = 1;
                for (int j = 0; j < m; j++) {
                    if (model.epsilon[(i + j) % n] == 0) {
                        k *= 2;
                    } else if (model.epsilon[(i + j) % n] == 1) {
                        k = 2 * k + 1;
                    }
                }
                P[k - 1]++;
            }

            //calculate psi_squared
	        double sum = 0;
            for (int i = (int)Math.Pow(2, m) - 1; i < (int)Math.Pow(2, m + 1) - 1; i++) {
                sum += Math.Pow(P[i], 2);
            }
            return (sum * Math.Pow(2, m) / (double)n) - (double)n;
        }
    }
}
