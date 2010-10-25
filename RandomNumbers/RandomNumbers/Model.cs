using System;
using System.Collections.Generic;
using System.Linq;
using RandomNumbers.Utils;

namespace RandomNumbers {
    /// <summary>
    /// Model class which contains the binary string data
    /// </summary>
    public class Model {

        /// <summary>
        /// Binary digits loaded into application, this will be used when running the tests
        /// </summary>
        public readonly List<int> epsilon;

        /// <summary>
        /// Report objects containing outputs from the statisitcal tests
        /// </summary>
        public readonly Dictionary<String, Report> reports;

        /// <summary>
        /// Constructor from a list of binary digits
        /// </summary>
        /// <param name="numbers">Binary string to be used in application</param>
        public Model(List<int> numbers) {
            this.epsilon = numbers;
            this.reports = new Dictionary<String, Report>();
        }

    }

}
