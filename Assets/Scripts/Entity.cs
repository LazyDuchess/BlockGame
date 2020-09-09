using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    //public GameObject gameObject;
    public Rigidbody rigidBody;
    public float height = 1.8f;
    public float size = 0.25f;
    public float pitch = 0f;
    public float yaw = 0f;
    public bool onground = false;
    // Start is called before the first frame update
    /*
    public Entity()
    {
        gameObject = new GameObject("Entity");
    }*/

    public virtual void Spawn()
    {
        //gameObject = new GameObject("Entity");
        var coll = gameObject.AddComponent<CapsuleCollider>();
        coll.radius = size;
        coll.height = height;
        //coll.size = new Vector3(size, height, size);
        coll.sharedMaterial = Resources.Load<PhysicMaterial>("Entity");
        rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.freezeRotation = true;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;

    }

    public virtual void Spawn(Vector3 position, Quaternion rotation)
    {
        //gameObject = new GameObject("Entity");
        var coll = gameObject.AddComponent<CapsuleCollider>();
        coll.radius = size;
        coll.height = height;
        //coll.size = new Vector3(size, height, size);
        coll.sharedMaterial = Resources.Load<PhysicMaterial>("Entity");
        rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.freezeRotation = true;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;

        gameObject.transform.position = position;
        gameObject.transform.rotation = rotation;
    }

    public virtual Vector3 getEyePos()
    {
        return gameObject.transform.position + 0.8f * Vector3.up;
    }

    public virtual Quaternion getEyeRotation()
    {
        return Quaternion.Euler(pitch, yaw, 0f);
    }

    public virtual Quaternion getRotation()
    {
        return Quaternion.Euler(0f, yaw, 0f);
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, (height/2) + 0.1f, 1 << 8))
        {
            onground = true;
            transform.position = new Vector3(transform.position.x, hit.point.y + (height / 2), transform.position.z);
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
        }
        else
            onground = false;
    }
}
