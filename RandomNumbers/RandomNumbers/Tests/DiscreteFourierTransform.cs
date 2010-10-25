using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using RandomNumbers.Utils;

namespace RandomNumbers.Tests {
    /// <summary>
    /// 6: Discrete Fourier Transform (Sprectral) Test
    /// </summary>
    /// <remarks>
    /// The focus of this test is the peak heights in the Discrete Fourier Transform of the sequence. The purpose
    /// of this test is to detect periodic features (i.e., repetitive patterns that are near each other) in the tested
    /// sequence that would indicate a deviation from the assumption of randomness. The intention is to detect
    /// whether the number of peaks exceeding the 95 % threshold is significantly different than 5 %.
    /// </remarks>
    public class DiscreteFourierTransform : Test {

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
        public DiscreteFourierTransform(int n, ref Model model)
            : base(ref model) {
                if (n > model.epsilon.Count || n <= 0) {
                    throw new ArgumentException("The value of n must be smaller than the size of the input data, and be greater than 0", "Discrete Fourier Transform n");
                }
                this.n = n;
        }

        /// <summary>
        /// Runs the test
        /// </summary>
        /// <param name="printResults">If true text output will be added to a log, otherwise not</param>
        /// <returns>The p_value(s) of the test based upon the input data</returns>
        public unsafe override double[] run(bool printResults) {
            
            double[] X = new double[n];
            

            for (int i = 0; i < n; i++) {
                X[i] = 2 * (int)model.epsilon[i] - 1; //map all bits in binary string (0->-1,1->1)
            }
            double[] m = new double[n / 2 + 1];
            //generate DFT of the binary string, using the unsafe functions implemented in C
            fixed (double* wsavePtr = &(new double[2 * n])[0]) {
                fixed (int* ifacPtr = &(new int[15])[0]) {
                    fixed (double* mPtr = &m[0]) {
                        fixed (double* XPtr = &X[0]) {
                            FFT.__ogg_fdrffti(n, wsavePtr, ifacPtr);		//init stage for work arrays
                            FFT.__ogg_fdrfftf(n, XPtr, wsavePtr, ifacPtr);  //apply FFT on data
                        }
                    }
                }
            }

            //get magnitude of the DFT produced (to convert complex domain to real domain)
            m[0] = Math.Sqrt(X[0] * X[0]);
            for (int i = 0; i < n / 2; i++) {
                if (2 * i + 2 >= X.Length) {
                    m[i + 1] = Math.Sqrt(Math.Pow(X[2 * i + 1], 2));
                } else {
                    m[i + 1] = Math.Sqrt(Math.Pow(X[2 * i + 1], 2) + Math.Pow(X[2 * i + 2], 2));
                }
            }

	        int N_l = 0;
	        double T = Math.Sqrt(2.995732274*n); //calculate upper bound (T) (the 95% peak height threshold)

            for (int i = 0; i < n / 2; i++) {
                if (m[i] < T) {
                    N_l++;    //count observed number of peaks in |DFT| greater than T
                }
            }
            double N_0 = 0.95 * n / 2.0; //expected number of peaks
            double d = (N_l - N_0) / Math.Sqrt(n / 4.0 * 0.95 * 0.05);

            //calculate p_value
            double p_value = Cephes.erfc(Math.Abs(d) / Math.Sqrt(2.0));

            if (printResults) {
                Report report = new Report("6: Discrete Fourier Transform (Sprectral) Test");
                report.Write("\t\t\t\tFFT TEST");
                report.Write("\t\t-------------------------------------------");
                report.Write("\t\tCOMPUTATIONAL INFORMATION:");
                report.Write("\t\t-------------------------------------------");
                report.Write("\t\t(-) Upper Bound= " + T);
                report.Write("\t\t(b) N_l        = " + N_l);
                report.Write("\t\t(c) N_o        = " + N_0);
                report.Write("\t\t(d) d          = " + d);
                report.Write("\t\t-------------------------------------------");

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
            return "6: Discrete Fourier Transform (Spectral) Test to run on " + n + " bits";
        }
    }
}
