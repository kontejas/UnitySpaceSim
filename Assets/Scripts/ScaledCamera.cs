using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaledCamera : MonoBehaviour
{
    [SerializeField] public GameObject objectToFollow;
    [SerializeField] public float ScaleFactor;

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
