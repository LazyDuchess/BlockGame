using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveGrenade : MonoBehaviour
{
    public float explosionForce = 10f;
    public float explosionRadius = 6f;
    public float timer = 1.5f;
    public GameObject body;
    public List<Rigidbody> misc;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Explode", timer);
    }

    public void Throw(Vector3 force)
    {
        foreach(var element in misc)
        {
            element.velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(3f,5f);
        }
        body.GetComponent<Rigidbody>().velocity = force;
    }

    void Explode()
    {
        var radius = 5;
        var height = 5;
        var blockPos = new Vector3(Mathf.Floor(body.transform.position.x), Mathf.Floor(body.transform.position.y), Mathf.Floor(body.transform.position.z));
        for(var i=-radius;i<radius;i++)
        {
            for(var n=-radius;n<radius;n++)
            {
                var dist = Vector2.Distance(new Vector2(i, n), Vector2.zero);
                var deep = height * (-(dist - radius)/radius);
                for(var j=-deep;j<deep;j++)
                {
                    var posi = new Vector3(i + blockPos.x, blockPos.y - j, n + blockPos.z);
                    Main.instance.RemoveBlockAt(posi, true);
                }
                
            }
        }
        var bodies = FindObjectsOfType<Rigidbody>();
        foreach (var element in bodies)
        {
            element.AddExplosionForce(explosionForce, body.transform.position, explosionRadius);
        }
        //Main.instance.RemoveBlockAt(blockPos, true);
        Destroy(gameObject);
    }
}
