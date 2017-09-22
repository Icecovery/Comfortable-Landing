using UnityEngine;
using KSP.Localization;

public class CL_Buoy : PartModule
{
    [KSPField]
    ModuleAnimateGeneric InflateAnim;

    [KSPField]
    AudioClip playSound;
    AudioSource audioSource;

    [KSPField]
    public float buoyancyAfterInflated = 1.2f;

    public string playSoundPath = "ComfortableLanding/Sounds/Inflate_B";
    public float volume = 1.0f;

    public override void OnStart(StartState state)
    {
        InflateAnim=part.Modules["ModuleAnimateGeneric"] as ModuleAnimateGeneric;
        if (InflateAnim == null)
            Debug.Log("<color=#FF8C00ff>[Comfortable Landing]</color>Animation Missing!");

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.bypassListenerEffects = true;
        audioSource.minDistance = .3f;
        audioSource.maxDistance = 1000;
        audioSource.priority = 10;
        audioSource.dopplerLevel = 0;
        audioSource.spatialBlend = 1;
        playSound = GameDatabase.Instance.GetAudioClip(playSoundPath);
        audioSource.clip = playSound;
        audioSource.loop = false;
        audioSource.time = 0;
        if (volume < 0)
        {
            volume = 0.0f;
        }
        else if (volume > 1)
        {
            volume = 1.0f;
        }
        audioSource.volume = volume;
    }
    public void Inflate()
    {
        ScreenMessages.PostScreenMessage("<color=#00ff00ff>[ComfortableLanding]Inflate!</color>", 3f, ScreenMessageStyle.UPPER_CENTER);
        audioSource.PlayOneShot(playSound);
        InflateAnim.allowManualControl = true;
        InflateAnim.Toggle();
        InflateAnim.allowManualControl = false;
        this.part.buoyancy = buoyancyAfterInflated;//This is a really buoy!
        Debug.Log("<color=#FF8C00ff>[Comfortable Landing]</color>Inflate!");
    }
}