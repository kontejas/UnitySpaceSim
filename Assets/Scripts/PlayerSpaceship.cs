/*
This file is part of a Unity-based space simulation framework.
Copyright (c) 2024 Tejaswi Gorti
Licensed under the MIT License. See the LICENSE file in the project root for more information.
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using Cinemachine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using System.Collections;
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


/// <summary>
/// Author: Tejaswi Gorti
/// Description: PlayerSpaceship is a singleton object which defines all methods for
///             the spaceship GameObject and its ability to traverse the universe.
///             Coroutines for warp travel and short range movement are defined and
///             tied to input controls/keybindings.
/// </summary>
/// 
[RequireComponent(typeof(Rigidbody))]

public class PlayerSpaceship : MonoBehaviour
{
    private static PlayerSpaceship _instance;
    public static PlayerSpaceship Instance { get { return _instance; } }

    [Header("=== Ship Movement Settings ===")]
    [SerializeField]
    private float yawTorque = 10f;
    [SerializeField]
    private float pitchTorque = 10f;
    [SerializeField]
    private float rollTorque = 10f;
    [SerializeField]
    private float thrust = 100f;
    [SerializeField]
    private float upThrust = 50f;
    [SerializeField]
    private float strafeThrust = 50f;

    [Header("=== Ship Movement Settings ===")]
    [SerializeField]
    private float maxBoostAmount = 20f;
    [SerializeField]
    private float boostDeprecationRate = 0.1f;
    [SerializeField]
    private float boostRechargeRate = 0.15f;
    [SerializeField]
    private float boostMultiplier = 10f;
    private bool isBoosting = false;
    private float currentBoostAmount;

    public CelestialObject WarpOrigin { get; set; }
    public CelestialObject WarpDestination { get; set; }
    [SerializeField]
    private DirectionalLightUpdate sunLight;
    [SerializeField]
    private GameObject mainCamera;
    [SerializeField]
    private GameObject solarSystemLevelCamera;
    [SerializeField]
    private GameObject planetaryLevelCamera;
    [SerializeField]
    private GameObject nearFieldCamera;
    [SerializeField]
    private CinemachineVirtualCamera virtualNearFieldCamera;
    [SerializeField]
    private GameObject globalVolume;
    [SerializeField]
    private GameObject CMBSphere;

    [SerializeField, Range(0.001f, 0.999f)]
    private float thrustGlideReduction = 0.999f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float upDownGlideReduction = 0.111f;
    [SerializeField, Range(0.001f, 0.999f)]
    private float leftRightGlideReduction = 0.111f;
    private float glide, verticalGlide, horizontalGlide = 0f;


    [Header("=== Thrust Vectoring Nozzles ===")]
    [SerializeField]
    private List<ParticleSystem> rollRightThrustVectorNozzles;
    [SerializeField]
    private List<ParticleSystem> rollLeftThrustVectorNozzles;
    [SerializeField]
    private List<ParticleSystem> strafeLeftThrustVectorNozzles;
    [SerializeField]
    private List<ParticleSystem> strafeRightThrustVectorNozzles;
    [SerializeField]
    private List<ParticleSystem> yawRightThrustVectorNozzles;
    [SerializeField]
    private List<ParticleSystem> yawLeftThrustVectorNozzles;
    [SerializeField]
    private List<ParticleSystem> pitchUpThrustVectorNozzles;
    [SerializeField]
    private List<ParticleSystem> pitchDownThrustVectorNozzles;
    [SerializeField]
    private List<ParticleSystem> upThrustVectorNozzles;
    [SerializeField]
    private List<ParticleSystem> downThrustVectorNozzles;
    [SerializeField]
    private List<ParticleSystem> mainThrustersNozzles;

    private Rigidbody rb;

    [SerializeField] private List<GameObject> shipMainThrusters;
    [SerializeField] private ParticleSystem movementParticleSystem;
    [SerializeField] private Material warpParticleSystemTrails;
    [SerializeField] private VisualEffect warpVFX;

    // Input Values
    private float thrust1D;
    private float upDown1D;
    private float strafe1D;
    private float roll1D;
    private Vector2 pitchYaw;

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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentBoostAmount = maxBoostAmount;
        warpVFX.Stop();
        virtualNearFieldCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0.0f;
        WarpOrigin = Universe.Instance.findObject("Earth");
    }

    private void FixedUpdate()
    {
        HandleBoosting();
        HandleMovement();
        HandleInterPlanetaryWarp();
        HandleStabilization();
    }

    private void HandleInterPlanetaryWarp()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (SelectionManager.Instance.SelectedObject.CheckDerived() == "Planet" || SelectionManager.Instance.SelectedObject.CheckDerived() == "NaturalSatellite")
            {
                WarpDestination = SelectionManager.Instance.SelectedObject;

                float distanceToDestination = (WarpDestination.transform.position - WarpOrigin.transform.position).magnitude;

                StartCoroutine(InterPlanetaryWarpRoutine(distanceToDestination / 2.0f));
            }
        }
    }

    private void HandleStabilization()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Stabilize(1000f);
        }
    }

    void Stabilize(float duration)
    {
        Vector3 currentAngularVelocity = rb.angularVelocity;
        virtualNearFieldCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0.0f;
        Vector3 dampingAmount = virtualNearFieldCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().Damping;
        virtualNearFieldCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().Damping = Vector3.zero;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            rb.angularVelocity = Vector3.Lerp(currentAngularVelocity, Vector3.zero, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
        }
        virtualNearFieldCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().Damping = dampingAmount;

    }

    void HandleBoosting()
    {
        if (isBoosting && currentBoostAmount > 0f)
        {
            virtualNearFieldCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0.1f;
            currentBoostAmount -= boostDeprecationRate;
            if (currentBoostAmount <= 0f)
            {
                virtualNearFieldCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0.0f;
                isBoosting = false;
            }
        }
        else
        {
            if (currentBoostAmount < maxBoostAmount)
            {
                currentBoostAmount += boostRechargeRate;
            }
        }
    }

    void HandleMovement()
    {
        // Roll
        rb.AddRelativeTorque(Vector3.back * roll1D * rollTorque * Time.deltaTime);
        // Pitch
        rb.AddRelativeTorque(Vector3.right * Mathf.Clamp(-pitchYaw.y, -1f, 1f) * pitchTorque * Time.deltaTime);
        // Yaw
        rb.AddRelativeTorque(Vector3.up * Mathf.Clamp(pitchYaw.x, -1f, 1f) * yawTorque * Time.deltaTime);

        // Thrust
        if (thrust1D > 0.1f || thrust1D < -0.1f)
        {
            float currentThrust;

            if (isBoosting)
            {
                currentThrust = thrust * boostMultiplier;
                setThrusterIntensity(4f);
            }
            else
            {
                currentThrust = thrust;
                setThrusterIntensity(2f);
            }

            rb.AddRelativeForce(Vector3.forward * thrust1D * currentThrust * Time.deltaTime);
            glide = thrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.forward * glide * Time.deltaTime);
            glide *= thrustGlideReduction;

            setThrusterIntensity(0f);
        }

        movementParticleSystem.transform.LookAt(rb.velocity);
        var psMain = movementParticleSystem.main;
        psMain.simulationSpeed = rb.velocity.magnitude;
        HandleThrustVectoring();

        // Up / Down
        if (upDown1D > 0.1f || upDown1D < -0.1f)
        {
            rb.AddRelativeForce(Vector3.up * upDown1D * upThrust * Time.fixedDeltaTime);
            verticalGlide = upDown1D * upThrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.up * verticalGlide * Time.fixedDeltaTime);
            verticalGlide *= upDownGlideReduction;
        }

        // Strafing
        if (strafe1D > 0.1f || strafe1D < -0.1f)
        {
            rb.AddRelativeForce(Vector3.right * strafe1D * upThrust * Time.fixedDeltaTime);
            horizontalGlide = strafe1D * strafeThrust;
        }
        else
        {
            rb.AddRelativeForce(Vector3.right * horizontalGlide * Time.fixedDeltaTime);
            horizontalGlide *= leftRightGlideReduction;
        }
    }

    private void setThrusterIntensity(float intensity)
    {
        foreach (GameObject thruster in shipMainThrusters)
        {
            thruster.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.white * intensity);
        }
        ParticleSystem.EmissionModule em;
        if (intensity > 0.1f)
        {
            foreach (ParticleSystem nozzle in mainThrustersNozzles)
            {
                em = nozzle.emission;
                em.enabled = true;
            }
        }
        else
        {
            foreach (ParticleSystem nozzle in mainThrustersNozzles)
            {
                em = nozzle.emission;
                em.enabled = false;
            }
        }
    }

    private void HandleThrustVectoring()
    {
        // if roll is to the right
        ParticleSystem.EmissionModule em;
        if (roll1D > 0.1f)
        {
            foreach (ParticleSystem nozzle in rollRightThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = true;
            }
        }
        else
        {
            foreach (ParticleSystem nozzle in rollRightThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = false;
            }
        }
        // if roll is to the left
        if (roll1D < -0.1f)
        {
            foreach (ParticleSystem nozzle in rollLeftThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = true;
            }
        }
        else
        {
            foreach (ParticleSystem nozzle in rollLeftThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = false;
            }
        }
        // if strafing right
        if (strafe1D > 0.1f)
        {
            foreach (ParticleSystem nozzle in strafeRightThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = true;
            }
        }
        else
        {
            foreach (ParticleSystem nozzle in strafeRightThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = false;
            }
        }
        // if strafing left
        if (strafe1D < -0.1f)
        {
            foreach (ParticleSystem nozzle in strafeLeftThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = true;
            }
        }
        else
        {
            foreach (ParticleSystem nozzle in strafeLeftThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = false;
            }
        }

        // if yaw right
        if (-pitchYaw.y > 0.1f)
        {
            foreach (ParticleSystem nozzle in yawRightThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = true;
            }
        }
        else
        {
            foreach (ParticleSystem nozzle in yawRightThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = false;
            }
        }
        // if yaw left
        if (-pitchYaw.y < -0.1f)
        {
            foreach (ParticleSystem nozzle in yawLeftThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = true;
            }
        }
        else
        {
            foreach (ParticleSystem nozzle in yawLeftThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = false;
            }
        }

        // if pitch up
        if (-pitchYaw.x > 0.1f)
        {
            foreach (ParticleSystem nozzle in pitchUpThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = true;
            }
        }
        else
        {
            foreach (ParticleSystem nozzle in pitchUpThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = false;
            }
        }
        // if pitch down
        if (-pitchYaw.x < -0.1f)
        {
            foreach (ParticleSystem nozzle in pitchDownThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = true;
            }
        }
        else
        {
            foreach (ParticleSystem nozzle in pitchDownThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = false;
            }
        }

        // if up
        if (upDown1D > 0.1f)
        {
            foreach (ParticleSystem nozzle in upThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = true;
            }
        }
        else
        {
            foreach (ParticleSystem nozzle in upThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = false;
            }
        }
        // if down
        if (upDown1D < -0.1f)
        {
            foreach (ParticleSystem nozzle in downThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = true;
            }
        }
        else
        {
            foreach (ParticleSystem nozzle in downThrustVectorNozzles)
            {
                em = nozzle.emission;
                em.enabled = false;
            }
        }
    }

    IEnumerator InterPlanetaryWarpRoutine(float duration)
    {
        Vector3 startPos = planetaryLevelCamera.transform.position;
        Vector3 warpDirection = WarpDestination.transform.position - startPos;
        warpDirection.Normalize();
        Vector3 endPos = WarpDestination.transform.position - (1.3f * (float)WarpDestination.GetComponent<CelestialObject>().radiusInEarthRadii) * warpDirection;
        WarpDestination.TryGetComponent<Planet>(out Planet destinationPlanet);
        if (destinationPlanet != null)
            WarpDestination.GetComponent<Planet>().SetXFadeLimits((endPos - startPos).magnitude);
        else
            yield break;

        float stabilizeDuration, alignmentDuration, warpDuration;
        if (duration > 10f)
        {
            stabilizeDuration = 1f;
            alignmentDuration = 5f;
            warpDuration = duration - 6f;
        }
        else
        {
            stabilizeDuration = 0.1f * duration;
            alignmentDuration = 0.2f * duration;
            warpDuration = 0.7f * duration;
        }

        Stabilize(stabilizeDuration);
        yield return AlignToDestination(alignmentDuration, startPos, endPos);
        WarpOrigin = WarpDestination;
        WarpDestination = null;
        yield return InterPlanetaryWarp(warpDuration, startPos, endPos);
    }

    private Coroutine alignToDestinationRoutine;
    private Coroutine warpToDestRoutine;


    private IEnumerator AlignToDestination(float animationDuration, Vector3 startPos, Vector3 endPos)
    {
        this.EnsureCoroutineStopped(ref alignToDestinationRoutine);

        Quaternion startRotation = planetaryLevelCamera.transform.rotation;
        Vector3 startForwardVector = planetaryLevelCamera.transform.forward;
        Vector3 endForwardVector = endPos - startPos;   // Vector looking into the center of the destination planet
        endForwardVector.Normalize();
        Quaternion endRotation = Quaternion.LookRotation(endForwardVector);

        Vector3 startPosition = transform.position;
        Vector3 endPosition = planetaryLevelCamera.transform.position;
        //virtualNearFieldCamera.TryGetComponent<CinemachineFramingTransposer>(out CinemachineFramingTransposer follow);

        yield return StartCoroutine(AnimationRoutine(
            animationDuration,
            delegate (float progress)
            {
                float easedProgress = Easing.easeInOutSine(0, 1, progress);
                Quaternion rot = Quaternion.Lerp(startRotation, endRotation, easedProgress);
                Vector3 pos = Vector3.Lerp(startPosition, endPosition, easedProgress);
                nearFieldCamera.transform.rotation = rot;
                planetaryLevelCamera.transform.rotation = rot;
                transform.rotation = rot;
                transform.position = pos;
                sunLight.UpdateLighting();
            },
            null
        ));
        nearFieldCamera.transform.rotation = endRotation;
        planetaryLevelCamera.transform.rotation = endRotation;
    }

    private IEnumerator InterPlanetaryWarp(float animationDuration, Vector3 startPos, Vector3 endPos)
    {
        float fovWidenRate = 2f;
        float warpVFXAmount = warpVFX.GetFloat("WarpAmount");
        float warpVFXCameraWidenAmount;
        ColorAdjustments colorAdjustments;
        VolumeProfile volumeProfile = globalVolume.GetComponent<Volume>()?.profile;
        if (!volumeProfile) throw new NullReferenceException(nameof(VolumeProfile));
        if (!volumeProfile.TryGet(out Vignette vignette)) throw new System.NullReferenceException(nameof(vignette));

        volumeProfile.TryGet<ColorAdjustments>(out colorAdjustments);
        if (colorAdjustments == null)
            Debug.LogError("No ColorAdjustments found on profile");

        float currentVignetteIntensity = vignette.intensity.GetValue<float>();


        this.EnsureCoroutineStopped(ref warpToDestRoutine);
        warpVFX.Play();

        yield return StartCoroutine(AnimationRoutine(
            animationDuration,
            delegate (float progress)
            {
                float easedProgress = Easing.easeInOutSine(0, 1, progress);
                warpVFXAmount = easedProgress * (1 - easedProgress);
                warpVFXCameraWidenAmount = 30f * warpVFXAmount + 1f;
                warpVFX.SetFloat("WarpAmount", warpVFXCameraWidenAmount);

                nearFieldCamera.GetComponent<Camera>().fieldOfView = warpVFXCameraWidenAmount * fovWidenRate + 40f;
                planetaryLevelCamera.GetComponent<Camera>().fieldOfView = warpVFXCameraWidenAmount * fovWidenRate + 40f;
                solarSystemLevelCamera.GetComponent<Camera>().fieldOfView = warpVFXCameraWidenAmount * fovWidenRate + 40f;
                mainCamera.GetComponent<Camera>().fieldOfView = warpVFXCameraWidenAmount * fovWidenRate + 40f;
                virtualNearFieldCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0.2f * warpVFXCameraWidenAmount;
                CMBSphere.GetComponent<MeshRenderer>().material.SetFloat("_Intensity", 0.08f * warpVFXAmount);
                vignette.intensity.Override(0.1f + 2f * warpVFXAmount);
                colorAdjustments.hueShift.Override(-56f * warpVFXAmount);

                setThrusterIntensity(warpVFXAmount * 5f);

                Vector3 pos = Vector3.Lerp(startPos, endPos, easedProgress);
                planetaryLevelCamera.transform.position = pos;
                nearFieldCamera.transform.position = pos;
                transform.position = pos;
                planetaryLevelCamera.GetComponent<ScaledCamera>().EstablishCameraOrientation();
                sunLight.UpdateLighting();
            },
            null
        ));
        warpVFX.SetFloat("WarpAmount", 0f);
        virtualNearFieldCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
        planetaryLevelCamera.transform.position = endPos;
        ////nearFieldCamera.transform.position = endPos;
        //transform.position = endPos;
        warpVFX.Stop();

    }

    private static IEnumerator AnimationRoutine(float duration, Action<float> changeFunction, Action onComplete)
    {
        float elapsedTime = 0;
        float progress = 0;
        while (progress <= 1)
        {
            changeFunction(progress);
            elapsedTime += Time.unscaledDeltaTime;
            progress = elapsedTime / duration;
            yield return null;
        }
        changeFunction(1);
        onComplete?.Invoke();
    }

    #region Input Methods
    public void OnThrust(InputAction.CallbackContext context)
    {
        thrust1D = context.ReadValue<float>();
    }

    public void OnStrafe(InputAction.CallbackContext context)
    {
        strafe1D = context.ReadValue<float>();
    }

    public void OnUpDown(InputAction.CallbackContext context)
    {
        upDown1D = context.ReadValue<float>();
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        roll1D = context.ReadValue<float>();
    }

    public void OnPitchYaw(InputAction.CallbackContext context)
    {
        pitchYaw = context.ReadValue<Vector2>();
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        isBoosting = context.performed;
    }
    #endregion
}
