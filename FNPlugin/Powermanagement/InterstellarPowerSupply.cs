﻿using System;
using FNPlugin.Power;
using FNPlugin.Resources;
using UnityEngine;

namespace FNPlugin.Powermanagement
{
    [KSPModule("Power Supply")]
    class InterstellarPowerSupply : ResourceSuppliableModule, IPowerSupply
    {
        [KSPField(guiActive = true, guiName = "#LOC_KSPIE_InterstellarPowerSupply_TotalPowerSupply", guiFormat = "F2", guiUnits = "#LOC_KSPIE_Reactor_megawattUnit")]//Total Power Supply
        public double totalPowerSupply;
        [KSPField(guiActive = false, guiName = "#LOC_KSPIE_InterstellarPowerSupply_Proces")]//Proces
        public string displayName = "";
        [KSPField(guiActive = false, guiName = "#LOC_KSPIE_InterstellarPowerSupply_PowerPriority")]//Power Priority
        public int powerPriority = 4;

        protected ResourceBuffers resourceBuffers;
        protected double currentPowerSupply;

        public int PowerPriority
        {
            get { return powerPriority; }
            set { powerPriority = value; }
        }

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        public override void OnStart(PartModule.StartState state)
        {
            displayName = part.partInfo.title;
            string[] resourcesToSupply = { ResourceSettings.Config.ElectricPowerInMegawatt };
            this.resourcesToSupply = resourcesToSupply;

            if (state == StartState.Editor) return;

            resourceBuffers = new ResourceBuffers();
            resourceBuffers.AddConfiguration(new ResourceBuffers.TimeBasedConfig(ResourceSettings.Config.ElectricPowerInMegawatt));
            if (!Kerbalism.IsLoaded)
                resourceBuffers.AddConfiguration(new ResourceBuffers.TimeBasedConfig(ResourceSettings.Config.ElectricPowerInKilowatt, 1000));
            resourceBuffers.Init(part);

            Debug.Log("[KSPI]: PowerSupply on " + part.name + " was Force Activated");
            this.part.force_activate();
        }

        public double ConsumeMegajoulesFixed(double powerRequest, double fixedDeltaTime)
        {
            return consumeFNResource(powerRequest, ResourceSettings.Config.ElectricPowerInMegawatt, fixedDeltaTime);
        }

        public double ConsumeMegajoulesPerSecond(double powerRequest)
        {
            return ConsumeFnResourcePerSecond(powerRequest, ResourceSettings.Config.ElectricPowerInMegawatt);
        }

        public void SupplyMegajoulesPerSecondWithMax(double supply, double maxsupply)
        {
            currentPowerSupply += supply;

            SupplyFnResourcePerSecondWithMax(supply, maxsupply, ResourceSettings.Config.ElectricPowerInMegawatt);
        }

        public override string getResourceManagerDisplayName()
        {
            return displayName;
        }

        public override string GetInfo()
        {
            return displayName;
        }

        public override int getPowerPriority()
        {
            return powerPriority;
        }

        public void Update()
        {
            if (HighLogic.LoadedSceneIsEditor)
                return;

            totalPowerSupply = GetCurrentResourceSupply(ResourceSettings.Config.ElectricPowerInMegawatt);
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            resourceBuffers.UpdateVariable(ResourceSettings.Config.ElectricPowerInMegawatt, currentPowerSupply);
            if (!Kerbalism.IsLoaded)
                resourceBuffers.UpdateVariable(ResourceSettings.Config.ElectricPowerInKilowatt, currentPowerSupply);
            resourceBuffers.UpdateBuffers();

            currentPowerSupply = 0;
        }
    }
}
