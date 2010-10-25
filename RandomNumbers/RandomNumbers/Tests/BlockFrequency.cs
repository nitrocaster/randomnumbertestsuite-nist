using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomNumbers.Utils;

namespace RandomNumbers.Tests {
    /// <summary>
    /// 2: Frequency Test within a Block
    /// </summary>
    /// <remarks>
    /// The focus of the test is the proportion of ones within M-bit blocks. The purpose of this test is to determine
    /// whether the frequency of ones in an M-bit block is approximately M/2, as would be expected under an
    /// assumption of randomness. For block size M=1, this test degenerates to test 1, the Frequency (Monobit)
    /// test.
    /// </remarks>
    public class BlockFrequency : Test {

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
        private int M { get; set; }

        /// <summary>
        /// Constructor of test, supplied with all neccessary values
        /// </summary>
        /// <param name="M">The length in bits of each block</param>
        /// <param name="n">The length of the bit string</param>
        /// <param name="model">Model containing the the binary string</param>
        /// <exception cref="ArgumentException"/>
        public BlockFrequency(int M, int n, ref Model model)
            : base(ref model) {
                if (n > model.epsilon.Count || n <= 0) {
                    throw new ArgumentException("The value of n must be smaller than the size of the input data, and greater than 0", "Block Frequency n");
                }
                if (M > n || M <= 0) {
                    throw new ArgumentException("The value of M must be smaller than that of n, and greater than 0", "Block Frequency M");
                }
                this.M = M;
                this.n = n;
        }

        /// <summary>
        /// Runs the test
        /// </summary>
        /// <param name="printResults">If true text output will be added to a log, otherwise not</param>
        /// <returns>The p_value(s) of the test based upon the input data</returns>
        public override double[] run(bool printResults) {

            int N = n / M; 	//number of blocks that can be made from the binary string

            //for each block calculate the proportion of ones
            double sum = 0.0;
            for (int i = 0; i < N; i++) {
                double pi = (double)model.epsilon.GetRange(i * M, M).Sum() / (double)M;   //sum of bits in block / length of the block
                sum += Math.Pow((pi - 0.5), 2);
            }

            //calculate p_value
            double chi_squared = 4.0 * M * sum;
            double p_value = Cephes.igamc(N / 2.0, chi_squared / 2.0);

            if (printResults) {
                Report report = new Report("2: Frequency Test within a Block");
                report.Write("\t\t\tBLOCK FREQUENCY TEST");
                report.Write("\t\t---------------------------------------------");
                report.Write("\t\tCOMPUTATIONAL INFORMATION:");
                report.Write("\t\t---------------------------------------------");
                report.Write("\t\t(a) Chi^2           = " + chi_squared);
                report.Write("\t\t(b) # of substrings = " + N);
                report.Write("\t\t(c) block length    = " + M);
                report.Write("\t\t(d) Note: " + n % M + " bits were discarded.");
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
            return "2: Frequency Test Within a Block to run on " + n + " bits and " + M + " blocks";
        }
    }
}
