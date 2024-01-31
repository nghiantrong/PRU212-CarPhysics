using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 1500f;
    public float rotationSpeed = 15f;

    public WheelJoint2D backWheel;
    public WheelJoint2D frontWheel;

    public Rigidbody2D rb;

    private float movement = 0f;
    private float rotation = 0f;

    public TextMeshProUGUI velocityText;
    public TextMeshProUGUI frictionText;
    public TextMeshProUGUI inertiaText;

    private float initialInertia;
    private void Start()
    {
        float inertia = rb.inertia;
        inertiaText.text = "Inertia: " + inertia.ToString("F2");
    }

    void Update()
    {
        movement = -Input.GetAxisRaw("Vertical") * speed;
        rotation = Input.GetAxisRaw("Horizontal");

        float velocity = rb.velocity.magnitude;

        // Calculate wheel angular velocities
        float frontWheelAngularVelocity = frontWheel.motor.motorSpeed;
        float backWheelAngularVelocity = backWheel.motor.motorSpeed;
        float carBodyAngularVelocity = rb.angularVelocity;

        // Calculate the difference in angular velocities as a measure of friction
        float friction = Mathf.Abs(frontWheelAngularVelocity - backWheelAngularVelocity) + Mathf.Abs(carBodyAngularVelocity);
        
        // Dynamic representation of inertia based on mass distribution
        float dynamicInertia = initialInertia + Mathf.Abs(carBodyAngularVelocity) * 0.1f;

        velocityText.text = "Velocity: " + velocity.ToString("F2");
        frictionText.text = "Friction: " + friction.ToString("F2");
        inertiaText.text = "Dynamic Inertia: " + dynamicInertia.ToString("F2");
    }
    void FixedUpdate()
    {
        if (movement == 0f)
        {
            backWheel.useMotor = false;
            frontWheel.useMotor = false;
        }
        else
        {
            backWheel.useMotor = true;
            frontWheel.useMotor = true;

            JointMotor2D motor = new JointMotor2D
            {
                motorSpeed = movement,
                maxMotorTorque = 10000
            };
            backWheel.motor = motor;
            frontWheel.motor = motor;
        }

        rb.AddTorque(rotation * rotationSpeed * Time.fixedDeltaTime);
    }
}
