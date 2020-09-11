using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    //public GameObject gameObject;
    public Rigidbody rigidBody;
    public float height = 1.8f;
    public float size = 0.75f;
    public float pitch = 0f;
    public float yaw = 0f;
    public float stepSize = 0.2f;
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

    public virtual float GetFOV()
    {
        return 1f;
    }
    public virtual void Spawn(Vector3 position, Quaternion rotation)
    {
        //gameObject = new GameObject("Entity");
        var coll = gameObject.AddComponent<BoxCollider>();
        coll.size = new Vector3(size, height-stepSize, size);
        coll.center = new Vector3(0f, stepSize, 0f);
        //coll.radius = size;
        //coll.height = height;
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
        var hits = Physics.BoxCastAll(transform.position - (height/2f) * Vector3.up + (0.2f * Vector3.up + stepSize * Vector3.up), new Vector3(size/2f-0.01f, 0.1f, size/2f-0.01f), Vector3.down, Quaternion.identity, 0.2f + stepSize, 1 << 8);
        if (hits.Length > 0)
        {
            RaycastHit highest = hits[0];
            foreach (var element in hits)
            {
                if (highest.point == Vector3.zero && element.point != Vector3.zero)
                    highest = element;
                if (element.point.y > highest.point.y && element.point != Vector3.zero)
                    highest = element;
            }
            
                rigidBody.useGravity = false;
                onground = true;
            if (highest.point != Vector3.zero)
                transform.position = new Vector3(transform.position.x, highest.point.y + (height / 2) + 0.01f, transform.position.z);
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
        }
        else
        {
            onground = false;
            rigidBody.useGravity = true;
        }
        /*
        if (Physics.Raycast(transform.position, Vector3.down, out hit, (height/2) + 0.1f, 1 << 8))
        {
            onground = true;
            transform.position = new Vector3(transform.position.x, hit.point.y + (height / 2), transform.position.z);
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
        }
        else
            onground = false;*/
    }
}
