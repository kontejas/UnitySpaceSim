/*
This file is part of a Unity-based space simulation framework.
Copyright (c) 2024 Tejaswi Gorti
Licensed under the MIT License. See the LICENSE file in the project root for more information.
*/

using UnityEngine;

/// <summary>
/// Author: Tejaswi Gorti
/// Description: DirectionalLightUpdate handles updating the directional light source
///             in the scene so that it always points towards the PlanetaryLevelCamera.
///             In Unity, the directional light source is more realistic than a point source
///             at illuminating planetary and satellite bodies.
/// </summary>
/// 
public class DirectionalLightUpdate : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    private float distanceToTarget;

    protected virtual void Start()
    {
        UpdateLighting();
    }

    public void UpdateLighting()
    {
        Vector3 dirToTarget = (_target.position - transform.parent.position).normalized;

        distanceToTarget = Vector3.Distance(transform.position, _target.position);
        transform.GetComponent<Light>().intensity = 10f*Mathf.Exp(-distanceToTarget / 100f);
        transform.parent.LookAt(transform.parent.position - dirToTarget, Vector3.up);
    }
}
