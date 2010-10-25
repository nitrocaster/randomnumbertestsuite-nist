using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomNumbers.Utils {
    /// <summary>
    /// Report object to store a title and a body to show information about a statisitcal test
    /// </summary>
    public class Report {

        /// <summary>
        /// Title of the test
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// Output from the test
        /// </summary>
        /// <remarks>
        /// Not actually stored as a string in itself, the builder is used for that
        /// </remarks>
        public string body {
            get {
                return bodyBuilder.ToString();
            }
        }

        /// <summary>
        /// Builder for the body, build up as caller wants to append stuff to the body
        /// </summary>
        private StringBuilder bodyBuilder;

        /// <summary>
        /// Constructor to create a report with a title and the start of a body
        /// </summary>
        /// <param name="title">Title of the test</param>
        /// <param name="body">Output from the test</param>
        public Report(string title, string body) {
            this.title = title;
            this.bodyBuilder = new StringBuilder(body);
        }

        /// <summary>
        /// Constructor to create a report with a title and a blank body
        /// </summary>
        /// <param name="title"></param>
        public Report(string title) {
            this.title = title;
            this.bodyBuilder = new StringBuilder();
        }

        /// <summary>
        /// Appends the string argument to the body
        /// </summary>
        /// <param name="str">String to append</param>
        /// <param name="newLine">Newline if true, same line otherwise</param>
        public void Write(string str, Boolean newLine) {
            if (newLine) Write(str);
            else bodyBuilder.Append(str);
        }

        /// <summary>
        /// Appends the string argument to the body, with a newline
        /// </summary>
        /// <param name="str">String to append</param>
        public void Write(string str) {
            bodyBuilder.AppendLine(str);
        }

    }
}
