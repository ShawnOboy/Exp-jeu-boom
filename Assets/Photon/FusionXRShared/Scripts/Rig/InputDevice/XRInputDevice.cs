using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/***
 * 
 * XRInputDevice detects the XR input devices and update the transform with actual position/rotation of the device if shouldSynchDevicePosition boolean is set to true
 * 
 ***/

namespace Fusion.XR.Shared.Rig
{
    public class XRInputDevice : MonoBehaviour
    {
        [Header("Detected input device")]
        [SerializeField] bool isDeviceFound = false;
        public InputDevice device;

        [Header("Synchronisation")]
        public bool shouldSynchDevicePosition = true;

        protected virtual InputDeviceCharacteristics DesiredCharacteristics => InputDeviceCharacteristics.TrackedDevice;

        public virtual void DetectDevice()
        {
            if (isDeviceFound) return;
            foreach (var d in DeviceLookup())
            {
                device = d;
                isDeviceFound = true;
                break;
            }
        }
        public virtual List<InputDevice> DeviceLookup()
        {
            InputDeviceCharacteristics desiredCharacteristics = DesiredCharacteristics;
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, devices);
            return devices;
        }

        protected virtual void Update()
        {
            if (shouldSynchDevicePosition)
            {
                DetectDevice();
                if (isDeviceFound && device.TryGetFeatureValue(CommonUsages.devicePosition, out var position))
                {
                    transform.localPosition = position;
                }
                if (isDeviceFound && device.TryGetFeatureValue(CommonUsages.deviceRotation, out var rotation))
                {
                    transform.localRotation = rotation;
                }
            }
        }
    }
}
