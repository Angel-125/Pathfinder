using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WildBlueIndustries
{
    public class GoldStrikeUtils
    {
        public static void GetBiomeAndPlanet(out string biomeName, out int planetID, Vessel vessel, ModuleAsteroid asteroid = null)
        {
            CBAttributeMapSO.MapAttribute biome;
            biomeName = "UNKNOWN";
            planetID = -1;

            if (asteroid != null)
            {
                biomeName = asteroid.AsteroidName;
                planetID = int.MaxValue;
            }

            else if (vessel != null)
            {
                if (vessel.situation == Vessel.Situations.LANDED || vessel.situation == Vessel.Situations.PRELAUNCH)
                {
                    biome = Utils.GetCurrentBiome(vessel);
                    if (biome != null)
                    {
                        biomeName = biome.name;
                        planetID = vessel.mainBody.flightGlobalsIndex;
                    }
                    else
                    {
                        biomeName = "UNKNOWN";
                        planetID = -1;
                    }
                }
            }

            else
            {
                biomeName = "UNKNOWN";
                planetID = -1;
            }
        }

        protected static double toRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}
