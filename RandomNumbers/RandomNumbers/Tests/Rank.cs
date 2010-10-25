using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomNumbers.Utils;

namespace RandomNumbers.Tests {
    /// <summary>
    /// 2.5 Binary Matrix Rank Test
    /// </summary>
    /// <remarks>
    /// The focus of the test is the rank of disjoint sub-matrices of the entire sequence. The purpose of this test is
    /// to check for linear dependence among fixed length substrings of the original sequence. Note that this test
    /// also appears in the DIEHARD battery of tests [7].
    /// </remarks>
    public class Rank : Test {

        /// <summary>
        /// Decision Rule (at the 1% Level)
        /// </summary>
        private const double ALPHA = 0.01;
        /// <summary>
        /// The number of rows in each matrix.
        /// </summary>
        /// <remarks>
        /// For the test suite, M has been set to 32. If other
        /// values of M are used, new approximations need to be computed.
        /// </remarks>
        private static int M = 32;
        /// <summary>
        /// The number of columns in each matrix.
        /// </summary>
        /// <remarks>
        /// For the test suite, Q has been set to 32. If other
        /// values of Q are used, new approximations need to be computed.
        /// </remarks>
        private static int Q = M;

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
        public Rank(int n, ref Model model)
            : base(ref model) {
                if (n / (M * Q) == 0 || n > model.epsilon.Count) {
                    throw new ArgumentException("The value of n must be smaller than the size of the input data, and large enough to make a " + M + "x" + Q + " matrix ", "Matrix Rank n");
                }
                this.n = n;
        }

        /// <summary>
        /// Runs the test
        /// </summary>
        /// <param name="printResults">If true text output will be added to a log, otherwise not</param>
        /// <returns>The p_value(s) of the test based upon the input data</returns>
        public override double[] run(bool printResults) {

            double[] Pm = new double[3];
	        int r = Math.Min(M, Q);		//compute predicted probabilities
		    double product = 1;
            for (int i = 0; i <= r - 1; i++) {
                product *= ((1.0 - Math.Pow(2, i - Q)) * (1.0 - Math.Pow(2, i - M))) / (1.0 - Math.Pow(2, i - r));
            }
            Pm[0] = Math.Pow(2, r * (M + Q - r) - M * Q) * product;
            r--;
		    product = 1;
            for (int i = 0; i <= r - 1; i++) {
                product *= ((1.0 - Math.Pow(2, i - Q)) * (1.0 - Math.Pow(2, i - M))) / (1.0 - Math.Pow(2, i - r));
            }
            Pm[1] = Math.Pow(2, r * (M + Q - r) - M * Q) * product;
            Pm[2] = 1 - (Pm[0] + Pm[1]);

            int N = n / (M * Q); //number of blocks
            int[] Fm = new int[3];
            for (int k = 0; k < N; k++) {
                //construct the matrix of MxQ in size
                int[,] matrix = new int[M, Q];
                for (int i = 0; i < M; i++) {
                    for (int j = 0; j < Q; j++) {
                        matrix[i,j] = model.epsilon[k * (M * Q) + j + i * M];
                    }
                }

                int R = computeRank(M, Q, matrix); //get the rank of the matrix

                if (R == M) {
                    Fm[0]++;	//full rank
                } else if (R == M - 1) {
                    Fm[1]++;    //full rank - 1
                }
            }
            Fm[2] = N - (Fm[0] + Fm[1]); //full rank - 2

            //compute p_value
            double chi_squared = (Math.Pow(Fm[0] - N * Pm[0], 2) / (double)(N * Pm[0]) +
                                  Math.Pow(Fm[1] - N * Pm[1], 2) / (double)(N * Pm[1]) +
                                  Math.Pow(Fm[2] - N * Pm[2], 2) / (double)(N * Pm[2]));
            double p_value = Cephes.igamc(1, chi_squared / 2.0);

            if (printResults) {
                Report report = new Report("2.5 Binary Matrix Rank Test");
                report.Write("\t\t\t\tRANK TEST");
                report.Write("\t\t---------------------------------------------");
                report.Write("\t\tCOMPUTATIONAL INFORMATION:");
                report.Write("\t\t---------------------------------------------");
                report.Write("\t\t(a) Probability P_"+M+" = " + Pm[0]);
                report.Write("\t\t(b)             P_"+(M - 1)+" = " + Pm[1]);
                report.Write("\t\t(c)             P_"+(M - 2)+" = " + Pm[2]);
                report.Write("\t\t(d) Frequency   F_"+M+" = " + Fm[0]);
                report.Write("\t\t(e)             F_"+(M - 1)+" = "+ Fm[1]);
                report.Write("\t\t(f)             F_"+(M - 2)+" = " + Fm[2]);
                report.Write("\t\t(g) # of matrices    = " + N);
                report.Write("\t\t(h) Chi^2            = "+chi_squared);
                report.Write("\t\t(i) NOTE: "+n % (M * Q)+" BITS WERE DISCARDED.");
                report.Write("\t\t---------------------------------------------");
                report.Write(p_value < ALPHA ? "FAILURE" : "SUCCESS"+"\t\tp_value = "+p_value);
                if (p_value < 0 || p_value > 1) {
                    report.Write("WARNING:  P_VALUE IS OUT OF RANGE.");
                }
                model.reports.Add(report.title, report);
            }

            return new double[] { p_value };
        }

