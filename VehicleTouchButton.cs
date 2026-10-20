using UnityEngine;
using UnityEngine.EventSystems;

public class VehicleTouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum ControlType { Accelerate, Reverse, SteerLeft, SteerRight, Brake }
    public ControlType controlType;
    public SimpleCarController targetCar;

    private bool held;

    public void OnPointerDown(PointerEventData eventData)
    {
        held = true;
        Apply();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        held = false;
        Apply();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        held = false;
        Apply();
    }

    private void Apply()
    {
        if (targetCar == null) return;
        switch (controlType)
        {
            case ControlType.Accelerate: targetCar.throttle = held ? 1f : 0f; break;
            case ControlType.Reverse: targetCar.throttle = held ? -1f : 0f; break;
            case ControlType.SteerLeft: targetCar.steering = held ? -1f : 0f; break;
            case ControlType.SteerRight: targetCar.steering = held ? 1f : 0f; break;
            case ControlType.Brake: targetCar.braking = held; break;
        }
    }
}
