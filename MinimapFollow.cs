using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform target;
    public float height = 60f;
    public bool rotateWithTarget = false;

    private void LateUpdate()
    {
        if (target == null) return;
        transform.position = new Vector3(target.position.x, target.position.y + height, target.position.z);
        transform.rotation = rotateWithTarget
            ? Quaternion.Euler(90f, target.eulerAngles.y, 0f)
            : Quaternion.Euler(90f, 0f, 0f);
    }
}
