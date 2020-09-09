using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTicket
{
    public bool visual = false;
    public int size = 2;
    public Entity parent;
    public Vector3 position;

    public LoadTicket(int size, Vector3 position, bool visual = false)
    {
        this.size = size;
        this.visual = visual;
        this.position = position;
    }

    public LoadTicket(int size, Entity parent, bool visual = false)
    {
        this.size = size;
        this.visual = visual;
        this.parent = parent;
    }
}
