using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 1500f;
    public float rotationSpeed = 15f;

    public float brakeForce = 500f; // Adjust the braking force as needed
    public float leanAnimationSpeed = 5f; // Adjust the leaning animation speed

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
        float initialInertia = rb.inertia;
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

        // Check for braking using space key
        if (Input.GetKey(KeyCode.Space))
        {
            // Apply braking force to both wheels
            rb.AddForce(-rb.velocity.normalized * brakeForce * Time.deltaTime, ForceMode2D.Force);
        }

        // Adjust motor speed based on the car's velocity to simulate leaning
        float leanSpeed = Mathf.Clamp(velocity * leanAnimationSpeed, 0f, speed);
        backWheel.motor = SetMotorSpeed(backWheel.motor, movement + leanSpeed);
        frontWheel.motor = SetMotorSpeed(frontWheel.motor, movement + leanSpeed);
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


     // Helper method to set motor speed and return the updated JointMotor2D
    private JointMotor2D SetMotorSpeed(JointMotor2D motor, float speed)
    {
        motor.motorSpeed = speed;
        return motor;
    }
}
