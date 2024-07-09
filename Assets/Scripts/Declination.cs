/*
This file is part of a Unity-based space simulation framework.
Copyright (c) 2024 Tejaswi Gorti
Licensed under the MIT License. See the LICENSE file in the project root for more information.
*/

using UnityEngine;

/// <summary>
/// Author: Tejaswi Gorti
/// Description: Declinaton is a "struct" class that holds a declination value, which is comprised of
///             degrees (int), minutes (int), and seconds (double) values. Additional methods
///             are included to convert to elevation angle in degrees/radians and to represent as string.
/// </summary>
public class Declination
{
    public int degrees;
    public int minutes;
    public double seconds;

    public Declination (string declination)
    {
        bool isNegative;
        int startIndex;
        if (declination[0] == '-')
        {
            isNegative = true;
            startIndex = 1;
        }
        else if (declination[0] == '+')
        {
            isNegative = false;
            startIndex = 1;
        }
        else
        {
            isNegative = false;
            startIndex = 0;
        }

        declination = declination.Substring(startIndex);

        char[] delimChars = { '°', '\u2032', '\u2033' };
        string[] words = declination.Split(delimChars);

        if (isNegative)
        {
            this.degrees = 0 - int.Parse(words[0]);
            this.minutes = 0 - int.Parse(words[1]);
            this.seconds = 0 - double.Parse(words[2]);
        }
        else
        {
            this.degrees = int.Parse(words[0]);
            this.minutes = int.Parse(words[1]);
            this.seconds = double.Parse(words[2]);
        }
    }

    public Declination (float dec)
    {
        this.degrees = (int)dec;
        this.minutes = (int)((dec - this.degrees) * 60);
        this.seconds = (double)(((dec - this.degrees) * 60 - this.minutes) * 60);
    }

    public Declination (int degrees, int minutes, double seconds)
    {
        this.degrees = degrees;
        this.minutes = minutes;
        this.seconds = seconds;
    }

    public double toDegrees()
    {
        double result = this.degrees + ((float)this.minutes / 60) + (this.seconds / 3600);
        return result;
    }

    public double toRadians()
    {
        double result = this.toDegrees() / 360 * 2 * Mathf.PI;
        return result;
    }

    public string toString()
    {
        return this.degrees + "° " + this.minutes + "' " + this.seconds + "\"";
    }
}