        /// <summary>
        /// Title of test and key data the object contains
        /// </summary>
        /// <returns>String with title and data</returns>
        public override string ToString() {
            return "5: Binary Matrix Rank Test to run on " + n + " bits";
        }

        /// <summary>
        /// Computes the rank of a binary matrix, using algorithm outlined in F.1
        /// </summary>
        /// <remarks>The code is taken directly from the NIST implmentation in C</remarks>
        /// <param name="M">Width of the matrix</param>
        /// <param name="Q">Height of the matrix</param>
        /// <param name="matrix">The matrix to calculate the rank of</param>
        /// <returns></returns>
        private int computeRank(int M, int Q, int[,] matrix) {
            
            /* FORWARD APPLICATION OF ELEMENTARY ROW OPERATIONS */
            for (int i = 0; i < Math.Min(M,Q) - 1; i++) {
                if (matrix[i, i] == 1) {
                    performElementaryRowOperations(true, i, M, Q, ref matrix);
                } else { 	/* matrix[i][i] = 0 */
                    if (findUnitElementAndSwap(true, i, M, Q, matrix)) {
                        performElementaryRowOperations(true, i, M, Q, ref matrix);
                    }
                }
            }

            /* BACKWARD APPLICATION OF ELEMENTARY ROW OPERATIONS */
            for (int i = Math.Min(M,Q) - 1; i > 0; i--) {
                if (matrix[i, i] == 1) {
                    performElementaryRowOperations(false, i, M, Q, ref matrix);
                } else { 	/* matrix[i][i] = 0 */
                    if (findUnitElementAndSwap(false, i, M, Q, matrix)) {
                        performElementaryRowOperations(false, i, M, Q, ref matrix);
                    }
                }
            }

            return determineRank(Math.Min(Q,M), M, Q, matrix);
        }

        private void performElementaryRowOperations(bool forward, int i, int M, int Q, ref int[,] A) {
            if (forward) {
                for (int j = i + 1; j < M; j++) {
                    if (A[j, i] == 1) {
                        for (int k = i; k < Q; k++) {
                            A[j, k] = (A[j, k] + A[i, k]) % 2;
                        }
                    }
                }
            } else {
                for (int j = i - 1; j >= 0; j--) {
                    if (A[j, i] == 1) {
                        for (int k = 0; k < Q; k++) {
                            A[j, k] = (A[j, k] + A[i, k]) % 2;
                        }
                    }
                }
            }
        }

        private bool findUnitElementAndSwap(bool forward, int i, int M, int Q, int[,] A) {
            if (forward) {
                int index = i + 1;
                while ((index < M) && (A[index, i] == 0)) {
                    index++;
                }
                if (index < M) {
                    swapRows(i, index, Q, ref A);
                    return true;
                }
            } else {
                int index = i - 1;
                while ((index >= 0) && (A[index, i] == 0)) {
                    index--;
                }
                if (index >= 0) {
                    swapRows(i, index, Q, ref A);
                    return true;
                }
            }
            return false;
        }

        private void swapRows(int i, int index, int Q, ref int[,] A) {
            for (int p = 0; p < Q; p++) {
                int temp = A[i,p];
                A[i,p] = A[index,p];
                A[index,p] = temp;
            }
        }

        private int determineRank(int m, int M, int Q, int[,] A) {
            /* DETERMINE RANK, THAT IS, COUNT THE NUMBER OF NONZERO ROWS */
            int rank = m;
            for (int i = 0; i < M; i++) {
                bool allZeroes = true;
                for (int j = 0; j < Q; j++) {
                    if (A[i, j] == 1) {
                        allZeroes = false;
                        break;
                    }
                }
                if (allZeroes) {
                    rank--;
                }
            }
            return rank;
        }


    }
}
