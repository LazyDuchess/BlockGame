using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPostRenderer : MonoBehaviour
{
    public Material mat;
    void OnPostRender()
    {

        if (Main.client.targeting)
        {
            //GL.PushMatrix();
            //GL.Clear(true, false, new Color(0.0f,0.0f,0.0f,0.0f));
            GL.Color(Color.black);
            GL.PushMatrix();
            mat.SetPass(0);
            GL.Begin(GL.LINES);
            
            GL.Vertex(Main.client.targetBlock);
            GL.Vertex(Main.client.targetBlock + Vector3.right);
            GL.Vertex(Main.client.targetBlock);
            GL.Vertex(Main.client.targetBlock - Vector3.up);
            GL.Vertex(Main.client.targetBlock);
            GL.Vertex(Main.client.targetBlock + Vector3.forward);
            GL.Vertex(Main.client.targetBlock - Vector3.up);
            GL.Vertex(Main.client.targetBlock - Vector3.up + Vector3.forward);
            GL.Vertex(Main.client.targetBlock - Vector3.up + Vector3.right);
            GL.Vertex(Main.client.targetBlock - Vector3.up + Vector3.right + Vector3.forward);
            GL.Vertex(Main.client.targetBlock + Vector3.right);
            GL.Vertex(Main.client.targetBlock + Vector3.right + Vector3.forward);
            GL.Vertex(Main.client.targetBlock + Vector3.right + Vector3.forward);
            GL.Vertex(Main.client.targetBlock + Vector3.forward);
            GL.Vertex(Main.client.targetBlock + Vector3.right + Vector3.forward - Vector3.up);
            GL.Vertex(Main.client.targetBlock + Vector3.forward - Vector3.up);
            GL.Vertex(Main.client.targetBlock + Vector3.right + Vector3.forward);
            GL.Vertex(Main.client.targetBlock + Vector3.right + Vector3.forward - Vector3.up);
            GL.Vertex(Main.client.targetBlock + Vector3.forward);
            GL.Vertex(Main.client.targetBlock + Vector3.forward - Vector3.up);
            GL.Vertex(Main.client.targetBlock + Vector3.right);
            GL.Vertex(Main.client.targetBlock + Vector3.right - Vector3.up);
            GL.Vertex(Main.client.targetBlock - Vector3.up);
            GL.Vertex(Main.client.targetBlock + Vector3.right - Vector3.up);
            GL.End();
            GL.PopMatrix();
            //GL.PopMatrix();
        }
    }
}
