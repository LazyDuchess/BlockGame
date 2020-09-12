using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class Chunk
{
    public bool dirty = false;
    bool async = false;
    public static Color ao = new Color(0.1f, 0.1f, 0.1f);
    public static List<Vector2> chunkCleanUp = new List<Vector2>();
    Renderer chunkRenderer;
    MeshFilter chunkFilter;
    Thread genThread;
    List<Vector3> verts;
    List<Vector3> normals;
    List<int> triangles;
    List<Color> colors;
    List<Vector2> uvs;
    bool isLoading = false;
    bool unloading = false;
    float loading = 0.0f;
    private bool doneGenerating = false;
    public Vector2 position;
    public GameObject obj;
    public Mesh mesh = new Mesh();
    public BlockRow[] blocks = new BlockRow[256];
    MaterialPropertyBlock chunkMat = new MaterialPropertyBlock();
    public Chunk(Vector2 id)
    {
        if (id.x != (int)id.x || id.y != (int)id.y)
        {
            Logger.Error("Tried to load a chunk at an invalid position! (" + id.ToString() + ")");
        }
        else
        {
            position = id;
            /*
            */
        }
    }

    public void ForceLoad()
    {
        if (!doneGenerating)
        {
            isLoading = true;
            unloading = false;
            Generate();
        }
    }
    public void Load()
    {
        if (!isLoading && !doneGenerating)
        {
            isLoading = true;
            unloading = false;
            ThreadStart threadDelegate = new ThreadStart(this.Generate);
            genThread = new Thread(threadDelegate);
            genThread.IsBackground = true;
            genThread.Start();
            /*
            var cpos = position + Vector2.up;
            if (Main.instance.chunks.ContainsKey(cpos) && Main.instance.chunks[cpos].IsGenerated())
                Main.instance.chunks[cpos].AsyncRegen(false);
            cpos = position - Vector2.up;
            if (Main.instance.chunks.ContainsKey(cpos) && Main.instance.chunks[cpos].IsGenerated())
                Main.instance.chunks[cpos].AsyncRegen(false);
            cpos = position + Vector2.right;
            if (Main.instance.chunks.ContainsKey(cpos) && Main.instance.chunks[cpos].IsGenerated())
                Main.instance.chunks[cpos].AsyncRegen(false);
            cpos = position - Vector2.right;
            if (Main.instance.chunks.ContainsKey(cpos) && Main.instance.chunks[cpos].IsGenerated())
                Main.instance.chunks[cpos].AsyncRegen(false);*/
        }
        else
        {
            unloading = false;
        }
    }

    public void Unload()
    {
        unloading = true;
    }
    void aRegen()
    {
        RegenerateModel();
        async = true;
    }

    public void AsyncRegen(bool doAdjacent = false)
    {
        genThread?.Abort();
        ThreadStart threadDelegate = new ThreadStart(this.aRegen);
        genThread = new Thread(threadDelegate);
        genThread.IsBackground = true;
        genThread.Start();
        if (doAdjacent)
        {
            var cpos = position + Vector2.up;
            if (Main.instance.chunks.ContainsKey(cpos) && Main.instance.chunks[cpos].IsGenerated())
                Main.instance.chunks[cpos].AsyncRegen(false);
            cpos = position - Vector2.up;
            if (Main.instance.chunks.ContainsKey(cpos) && Main.instance.chunks[cpos].IsGenerated())
                Main.instance.chunks[cpos].AsyncRegen(false);
            cpos = position + Vector2.right;
            if (Main.instance.chunks.ContainsKey(cpos) && Main.instance.chunks[cpos].IsGenerated())
                Main.instance.chunks[cpos].AsyncRegen(false);
            cpos = position - Vector2.right;
            if (Main.instance.chunks.ContainsKey(cpos) && Main.instance.chunks[cpos].IsGenerated())
                Main.instance.chunks[cpos].AsyncRegen(false);
        }
    }
    public void Update()
    {
        if (dirty)
        {
            AsyncRegen(true);
            dirty = false;
        }
        if (doneGenerating && async)
        {
            mesh.Clear();
            try
            {
                toMesh(mesh);
                async = false;
                obj.GetComponent<MeshCollider>().sharedMesh = mesh;
                chunkFilter.sharedMesh = mesh;
            }
            catch
            {
                AsyncRegen();
            }
            

        }
        if (doneGenerating && isLoading)
        {
            isLoading = false;
            mesh = new Mesh();
            toMesh(mesh);
            verts.Clear();
            normals.Clear();
            triangles.Clear();
            obj = new GameObject("Chunk");
            obj.layer = 8;
            obj.transform.position = new Vector3(position.x * 16f, 0f, position.y * 16f);
            //obj.AddComponent<MeshFilter>();
            //obj.GetComponent<MeshFilter>().sharedMesh = mesh;
            //obj.AddComponent<MeshRenderer>().sharedMaterial = Main.instance.blockMaterial;
            obj.AddComponent<MeshCollider>().sharedMesh = mesh;
            chunkRenderer = obj.AddComponent<MeshRenderer>();
            chunkFilter = obj.AddComponent<MeshFilter>();
            chunkFilter.sharedMesh = mesh;
            chunkRenderer.sharedMaterial = Main.instance.blockMaterial;
        }
        if (doneGenerating)
        {
            if (unloading)
            {
                loading = Mathf.Clamp(loading - 1f * Time.deltaTime, 0f, 1f);
                if (loading <= 0f)
                {
                    mesh = null;
                    GameObject.Destroy(obj);
                    chunkCleanUp.Add(position);
                    //Main.instance.chunks[position] = null;
                }
            }
            else
                loading = Mathf.Clamp(loading + 1f * Time.deltaTime, 0f, 1f);
            if (doneGenerating)
            {
                chunkMat.SetFloat("_LoadProgress", loading);
                chunkRenderer.SetPropertyBlock(chunkMat);
                //Graphics.DrawMesh(mesh, new Vector3(position.x * 16f, 0f, position.y * 16f), Quaternion.identity, Main.instance.blockMaterial, 0, Main.instance.getCamera().GetComponent<Camera>(), 0, chunkMat);
            }
            
        }
    }
    void RegenerateModel()
    {
        verts = new List<Vector3>();
        normals = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
        uvs = new List<Vector2>();
        var vertexAmount = 0;
        for (var i = 0; i < blocks.Length; i++)
        {
            foreach (var element in blocks[i].blocks)
            {
                if (element.block == Blocks.AIR)
                    continue;
                var model = element.block.GetModel();
                int x = (int)(i - (Mathf.Floor(i / 16) * 16));
                var y = element.yMax;
                int z = (int)(Mathf.Floor(i / 16));

                var rightRow = getRowAt(x + 1, z);
                var leftRow = getRowAt(x - 1, z);
                var frontRow = getRowAt(x, z + 1);
                var backRow = getRowAt(x, z - 1);
                var botBlock = blocks[i].getBlockAt(element.yMin - 1);
                var topBlock = blocks[i].getBlockAt(element.yMax + 1);

                float texWidth = element.block.textureWidth;
                float texHeight = element.block.textureHeight;
                if (topBlock == Blocks.AIR)
                {
                    var side = model[0].sides[0];
                    var uv1 = new Vector2(side.uv1.x / texWidth, side.uv1.y / texHeight);
                    var uv2 = new Vector2(side.uv2.x / texWidth, side.uv2.y / texHeight);
                    uvs.Add(uv1);
                    verts.Add(new Vector3(x, y, z));
                    vertexAmount++;
                    uvs.Add(new Vector2(uv2.x, uv1.y));
                    verts.Add(new Vector3(x + 1f, y, z));
                    vertexAmount++;
                    uvs.Add(new Vector2(uv2.x, uv2.y));
                    verts.Add(new Vector3(x + 1f, y, z + 1f));
                    vertexAmount++;
                    triangles.Add(verts.Count);

                    triangles.Add(verts.Count - 2);

                    triangles.Add(verts.Count - 3);

                    uvs.Add(new Vector2(uv1.x, uv2.y));
                    verts.Add(new Vector3(x, y, z + 1f));
                    vertexAmount++;

                    triangles.Add(verts.Count - 1);

                    triangles.Add(verts.Count - 2);

                    triangles.Add(verts.Count - 3);

                    normals.Add(Vector3.up);
                    normals.Add(Vector3.up);
                    normals.Add(Vector3.up);
                    normals.Add(Vector3.up);
                }
                if (botBlock == Blocks.AIR)
                {
                    y = element.yMin - 1;
                    var side = model[0].sides[1];
                    var uv1 = new Vector2(side.uv1.x / texWidth, side.uv1.y / texHeight);
                    var uv2 = new Vector2(side.uv2.x / texWidth, side.uv2.y / texHeight);
                    uvs.Add(new Vector2(uv1.x, uv2.y));
                    verts.Add(new Vector3(x, y, z));
                    vertexAmount++;
                    uvs.Add(new Vector2(uv2.x, uv2.y));
                    verts.Add(new Vector3(x + 1f, y, z));
                    vertexAmount++;
                    uvs.Add(new Vector2(uv2.x, uv1.y));
                    verts.Add(new Vector3(x + 1f, y, z + 1f));
                    vertexAmount++;

                    triangles.Add(verts.Count - 3);

                    triangles.Add(verts.Count - 2);

                    triangles.Add(verts.Count);


                    uvs.Add(new Vector2(uv1.x, uv1.y));
                    verts.Add(new Vector3(x, y, z + 1f));
                    vertexAmount++;

                    triangles.Add(verts.Count - 3);

                    triangles.Add(verts.Count - 2);

                    triangles.Add(verts.Count - 1);

                    normals.Add(Vector3.down);
                    normals.Add(Vector3.down);
                    normals.Add(Vector3.down);
                    normals.Add(Vector3.down);

                    //vertexAmount += 4;
                }

                for (var n = element.yMax; n >= element.yMin; n--)
                {
                    if (rightRow.getBlockAt(n) == Blocks.AIR)
                    {
                        var side = model[0].sides[4];
                        var uv1 = new Vector2(side.uv1.x / texWidth, side.uv1.y / texHeight);
                        var uv2 = new Vector2(side.uv2.x / texWidth, side.uv2.y / texHeight);
                        uvs.Add(new Vector2(uv1.x, uv1.y));
                        verts.Add(new Vector3(x + 1, n, z));
                        uvs.Add(new Vector2(uv2.x, uv1.y));
                        verts.Add(new Vector3(x + 1, n, z + 1));
                        uvs.Add(new Vector2(uv2.x, uv2.y));
                        verts.Add(new Vector3(x + 1, n - 1, z + 1));

                        triangles.Add(verts.Count - 3);

                        triangles.Add(verts.Count - 2);

                        triangles.Add(verts.Count - 1);
                        uvs.Add(new Vector2(uv1.x, uv2.y));
                        verts.Add(new Vector3(x + 1, n - 1, z));

                        triangles.Add(verts.Count - 4);

                        triangles.Add(verts.Count - 2);

                        triangles.Add(verts.Count - 1);

                        normals.Add(Vector3.right);
                        normals.Add(Vector3.right);
                        normals.Add(Vector3.right);
                        normals.Add(Vector3.right);
                    }

                    if (leftRow.getBlockAt(n) == Blocks.AIR)
                    {
                        var side = model[0].sides[5];
                        var uv1 = new Vector2(side.uv1.x / texWidth, side.uv1.y / texHeight);
                        var uv2 = new Vector2(side.uv2.x / texWidth, side.uv2.y / texHeight);
                        uvs.Add(new Vector2(uv1.x, uv1.y));
                        verts.Add(new Vector3(x, n, z));
                        uvs.Add(new Vector2(uv2.x, uv1.y));
                        verts.Add(new Vector3(x, n, z + 1));
                        uvs.Add(new Vector2(uv2.x, uv2.y));
                        verts.Add(new Vector3(x, n - 1, z + 1));

                        triangles.Add(verts.Count - 1);

                        triangles.Add(verts.Count - 2);

                        triangles.Add(verts.Count - 3);

                        uvs.Add(new Vector2(uv1.x, uv2.y));
                        verts.Add(new Vector3(x, n - 1, z));

                        triangles.Add(verts.Count - 1);

                        triangles.Add(verts.Count - 2);

                        triangles.Add(verts.Count - 4);

                        normals.Add(Vector3.left);
                        normals.Add(Vector3.left);
                        normals.Add(Vector3.left);
                        normals.Add(Vector3.left);
                    }

                    if (frontRow.getBlockAt(n) == Blocks.AIR)
                    {
                        var side = model[0].sides[2];
                        var uv1 = new Vector2(side.uv1.x / texWidth, side.uv1.y / texHeight);
                        var uv2 = new Vector2(side.uv2.x / texWidth, side.uv2.y / texHeight);
                        uvs.Add(new Vector2(uv1.x, uv1.y));
                        verts.Add(new Vector3(x, n, z + 1));
                        uvs.Add(new Vector2(uv2.x, uv2.y));
                        verts.Add(new Vector3(x + 1, n - 1, z + 1));
                        uvs.Add(new Vector2(uv1.x, uv2.y));
                        verts.Add(new Vector3(x, n - 1, z + 1));

                        triangles.Add(verts.Count - 1);

                        triangles.Add(verts.Count - 2);

                        triangles.Add(verts.Count - 3);

                        uvs.Add(new Vector2(uv2.x, uv1.y));
                        verts.Add(new Vector3(x + 1, n, z + 1));

                        triangles.Add(verts.Count - 4);

                        triangles.Add(verts.Count - 3);

                        triangles.Add(verts.Count - 1);

                        normals.Add(Vector3.forward);
                        normals.Add(Vector3.forward);
                        normals.Add(Vector3.forward);
                        normals.Add(Vector3.forward);
                    }

                    if (backRow.getBlockAt(n) == Blocks.AIR)
                    {
                        var side = model[0].sides[3];
                        var uv1 = new Vector2(side.uv1.x / texWidth, side.uv1.y / texHeight);
                        var uv2 = new Vector2(side.uv2.x / texWidth, side.uv2.y / texHeight);
                        uvs.Add(new Vector2(uv1.x, uv1.y));
                        verts.Add(new Vector3(x, n, z));
                        uvs.Add(new Vector2(uv2.x, uv2.y));
                        verts.Add(new Vector3(x + 1, n - 1, z));
                        uvs.Add(new Vector2(uv1.x, uv2.y));
                        verts.Add(new Vector3(x, n - 1, z));

                        triangles.Add(verts.Count - 3);

                        triangles.Add(verts.Count - 2);

                        triangles.Add(verts.Count - 1);

                        uvs.Add(new Vector2(uv2.x, uv1.y));
                        verts.Add(new Vector3(x + 1, n, z));

                        triangles.Add(verts.Count - 1);

                        triangles.Add(verts.Count - 3);

                        triangles.Add(verts.Count - 4);

                        normals.Add(Vector3.back);
                        normals.Add(Vector3.back);
                        normals.Add(Vector3.back);
                        normals.Add(Vector3.back);
                    }
                }
            }
        }
    }
    void Generate()
    {
        for (var i = 0; i < blocks.Length; i++)
        {
            
            var x = i - (Mathf.Floor(i / 16) * 16) + position.x * 16;
            var z = Mathf.Floor(i / 16) + position.y * 16;
            var y = 16 + (int)(Perlin.Noise(Main.instance.perlinX + x * 0.1f, Main.instance.perlinY + z * 0.1f) * 3);
            blocks[i] = BlockRow.getAt(new Vector2(x, z));
        }
        doneGenerating = false;
        RegenerateModel();
        doneGenerating = true;
    }
    public BlockRow getRowAt(int x, int y)
    {
        if (x >= 16 || x < 0 || y < 0 || y >= 16)
            return BlockRow.getAt(new Vector2(position.x*16+x, position.y*16+y));
        int location = x + (y * 16);
        return blocks[location];
    }

    public static BlockRow getNaturalRowAt(int x, int y)
    {
        return BlockRow.getAt(new Vector2(x,y));
    }

    public void toMesh(Mesh mesh)
    {
        var verts2 = new List<Vector3>(verts);
        var tris2 = new List<int>(triangles);
        var norms2 = new List<Vector3>(normals);
        var colors2 = new List<Color>(colors);
        var uvs2 = new List<Vector2>(uvs);
        if (verts2.Count != uvs2.Count || verts2.Count != norms2.Count )
        {
            AsyncRegen(true);
            return;
        }
        mesh.SetVertices(verts2);
        mesh.SetTriangles(tris2, 0);
        mesh.SetNormals(norms2);
        mesh.SetColors(colors2);
        mesh.SetUVs(0, uvs2);
        verts.Clear();
        triangles.Clear();
        normals.Clear();
        colors.Clear();
        uvs.Clear();
        
        //mesh.RecalculateNormals();
    }

    public bool IsGenerated()
    {
        return doneGenerating;
    }
}
