using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using ModuleWheels;

/*
Source code copyright 2018, by Michael Billard (Angel-125)
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
    public class WBIScoopTurbine : PartModule
    {
        List<BaseConverter> converters;
        List<ModuleResourceIntake> intakes;

        [KSPField()]
        public string rotationTransform = string.Empty;

        [KSPField()]
        public float rotationRate;

        [KSPField()]
        public string rotationAxis = string.Empty;

        protected float rotationPerFrame;
        protected float currentAngle;
        protected Transform rotator;
        protected Vector3 axisRate = new Vector3(0, 1, 0);

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            converters = this.part.FindModulesImplementing<BaseConverter>();
            intakes = this.part.FindModulesImplementing<ModuleResourceIntake>();

            //Get rotations per frame
            rotationPerFrame = rotationRate * TimeWarp.fixedDeltaTime;

            //Get the rotation transform
            if (string.IsNullOrEmpty(rotationTransform) == false)
                rotator = this.part.FindModelTransform(rotationTransform);

            //Get the rotation axis
            if (string.IsNullOrEmpty(rotationAxis) == false)
            {
                string[] axisValues = rotationAxis.Split(',');
                float value;
                if (axisValues.Length == 3)
                {
                    if (float.TryParse(axisValues[0], out value))
                        axisRate.x = value * rotationPerFrame;
                    if (float.TryParse(axisValues[1], out value))
                        axisRate.y = value * rotationPerFrame;
                    if (float.TryParse(axisValues[2], out value))
                        axisRate.z = value * rotationPerFrame;
                }
            }

            else //Default is to rotate along the z-axis.
            {
                axisRate.z = 1 * rotationPerFrame;
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            bool isActivated = false;
            int totalCount;

            if (!HighLogic.LoadedSceneIsFlight)
                return;

            //Find a running intake
            if (intakes == null)
                intakes = this.part.FindModulesImplementing<ModuleResourceIntake>();
            if (intakes != null)
            {
                totalCount = intakes.Count;
                for (int index = 0; index < totalCount; index++)
                {
                    if (intakes[index].intakeEnabled)
                    {
                        isActivated = true;
                        break;
                    }
                }
            }

            //If no intakes are active, see if we have a running converter
            if (!isActivated)
            {
                if (converters == null)
                    converters = this.part.FindModulesImplementing<BaseConverter>();
                if (converters != null)
                {
                    totalCount = converters.Count;
                    for (int index = 0; index < totalCount; index++)
                    {
                        if (converters[index].IsActivated)
                        {
                            isActivated = true;
                            break;
                        }
                    }
                }
            }

            //Spin the turbine
            if (isActivated)
            {
                rotator.Rotate(axisRate.x, axisRate.y, axisRate.z);
            }
        }
    }
}
