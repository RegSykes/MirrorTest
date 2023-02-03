using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private GameObject cameraParent;

    private void LateUpdate()
    {
        if (cameraParent)
        {
            cameraRotation();
        }
    }

    private void cameraRotation()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            float angle = (Input.GetAxis("Mouse X") * 10f) * 0.5f;
            float num2 = ((-Input.GetAxis("Mouse Y") * 10f) * 0.5f) * 1;
            cameraParent.transform.RotateAround(cameraParent.transform.position, Vector3.up, angle);
            cameraParent.transform.RotateAround(cameraParent.transform.position, transform.right, num2);
        }
    }
}
