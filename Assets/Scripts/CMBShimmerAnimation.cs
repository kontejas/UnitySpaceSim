/*
This file is part of a Unity-based space simulation framework.
Copyright (c) 2024 Tejaswi Gorti
Licensed under the MIT License. See the LICENSE file in the project root for more information.
*/

using UnityEngine;

/// <summary>
/// Author: Tejaswi Gorti
/// Description: CMBShimmerAnimation is a script that adds a shimmer effect to the
///             Cosmic Microwave Background (CMB) radiation SkySphere.
/// </summary>
/// 
public class CMBShimmerAnimation : MonoBehaviour
{
    [Range(0.01f, 2f)]
    [SerializeField] public float CMBShimmerFrequency;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.GetComponent<MeshRenderer>().material.SetFloat("_ShimmerMagnitude", Time.time);
    }
}
