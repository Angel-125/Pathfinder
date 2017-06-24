using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WildBlueIndustries
{
    public class GoldStrikeUtils
    {
        //Adapted from https://rosettacode.org/wiki/Haversine_formula#C.23
        //Released under GNU Free Documentation License 1.2
        //Returns kilometers
        public static double HaversineDistance(double lon1, double lat1, double lon2, double lat2, CelestialBody body)
        {
            var R = body.Radius / 1000f; // In kilometers
            var dLat = toRadians(lat2 - lat1);
            var dLon = toRadians(lon2 - lon1);
            lat1 = toRadians(lat1);
            lat2 = toRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Asin(Math.Sqrt(a));
            return R * 2 * Math.Asin(Math.Sqrt(a));
        }

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
                    biomeName = biome.name;
                    planetID = vessel.mainBody.flightGlobalsIndex;
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
