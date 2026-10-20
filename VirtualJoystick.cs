using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField, Range(0.2f, 1f)] private float handleRange = 0.65f;

    public Vector2 Value { get; private set; }

    private void Awake()
    {
        if (background == null) background = transform as RectTransform;
    }

    public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

    public void OnDrag(PointerEventData eventData)
    {
        if (background == null || handle == null) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            Vector2 half = background.rect.size * 0.5f;
            Vector2 normalized = new Vector2(
                half.x > 0 ? localPoint.x / half.x : 0,
                half.y > 0 ? localPoint.y / half.y : 0);
            Value = Vector2.ClampMagnitude(normalized, 1f);
            handle.anchoredPosition = Value * (Mathf.Min(half.x, half.y) * handleRange);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Value = Vector2.zero;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
    }
}
