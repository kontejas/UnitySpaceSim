/*
This file is part of a Unity-based space simulation framework.
Copyright (c) 2024 Tejaswi Gorti
Licensed under the MIT License. See the LICENSE file in the project root for more information.
*/
using UnityEngine;

/// <summary>
/// Author: Tejaswi Gorti
/// Description: ScaledCamera is attached to a particular camera layer in the camera stack
///             so that it is continually updates its position and rotation relative to another
///             camera in the stack. It ensures that all cameras are synced according to a
///             ScaleFactor. For example, the Star-Level camera moves at 1/10000th the speed of the
///             Planetary-Level camera.
/// </summary>
/// 
public class ScaledCamera : MonoBehaviour
{
    [SerializeField] public GameObject objectToFollow;
    [SerializeField] private float ScaleFactor;

    private Vector3 pos, fw, up;
    private Vector3 offset;

    void Start()
    {
        EstablishCameraOrientation();
    }

    public void EstablishCameraOrientation()
    {
        offset = transform.position;
        pos = objectToFollow.transform.InverseTransformPoint(transform.position);
        fw = objectToFollow.transform.InverseTransformDirection(transform.forward);
        up = objectToFollow.transform.InverseTransformDirection(transform.up);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var newpos = objectToFollow.transform.TransformPoint(pos) * ScaleFactor + offset;
        var newfw = objectToFollow.transform.TransformDirection(fw);
        var newup = objectToFollow.transform.TransformDirection(up);
        var newrot = Quaternion.LookRotation(newfw, newup);
        transform.position = newpos;
        transform.rotation = newrot;
        //this.transform.position = nearFieldCamera.transform.position * scaleFactor;
        //this.transform.rotation = nearFieldCamera.transform.rotation;
    }
}
