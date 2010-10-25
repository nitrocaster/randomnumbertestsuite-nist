using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using RandomNumbers.Utils;

namespace RandomNumbers.Tests {
    public partial class ReportsForm : Form {

        private Dictionary<string, Report> reports;

        public ReportsForm(Dictionary<string,Report> reports) {
            this.reports = reports;
            InitializeComponent();
            this.reportBox.Items.AddRange(reports.Keys.ToArray());
            this.reportBox.SelectedIndex = 0;
        }

        private void reportBox_SelectedIndexChanged(object sender, EventArgs e) {
            Report r;
            reports.TryGetValue(reportBox.Text, out r);
            reportBody.Text = r.body;
        }

        private void button1_Click(object sender, EventArgs e) {
            if (saveReportDialog.ShowDialog() == DialogResult.OK) {
                string filePath = saveReportDialog.FileName;
                try {
                    using (StreamWriter output = new StreamWriter(filePath)) {
                        foreach (Report r in reports.Values) {
                            output.WriteLine(r.body);
                        }
                    }
                } catch (ArgumentNullException) {
                    throw new ArgumentNullException("The input file path is not a valid one:\r\n\r\n" + filePath);
                } catch (FormatException) {
                    throw new FormatException("The input directory does not exist:\r\n\r\n" + filePath);
                } catch (ObjectDisposedException) {
                    throw new ObjectDisposedException("The user does not have the permissions to access this file:\r\n\r\n" + filePath);
                } catch (IOException) {
                    throw new IOException("Failed to read from file successfully:\r\n\r\n" + filePath);
                }
            }
        }
    }


}
