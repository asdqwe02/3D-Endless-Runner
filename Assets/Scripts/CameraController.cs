using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// basic camera script
public class CameraController : MonoBehaviour 
{
    Vector3 _offset;
    Transform _target;
    // Start
    private void Start() {
        _target = PlayerController.instance.transform;
        _offset = transform.position - _target.position;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = _offset+_target.position;
        LevelManager.instance.CheckBoundary(transform);

    }
}
