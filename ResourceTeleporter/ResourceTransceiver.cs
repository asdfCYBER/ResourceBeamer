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

namespace ResourceTeleporter
{
    [KSPModule("Resource Transceiver")] //copied from interstellarfuelswitch: [KSPModule("#LOC_IFS_TextureSwitch_moduleName")], probably gets loaded from cfg
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
        public int GetResourceID(string ResourceName)
        {
            PartResourceDefinition Resource = PartResourceLibrary.Instance.GetDefinition(ResourceName);
            return Resource.id;
        }

        /// <summary>Get the total cached resources on the vessel, or the total available cache if GetAvailableCache = true</summary>
        public double GetVesselCache(Vessel vessel, int resource, bool GetAvailableCache = false)
        {
            Cache = 0;
            AvailableCache = 0;
            if (vessel.loaded)
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
            else
            {
                foreach (ProtoPartSnapshot ppart in vessel.protoVessel.protoPartSnapshots) // strongly derived from ESLDBeacons
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
            if (GetAvailableCache) return AvailableCache;
            else return Cache;
        }

        public override void OnStart(StartState state)
        {
            //GetSelectableVessels();
            if (SelectableVessels.Count >= 1)
            {
                Debug.Log("[RT] First SelectableVessels element is " + SelectableVessels[0]);
            }
        }

        public override void OnUpdate()
        {
            if (SelectableVessels.Count >= 1)
            {
                double _AvailableCache = GetVesselCache(SelectableVessels[0], GetResourceID(ResourceName), true);
                Debug.Log("[RTDEBUGSPAM] SelectableVessels[0] " + " has " + _AvailableCache + " cache available");
            }
        }

        /// <summary>Get a list of vessels with an activated ResourceTransceiver module and nonzero MaxResourceCache</summary>
        [KSPEvent(active = true, guiActive = true, name = "GetSelectableVessels", guiName = "Get Selectable Vessels")]
        public void GetSelectableVessels()
        {
            SelectableVessels.Clear();
            foreach (Vessel vessel in FlightGlobals.VesselsUnloaded) // find unloaded vessels, strongly derived from ESLDBeacons
            {
                Debug.Log("[RT] Checking if unloaded vessel " + vessel + " is selectable");
                if (vessel == this.vessel) continue; // discard the calling vessel
                if (vessel == FlightGlobals.ActiveVessel) continue; // discard other controlled vessels too
                Debug.Log("[RT] Nope test passed");

                //bool CanReceive = false;
                //bool CanTransmit = false;

                foreach (ProtoPartSnapshot ppart in vessel.protoVessel.protoPartSnapshots)
                {
                    foreach (ProtoPartModuleSnapshot pmod in ppart.modules.FindAll((ProtoPartModuleSnapshot p) => p.moduleName == "ResourceTransceiver"))
                    {
                        int _mode = int.Parse(pmod.moduleValues.GetValue("mode"));
                        if (_mode == 0) continue;
                        //else if (_mode == 1) CanReceive = true;
                        //else if (_mode == 2) CanTransmit = true;

                        double _AvailableCache = GetVesselCache(vessel, GetResourceID(pmod.moduleValues.GetValue("ResourceName")), true);
                        Debug.Log("[RT] " + vessel + " has " + _AvailableCache + " cache available");
                        if (_AvailableCache > 0)
                        {
                            if (!SelectableVessels.Contains(vessel))
                            {
                                SelectableVessels.Add(vessel);
                                Debug.Log("[RT] Unloaded vessel " + vessel + " was added to SelectableVessels");
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
