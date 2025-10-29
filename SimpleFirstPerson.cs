using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleFirstPerson : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    private CharacterController characterController;
    private float rotationX = 0;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Mouse look
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        float rotationY = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * lookSpeed;
        transform.localEulerAngles = new Vector3(rotationX, rotationY, 0);

        // Movement
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        float v = Input.GetAxis("Vertical") * walkSpeed;
        float h = Input.GetAxis("Horizontal") * walkSpeed;
        Vector3 move = (forward * v + right * h) * Time.deltaTime;
        characterController.Move(move);

        // Simple escape to unlock cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
