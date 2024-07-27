/*
This file is part of a Unity-based space simulation framework.
Copyright (c) 2024 Tejaswi Gorti
Licensed under the MIT License. See the LICENSE file in the project root for more information.
*/

using UnityEngine;

/// <summary>
/// Author: Tejaswi Gorti
/// Description: Planet class derives from CelestialObject and contains members which describe
///             characteristics of planetary bodies which orbit a parentStar. Most importantly
///             the scale of the planet is updated in FixedUpdate, which allows for creating the
///             illusion of scale during interplanetary warp travel.
/// </summary>
/// 
public class Planet : CelestialObject
{
    [SerializeField] public string planetType;
    [SerializeField] public GameObject parentStar;
    [SerializeField] public double radius;
    //[SerializeField] public float radiusInEarthRadii;
    [SerializeField] public string chemicalComposition;
    [SerializeField] public double avgSurfaceTemperature;
    [SerializeField] public int numberOfSatellites;
    [SerializeField] public float absoluteMagnitude;
    [SerializeField] public GameObject highLODObject;
    [SerializeField] public GameObject lowLODObject;
    [SerializeField] private Camera planetaryLevelCamera;
    [SerializeField] private float distanceToPlanet;
    //public List<NaturalSatellite> NaturalSatellites { get; set; }

    public float xFadeStopDistanceLimit;
    public float xFadeStartDistanceLimit;

    private float maxEmissionIntensity;

    public override string CheckDerived() { return "Planet"; }

    private void Start()
    {
        radiusInEarthRadii = (float) (radius / Constants.EARTH_RADIUS);
        SetPlanetScale();
        maxEmissionIntensity = 20f - 2f * absoluteMagnitude;
    }

    public Planet(string name, string secondaryName, RightAscension rightAscension, Declination declination, double distance) : base(name, secondaryName, rightAscension, declination, distance)
    {

    }

    public void SetXFadeLimits(float distanceToTravel)
    {
        xFadeStopDistanceLimit = 0.3f * distanceToTravel;
        xFadeStartDistanceLimit = 0.5f * distanceToTravel;
    }

    public void SetPlanetScale()
    {
        distanceToPlanet = Vector3.Distance(planetaryLevelCamera.transform.position, transform.position);
        float scaleFactor = (float)radiusInEarthRadii * Mathf.Exp(-distanceToPlanet / 10f);
        if (scaleFactor < 0.1f) scaleFactor = 0.1f;
        transform.localScale = scaleFactor * Vector3.one;

        Material distantPlanetShaderGraphMat = lowLODObject.GetComponentInChildren<Renderer>().material;

        if (distanceToPlanet > xFadeStopDistanceLimit && distanceToPlanet < xFadeStartDistanceLimit)
        {
            distantPlanetShaderGraphMat.SetFloat("_XFade", 1f - (distanceToPlanet - xFadeStopDistanceLimit) / (xFadeStartDistanceLimit - xFadeStopDistanceLimit));
        }
        if (distanceToPlanet > 16)
        {
            distantPlanetShaderGraphMat.SetFloat("_EmissionIntensity", maxEmissionIntensity / (1 + Mathf.Exp(-2*(distanceToPlanet - xFadeStartDistanceLimit))));
        }
    }

    private void FixedUpdate()
    {
        SetPlanetScale();
    }
}
