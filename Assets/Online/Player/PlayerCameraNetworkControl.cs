using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraNetworkControl : NetworkBehaviour
{
    [SerializeField] private GameObject CameraParentObject;

    public override void OnStartAuthority()
    {
        CameraParentObject.SetActive(true);
        CameraParentObject.transform.parent = null;
    }
}
