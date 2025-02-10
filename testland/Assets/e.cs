using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class e : MonoBehaviour
{
    public Material[] materials;
    public Transform top;
    public Transform bottom;
    public GameObject nozzle,nozzleinner;
    public Vector3 calculatedtop;
    public Vector3 calculatedbottom;
    void Update ()
    {
        calculatedtop = top.lossyScale;
        calculatedbottom = bottom.lossyScale;
        nozzle = MakeUVCone.Create (calculatedtop,calculatedbottom, 14, 2,nozzle);
        nozzleinner = MakeUVCone.Create(calculatedbottom, calculatedtop, 14, 2, nozzleinner);
    }
}