/*
This file is part of a Unity-based space simulation framework.
Copyright (c) 2024 Tejaswi Gorti
Licensed under the MIT License. See the LICENSE file in the project root for more information.
*/

/// <summary>
/// Author: Tejaswi Gorti
/// Description: Constants is a static class that defines all scientific
///             constants in SI units used in calculations
/// </summary>
/// 
public static class Constants
{
    public const int MIN_STAR_TEMP = 2000;                                  // Kelvin
    public const double SIGMA = 5.67e-8;                                    // Stefan-Boltzmann Constant (units: W m^-2 K^-4)
    public const double SOLAR_LUMINOSITY = 3.828e26;                        // Watts
    public const double SOLAR_RADIUS = 695700;                              // km
    public const double KMS_IN_LIGHT_YEAR = 9460730000000;                  // km
    public const double EARTH_RADIUS = 6.378e+3;                            // km
}
