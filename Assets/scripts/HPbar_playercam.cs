using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, null, "Billboard")]
public class HPbar_playercam : MonoBehaviour
{
    Transform cam;
    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.rotation * Vector3.forward, cam.rotation * Vector3.up);
    }
}
