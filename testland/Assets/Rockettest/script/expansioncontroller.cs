using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class expansioncontroller : MonoBehaviour
{
    #region declerations
    
    #region player driven

    [Header("------------------Player Driven Variables------------------")]
    [Space(2)]
    [Tooltip("0-1, higher=more clean")] [Range(0,1)] public float mixtureRatio;
    [Tooltip("10-400 Bar")] public float chamberPressure;
    [Tooltip("todo")] public float throatradius;
    [Tooltip("todo")] public float exitradius;
    [Tooltip("Fuel Combination")] [SerializeField] fuelComp FC = new fuelComp();
    [Tooltip("Engine Cycle")][SerializeField] engineCycle EC = new engineCycle();
    [Tooltip("70-120 during normal runs")] [Range(70,120)] public float throttle;
    
    [Space(2)]
    #endregion

    #region engine driven
    
    [Header("-----------------'Engine' Driven Variables------------------")]
    [Space(2)]
    [Tooltip("0-1, higher=more clean")] [Range(0,1)] public float currentmixratio;
    [Tooltip("0-1, higher=more clean")] [Range(0,1)] [SerializeField] private float stoichemetry;
    [Tooltip("1000-6000 km/s")] [SerializeField] private float exhaustVelocity;
    [Tooltip("Exhaust products")] [SerializeField] exhaust ex = new exhaust();
    [Tooltip("0.8-4 atmo")] [SerializeField] private float exhaustPressure;
    [Tooltip("0.8-4 atmo")] [SerializeField] private float exhaustTemperature;
    [Tooltip("todo")] [SerializeField] private float flowRate;
    [Tooltip("todo")] [SerializeField] private float exitArea;
    [Tooltip("todo")] [SerializeField] private float exitRatio;
    [Tooltip("stands at 1 atmo always")] [SerializeField] private float atmosphericPressure=1;
    [Tooltip("stands at 9.8ms always always")] [SerializeField] private float gravity=9.8f;
    [Tooltip("500-5000 degrees Kelvin")] [SerializeField] private float chamberTemperature = 3670;
    [Tooltip("MN")] public float thrust;
    [Tooltip("Seconds")] public float ISP;
    [Space(2)]
    #endregion

    #region displayed
    
    [Header("-------------------Displayed Variables---------------------")]
    [Space(2)]
    [Tooltip("MN")] public float displayedthrust;
    [Tooltip("s")] public float displayedisp;
    [Tooltip("60-140")] public float trueThrottle;
    [Tooltip("60-140")] public float throttlevalue;
    
    
    #endregion

    #region other
    
    [Space(2)]
    [Header("-------------------Other---------------------")]
    [Space(2)]
    [SerializeField] private float pressure;
    public Animator anim;
    public ParticleSystem ps, ps2,mach;
    public Transform holder;
    [SerializeField] private Gradient uncleansl, uncleanalt, stoichometricsl, stoichometricalt, sealevel, altitude, result, uncleanpsl, uncleanpalt, stoichometricpsl,stoichometricpalt, sealevelp, altitudep, resultp;
    [SerializeField] private AnimationCurve sealevelc, altitudec, resultc;
    [SerializeField] private List<ParticleSystem> pses;
    

    #endregion
    
    #endregion

    #region enums
    
    enum fuelComp
    {
        Kerolox,
        Methalox,
        Hydrolox,
        Hypergolic
    }

    enum exhaust
    {
        Sooty,
        Water,
        Methane,
        Cancerjuice
    }
    enum engineCycle
    {
        Open,
        ClosedFuelRich,
        ClosedOxRich,
        FullFlowStaged,
        PressureFed
    }
    
    #endregion

    public ParticleSystem part;
    public void Start()
    {
        resultc.CopyFrom(sealevelc);
    }

    public void FixedUpdate()
    {
        #region Nozzle Calculations
        
        exitArea = Mathf.PI*(exitradius*Mathf.Exp(2));
        exitRatio = exitradius/throatradius;
        //chamberTemperature = stoichemetry;
        
        #endregion
        
        #region exhaust calculations
        
        flowRate = chamberPressure * (exitArea / exitRatio);
        exhaustPressure = (chamberPressure/100) / exitRatio;
        exhaustTemperature = chamberTemperature * exhaustPressure;
        exhaustVelocity = flowRate / (atmosphericPressure*exhaustPressure);
        
        #endregion
        
        #region Thrust and ISP calc
        thrust = flowRate / exhaustVelocity*exhaustPressure;
        ISP = exhaustVelocity / gravity;
        
        #endregion

        #region visexhaust calculations

        stoichemetry = currentmixratio;
        pressure = Mathf.Sqrt(exhaustPressure / atmosphericPressure);

        #endregion
        
        #region gradientconversions
        sealevel = gradientconvert.Lerp(uncleansl, stoichometricsl, stoichemetry);
        altitude = gradientconvert.Lerp(uncleanalt, stoichometricalt, stoichemetry);
        result = gradientconvert.Lerp(sealevel, altitude, pressure);
        sealevelp = gradientconvert.Lerp(uncleanpsl, stoichometricpsl, stoichemetry);
        altitudep = gradientconvert.Lerp(uncleanpalt, stoichometricpalt, stoichemetry);
        resultp = gradientconvert.Lerp(sealevelp, altitudep, pressure);
        #endregion
        
        #region particlesystem works
        var sz = ps.sizeOverLifetime;
        var cz = ps.colorOverLifetime;
        var czp = ps2.colorOverLifetime;
        var mainModule = ps.main;
        var machmain= mach.main;
        holder.transform.localScale = new Vector3(1, Mathf.Lerp(1, 0.1f, pressure*throttlevalue), 1);
        machmain.startColor = Color.Lerp(Color.white, new Color(255, 255, 255, 0),pressure);
        mainModule.startSize = Mathf.SmoothStep(4, 40, pressure);
        mainModule.startLifetime = Mathf.SmoothStep(0.2f, 0.4f, pressure);
        mainModule.startSpeed =Mathf.SmoothStep(120*throttlevalue, 160*throttlevalue, pressure);
        cz.color = new ParticleSystem.MinMaxGradient(result);
        czp.color = new ParticleSystem.MinMaxGradient(resultp);
        sz.size = new ParticleSystem.MinMaxCurve(1f, resultc);
        var mainpar = ps2.main;
        mainpar.startSize = Mathf.SmoothStep(20, 70, pressure);
        mainpar.startSpeed =Mathf.SmoothStep(250*throttlevalue, 320*throttlevalue, pressure);
        mainpar.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 1, 1, Mathf.Lerp(0.7f, 0.001f, pressure)));
        foreach (ParticleSystem ps in pses)
        {
            var sz1 = ps.sizeOverLifetime;
            var cz1 = ps.colorOverLifetime;
            var mainModule1 = ps.main;
            cz1.color = new ParticleSystem.MinMaxGradient(result);
            sz1.size = new ParticleSystem.MinMaxCurve(1.2f, resultc);
            mainModule1.startSize = Mathf.SmoothStep(4, 30*pses.IndexOf(ps)-0.25f, pressure);
            mainModule1.startLifetime = Mathf.SmoothStep(0.2f, 0.45f, pressure);
            mainModule1.startSpeed =Mathf.SmoothStep(120*throttlevalue, 160*throttlevalue*pses.IndexOf(ps), pressure);
        }
        for (int i = 0; i < sealevelc.length; i++)
        {
            resultc.MoveKey(i, new Keyframe(altitudec[i].time, Mathf.SmoothStep(sealevelc[i].value,altitudec[i].value, pressure)));
            resultc.SmoothTangents(i,0.2f);
        }
        #endregion
    }
    public void Update()
    {
        #region throttlecalc

        
        throttle = Mathf.Clamp(throttle, 70, 120);
        trueThrottle = Mathf.Lerp(trueThrottle, throttle, 0.05f);
        throttlevalue = trueThrottle/100;

        #endregion
        #region input
        if (Input.GetKey(KeyCode.E))
        {
            anim.enabled = true;
            anim.Play("Startup");
        }
        if (Input.GetKey(KeyCode.Q))
        {
            anim.enabled = true;
            anim.Play("Shutdown");
        }
        if (Input.GetKey(KeyCode.R))
        {
            throttle += 1f;
        }
        if (Input.GetKey(KeyCode.F))
        {
            throttle -= 1f;
        }
        #endregion
    }

    
    public void animshut()
    {
        anim.enabled = false;
    }

    public void trenchtest()
    {
        var emissionModule = part.emission;
        emissionModule.enabled = true;
    }
    public IEnumerator transient()
    {
        while (currentmixratio <= mixtureRatio-0.05f)
        {
            currentmixratio = Mathf.Lerp(currentmixratio, mixtureRatio, 0.04f);
            yield return new WaitForFixedUpdate();
        }
        animshut();
        while (trueThrottle <= throttle-0.05f)
        {
            trueThrottle = Mathf.Lerp(trueThrottle, throttle, 0.02f);
            yield return new WaitForFixedUpdate();
        }
    }
    public IEnumerator shutdowntransient()
    {
        while (currentmixratio >= 0.2f)
        {
            Debug.Log("called");
            currentmixratio = Mathf.Lerp(currentmixratio, 0.1f, 0.01f);
            yield return null;
        }
        
        var emissionModule = part.emission;
        emissionModule.enabled = false;
    }
}
