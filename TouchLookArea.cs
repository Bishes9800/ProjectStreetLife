using UnityEngine;
using UnityEngine.EventSystems;

public class TouchLookArea : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public ThirdPersonPlayer player;
    [Range(0.02f, 1f)] public float lookSensitivity = 0.15f;

    private Vector2 lastPosition;
    private bool activePointer;

    public void OnPointerDown(PointerEventData eventData)
    {
        lastPosition = eventData.position;
        activePointer = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (player == null) return;
        Vector2 delta = eventData.position - lastPosition;
        lastPosition = eventData.position;
        player.AddCameraLook(delta * lookSensitivity);
    }
}
