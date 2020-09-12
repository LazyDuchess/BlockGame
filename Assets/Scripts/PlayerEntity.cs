using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity
{
    public bool sneaking = false;
    float fovLerp = 0f;
    public float bobScale = 0.06f;
    public float bobSpeed = 11f;

    public float jumpForce = 5f;
    public float acceleration = 20f;
    public float topSpeed = 6f;
    public float deacceleration = 30f;

    public float sprintTopSpeed = 9f;
    public float sprintFOV = 1.25f;

    public float airAcceleration = 5f;
    public float airTopSpeed = 4f;

    public float sneakTopSpeed = 3f;

    public Vector3 movementAxis = Vector3.zero;
    public bool jump = false;
    public bool sprint = false;

    public override Vector3 getEyePos()
    {
        var hei = 0.8f;
        if (sneaking)
            hei = 0.5f;
        var movingAmount = rigidBody.velocity.magnitude / topSpeed;
        if (!onground)
            movingAmount = 0f;
        return gameObject.transform.position + hei * Vector3.up + (Vector3.up * Mathf.Sin(Time.time*bobSpeed) * bobScale * movingAmount);
    }

    public override float GetFOV()
    {
        return Mathf.Lerp(1f, sprintFOV, fovLerp);
    }
    protected override void Update()
    {
        
        base.Update();
        if (sneaking)
            sprint = false;
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
            
            var top = topSpeed;
            if (sprint)
                top = sprintTopSpeed;
            if (sneaking)
                top = sneakTopSpeed;
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
            if (sneaking)
            {
                var myBlockPos = new Vector3(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y), Mathf.Floor(transform.position.z));
                var blockPos = new Vector3(Mathf.Floor(groundPoint.x), myBlockPos.y, Mathf.Floor(groundPoint.z));
                var off = transform.position - blockPos;
                if (off.x <= -(size / 2f - 0.25f))
                {
                    if (Main.instance.GetBlockAtAbsolute(blockPos + Vector3.left,true) == Blocks.AIR)
                        rigidBody.velocity = new Vector3(Mathf.Max(0f, rigidBody.velocity.x), rigidBody.velocity.y, rigidBody.velocity.z);
                }
                if (off.z <= -(size / 2f - 0.25f))
                {
                    if (Main.instance.GetBlockAtAbsolute(blockPos + Vector3.back, true) == Blocks.AIR)
                        rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, Mathf.Max(0f, rigidBody.velocity.z));
                }

                if (off.x >= 1f+(size / 2f - 0.25f))
                {
                    if (Main.instance.GetBlockAtAbsolute(blockPos + Vector3.right, true) == Blocks.AIR)
                        rigidBody.velocity = new Vector3(Mathf.Min(0f, rigidBody.velocity.x), rigidBody.velocity.y, rigidBody.velocity.z);
                }
                if (off.z >= 1f+(size / 2f - 0.25f))
                {
                    if (Main.instance.GetBlockAtAbsolute(blockPos + Vector3.forward, true) == Blocks.AIR)
                        rigidBody.velocity = new Vector3(rigidBody.velocity.x, rigidBody.velocity.y, Mathf.Min(0f, rigidBody.velocity.z));
                }
            }
            if (jump)
            {
                onground = false;
                transform.position += Vector3.up * 0.1f;
                rigidBody.velocity += Vector3.up * jumpForce;
            }
        }
        else
        {
            if (Vector3.Dot(rigidBody.velocity, movementAxis.normalized) <= airTopSpeed)
                rigidBody.velocity += movementAxis * airAcceleration * Time.deltaTime;
        }
        jump = false;
    }
}
