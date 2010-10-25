using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using RandomNumbers.Utils;
using RandomNumbers.Tests;

namespace RandomNumbers {

    /// <summary>
    /// Controller of the test suite, provides an interface for the view for the model
    /// </summary>
    internal class Control {

        /// <summary>
        /// Store a reference to the view in the application
        /// </summary>
        private Front view;

        /// <summary>
        /// Constructor, takes reference for view object
        /// </summary>
        /// <remarks>Makes a reference in the view object passed to this control.
        /// This connection cannot be broken, a new controller object is created</remarks>
        /// <param name="view">The view reference for interface</param>
        internal Control(ref Front view) {
            this.view = view;
            view.Control = this;
        }

        /// <summary>
        /// Method for running tests selected in the view
        /// </summary>
        /// <remarks>
        /// Gets data from the view object, such as file for binary string file, and which tests to run with which data
        /// </remarks>
        internal void runTests() {
            Model model = new Model(Util.str2ints(Util.loadData(view.getFilePath()), view.getSplitStr()));
            List<Test> tests = new List<Test>();
            if (view.performFrequencyTest()) {
                tests.Add(new Frequency(view.getFrequency_n(), ref model));
            }
            if (view.performBlockFrequencyTest()) {
                tests.Add(new BlockFrequency(view.getBlockFrequency_M(), view.getBlockFrequency_n(), ref model));
            }
            if (view.performRunsTest()) {
                tests.Add(new Runs(view.getRuns_n(), ref model));
            }
            if (view.performLongestRunOfOnesTest()) {
                tests.Add(new LongestRunOfOnes(view.getLongestRunOfOnes_n(), ref model));
            }
            if (view.performMatrixRankTest()) {
                tests.Add(new Rank(view.getMatrixRank_n(), ref model));
            }
            if (view.performDiscreteFourierTransformTest()) {
                tests.Add(new DiscreteFourierTransform(view.getDiscreteFourierTransform_n(), ref model));
            }
            if (view.performNonOverlappingTemplateMatchingTest()) {
                if (view.performSingleNonOverlappingTemplateMatchingTest()) {
                    tests.Add(new NonOverlappingTemplateMatching(view.getNonOverlappingTemplateMatching_B(), view.getNonOverlappingTemplateMatching_n(), ref model));
                } else {
                    tests.Add(new NonOverlappingTemplateMatching(view.getNonOverlappingTemplateMatching_m(), view.getNonOverlappingTemplateMatching_n(), ref model));
                }
            }
            if (view.performOverlappingTemplateMatchingTest()) {
                tests.Add(new OverlappingTemplateMatching(view.getOverlappingTemplateMatching_B(), view.getOverlappingTemplateMatching_n(), ref model));
            }
            if (view.performUniversalTest()) {
                if (view.performCustomUniversalTest()) {
                    tests.Add(new Universal(view.getUniversal_L(), view.getUniversal_Q(), view.getUniversal_n(), ref model));
                } else {
                    tests.Add(new Universal(view.getUniversal_n(), ref model));
                }
            }
            if (view.performLinearComplexityTest()) {
                tests.Add(new LinearComplexity(view.getLinearComplexity_M(), view.getLinearComplexity_n(), ref model));
            }
            if (view.performSerialTest()) {
                tests.Add(new Serial(view.getSerial_m(), view.getSerial_n(), ref model));
            }
            if (view.performApproximateEntropyTest()) {
                tests.Add(new ApproximateEntropy(view.getApproximateEntropy_m(), view.getApproximateEntropy_n(), ref model));
            }
            if (view.performCumulativeSumsTest()) {
                tests.Add(new CumulativeSums(view.performForwardCumulativeSumsTest(), view.getCumulativeSums_n(), ref model));
            }
            if (view.performRandomExcursionsTest()) {
                tests.Add(new RandomExcursions(view.getRandomExcursions_n(), ref model));
            }
            if (view.performRandomExcursionsVariantTest()) {
                tests.Add(new RandomExcursionsVariant(view.getRandomExcursionsVariant_n(), ref model));
            }
            view.showProgressBar();
            view.updateProgressBar(10);
            Report full = new Report("All Tests");
            foreach (Test t in tests) {
                t.run(true);
                view.updateProgressBar(90 / tests.Count);
                full.Write(model.reports.Last().Value.body);
            }
            model.reports.Add(full.title, full);
            new ReportsForm(model.reports).Show();
            view.hideProgressBar();
        }
    }
}
