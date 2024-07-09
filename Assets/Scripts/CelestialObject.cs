/*
This file is part of a Unity-based space simulation framework.
Copyright (c) 2024 Tejaswi Gorti
Licensed under the MIT License. See the LICENSE file in the project root for more information.
*/

using UnityEngine;

/// <summary>
/// Author: Tejaswi Gorti
/// Description: Celestial Object defines the members and methods shared by all celestial objects in the universe.
///             It inherits from Monobehaviour so all derived classes (Star, Planet, etc) inherit these methods.
/// </summary>
/// 
public class CelestialObject : MonoBehaviour
{
    [SerializeField] public string objectName;
    [SerializeField] public string secondaryName;

    // Equatorial coordinates
    public RightAscension rightAscension;                   // in H M S
    public Declination declination;                         // in D M S
    [SerializeField] public double distance;                // in light years
    [SerializeField] public string RA;
    [SerializeField] public string dec;
    [SerializeField] public float radiusInEarthRadii;

    public virtual string CheckDerived() { throw new System.NotImplementedException(); }

    public CelestialObject(string name, string secondaryName, RightAscension rightAscension, Declination declination, double distance)
    {
        this.objectName = name;
        this.secondaryName = secondaryName;
        this.rightAscension = rightAscension;
        this.RA = rightAscension.toString();
        this.declination = declination;
        this.dec = declination.toString();
        this.distance = distance;
    }

    public RightAscension getRightAscension()
    {
        return this.rightAscension;
    }

    public Declination getDeclination()
    {
        return this.declination;
    }

    public double getDistance()
    {
        return this.distance;
    }

    public void setRightAscension(RightAscension newRightAscensionValue)
    {
        this.rightAscension = newRightAscensionValue;
    }

    public void setDeclination(Declination newDeclinationValue)
    {
        this.declination = newDeclinationValue;
    }

    public void setDistance(double newDistanceValue)
    {
        this.distance = newDistanceValue;
    }

    // This method gives the real-life rectangular coordinates of the object converted from equatorial celestial coordinates.
    public (double X, double Y, double Z) toActualRectangularCoordinates()
    {
        double X = this.distance * Mathf.Sin((Mathf.PI / 2) - (float)this.declination.toRadians()) * Mathf.Cos((float)this.rightAscension.toRadians());
        double Y = this.distance * Mathf.Sin((Mathf.PI / 2) - (float)this.declination.toRadians()) * Mathf.Sin((float)this.rightAscension.toRadians());
        double Z = this.distance * Mathf.Cos((Mathf.PI / 2) - (float)this.declination.toRadians());

        return (X, Y, Z);
    }

    public (double gameX, double gameY, double gameZ) toGameRectangularCoordinates()
    {
        double gameDistance = 100;

        double gameX = -gameDistance * Mathf.Sin((Mathf.PI / 2) - (float)this.declination.toRadians()) * Mathf.Sin((float)this.rightAscension.toRadians());
        double gameY = gameDistance * Mathf.Sin((Mathf.PI / 2) - (float)this.declination.toRadians()) * Mathf.Cos((float)this.rightAscension.toRadians());
        double gameZ = gameDistance * Mathf.Cos((Mathf.PI / 2) - (float)this.declination.toRadians());

        return (gameX, gameY, gameZ);
    }
}
