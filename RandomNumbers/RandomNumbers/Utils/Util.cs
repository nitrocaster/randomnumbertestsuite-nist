using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RandomNumbers.Utils {
    class Util {

        /// <summary>
        /// Loads file contents into string to be returned
        /// </summary>
        /// <param name="filePath">File path of file to be loaded into String</param>
        /// <returns>String of contents of the file at filePath</returns>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="DirectoryNotFoundException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="UnauthorizedAccessException"/>
        /// <exception cref="IOException"/>
        /// <exception cref="OutOfMemoryException"/>
        public static string loadData(String filePath) {

            string str = string.Empty;
            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            try {
                using (StreamReader sr = new StreamReader(filePath)) {
                    str = sr.ReadToEnd();
                }
            } catch (ArgumentException) {
                throw new ArgumentException("The input file path is not a valid one:\r\n\r\n" + filePath);
            } catch (FileNotFoundException) {
                throw new FileNotFoundException("The input file does not exist:\r\n\r\n" + filePath);
            } catch (DirectoryNotFoundException) {
                throw new DirectoryNotFoundException("The input directory does not exist:\r\n\r\n" + filePath);
            } catch (UnauthorizedAccessException) {
                throw new UnauthorizedAccessException("The user does not have the permissions to access this file:\r\n\r\n" + filePath);
            } catch (IOException) {
                throw new IOException("Failed to read from file successfully:\r\n\r\n" + filePath);
            } catch (OutOfMemoryException) {
                throw new OutOfMemoryException("System out of memory");
            }
            
            return str;
        }

        /// <summary>
        /// Converts a string of numbers into a list of integers
        /// </summary>
        /// <param name="str">String to convert into integers</param>
        /// <param name="split">How the string is split up between integers</param>
        /// <returns>List of ints represented by str when seperated by split</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="FormatException"/>
        /// <exception cref="OverflowException"/>
        public static List<int> str2ints(string str, string split) {
            List<int> numbers = new List<int>();
            string[] numberStrings;
            try {
                numberStrings = Regex.Split(str, split);
            } catch (ArgumentNullException) {
                throw new ArgumentNullException("Either the split string or the input data was null");
            } catch (ArgumentException) {
                throw new ArgumentException("The split string was not valid:\r\n\r\n" + split);
            } 
            foreach (String s in numberStrings) {
                try {
                    numbers.Add(Convert.ToInt32(s));
                } catch (FormatException) {
                    throw new FormatException("The input data did not consist of a an optional "+
                            "sign followed by a sequence of digits (0 through 9):\r\n\r\n" + s);
                } catch (OverflowException) {
                    throw new OverflowException("The input string was not of a number within the program's ranges:\r\n\r\n" + s);
                }
            }
            return numbers;
        }

        /// <summary>
        /// Checks if a string is of binary digits {0,1}
        /// </summary>
        /// <param name="str">String to check</param>
        /// <returns>True if str is of binary digits, false otherwise</returns>
        public static bool isBinaryString(string str) {
            bool binary = true;
            foreach (char c in str.ToCharArray()) {
                if  ((c != '1') && (c != '0')) {
                    binary = false;
                }
            }
            return binary;
        }

        /// <summary>
        /// Removes all the non bianry digits {0,1} from a string
        /// </summary>
        /// <param name="str">String to remove non binary digits from</param>
        /// <returns>str, without the non binary digits</returns>
        public static string removeNonBinaryChars(string str) {
            for (int i = 0; i < str.Length; i++) {
                if (str.ElementAt(i) != '0' && str.ElementAt(i) != '1') {
                    str = str.Remove(i--, 1);
                }
            }
            return str;
        }
    }
}
