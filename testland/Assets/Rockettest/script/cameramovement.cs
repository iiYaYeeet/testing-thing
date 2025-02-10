using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameramovement : MonoBehaviour
{
    //comps
    public Rigidbody RB;
    //floats
    public float MouseSensitivity = 3;
    public float moveSensitivity = 0.8f;
    public float moveSpeed = 10;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
            //get mouseaxi
            float xRot = Input.GetAxis("Mouse X") * MouseSensitivity;
            float yRot = -Input.GetAxis("Mouse Y") * MouseSensitivity;
            //get rot
            Vector3 Prot = transform.localRotation.eulerAngles;
            //add changetorot
            Prot.x += yRot;
            Prot.y += xRot;
            //plug back in
            transform.localRotation = Quaternion.Euler(Prot);
            //control movespeed
            moveSpeed += Input.mouseScrollDelta.y * moveSensitivity;
            if (moveSpeed <= 0)
            {
                moveSpeed = 0;
            }

            if (moveSpeed > 0)
            {
                //set 0
                Vector3 move = Vector3.zero;
                //fore
                if (Input.GetKey(KeyCode.W))
                    move += transform.forward;
                //aft
                if (Input.GetKey(KeyCode.S))
                    move -= transform.forward;
                //left
                if (Input.GetKey(KeyCode.A))
                    move -= transform.right;
                //right
                if (Input.GetKey(KeyCode.D))
                    move += transform.right;
                //setspeed
                move = move.normalized * moveSpeed;
                //plug back in
                RB.velocity = move;
            }
    }
}
