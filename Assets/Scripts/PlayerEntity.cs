using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity
{
    public float jumpForce = 5f;
    public float acceleration = 20f;
    public float topSpeed = 7f;
    public float deacceleration = 30f;

    public float airAcceleration = 5f;
    public float airTopSpeed = 4f;

    public Vector3 movementAxis = Vector3.zero;
    public bool jump = false;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (onground)
        {
            if (jump)
            {
                onground = false;
                transform.position += Vector3.up * 0.1f;
                rigidBody.velocity += Vector3.up * jumpForce;
                jump = false;
            }
            rigidBody.velocity += movementAxis * acceleration * Time.deltaTime;
            var velocityAxis = rigidBody.velocity.normalized;
            var velocityFinal = rigidBody.velocity.magnitude;
            velocityFinal -= deacceleration * Time.deltaTime;
            if (velocityFinal <= 0f)
                velocityFinal = 0f;
            var velocityVector = velocityFinal * velocityAxis;
            velocityVector -= Vector3.Project(velocityVector, movementAxis);
            velocityVector += Vector3.Project(rigidBody.velocity, movementAxis);
            rigidBody.velocity = velocityVector;
            if (rigidBody.velocity.magnitude >= topSpeed)
                rigidBody.velocity = topSpeed * rigidBody.velocity.normalized;
        }
        else
        {
            if (Vector3.Dot(rigidBody.velocity, movementAxis.normalized) <= airTopSpeed)
                rigidBody.velocity += movementAxis * airAcceleration * Time.deltaTime;
        }
    }
}
