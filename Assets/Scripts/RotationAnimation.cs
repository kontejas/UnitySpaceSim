using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAnimation : MonoBehaviour
{
    [SerializeField] public float degreesPerSecond = 1;
    [SerializeField] public string axis = "y";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (axis == "x")
            transform.Rotate(new Vector3(degreesPerSecond, 0, 0) * Time.deltaTime);
        else if (axis == "y")
            transform.Rotate(new Vector3(0, degreesPerSecond, 0) * Time.deltaTime);
        else if (axis == "z")
            transform.Rotate(new Vector3(0, 0, degreesPerSecond) * Time.deltaTime);
        else
            throw new System.ArgumentException("Rotational Axis for gameObject" + gameObject.name +
                " is invalid! (must be specified as x, y, or z)");
    }
}
