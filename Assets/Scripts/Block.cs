using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public int textureWidth = 160;
    public int textureHeight = 32;
    public BoxShape[] Shape;
    public virtual BoxShape[] GetModel()
    {
        return Shape;
    }
}
