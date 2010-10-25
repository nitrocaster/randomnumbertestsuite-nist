using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using RandomNumbers.Utils;

namespace RandomNumbers.Tests {
    /// <summary>
    /// 7: Non-overlapping Template Matching Test
    /// </summary>
    /// <remarks>
    /// The focus of this test is the number of occurrences of pre-specified target strings. The purpose of this
    /// test is to detect generators that produce too many occurrences of a given non-periodic (aperiodic) pattern.
    /// For this test and for the Overlapping Template Matching test of Section 2.8, an m-bit window is used to
    /// search for a specific m-bit pattern. If the pattern is not found, the window slides one bit position. If the
    /// pattern is found, the window is reset to the bit after the found pattern, and the search resumes.
    /// </remarks>
    public class NonOverlappingTemplateMatching : Test {

        /// <summary>
        /// Decision Rule (at the 1% Level)
        /// </summary>
        private const double ALPHA = 0.01;
        private const String TEMPLATE_DIR = "RandomNumbers.Resources";
        private const String TEMPLATESPLITSTR = " ";
        private static int M;
        private static int N = 8;
        private static double mu;
        private static double sigma;

        /// <summary>
        /// The length of the bit string
        /// </summary>
        private int n { get; set; }
        /// <summary>
        /// The length in bits of each template.
        /// </summary>
        /// <remarks>
        ///  The template is the target string.
        /// </remarks>
        private int m { get; set; }
        /// <summary>
        /// The m-bit template to be matched.
        /// </summary>
        /// <remarks>
        /// The m-bit template to be matched; B is a string of ones and zeros (of length m) which is
        /// defined in a template library of non-periodic patterns contained within the test code.
        /// </remarks>
        private int[] B { get; set; }

        /// <summary>
        /// Constructor of test, supplied with all neccessary values
        /// </summary>
        /// <param name="m">The length in bits of each template</param>
        /// <param name="n">The length of the bit string</param>
        /// <param name="model">Model containing the the binary string</param>
        /// <exception cref="ArgumentException"/>
        public NonOverlappingTemplateMatching(int m, int n, ref Model model)
            : base(ref model) {
                if (n > model.epsilon.Count || n <= 0) {
                    throw new ArgumentException("The value of n must be smaller than the size of the input data, and be greater than 0", "Non Overlapping Template Matching n");
                }
                if (m > 21 || m < 0) {
                    throw new ArgumentException("The value of m must and be between 0 and 21 inclusive", "Non Overlapping Template Matching m");
                }
                this.n = n;
                this.m = m;
                M = n / N;
        }

        /// <summary>
        /// Constructor of test, supplied with all neccessary values
        /// </summary>
        /// <param name="B">The m-bit template to match</param>
        /// <param name="n">The length of the bit string</param>
        /// <param name="model">Model containing the the binary string</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref=""
        /// <exception cref="OverflowException"/>
        public NonOverlappingTemplateMatching(String B, int n, ref Model model)
            : base(ref model) {
                if (n > model.epsilon.Count || n <= 0) {
                    throw new ArgumentException("The value of n must be smaller than the size of the input data, and be greater than 0", "Non Overlapping Template Matching n");
                }
                this.n = n;
                this.B = new int[B.Length];
                for (int i = 0; i < B.Length; i++) {
                    try {
                        this.B[i] = Convert.ToInt32(B.Substring(i, 1));
                    } catch (FormatException) {
                        throw new FormatException("The input data did not consist of a an optional " +
                                "sign followed by a sequence of digits (0 through 9):\r\n\r\n" + B);
                    } catch (OverflowException) {
                        throw new OverflowException("The input string was not of a number within the program's ranges:\r\n\r\n" + B);
                    }
                }

                M = n / N;
        }

        /// <summary>
        /// Runs the test
        /// </summary>
        /// <param name="printResults">If true text output will be added to a log, otherwise not</param>
        /// <returns>The p_value(s) of the test based upon the input data</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="OverflowException"/>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        public override double[] run(bool printResults) {

            if (B != null) { //as we know it is to run on a single test
                mu = (M - B.Length + 1) / Math.Pow(2, B.Length);
                sigma = M * (1.0 / Math.Pow(2.0, B.Length) - (2.0 * B.Length - 1.0) / Math.Pow(2.0, 2.0 * B.Length));
                return run(B, printResults);
            }
            //otherwise on a template of tests

            int[] numOfTemplates = {0, 0, 2, 4, 6, 12, 20, 40, 74, 148, 284, 568, 1116,
						    2232, 4424, 8848, 17622, 35244, 70340, 140680, 281076, 562152};
	        

