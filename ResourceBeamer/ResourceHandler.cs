using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI;

namespace ResourceBeamer
{
    class ResourceHandler // All methods about reading or editing the resource contents of a vessel
    {
        /// <summary>Returns the amount of the selected resource on a (loaded) vessel, or the mamimum amount if GetMaxResource = true</summary>
        /// <param name="GetMaxResource">Return the maximum amount of the resource instead of the current amount if true</param>
        public double GetVesselResource(Vessel vessel, int resource, bool GetMaxResource = false)
        {
            if (!vessel.loaded) // GetConnectedResourceTotals doesn't work on unloaded vessels, so prevent errors by returning 0
            {
                Debug.LogWarning("[RT] Vessel " + vessel + " is not loaded, no resources found");
                return 0;
            }

            vessel.GetConnectedResourceTotals(resource, out double ResourceAmount, out double MaxResourceAmount);
            if (GetMaxResource) return MaxResourceAmount; // Return the max amount of resource if the optional parameter was true, else return the current amount
            else return ResourceAmount;
        }

        /// <summary>Hold the resource transceiver upside down and hope there is enough space</summary>
        /// <param name="transceiver"></param>
        public void EmptyResourceCache(ResourceTransceiver transceiver) // Todo: (perhaps as difficulty option): cache that could not be emptied gets destroyed
        {
            Vessel vessel = transceiver.vessel;
            if (!vessel.loaded) // This doesn't work on unloaded vessels, so prevent errors by returning 0
            {
                Debug.LogWarning("[RT] Vessel " + vessel + " is not loaded, cache not emptied");
                return;
            }

            int ResourceID = ResourceTransceiver.GetResourceID(transceiver.ResourceName);
            double PushedCache = vessel.RequestResource(transceiver.part, ResourceID, -transceiver.Cache, true); // Empty the cache into the resource partmodules
            transceiver.Cache -= PushedCache;// Set the transceiver cache to the leftovers
            Debug.Log("[RT] " + PushedCache + " of " + transceiver.ResourceName + " Resource Transceiver cache was pushed to the main storage on vessel " + vessel.name);

            if (transceiver.Cache < 0) // I'm pretty sure this is impossible but it's annoying if it does happen
            {
                transceiver.Cache = 0;
                Debug.LogWarning("[RT] Negative cache occured on vessel " + vessel.name + " after pushing, the cache on this part has been reset");
            }
        }
    }
}
