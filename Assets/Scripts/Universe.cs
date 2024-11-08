/*
This file is part of a Unity-based space simulation framework.
Copyright (c) 2024 Tejaswi Gorti
Licensed under the MIT License. See the LICENSE file in the project root for more information.
*/

using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Author: Tejaswi Gorti
/// Description: The Universe class is a singleton MonoBehaviour which is attached to an 
///             empty game object that is the parent of all celestial objects.
/// </summary>
/// 
public class Universe : MonoBehaviour
{
    private static Universe _instance;
    public static Universe Instance { get { return _instance; } }
    [SerializeField] private GameObject starPrefab;
    public List<CelestialObject> objects;

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
        objects = new List<CelestialObject>();
        getSolarSystemObjects();
        var path = "Assets\\Scripts\\bright_star_catalogue.csv";
        StreamReader reader = new StreamReader(path);
        var fileContent = reader.ReadToEnd();

        string name, secondaryName, rightAscension, declination, spectralClass, evolutionaryClass;
        double distance_Ly, radius, temperature;

        string[] starEntries = fileContent.Split("\n");
        for (int i = 0; i < starEntries.Length; i++)
        {
            var starEntry = starEntries[i];
            string[] starInfo = starEntry.Split(',');
            if (starInfo.Length == 0)
            {
                continue;
            }
            name = starInfo[0];
            secondaryName = starInfo[1];
            if (name == "" && secondaryName != "")
                name = secondaryName;
            rightAscension = starInfo[2];
            declination = starInfo[3];
            distance_Ly = double.Parse(starInfo[4]);
            radius = double.Parse(starInfo[5]);
            spectralClass = starInfo[6];
            evolutionaryClass = starInfo[7];
            temperature = double.Parse(starInfo[8]);

            GameObject starGameObject = GameObject.Instantiate(starPrefab, this.transform);
            Star s = starGameObject.GetComponentInChildren<Star>();
            int stellarLayer = LayerMask.NameToLayer("Stellar-Level Camera");
            starGameObject.gameObject.layer = stellarLayer;
            foreach (Transform child in s.transform.parent)
            {
                child.gameObject.layer = stellarLayer;
            }
            
            s.create(name, secondaryName, new RightAscension(rightAscension), new Declination(declination), distance_Ly, radius, spectralClass, evolutionaryClass, temperature);
            objects.Add(s);
        }
    }

    private void OnDestroy() { if (this == _instance) { _instance = null; } }


    public CelestialObject findObject(string name)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            if (name == objects[i].objectName)
            {
                return objects[i];
            }
        }
        return null;
    }

    private void getSolarSystemObjects()
    {
        objects.Add(GameObject.Find("/Universe/Solar System/Sun").GetComponent<CelestialObject>());
        objects.Add(GameObject.Find("/Universe/Solar System/Mercury").GetComponent<CelestialObject>());
        objects.Add(GameObject.Find("/Universe/Solar System/Venus").GetComponent<CelestialObject>());

        CelestialObject earth = GameObject.Find("/Universe/Solar System/Earth").GetComponent<CelestialObject>();
        CelestialObject moon = GameObject.Find("/Universe/Solar System/Earth/Moon").GetComponent<CelestialObject>();
        objects.Add(earth);
        objects.Add(moon);
        //((Planet)earth).NaturalSatellites = new List<NaturalSatellite>();
        //((Planet)earth).NaturalSatellites.Add((NaturalSatellite)moon);


        objects.Add(GameObject.Find("/Universe/Solar System/Mars").GetComponent<CelestialObject>());

        CelestialObject jupiter = GameObject.Find("/Universe/Solar System/Jupiter").GetComponent<CelestialObject>();
        //CelestialObject io = GameObject.Find("/Universe/Solar System/Jupiter/Io").GetComponent<CelestialObject>();
        //CelestialObject europa = GameObject.Find("/Universe/Solar System/Jupiter/Europa").GetComponent<CelestialObject>();
        //CelestialObject ganymede = GameObject.Find("/Universe/Solar System/Jupiter/Ganymede").GetComponent<CelestialObject>();
        //CelestialObject callisto = GameObject.Find("/Universe/Solar System/Jupiter/Callisto").GetComponent<CelestialObject>();
        objects.Add(jupiter);
        //objects.Add(io);
        //objects.Add(europa);
        //objects.Add(ganymede);
        //objects.Add(callisto);
        //((Planet)jupiter).NaturalSatellites = new List<NaturalSatellite>();
        //((Planet)jupiter).NaturalSatellites.Add((NaturalSatellite)io);
        //((Planet)jupiter).NaturalSatellites.Add((NaturalSatellite)europa);
        //((Planet)jupiter).NaturalSatellites.Add((NaturalSatellite)ganymede);
        //((Planet)jupiter).NaturalSatellites.Add((NaturalSatellite)callisto);

        objects.Add(GameObject.Find("/Universe/Solar System/Saturn").GetComponent<CelestialObject>());
        objects.Add(GameObject.Find("/Universe/Solar System/Uranus").GetComponent<CelestialObject>());
        objects.Add(GameObject.Find("/Universe/Solar System/Neptune").GetComponent<CelestialObject>());
    }
}
