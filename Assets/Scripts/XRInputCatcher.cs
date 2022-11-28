using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

[System.Serializable]
public class PrimaryButtonEvent : UnityEvent<bool> { }
public class SecondaryButtonEvent : UnityEvent<bool> { }


public class XRInputCatcher : MonoBehaviour
{
    public PrimaryButtonEvent primaryButtonPress;
    public SecondaryButtonEvent secondaryButtonPress;
    
    public Vector2 rightStickValue;
    public bool active;
    public bool joystickUp = false, joystickDown = false;


    private void Start() 
    {
        InvokeRepeating("CheckVideoControl", 2.0f, 0.3f);
        active = true;
    }
    void OnEnable()
    {
        List<InputDevice> allDevices = new List<InputDevice>();
        InputDevices.GetDevices(allDevices);
        foreach(InputDevice device in allDevices)
            InputDevices_deviceConnected(device);

        InputDevices.deviceConnected += InputDevices_deviceConnected;
        InputDevices.deviceDisconnected += InputDevices_deviceDisconnected;
    }

    private void OnDisable()
    {
        InputDevices.deviceConnected -= InputDevices_deviceConnected;
        InputDevices.deviceDisconnected -= InputDevices_deviceDisconnected;
    }

    private void InputDevices_deviceConnected(InputDevice device)
    {
        bool discardedValue;
    }

    private void InputDevices_deviceDisconnected(InputDevice device)
    {
    }

    void Update()
    {
        if(active)
        {
            var rightStick = GetStickValue(XRNode.RightHand,"Primary2DAxis");
            
            if (rightStick == null)
                return;
            else
            {
                rightStickValue = rightStick.Value;
            }
        }
    }

    private Vector2? GetStickValue(XRNode node,string axisName)
    {
        Vector2 value;
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        if (!InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(new InputFeatureUsage<Vector2>(axisName), out value))
            return null;
        return value;
    }


}