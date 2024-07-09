/*
This file is part of a Unity-based space simulation framework.
Copyright (c) 2024 Tejaswi Gorti
Licensed under the MIT License. See the LICENSE file in the project root for more information.
*/

using System;
using UnityEngine;

// enum for defining the Morgan-Keenan (MK) spectral classification
public enum SpectralClass
{
    O, B, A, F, G, K, M, L, T, S, W, C
}

// enum for defining the MK luminosity class
public enum LuminosityClass
{
    Hypergiant,
    LuminousSupergiant,
    BrightGiant,
    NormalGiant,
    SubGiant,
    MainSequence,
    SubDwarf,
    WhiteDwarf,
    CepheidVariable
}

/// <summary>
/// Author: Tejaswi Gorti
/// Description: Star class derives from CelestialObject and contains members which describe
///             characteristics of stellar bodies. The create method when paired with a Star
///             prefab instantiation helps determine certain ShaderGraph properties such as
///             color and luminosity (emission strength).
/// </summary>
/// 
public class Star : CelestialObject
{
    [SerializeField] public SpectralClass spectralClass;
    [SerializeField] public LuminosityClass evolutionaryClass;
    [SerializeField] public double radius;                                  // kilometers (km)
    [SerializeField] public double effectiveTemperature;                    // Kelvin
    [SerializeField] public Color starColor;
    [SerializeField] public string colorString;
    [SerializeField] public double luminosity;                              // Watts
    [SerializeField] public double apparentBrightness;                      // Watts / m^2

    public static Color BLUE_WHITE = new Color(0.1f, 0.3f, 0.9f);
    public static Color BLUE = new Color(0.1f, 0.4f, 1f);
    public static Color YELLOW_WHITE = new Color(1.00f, 1.00f, 0.70f);
    public static Color ORANGE = new Color(1.00f, 0.70f, 0.00f);
    public static Color MAROON = new Color(0.50f, 0.00f, 0.00f);
    public static Color BROWN = new Color(0.59f, 0.29f, 0.00f);
    public static Color RED = new Color(1f, 0.4f, 0.4f);

    public override string CheckDerived() { return "Star"; }

    public Star(string name, string secondaryName, RightAscension rightAscension, Declination declination, double distance) : base(name, secondaryName, rightAscension, declination, distance)
    {
    }

