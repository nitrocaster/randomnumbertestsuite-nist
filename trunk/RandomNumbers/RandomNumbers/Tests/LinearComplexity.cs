using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomNumbers.Utils;

namespace RandomNumbers.Tests {
    /// <summary>
    /// 10: Linear Complexity Test
    /// </summary>
    /// <remarks>
    /// The focus of this test is the length of a linear feedback shift register (LFSR). The purpose of this test is to
    /// determine whether or not the sequence is complex enough to be considered random. Random sequences
    /// are characterized by longer LFSRs. An LFSR that is too short implies non-randomness.
    /// </remarks>
    public class LinearComplexity : Test {

        /// <summary>
        /// Decision Rule (at the 1% Level)
        /// </summary>
        private const double ALPHA = 0.01;
        /// <summary>
        /// The number of degrees of freedom
        /// </summary>
        /// <remarks>
        /// K = 6 has been hard coded into the test.
        /// </remarks>
        private static int K = 6;

        /// <summary>
        /// The length of the bit string
        /// </summary>
        private int n { get; set; }
        /// <summary>
        /// The length in bits of a block
        /// </summary>
        private int M { get; set; }

        /// <summary>
        /// Constructor of test, supplied with all neccessary values
        /// </summary>
        /// <param name="M">The length in bits of a block</param>
        /// <param name="n">The length of the bit string</param>
        /// <param name="model">Model containing the the binary string</param>
        /// <exception cref="ArgumentException"/>
        public LinearComplexity(int M, int n, ref Model model)
            : base(ref model) {
                if (M < 500 || M > 5000) {
                    throw new ArgumentException("The value of M must be between 500 and 5000 inclusive", "Linear Complexity M");
                }
                if (n > model.epsilon.Count || n <= 0) {
                    throw new ArgumentException("The value of n must be smaller than the size of the input data, and be greater than 0", "Linear Complexity n");
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
            
	        double[]    pi = { 0.01047, 0.03125, 0.12500, 0.50000, 0.25000, 0.06250, 0.020833 };

            int N = n/M;    //number of blocks
	       
            double mu = M / 2.0 + (9.0 + Math.Pow(-1, M+1)) / 36.0 - 1.0 / Math.Pow(2, M) * (M / 3.0 + 2.0 / 9.0);

            double[] v = new double[K+1];

	        for ( int i=0; i<N; i++ ) {
                int[] T = new int[M];   //initialize the work arrays
                int[] P = new int[M];
                int[] C = new int[M];
                int[] B = new int[M];
		        
		        int L = 0;
		        int m = -1;
		        int d = 0;
		        C[0] = 1;
		        B[0] = 1;
		
		        //calculate the linear complexity of the block
		        int blockPos = 0;
		        while ( blockPos < M ) {
                    d = (int)model.epsilon[i * M + blockPos];
                    for (int j = 1; j <= L; j++) {
                        d += C[j] * model.epsilon[i * M + blockPos - j];
                    }
                    d = d % 2;
			        if ( d == 1 ) {
                        for (int j = 0; j < M; j++) {
                            T[j] = C[j];
                            P[j] = 0;
                        }
                        for (int j = 0; j < M; j++) {
                            if (B[j] == 1) {
                                P[j + blockPos - m] = 1;
                            }
                        }
                        for (int j = 0; j < M; j++) {
                            C[j] = (C[j] + P[j]) % 2;
                        }
				        if ( L <= blockPos/2 ) {
					        L = blockPos + 1 - L;
					        m = blockPos;
                            for (int j = 0; j < M; j++) {
                                B[j] = T[j];
                            }
				        }
			        }
			        blockPos++;
		        }
                
                double Ti = Math.Pow(-1, M) * (L - mu) + 2.0 / 9.0;
		
                //record result of complexity test
		        if ( Ti <= -2.5 )
			        v[0]++;
		        else if ( Ti > -2.5 && Ti <= -1.5 )
			        v[1]++;
		        else if ( Ti > -1.5 && Ti <= -0.5 )
			        v[2]++;
		        else if ( Ti > -0.5 && Ti <= 0.5 )
			        v[3]++;
		        else if ( Ti > 0.5 && Ti <= 1.5 )
			        v[4]++;
		        else if ( Ti > 1.5 && Ti <= 2.5 )
			        v[5]++;
		        else
			        v[6]++;
	        }

            //calculate p_value
	        double chi_squared = 0;
            for (int i = 0; i < K + 1; i++) {
                chi_squared += Math.Pow(v[i] - N * pi[i], 2) / (N * pi[i]);
            }
	        double p_value = Cephes.igamc(K/2.0, chi_squared/2.0);

            if (printResults) {
                Report report = new Report("10: Linear Complexity Test");
                report.Write("-----------------------------------------------------");
                report.Write("\tL I N E A R  C O M P L E X I T Y");
                report.Write("-----------------------------------------------------");
                report.Write("\tM (substring length)     = {0}" + M);
                report.Write("\tN (number of substrings) = {0}" + N);
                report.Write("-----------------------------------------------------");
                report.Write("        F R E Q U E N C Y                            ");
                report.Write("-----------------------------------------------------");
                report.Write("  C0   C1   C2   C3   C4   C5   C6    CHI2    P-value");
                report.Write("-----------------------------------------------------");
                report.Write("\tNote: " + n % M + " bits were discarded!");
                for (int i = 0; i < K + 1; i++) {
                    report.Write(((int)v[i]).ToString(), false);
                }
                report.Write("");
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
            return "10: Linear Complexity Test to run on " + n + " bits, and blocks of size "+M;
        }
    }
}
