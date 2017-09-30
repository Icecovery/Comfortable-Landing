using UnityEngine;
using KSP.Localization;

public class CL_ControlTool : PartModule
{
    [KSPField]
    public bool IsActivate = false;
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

    [KSPAction("Toggle Pre-Landing Mode", KSPActionGroup.None)]
    public void ActionToggle(KSPActionParam param)
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

    [KSPAction("Activate Pre-Landing Mode", KSPActionGroup.None)]
    public void ActionActivate(KSPActionParam param)
    {
            Activate();
    }

    [KSPAction("Deactivate Pre-Landing Mode", KSPActionGroup.None)]
    public void ActionDeactivate(KSPActionParam param)
    {
            Deactivate();
    }

    public override void OnStart(PartModule.StartState state)
    {
        burn = part.Modules["CL_LandingBurn"] as CL_LandingBurn;
        if (burn == null)
        {
            Debug.Log("<color=#FF8C00ff>[Comfortable Landing]</color>Not detected CL_LandingBurn");
        }

        buoy = part.Modules["CL_Buoy"] as CL_Buoy;
        if (buoy == null)
        {
            Debug.Log("<color=#FF8C00ff>[Comfortable Landing]</color>Not detected CL_Buoy");
        }
        airbag=part.Modules["CL_AirBag"] as CL_AirBag;

        if(airbag==null)
        {
            Debug.Log("<color=#FF8C00ff>[Comfortable Landing]</color>Not detected CL_AirBag");
        }

        if (burn == null && buoy == null && airbag == null)
        {
            Events["Deactivate"].guiActive = false;
            Events["Activate"].guiActive = false;
            IsActivate = false;
            Debug.Log("<color=#FF8C00ff>[Comfortable Landing]</color>Not detected any CL Module.");
        }

        if (buoy != null && airbag != null)//They are repetitive.
        {
            airbag = null;//So it will not activate in FixedUpdate.
            Debug.Log("<color=#FF8C00ff>[Comfortable Landing]</color>Detected CL_Buoy and CL_ Airbag, only activate CL_Buoy.");
        }
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
                        alreadyInflated = true;
                    }
                }
            }
            //Airbag
            if (airbag != null)
            {
                //Inflate Airbag
                if (alreadyInflatedAirBag == false)
                {
                    if (vessel.radarAltitude <= airbag.inflateAltitude)
                    {
                        airbag.Inflate();
                        alreadyInflatedAirBag = true;
                    }
                }
                //Deflate Airbag
                if (alreadyInflatedAirBag == true && alreadyDeflatedAirBag == false)
                {
                    if (vessel.Landed)
                    {
                        airbag.Deflate();
                        alreadyDeflatedAirBag = true;
                    }
                    else if (vessel.Splashed && airbag.damageAfterSplashed == true)
                    {
                        airbag.Deflate();
                        alreadyDeflatedAirBag = true;
                    }
                    else if(vessel.Splashed && airbag.damageAfterSplashed == false)
                    {
                        airbag.Touchdown();
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
                Debug.Log("<color=#FF8C00ff>[Comfortable Landing]</color>The vessel has landed or splashed, deactivate pre-landing mode.");
            }
        }
    }

}