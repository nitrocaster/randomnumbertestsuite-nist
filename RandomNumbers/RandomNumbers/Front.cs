using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using RandomNumbers.Utils;

namespace RandomNumbers
{
    internal partial class Front : Form {

        private Control control;
        internal Control Control {
            get {
                return control;
            }
            set {
                if (control == null) control = value;
            }
        }


        internal Front() {
            InitializeComponent();
        }

        private void browseButton_Click(object sender, EventArgs e) {
            openNumbersFile.InitialDirectory = fileNameBox.Text;
            if (openNumbersFile.ShowDialog() == DialogResult.OK) {
                fileNameBox.Text = openNumbersFile.FileName;
            }
        }

        private void runButton_Click(object sender, EventArgs e) {
            try {
                control.runTests();
            } catch (ArgumentException ex) {
                MessageBox.Show(ex.Message, "Bad Argument");
            } catch (FileNotFoundException ex) {
                MessageBox.Show(ex.Message, "File Not Found");
            } catch (DirectoryNotFoundException ex) {
                MessageBox.Show(ex.Message, "Directory Not Found");
            } catch (UnauthorizedAccessException ex) {
                MessageBox.Show(ex.Message, "Incorrect Privilages");
            } catch (IOException ex) {
                MessageBox.Show(ex.Message, "IO Error");
            } catch (OutOfMemoryException ex) {
                MessageBox.Show(ex.Message, "Out Of Memory");
            } catch (FormatException ex) {
                MessageBox.Show(ex.Message, "Wrong Data Format");
            }
        }

        private void nonOverlappingTemplateMatchingSingleCheck_CheckedChanged(object sender, EventArgs e) {
            nonOverlappingTemplateMatchingMBLabel.Text = nonOverlappingTemplateMatchingMBLabel.Text.Equals("m") ?
                "B" : "m";
            nonOverlappingTemplateMatching_m.Visible = !nonOverlappingTemplateMatching_m.Visible;
            nonOverlappingTemplateMatching_B.Visible = !nonOverlappingTemplateMatching_B.Visible;
        }

        private void univeresalCustomCheck_CheckedChanged(object sender, EventArgs e) {
            universalLLabel.Visible = !universalLLabel.Visible;
            universalQLabel.Visible = !universalQLabel.Visible;
            universal_L.Visible = !universal_L.Visible;
            universal_Q.Visible = !universal_Q.Visible;
        }

        private void binaryString_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
            foreach (char c in ((TextBox)sender).Text.ToCharArray()) {
                if ((c != '1') && (c != '0')) {
                    MessageBox.Show("The input must be a binary string", "Bad Argument");
                    ((TextBox)sender).Text = Util.removeNonBinaryChars(((TextBox)sender).Text);
                    ((TextBox)sender).Focus();
                    break;
                }
            }
        }

        internal String getFilePath() {
            return this.fileNameBox.Text;
        }

        internal String getSplitStr() {
            return this.splitCharBox.Text;
        }

        internal bool performFrequencyTest() {
            return frequencyCheck.Checked;
        }

        internal bool performBlockFrequencyTest() {
            return blockFrequencyCheck.Checked;
        }

        internal bool performRunsTest() {
            return runsCheck.Checked;
        }

        internal bool performLongestRunOfOnesTest() {
            return longestRunOfOnesCheck.Checked;
        }

        internal bool performMatrixRankTest() {
            return matrixRankCheck.Checked;
        }

        internal bool performDiscreteFourierTransformTest() {
            return discreteFourierTransformCheck.Checked;
        }

        internal bool performNonOverlappingTemplateMatchingTest() {
            return nonOverlappingTemplateMatchingCheck.Checked;
        }

        internal bool performOverlappingTemplateMatchingTest() {
            return overlappingTemplateMatchingCheck.Checked;
        }

        internal bool performLinearComplexityTest() {
            return linearComplexityCheck.Checked;
        }

        internal bool performUniversalTest() {
            return universalCheck.Checked;
        }

        internal bool performSerialTest() {
            return serialCheck.Checked;
        }

        internal bool performApproximateEntropyTest() {
            return approximateEntropyCheck.Checked;
        }

        internal bool performCumulativeSumsTest() {
            return cumulativeSumsCheck.Checked;
        }

        internal bool performRandomExcursionsTest() {
            return randomExcursionsCheck.Checked;
        }

        internal bool performRandomExcursionsVariantTest() {
            return randomExcursionsVariantCheck.Checked;
        }

        internal int getFrequency_n() {
            return (int) frequency_n.Value;
        }

        internal int getBlockFrequency_n() {
            return (int) blockFrequency_n.Value;
        }

        internal int getBlockFrequency_M() {
            return (int) blockFrequency_M.Value;
        }

        internal int getRuns_n() {
            return (int) runs_n.Value;
        }

        internal int getLongestRunOfOnes_n() {
            return (int)longestRunOfOnes_n.Value;
        }

        internal int getMatrixRank_n() {
            return (int)matrixRank_n.Value;
        }

        internal int getDiscreteFourierTransform_n() {
            return (int)discreteFourierTransform_n.Value;
        }

        internal int getNonOverlappingTemplateMatching_n() {
            return (int)nonOverlappingTemplateMatching_n.Value;
        }

        internal int getNonOverlappingTemplateMatching_m() {
            return (int)nonOverlappingTemplateMatching_m.Value;
        }

        internal String getNonOverlappingTemplateMatching_B() {
            return nonOverlappingTemplateMatching_B.Text;
        }

        internal bool performSingleNonOverlappingTemplateMatchingTest() {
            return nonOverlappingTemplateMatchingSingleCheck.Checked;
        }

        internal int getOverlappingTemplateMatching_n() {
            return (int)overlappingTemplateMatching_n.Value;
        }
        
        internal String getOverlappingTemplateMatching_B() {
            return overlappingTemplateMatching_B.Text;
        }

        internal int getUniversal_n() {
            return (int)universal_n.Value;
        }

        internal int getUniversal_L() {
            return (int)universal_L.Value;
        }

        internal int getUniversal_Q() {
            return (int)universal_Q.Value;
        }

        internal bool performCustomUniversalTest() {
            return universalCustomCheck.Checked;
        }

        internal int getLinearComplexity_n() {
            return (int)linearComplexity_n.Value;
        }

        internal int getLinearComplexity_M() {
            return (int)linearComplexity_M.Value;
        }

        internal int getSerial_n() {
            return (int)serial_n.Value;
        }

        internal int getSerial_m() {
            return (int)serial_m.Value;
        }

        internal int getApproximateEntropy_n() {
            return (int)approximateEntropy_n.Value;
        }

        internal int getApproximateEntropy_m() {
            return (int)approximateEntropy_m.Value;
        }

        internal int getCumulativeSums_n() {
            return (int)cumulativeSums_n.Value;
        }

        internal bool performForwardCumulativeSumsTest() {
            return cumulativeSumsForwardCheck.Checked;
        }

        internal int getRandomExcursions_n() {
            return (int)randomExcursions_n.Value;
        }

        internal int getRandomExcursionsVariant_n() {
            return (int)randomExcursionsVariant_n.Value;
        }

        internal void updateProgressBar(int percent) {
            if (this.progressBar1.Value + percent <= 100) {
                this.progressBar1.Value += percent;
            } else {
                this.progressBar1.Value = 100;
            }
        }

        internal void showProgressBar() {
            this.progressBar1.Show();
        }

        internal void hideProgressBar() {
            this.progressBar1.Hide();
        }
    }
}