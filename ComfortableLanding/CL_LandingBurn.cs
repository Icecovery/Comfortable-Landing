using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KSP.Localization;

public class CL_LandingBurn : PartModule
{
    [KSPField]
    ModuleEngines engine;

    [KSPField]
    public double burnAltitude = 3.0;

    public override void OnStart(PartModule.StartState state)
    {
        engine = part.Modules["ModuleEngines"] as ModuleEngines;
        if (engine == null)
            engine = part.Modules["ModuleEnginesFX"] as ModuleEngines;
        if (engine == null)
            engine = part.Modules["ModuleEnginesRF"] as ModuleEngines;
        if (engine == null)
            Debug.Log("<color=#FF8C00ff>Comfortable Landing:</color>Engine Missing!");
    }
    public void Fire()
    {
        Debug.Log("<color=#FF8C00ff>Comfortable Landing:</color>Landing Burn!");
        ScreenMessages.PostScreenMessage("<color=#00ff00ff>[ComfortableLanding]Landing Burn!</color>", 3f, ScreenMessageStyle.UPPER_CENTER);
        engine.Activate();        
    }
 }