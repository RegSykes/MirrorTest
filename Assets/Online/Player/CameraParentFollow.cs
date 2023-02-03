using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraParentFollow : MonoBehaviour
{
    [SerializeField] private GameObject Target;
    [SerializeField] private GameObject CameraParent;

    private void LateUpdate() 
    { 
        if (Target && CameraParent)
            CameraParent.transform.position = Target.transform.position; 
    }
}
