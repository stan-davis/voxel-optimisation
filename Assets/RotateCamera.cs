using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100;
    [SerializeField] private Transform parentBody;

    float xRotation = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        xRotation = 90;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        parentBody.Rotate(Vector3.up * mouseX);
    }
}
