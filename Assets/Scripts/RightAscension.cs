/*
This file is part of a Unity-based space simulation framework.
Copyright (c) 2024 Tejaswi Gorti
Licensed under the MIT License. See the LICENSE file in the project root for more information.
*/

using UnityEngine;

/// <summary>
/// Author: Tejaswi Gorti
/// Description: Right Ascension is a "struct" class that holds a right ascension value, which is comprised of
///             hours (int), minutes (int), and seconds (double) values. Additional methods
///             are included to convert to azimuth angle in degrees/radians and to represent as string.
/// </summary>
public class RightAscension
{
    public int hours;
    public int minutes;
    public double seconds;

    public RightAscension(string rightAscension)
    {
        char[] delimChars = { 'h', 'm', 's' };

        bool formatIsValid = true;
        foreach (char c in delimChars)
        {
            if (!rightAscension.Contains(c))
            {
                formatIsValid = false;
            }
        }

        if (!formatIsValid)
            throw new System.ArgumentException("RightAscension constructor input argument must be of the format 'xxh xxm xx.xxs'");

        string[] words = rightAscension.Split(delimChars);
        this.hours = int.Parse(words[0]);
        this.minutes = int.Parse(words[1]);
        this.seconds = double.Parse(words[2]);
    }

    public RightAscension(float RA)
    {
        this.hours = (int)RA;
        this.minutes = (int)((RA - this.hours)*60);
        this.seconds = (double)(((RA - (int)(RA))*60 - this.minutes)*60);
    }

    public RightAscension(int hours, int minutes, double seconds)
    {
        this.hours = hours;
        this.minutes = minutes;
        this.seconds = seconds;
    }

    public double toDegrees()
    {
        double result = 360 * (((double)this.hours / 24) + ((double)this.minutes / 1440) + (this.seconds / 86400));
        return result;
    }

    public double toRadians()
    {
        double result = 2 * Mathf.PI * (((double)this.hours / 24) + ((double)this.minutes / 1440) + (this.seconds / 86400));
        return result;
    }

    public string toString()
    {
        return this.hours + "h " + this.minutes + "m " + this.seconds + "s";
    }
}
