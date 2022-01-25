using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float MoveSpeed = 10;
    private Rigidbody rb;
    private CharacterController cc;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float h_input = Input.GetAxis("Horizontal");
        float v_input = Input.GetAxis("Vertical");
        float ud_input = Input.GetAxis("UpDown");

        Vector3 velocity = transform.right * h_input + transform.up * ud_input + transform.forward * v_input;
        cc.Move(velocity * MoveSpeed * Time.deltaTime);
    }
}
