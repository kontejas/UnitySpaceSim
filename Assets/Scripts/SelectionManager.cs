/*
This file is part of a Unity-based space simulation framework.
Copyright (c) 2024 Tejaswi Gorti
Licensed under the MIT License. See the LICENSE file in the project root for more information.
*/

using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Author: Tejaswi Gorti
/// Description: SelectionManager class defines the user's click and point interaction
///             with the Universe. It defines the methods for populating the UI with
///             CelestialObject information and setting warp destinations.This is a singleton class.
/// </summary>
/// 
public class SelectionManager : MonoBehaviour
{
    private static SelectionManager _instance;
    public static SelectionManager Instance { get { return _instance; } }
    private int stellarLayerMask;
    private int planetaryLayerMask;

    private const string selectableTag = "Selectable";
    [SerializeField] private Universe universe;

    [SerializeField] private Text objectNameText;
    [SerializeField] private Text objectInfoText;
    [SerializeField] private Text objectInfoTitles;
    [SerializeField] private Camera starSystemLevelCamera;
    [SerializeField] private Camera planetaryLevelCamera;
    [SerializeField] public CelestialObject SelectedObject { get; set; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void OnDestroy() { if (this == _instance) { _instance = null; } }

    // Start is called before the first frame update
    void Start()
    {
        stellarLayerMask = 1 << LayerMask.NameToLayer("Stellar-Level Camera");
        planetaryLayerMask = 1 << LayerMask.NameToLayer("Planetary-Level Camera");
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit planetaryHit, stellarHit;
            float radius = 5f;
            float distance = 500f;
            
            Ray ray_planetary = planetaryLevelCamera.ScreenPointToRay(Input.mousePosition);
            Ray ray_stellar = starSystemLevelCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.SphereCast(ray_planetary, radius, out planetaryHit, distance, planetaryLayerMask))
            {
                var celestialObject = planetaryHit.transform.GetComponent<CelestialObject>();
                
                if (celestialObject.CheckDerived() == "Planet")
                {
                    SelectedObject = celestialObject;
                    objectNameText.text = celestialObject.objectName;
                    objectInfoTitles.text = "Object Type:\nSub Type:\nParent Star:\nRadius:\nChemical Composition:\nAverage Surface Temperature:\nNumber of Satellites";
                    var planet = (Planet)celestialObject;
                    string infoText =
                        celestialObject.CheckDerived() + "\n" +
                        planet.planetType + "\n" +
                        planet.parentStar.name + "\n" +
                        planet.radius + "km\n" +
                        planet.chemicalComposition + "\n" +
                        planet.avgSurfaceTemperature + "\n" +
                        planet.numberOfSatellites + "\n";

                    objectInfoText.text = infoText;
                }
            }
            else if (Physics.SphereCast(ray_stellar, radius, out stellarHit, distance, stellarLayerMask))
            {
                var celestialObject = stellarHit.transform.GetComponent<CelestialObject>();

                if (celestialObject.CheckDerived() == "Star")
                {
                    SelectedObject = celestialObject;
                    objectNameText.text = celestialObject.objectName;
                    objectInfoTitles.text =
                        "Object Type:\nClass:\nRadius:\nTemp:\nDistance:\nRA:\nDec:";

                    var star = (Star)celestialObject;
                    string infoText =
                        celestialObject.CheckDerived() + "\n" +
                        star.spectralClass + "-type " + star.colorString + " " + star.evolutionaryClass + "\n" +
                        star.radius / Constants.SOLAR_RADIUS + " R☉\n" +
                        star.effectiveTemperature + "K\n" +
                        star.getDistance() + " ly\n" +
                        star.getRightAscension().toString() + "\n" +
                        star.getDeclination().toString() + "\n";

                    objectInfoText.text = infoText;
                }
            }
            else
            {
                objectNameText.text = "";
                objectInfoTitles.text = "";
                objectInfoText.text = "";
            }
        }
    }
}