    public void create(string name, string secondaryName, RightAscension rightAscension, Declination declination, double distance, double r,
        string spectralClass, string luminosityClass, double temperature)
    {
        this.objectName = name;
        this.secondaryName = secondaryName;
        this.rightAscension = rightAscension;
        this.declination = declination;
        this.distance = distance;
        if (spectralClass == null)
        {
            this.spectralClass = SpectralClass.A;
        }
        if (luminosityClass == null)
        {
            this.evolutionaryClass = LuminosityClass.MainSequence;
        }
        if (rightAscension == null)
        {

        }

        switch (spectralClass)
        {
            case "O":
                this.spectralClass = SpectralClass.O;
                this.starColor = BLUE;
                this.colorString = "Blue";
                break;
            case "B":
                this.spectralClass = SpectralClass.B;
                this.starColor = BLUE_WHITE;
                this.colorString = "Blue-White";
                break;
            case "A":
                this.spectralClass = SpectralClass.A;
                this.starColor = Color.white;
                this.colorString = "White";
                break;
            case "F":
                this.spectralClass = SpectralClass.F;
                this.starColor = YELLOW_WHITE;
                this.colorString = "Yellow-White";
                break;
            case "G":
                this.spectralClass = SpectralClass.G;
                this.starColor = Color.yellow;
                this.colorString = "Yellow";
                break;
            case "K":
                this.spectralClass = SpectralClass.K;
                this.starColor = ORANGE;
                this.colorString = "Orange";
                break;
            case "M":
                this.spectralClass = SpectralClass.M;
                this.starColor = RED;
                this.colorString = "Red";
                break;
            case "L":
                this.spectralClass = SpectralClass.L;
                this.starColor = MAROON;
                this.colorString = "Red";
                break;
            case "T":
                this.spectralClass = SpectralClass.T;
                this.starColor = BROWN;
                this.colorString = "Brown";
                break;
            case "S":
                this.spectralClass = SpectralClass.S;
                this.starColor = RED;
                this.colorString = "Red";
                break;
            case "W":
                this.spectralClass = SpectralClass.W;
                this.starColor = Color.white;
                this.colorString = "Wolf-Rayet Star";
                break;
            case "C":
                this.spectralClass = SpectralClass.C;
                this.starColor = RED;
                this.colorString = "Carbon Star";
                break;
            default:
                Debug.Log("Invalid spectral class: " + spectralClass + " for object " + name + ".");
                throw new System.ArgumentException("Invalid spectral class: " + spectralClass + " for object " + name + ".");
        }

        switch (luminosityClass)
        {
            case "hypergiant":
                this.evolutionaryClass = LuminosityClass.Hypergiant;
                break;
            case "supergiant":
                this.evolutionaryClass = LuminosityClass.LuminousSupergiant;
                break;
            case "brightgiant":
                this.evolutionaryClass = LuminosityClass.BrightGiant;
                break;
            case "normalgiant":
                this.evolutionaryClass = LuminosityClass.NormalGiant;
                break;
            case "subgiant":
                this.evolutionaryClass = LuminosityClass.SubGiant;
                break;
            case "mainsequence":
                this.evolutionaryClass = LuminosityClass.MainSequence;
                break;
            case "subdwarf":
                this.evolutionaryClass = LuminosityClass.SubDwarf;
                break;
            case "whitedwarf":
                this.evolutionaryClass = LuminosityClass.WhiteDwarf;
                break;
            case "cepheid_variable":
                this.evolutionaryClass = LuminosityClass.CepheidVariable;
                break;
            default:
                this.evolutionaryClass = LuminosityClass.MainSequence;
                // Debug.Log("Invalid luminosity class specified: " + luminosityClass + " for object " + name + ". Making this a Main Sequence star.");
                break;
                //throw new System.ArgumentException("Invalid luminosity class: " + luminosityClass + " for object " + name + ".");
        }

        this.radius = r * Constants.SOLAR_RADIUS;
        double gameObjRadius, radiusReduxFactor = 0.5 * Mathf.Log10((float)(this.getDistance() * Constants.KMS_IN_LIGHT_YEAR));
        if (radiusReduxFactor > 0)
        {
            gameObjRadius = r / radiusReduxFactor;
        }
        else
        {
            gameObjRadius = r;
        }
        if (r > 30)
        {
            gameObjRadius = 0.324;
        }
        else if (r > 10)
        {
            gameObjRadius = 0.27;
        }
        else if (r > 2)
        {
            gameObjRadius = 0.162;
        }
        else
        {
            gameObjRadius = 0.162;
        }

        double gameApparentBrightness;
        if (temperature > Constants.MIN_STAR_TEMP)
        {
            this.effectiveTemperature = temperature;
            // Luminosity: L = 4 * pi * R^2 * sigma * T^4 | source: https://www.atnf.csiro.au/outreach/education/senior/astrophysics/photometry_luminosity.html
            this.luminosity = 4.0 * Mathf.PI * Mathf.Pow((float)this.radius * 1000, 2) * Constants.SIGMA * Mathf.Pow((float)this.effectiveTemperature, 4);
            this.apparentBrightness = (this.luminosity) / (4.0 * Mathf.PI * Math.Pow((this.distance * Constants.KMS_IN_LIGHT_YEAR * 1000), 2.0));
            gameApparentBrightness = this.apparentBrightness * Math.Pow(10, 9);
        }
        else
        {
            Debug.Log("Surface temperature of " + temperature + " is too low for star " + name + ".");
            throw new System.ArgumentException("Surface temperature of " + temperature + " is too low for star " + name + ".");
        }

        // Debug.Log(this.objectName + " - Celestial coordinates: (" + this.getDistance() + "ly, " + this.getRightAscension().toString() + ", " + this.getDeclination().toString() + ")");

        var (X, Y, Z) = this.toGameRectangularCoordinates();
        Vector3 celestialRectangularCoordinates = new Vector3((float)X, (float)Z, (float)Y);

        this.gameObject.transform.position = celestialRectangularCoordinates;
        this.gameObject.layer = 0;

        gameObject.transform.name = this.objectName;
        // Debug.Log(gameObject.transform.name + " - Game object coordinates: " + gameObject.transform.position);
        gameObject.transform.localScale = new Vector3((float)gameObjRadius, (float)gameObjRadius, (float)gameObjRadius);

        var starEmission = gameObject.GetComponentInChildren<Renderer>().material;
        starEmission.SetColor("BaseColor", this.starColor);
        starEmission.SetFloat("Intensity", (float)(gameApparentBrightness));
    }
}
