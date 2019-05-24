// Todo: add commnet check
// Todo: add teleportation of resources or cache on one vessel to cache on another vessel
// Todo: add transfer efficiency based on commnet connection status and distance
// Todo: add GUI (select vessel from SelectableVessels, select transfer amount, view transfer cost, view transfer efficiency)
// Todo: add localization (low priority if any at all)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using CommNet;
using KSP.UI;

namespace ResourceBeamer // The main partmodule namespace
{
    [KSPModule("Resource Transceiver")]
    public class ResourceTransceiver : PartModule
    {
        [KSPField(isPersistant = true)]
        public string ResourceName;

        public int ResourceID;

        [KSPField(isPersistant = true)]
        public int mode; //0 = off, 1 = receive, 2 = transmit, maybe 3 = relay?

        [KSPField(isPersistant = true, guiActive = true, guiName = "Transceiver Cache")]
        public double ResourceCache;

        [KSPField(isPersistant = true)]
        public double MaxResourceCache;

        public List<Vessel> SelectableVessels;

        public double Cache;
        public double AvailableCache;

        /// <summary>Get resourceID from name</summary>
        public static int GetResourceID(string ResourceName) // While it could and should reside in ResourceHandler, this method is too useful here
        {
            PartResourceDefinition Resource = PartResourceLibrary.Instance.GetDefinition(ResourceName);
            return Resource.id;
        }

        /// <summary>Get the total cached resources on the vessel, or the total available cache if GetAvailableCache = true</summary>
        /// <param name="GetAvailableCache">Return AvailableCache instead of Cache if true</param>
        public double GetVesselCache(Vessel vessel, int resource, bool GetAvailableCache = false)
        {
            Cache = 0;
            AvailableCache = 0;
            if (vessel.loaded) // Get the cache on a loaded vessel
            {
                foreach (ResourceTransceiver transceiver in vessel.FindPartModulesImplementing<ResourceTransceiver>())
                {
                    if (GetResourceID(transceiver.ResourceName) == resource)
                    {
                        Cache += ResourceCache;
                        AvailableCache += MaxResourceCache - ResourceCache;
                    }
                }
            }
            else // Get the cache on an unloaded vessel
            {
                foreach (ProtoPartSnapshot ppart in vessel.protoVessel.protoPartSnapshots)
                {
                    foreach (ProtoPartModuleSnapshot pmod in ppart.modules.FindAll((ProtoPartModuleSnapshot p) => p.moduleName == "ResourceTransceiver"))
                    {
                        if (GetResourceID(pmod.moduleValues.GetValue("ResourceName")) == resource)
                        {
                            Cache += double.Parse(pmod.moduleValues.GetValue("ResourceCache"));
                            AvailableCache += double.Parse(pmod.moduleValues.GetValue("MaxResourceCache")) - double.Parse(pmod.moduleValues.GetValue("ResourceCache"));
                        }
                    }
                }
            }
            if (GetAvailableCache) return AvailableCache; // Return the available cache if the optional parameter was true, else return the cache
            else return Cache;
        }

        public override void OnStart(StartState state)
        {
            
            /*GetSelectableVessels();
            if (SelectableVessels.Count >= 1)
            {
                Debug.Log("[RT] First SelectableVessels element is " + SelectableVessels[0]);
            }*/
        }

        public override void OnUpdate() // TODO: remove this nonsense and maybe replace it with something useful
        {
            if (SelectableVessels.Count >= 1)
            {
                double _AvailableCache = GetVesselCache(SelectableVessels[0], GetResourceID(ResourceName), true);
                Debug.Log("[RTDEBUGSPAM] SelectableVessels[0] " + " has " + _AvailableCache + " cache available");
            }
        }

        // Todo: Make this usable from other places
        /// <summary>Get a list of vessels with an activated ResourceTransceiver module and nonzero MaxResourceCache</summary>
        [KSPEvent(active = true, guiActive = true, name = "GetSelectableVessels", guiName = "Get Selectable Vessels")] // For debugging purposes
        public void GetSelectableVessels()
        {
            SelectableVessels.Clear();
            foreach (Vessel vessel in FlightGlobals.VesselsUnloaded) // find unloaded vessels
            {
                Debug.Log("[RT] Checking if unloaded vessel " + vessel + " is selectable");
                if (vessel == this.vessel) continue; // discard the calling vessel
                if (vessel == FlightGlobals.ActiveVessel) continue; // discard other controlled vessels too

                foreach (ProtoPartSnapshot ppart in vessel.protoVessel.protoPartSnapshots)
                {
                    foreach (ProtoPartModuleSnapshot pmod in ppart.modules.FindAll((ProtoPartModuleSnapshot p) => p.moduleName == "ResourceTransceiver"))
                    {
                        int _mode = int.Parse(pmod.moduleValues.GetValue("mode")); // if this throws an exception it's really all your fault
                        if (_mode == 0) continue;

                        double _AvailableCache = GetVesselCache(vessel, GetResourceID(pmod.moduleValues.GetValue("ResourceName")), true);

                        //Debug.Log("[RT] " + vessel + " has " + _AvailableCache + " cache available");

                        if (_AvailableCache > 0)
                        {
                            if (!SelectableVessels.Contains(vessel))
                            {
                                SelectableVessels.Add(vessel);
                                Debug.Log("[RT] Unloaded vessel " + vessel + " was added to SelectableVessels (available cache: " + _AvailableCache + ")");
                            }
                        }
                    }
                }
            }
            foreach (Vessel vessel in FlightGlobals.VesselsLoaded) // find loaded vessels
            {
                Debug.Log("[RT] Checking if loaded vessel " + vessel + " is selectable");
                if (vessel == this.vessel) continue;
                if (vessel == FlightGlobals.ActiveVessel) continue;
                Debug.Log("[RT] Nope test passed");

                if (vessel.FindPartModulesImplementing<ResourceTransceiver>().Count > 0)
                {
                    foreach (ResourceTransceiver transceiver in vessel.FindPartModulesImplementing<ResourceTransceiver>())
                    {
                        if (transceiver.mode == 0) continue;
                        double _AvailableCache = GetVesselCache(vessel, GetResourceID(transceiver.ResourceName), true);
                        Debug.Log("[RT] " + vessel + " has " + _AvailableCache + " cache available");
                        if (_AvailableCache > 0)
                        {
                            if (!SelectableVessels.Contains(vessel))
                            {
                                SelectableVessels.Add(vessel);
                                Debug.Log("[RT] Loaded vessel " + vessel + " was added to SelectableVessels");
                            }
                        }
                    }
                }
            }
        }
    }
}
