using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KSP.Localization;

public class CL_ControlTool : PartModule
{
    [KSPField]
    public bool IsActivate = false;
    private float originalCrashTolerance = 0.0f;
    private bool alreadyFired = false;
    private bool alreadyInflated = false;
    private bool alreadyInflatedAirBag = false;
    private bool alreadyDeflatedAirBag = false;

    CL_LandingBurn burn;
    CL_Buoy buoy;
    CL_AirBag airbag;

    [KSPEvent(name = "Activate", guiName = "Activate Pre-Landing Mode", active = true, guiActive = true)]
    public void Activate()
    {
       IsActivate = true;
       Events["Deactivate"].guiActive = true;
       Events["Activate"].guiActive = false;
    }

    [KSPEvent(name = "Deactivate", guiName = "Deactivate Pre-Landing Mode", active = true, guiActive = false)]
    public void Deactivate()
    {
        IsActivate = false;
        Events["Deactivate"].guiActive = false;
        Events["Activate"].guiActive = true;
       
    }

    [KSPAction("Toggle", KSPActionGroup.None, guiName = "Toggle Pre-Landing Mode")]
    public void ActionActivate(KSPActionParam param)
    {
        if (IsActivate == false)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    public override void OnStart(PartModule.StartState state)
    {
        burn = part.Modules["CL_LandingBurn"] as CL_LandingBurn;
        if (burn == null)
        {
            Debug.Log("<color=#FF8C00ff>Comfortable Landing:</color>Not detected");
        }

        buoy = part.Modules["CL_Buoy"] as CL_Buoy;
        if (buoy == null)
        {
            Debug.Log("<color=#FF8C00ff>Comfortable Landing:</color>Not detected CL_Buoy");
        }
        airbag=part.Modules["CL_airbag"] as CL_AirBag;

        if(airbag==null)
        {
            Debug.Log("<color=#FF8C00ff>Comfortable Landing:</color>Not detected CL_AirBagg");
        }

        if (burn == null && buoy == null && airbag == null)
        {
            Events["Deactivate"].guiActive = false;
            Events["Activate"].guiActive = false;
            IsActivate = false;
            Debug.Log("<color=#FF8C00ff>Comfortable Landing:</color>Not detected any CL Module.");
        }

        originalCrashTolerance = this.part.crashTolerance;
    }
    
    public void FixedUpdate()
    {
        if (IsActivate == true)
        {
            //Landing Burn
            if (burn != null)
            {
                if (alreadyFired == false)
                {
                    if (vessel.radarAltitude <= burn.burnAltitude)
                    {
                        burn.Fire();
                        alreadyFired = true;
                    }
                }
            }
            //Inflate Buoy
            if (buoy != null)
            {
                if (alreadyInflated == false)
                {
                    if (vessel.Splashed)
                    {
                        buoy.Inflate();
                        this.part.buoyancy = buoy.buoyancyAfterInflated;//this is a real buoy!
                        alreadyInflated = true;
                    }
                }
            }
            //Inflate Airbag
            if (airbag != null)
            {
                if (alreadyInflatedAirBag == false)
                {
                    if (vessel.radarAltitude <= airbag.inflateAltitude)
                    {
                        airbag.Inflate();
                        this.part.crashTolerance = airbag.crashToleranceAfterInflated;
                        alreadyInflatedAirBag = true;
                    }
                }
            }
            //Deflate Airbag
            if (airbag != null)
            {
                if(alreadyInflatedAirBag==true && alreadyDeflatedAirBag == false)
                {
                    if (vessel.Landed)
                    {
                        airbag.Deflate();
                        this.part.crashTolerance = originalCrashTolerance;
                        alreadyDeflatedAirBag = true;
                    }
                }
            }
            //Check
            if (vessel.Landed|| vessel.Splashed)
            {
                Events["Deactivate"].guiActive = false;
                Events["Activate"].guiActive = false;
                IsActivate = false;
                Debug.Log("<color=#FF8C00ff>Comfortable Landing:</color>The vessel has landed or splashed, deactivate pre-landing mode.");
            }
        }
    }

}