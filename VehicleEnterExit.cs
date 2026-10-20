using UnityEngine;

public class VehicleEnterExit : MonoBehaviour
{
    public ThirdPersonPlayer player;
    public Transform playerCamera;
    public Transform driverSeat;
    public Transform exitPoint;
    public SimpleCarController carController;
    public float interactDistance = 3.5f;
    public bool readKeyboardInput = true;

    private bool occupied;

    private void Awake()
    {
        if (carController == null) carController = GetComponent<SimpleCarController>();
    }

    private void Update()
    {
        if (!occupied || carController == null || !readKeyboardInput) return;

        float drive = Input.GetAxisRaw("Vertical");
        float steer = Input.GetAxisRaw("Horizontal");
        bool brake = Input.GetKey(KeyCode.Space);
        carController.SetInput(drive, steer, brake);
    }

    public void Interact()
    {
        if (player == null || carController == null) return;

        if (!occupied)
        {
            if (Vector3.Distance(player.transform.position, transform.position) > interactDistance) return;
            occupied = true;
            carController.hasDriver = true;
            player.MovementLocked = true;
            player.gameObject.SetActive(false);
            if (driverSeat != null) player.transform.position = driverSeat.position;
            if (playerCamera != null)
            {
                playerCamera.SetParent(transform, false);
                playerCamera.localPosition = new Vector3(0f, 3f, -6f);
                playerCamera.localRotation = Quaternion.Euler(15f, 0f, 0f);
            }
        }
        else
        {
            occupied = false;
            carController.hasDriver = false;
            carController.SetInput(0, 0, true);
            if (playerCamera != null) playerCamera.SetParent(null, true);
            player.gameObject.SetActive(true);
            player.MovementLocked = false;
            Transform point = exitPoint != null ? exitPoint : transform;
            player.transform.position = point.position + point.right * 2f + Vector3.up * 0.2f;
        }
    }
}
