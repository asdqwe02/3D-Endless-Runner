using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleStopScript : MonoBehaviour
{  
    void Start()
    {
        var main = GetComponent<ParticleSystem>().main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    void OnParticleSystemStopped()
    {
        transform.parent = ObjectPooler.instance.transform;
        transform.localPosition = Vector3.zero;
        transform.gameObject.SetActive(false);
    }
}
