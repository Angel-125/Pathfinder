using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using System.Reflection;

/*
Source code copyright 2018, by Michael Billard (Angel-125)
License: GPLV3

Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.
Note that Wild Blue Industries is a ficticious entity 
created for entertainment purposes. It is in no way meant to represent a real entity.
Any similarity to a real entity is purely coincidental.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    public class KKWrapper
    {
        public static Assembly kkAssembly;

        public static void Init()
        {
            if (kkAssembly == null)
            {
                foreach (AssemblyLoader.LoadedAssembly loadedAssembly in AssemblyLoader.loadedAssemblies)
                {
                    if (loadedAssembly.name == "KerbalKonstructs")
                    {
                        kkAssembly = loadedAssembly.assembly;
                        break;
                    }
                }

                if (kkAssembly == null)
                    return;
            }

            //Now init the classes and methods
            KKAPI.InitClass(kkAssembly);
        }

        public static bool isKerbalKonstructsInstalled()
        {
            if (kkAssembly != null)
                return true;
            else
                return false;
        }
    }

    public class KKAPI
    {
        static Type apiType;
        static MethodInfo miSpawnObject;

        public static void InitClass(Assembly kkAssembly)
        {
            apiType = kkAssembly.GetTypes().First(t => t.Name.Equals("API"));
            miSpawnObject = apiType.GetMethods().First(t => t.Name.Equals("SpawnObject"));
        }

        public static string SpawnObject(string objectName)
        {
            string identifier = (string)miSpawnObject.Invoke(null, new object[] { objectName });
            return identifier;
        }
    }
}
