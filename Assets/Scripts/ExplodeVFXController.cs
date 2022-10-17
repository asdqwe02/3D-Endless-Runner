using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeVFXController : MonoBehaviour
{
    [SerializeField] Transform _blast;
    [SerializeField] Transform _spark;
    ParticleSystemRenderer _mainPSR, _sparkPSR; // PSR: particle system renderer
    LineRenderer _blastLR; // blast line renderer 
    private void Awake()
    {
        _blastLR = _blast.GetComponent<LineRenderer>();
        _sparkPSR = _spark.GetComponent<ParticleSystemRenderer>();
        _mainPSR = GetComponent<ParticleSystemRenderer>();
    }
    public void SetUp(Color color)
    {
        // main explosion material main color and hdr color
        _mainPSR.material.color = color;
        _mainPSR.material.EnableKeyword("_EMISSION");
        _mainPSR.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
        _mainPSR.material.SetColor("_EmissionColor", color * 1.427857f); // hdr intensity should be 1.5f or 1.43...

        // spark material main color and hdr color
        _sparkPSR.material = _mainPSR.material;
        _sparkPSR.trailMaterial = _mainPSR.material;

        // blast material main color and hdr color
        _blastLR.material = _mainPSR.material;



    }
}