            //compute predicted probabilities
            int K = 5;
            mu = (M - m + 1) / Math.Pow(2, m);
            sigma = M * (1.0 / Math.Pow(2.0, m) - (2.0 * m - 1.0) / Math.Pow(2.0, 2.0 * m));

            Report report = new Report("7: Non-overlapping Template Matching Test");
            if (printResults) {
                report.Write("\t\t  NONPERIODIC TEMPLATES TEST");
                report.Write("-------------------------------------------------------------------------------------");
                report.Write("\t\t  COMPUTATIONAL INFORMATION");
                report.Write("-------------------------------------------------------------------------------------");
                report.Write("\tLAMBDA = " + mu + "\tM = " + M + "\tN = " + N + "\tm = " + m + "\tn = " + n + "");
                report.Write("-------------------------------------------------------------------------------------");
            }

            double[] p_values = new double[numOfTemplates[m]];
            try {
                using (StreamReader sr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(TEMPLATE_DIR + ".template" + m))) {
                    for (int i = 0; i < numOfTemplates[m]; i++) {

                        String line = sr.ReadLine();    //load in the template string
                        String[] numbers = Regex.Split(line, TEMPLATESPLITSTR);
                        B = new int[m];
                        for (int k = 0; k < m; k++) {
                            try {
                                B[k] = Convert.ToInt32(numbers[k]); //parse into an array
                            } catch (FormatException) {
                                throw new FormatException("The input data did not consist of a an optional " +
                                        "sign followed by a sequence of digits (0 through 9):\r\n\r\n" + line);
                            } catch (OverflowException) {
                                throw new OverflowException("The input string was not of a number within the program's ranges:\r\n\r\n" + line);
                            }
                        }

                        p_values[i] = this.run(B, false)[0];    //get p_value for this template string

                        if (printResults) {
                            if (p_values[i] < 0 || p_values[i] > 1) {
                                report.Write("\t\tWARNING:  P_VALUE IS OUT OF RANGE.");
                            }
                            report.Write(i + "\t: " + (p_values[i] < ALPHA ? "FAILURE" : "SUCCESS") + "\t\t" + p_values[i]);
                        }

                    }
                }
            } catch (ArgumentException) {
                throw new ArgumentException("The input file path is not a valid one:\r\n\r\n" + TEMPLATE_DIR + "template" + m);
            } catch (FileNotFoundException) {
                throw new FileNotFoundException("The input file does not exist:\r\n\r\n" + TEMPLATE_DIR + "template" + m);
            } catch (DirectoryNotFoundException) {
                throw new DirectoryNotFoundException("The input directory does not exist:\r\n\r\n" + TEMPLATE_DIR + "template" + m);
            } catch (UnauthorizedAccessException) {
                throw new UnauthorizedAccessException("The user does not have the permissions to access this file:\r\n\r\n" + TEMPLATE_DIR + "template" + m);
            } catch (IOException) {
                throw new IOException("Failed to read from file successfully:\r\n\r\n" + TEMPLATE_DIR + "template" + m);
            } catch (OutOfMemoryException) {
                throw new OutOfMemoryException("System out of memory");
            }
            model.reports.Add(report.title, report);
            return p_values;
        }

        /// <summary>
        /// Title of test and key data the object contains
        /// </summary>
        /// <returns>String with title and data</returns>
        public override string ToString() {
            return "7: Non Overrlapping Template Matching Test to run on " + n + " bits compared with a " + m + " length templates";
        }

        private double[] run(int[] B, bool printResults) {
            if (n > model.epsilon.Count || n <= 0) {
                throw new ArgumentException("The value of n must be smaller than the size of the input data, and be greater than 0", "Frequency n");
            }

            int[] Wj = new int[N];

            for (int i = 0; i < N; i++) { //for each start position
                for (int j = 0; j < M - B.Length + 1; j++) {    //is there a match for the template
                    bool match = true;
                    for (int k = 0; k < B.Length; k++) {
                        if ((int)B[k] != (int)model.epsilon[i * M + j + k]) {
                            match = false;  
                            break;
                        }
                    }
                    if (match) {
                        Wj[i]++;    //if there was a match, record it
                    }
                }
            }

            //calculate p_value
            double chi2 = 0.0;
            for (int i = 0; i < N; i++) {
                chi2 += Math.Pow(((double)Wj[i] - mu) / Math.Pow(sigma, 0.5), 2);
            }
            double p_value = Cephes.igamc(N / 2.0, chi2 / 2.0);

            if (printResults) {
                Report report = new Report("7: Non-overlapping Template Matching Test");
                report.Write("chi^2   = " + chi2);
                report.Write("p_value = " + p_value);
                model.reports.Add(report.title, report);
            }

            return new double[] { p_value };
        }
    }
}
