using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

/*
Source code copyright 2016, by Michael Billard (Angel-125)
License: GNU General Public License Version 3
License URL: http://www.gnu.org/licenses/
Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.
Note that Wild Blue Industries is a ficticious entity 
created for entertainment purposes. It is in no way meant to represent a real entity.
Any similarity to a real entity is purely coincidental.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    class WBIBiomeAnalysis
    {
        public const string kBiomeAnalysisID = "WBIBiomeAnalysis";

        public static float GetScienceCap(Part part)
        {
            ScienceExperiment experiment = ResearchAndDevelopment.GetExperiment(kBiomeAnalysisID);
            ScienceSubject subject = ResearchAndDevelopment.GetExperimentSubject(experiment, ScienceUtil.GetExperimentSituation(part.vessel),
                part.vessel.mainBody, Utils.GetCurrentBiome(part.vessel).name);

            return subject.scienceCap;
        }

        public static ScienceData CreateData(Part part, float amount)
        {
            ScienceExperiment experiment = ResearchAndDevelopment.GetExperiment(kBiomeAnalysisID);
            ScienceSubject subject = ResearchAndDevelopment.GetExperimentSubject(experiment, ScienceUtil.GetExperimentSituation(part.vessel),
                part.vessel.mainBody, Utils.GetCurrentBiome(part.vessel).name);

            //Kerbin low orbit has a science multiplier of 1.
            ScienceSubject subjectLEO = ResearchAndDevelopment.GetExperimentSubject(experiment, ExperimentSituations.InSpaceLow,
                FlightGlobals.GetHomeBody(), "");

            //This ensures you can re-run the experiment.
            subjectLEO.science = 0f;
            subjectLEO.scientificValue = 1f;

            //Create science data
            ScienceData data = new ScienceData(amount, 1f, 0f, subjectLEO.id, subject.title);

            return data;
        }

        public static void ResetScienceGains(Part part)
        {
            ScienceExperiment experiment = ResearchAndDevelopment.GetExperiment(kBiomeAnalysisID);

            //Kerbin low orbit has a science multiplier of 1.
            ScienceSubject subjectLEO = ResearchAndDevelopment.GetExperimentSubject(experiment, ExperimentSituations.InSpaceLow,
                FlightGlobals.GetHomeBody(), "");

            //This ensures you can re-run the experiment.
            subjectLEO.science = 0f;
            subjectLEO.scientificValue = 1f;
        }
    }
}
