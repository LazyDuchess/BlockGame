#define DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.PostProcessing;

public class Main : MonoBehaviour
{
    public GameObject grenadePrefab;
    public PostProcessProfile fxProfile;

    public static Main instance;
    public static Client client;
    public int spawnDist = 2;
    public int viewDist = 5;
    public int worldSeed;
    public bool randomSeed = true;
    public System.Random random;
    public float perlinX;
    public float perlinY;
    public double maxVal = 250000d;
    public Material blockMaterial;
    public Dictionary<Vector2,Chunk> chunks = new Dictionary<Vector2,Chunk>();
    public List<LoadTicket> tickets = new List<LoadTicket>();

    public Chunk getChunkAt(Vector3 pos, bool forceLoad)
    {
        var pos2 = new Vector2(Mathf.Floor(pos.x / 16), Mathf.Floor(pos.z / 16));
        if (chunks.ContainsKey(pos2))
            return chunks[pos2];
        else
        {
            if (!forceLoad)
                return null;
            else
            {
                var choonk = new Chunk(pos2);
                choonk.Load();
                chunks[pos2] = choonk;
                return choonk;
            }
        }
    }
    public Block GetBlockAtAbsolute(Vector3 blockPos, bool forceLoad)
    {
        var chunk = getChunkAt(blockPos, forceLoad);
        if (chunk == null)
            return null;
        var row = chunk.getRowAt((int)(blockPos.x - (chunk.position.x * 16)), (int)(blockPos.z - (chunk.position.y * 16)));
        return row.getBlockAt((int)blockPos.y);
    }
    public Block GetBlockAt(Vector3 blockPos, bool forceLoad)
    {
        var chunk = getChunkAt(blockPos, forceLoad);
        if (chunk == null)
            return null;
        var row = chunk.getRowAt((int)(chunk.position.x+blockPos.x), (int)(chunk.position.y+blockPos.z));
        return row.getBlockAt((int)blockPos.y);
    }
    public BlockRow GetRowAt(Vector3 blockPos, bool forceLoad)
    {
        var chunk = getChunkAt(blockPos, forceLoad);
        if (chunk == null)
            return null;
        var row = chunk.getRowAt((int)(chunk.position.x + blockPos.x), (int)(chunk.position.y + blockPos.z));
        return row;
    }

    public bool PlaceBlockAt(Vector3 blockPos, Block block, bool forceLoad)
    {
        //Logger.Log("Debuggi");
        var chunk = getChunkAt(blockPos, forceLoad);
        if (chunk == null)
        {
            //Logger.Log("fak");
            return false;
        }
        var row = chunk.getRowAt((int)(blockPos.x - (chunk.position.x * 16)), (int)(blockPos.z - (chunk.position.y * 16)));
        return row.PlaceBlockAt((int)blockPos.y, block, chunk);
    }
    /*
    public Block GetBlockAt(Vector3 blockPos, bool forceLoad)
    {
        //Logger.Log("Debuggi");
        var chunk = getChunkAt(blockPos, forceLoad);
        if (chunk == null)
        {
            //Logger.Log("fak");
            return;
        }
        var row = chunk.getRowAt((int)(blockPos.x - (chunk.position.x * 16)), (int)(blockPos.z - (chunk.position.y * 16)));
        return row.getBlockAt((int)blockPos.y);
    }*/

    public void RemoveBlockAt(Vector3 blockPos, bool forceLoad)
    {
        var chunk = getChunkAt(blockPos, forceLoad);
        if (chunk == null)
        {
            return;
        }
        var row = chunk.getRowAt((int)(blockPos.x - (chunk.position.x * 16) ), (int)( blockPos.z - (chunk.position.y * 16)));
        row.RemoveAt((int)blockPos.y, chunk);
    }

    public void DebugBlockAt(Vector3 blockPos)
    {
        Logger.Log("-- ROW DEBUG --");
        var chunk = getChunkAt(blockPos, false);
        if (chunk == null)
        {
            //Logger.Log("fak");
            return;
        }
        var row = chunk.getRowAt((int)(blockPos.x - (chunk.position.x * 16)), (int)(blockPos.z - (chunk.position.y * 16)));
        foreach(var element in row.blocks)
        {
            Logger.Log("Element: YMin = " + element.yMin.ToString() + ", YMax = " + element.yMax.ToString() + ", Block = " + element.block.ToString());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!instance)
            instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }
        Blocks.Initialize();
        if (randomSeed)
            worldSeed = UnityEngine.Random.Range(0, 999999999);
        client = new Client();
        client.Start();
        random = new System.Random(worldSeed);
        perlinX = (float)(random.NextDouble() * maxVal);
        perlinY = (float)(random.NextDouble() * maxVal);
        for(var i=-spawnDist;i<spawnDist;i++)
        {
            for(var n=-spawnDist;n<spawnDist;n++)
            {
                var choonk = new Chunk(new Vector2(i,n));
                choonk.ForceLoad();
                chunks[new Vector2(i, n)] = choonk;
            }
        }
        var playerObject = new GameObject("Player");
        client.localPlayer = playerObject.AddComponent<PlayerEntity>();
        client.localPlayer.Spawn(new Vector3(0f,80f,0f),Quaternion.identity);
        tickets.Add(new LoadTicket(viewDist, client.localPlayer, true));
    }

    public GameCamera getCamera()
    {
        return GameCamera.instance;
    }

    private void FixedUpdate()
    {
        client.FixedUpdate();
    }
    // Update is called once per frame
    void Update()
    {
        getCamera().transform.position = client.localPlayer.getEyePos();
        getCamera().transform.rotation = client.localPlayer.getEyeRotation();
        var cams = getCamera().GetComponentsInChildren<Camera>();
        foreach(var element in cams)
        {
            element.fieldOfView = 90f * client.localPlayer.GetFOV();
        }
        client.Update();
        
        List<Vector2> chunksToLoad = new List<Vector2>();
        foreach(var element in tickets)
        {
            var pos = element.position;
            if (element.parent != null)
                pos = element.parent.gameObject.transform.position;
            Vector2 pos2d = new Vector2(Mathf.Floor(pos.x / 16f), Mathf.Floor(pos.z / 16f));
            for (var i = -element.size; i < element.size; i++)
            {
                for (var n = -element.size; n < element.size; n++)
                {
                    chunksToLoad.Add(pos2d + new Vector2(i, n));
                    if (!chunks.ContainsKey(pos2d + new Vector2(i, n)))
                    {
                        var choonk = new Chunk(pos2d + new Vector2(i, n));
                        choonk.Load();
                        chunks[pos2d + new Vector2(i, n)] = choonk;

                    }
                    else
                        chunks[pos2d + new Vector2(i, n)].Load();
                }
            }
        }
        foreach (var element in chunks)
        {
            element.Value.Update();
            if (chunksToLoad.IndexOf(element.Key) <= -1)
                element.Value.Unload();
        }
        foreach(var element in Chunk.chunkCleanUp)
        {
            chunks.Remove(element);
        }
        Chunk.chunkCleanUp.Clear();
    }
}
