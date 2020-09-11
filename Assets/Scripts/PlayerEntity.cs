using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity
{
    float fovLerp = 0f;
    public float bobScale = 0.06f;
    public float bobSpeed = 11f;

    public float jumpForce = 9f;
    public float acceleration = 20f;
    public float topSpeed = 6f;
    public float deacceleration = 30f;

    public float sprintTopSpeed = 9f;
    public float sprintFOV = 1.25f;

    public float airAcceleration = 5f;
    public float airTopSpeed = 4f;

    public Vector3 movementAxis = Vector3.zero;
    public bool jump = false;
    public bool sprint = false;

    public override Vector3 getEyePos()
    {
        var movingAmount = rigidBody.velocity.magnitude / topSpeed;
        if (!onground)
            movingAmount = 0f;
        return gameObject.transform.position + 0.8f * Vector3.up + (Vector3.up * Mathf.Sin(Time.time*bobSpeed) * bobScale * movingAmount);
    }

    public override float GetFOV()
    {
        return Mathf.Lerp(1f, sprintFOV, fovLerp);
    }
    private void Update()
    {
        if (sprint)
            fovLerp = Mathf.Lerp(fovLerp, 1f, 20f * Time.deltaTime);
        else
            fovLerp = Mathf.Lerp(fovLerp, 0f, 20f * Time.deltaTime);
    }
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
            }
            var top = topSpeed;
            if (sprint)
                top = sprintTopSpeed;
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
            if (rigidBody.velocity.magnitude >= top)
                rigidBody.velocity = top * rigidBody.velocity.normalized;
        }
        else
        {
            if (Vector3.Dot(rigidBody.velocity, movementAxis.normalized) <= airTopSpeed)
                rigidBody.velocity += movementAxis * airAcceleration * Time.deltaTime;
        }
        jump = false;
    }
}
