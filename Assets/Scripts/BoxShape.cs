using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BoxSide
{
    public Vector2 uv1;
    public Vector2 uv2;

    public BoxSide(Vector2 uv1, Vector2 uv2)
    {
        this.uv1 = uv1;
        this.uv2 = uv2;
    }
}
public class BoxShape
{
    public BoxSide[] sides;
    public Vector3 from;
    public Vector3 to;
    public string texture;

    public BoxShape(string texture, Vector3 from, Vector3 to, Vector2 uv, float textureSize)
    {
        this.from = from;
        this.to = to;
        this.texture = texture;
        var u1 = new Vector2(uv.x, uv.y);
        var ud = new Vector2(textureSize, -textureSize);
        var u2 = u1 + ud;
        this.sides = new BoxSide[] { new BoxSide(u1,u2), new BoxSide(u1, u2), new BoxSide(u1, u2), new BoxSide(u1, u2), new BoxSide(u1, u2), new BoxSide(u1, u2) };
    }

    public BoxShape setTop(Vector2 uv, Vector2 uv2)
    {
        var u1 = new Vector2(uv.x, uv.y);
        var ud = new Vector2(uv2.x, -uv2.y);
        var u2 = u1 + ud;
        this.sides[0] = new BoxSide(u1, u2);
        return this;
    }

    public BoxShape setBottom(Vector2 uv, Vector2 uv2)
    {
        var u1 = new Vector2(uv.x, uv.y);
        var ud = new Vector2(uv2.x, -uv2.y);
        var u2 = u1 + ud;
        this.sides[1] = new BoxSide(u1, u2);
        return this;
    }

    public BoxShape setSides(Vector2 uv, Vector2 uv2)
    {
        var u1 = new Vector2(uv.x, uv.y);
        var ud = new Vector2(uv2.x, -uv2.y);
        var u2 = u1 + ud;
        this.sides[2] = new BoxSide(u1, u2);
        this.sides[3] = new BoxSide(u1, u2);
        this.sides[4] = new BoxSide(u1, u2);
        this.sides[5] = new BoxSide(u1, u2);
        return this;
    }

    public BoxShape setFront(Vector2 uv, Vector2 uv2)
    {
        var u1 = new Vector2(uv.x, uv.y);
        var ud = new Vector2(uv2.x, -uv2.y);
        var u2 = u1 + ud;
        this.sides[2] = new BoxSide(u1, u2);
        return this;
    }

    public BoxShape setBack(Vector2 uv, Vector2 uv2)
    {
        var u1 = new Vector2(uv.x, uv.y);
        var ud = new Vector2(uv2.x, -uv2.y);
        var u2 = u1 + ud;
        this.sides[3] = new BoxSide(u1, u2);
        return this;
    }

    public BoxShape setRight(Vector2 uv, Vector2 uv2)
    {
        var u1 = new Vector2(uv.x, uv.y);
        var ud = new Vector2(uv2.x, -uv2.y);
        var u2 = u1 + ud;
        this.sides[4] = new BoxSide(u1, u2);
        return this;
    }

    public BoxShape setLeft(Vector2 uv, Vector2 uv2)
    {
        var u1 = new Vector2(uv.x, uv.y);
        var ud = new Vector2(uv2.x, -uv2.y);
        var u2 = u1 + ud;
        this.sides[5] = new BoxSide(u1, u2);
        return this;
    }
}
