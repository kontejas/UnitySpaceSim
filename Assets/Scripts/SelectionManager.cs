using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class SelectionManager : MonoBehaviour
{
    private static SelectionManager _instance;
    public static SelectionManager Instance { get { return _instance; } }
    private int stellarLayerMask;
    private int planetaryLayerMask;
    private int satelliteLayerMask;

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
        satelliteLayerMask = 1 << LayerMask.NameToLayer("Satellite");
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit planetaryHit, stellarHit, satelliteHit;
            float radius = 5f;
            float distance = 500f;
            
            Ray ray_planetary = planetaryLevelCamera.ScreenPointToRay(Input.mousePosition);
            Ray ray_stellar = starSystemLevelCamera.ScreenPointToRay(Input.mousePosition);
            Ray ray_satellite = planetaryLevelCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.SphereCast(ray_planetary, radius, out planetaryHit, distance, planetaryLayerMask))
            {
                Debug.Log(planetaryHit.transform.name);
                var celestialObject = planetaryHit.transform.GetComponent<CelestialObject>();
                Debug.DrawRay(planetaryLevelCamera.transform.position, celestialObject.transform.position, Color.cyan, 1f);
                
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
                //Debug.Log(stellarHit.transform.name);
                var celestialObject = stellarHit.transform.GetComponent<CelestialObject>();
                //Debug.DrawRay(starSystemLevelCamera.transform.position, celestialObject.transform.position, Color.green, 1f);

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
