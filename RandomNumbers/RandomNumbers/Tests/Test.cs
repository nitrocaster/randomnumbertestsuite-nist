using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomNumbers.Tests {
    /// <summary>
    /// Abstract class to build all tests upon in the suite
    /// </summary>
    public abstract class Test {  
     
        /// <summary>
        /// Model object to run the test on
        /// </summary>
        protected Model model;

        /// <summary>
        /// Constructor of the Test, must contain a model reference
        /// </summary>
        /// <param name="model">Reference to a model object to run the test on</param>
        public Test(ref Model model) {
            this.model = model;
        }

        /// <summary>
        /// Runs the test
        /// </summary>
        /// <param name="printResults">If true text output will be added to a log, otherwise not</param>
        /// <returns>The p_value(s) of the test based upon the input data</returns>
        abstract public double[] run(bool printResults);

        /// <summary>
        /// Title of test and key data the object contains
        /// </summary>
        /// <returns>String with title and data</returns>
        abstract public override string ToString();
    }
}
