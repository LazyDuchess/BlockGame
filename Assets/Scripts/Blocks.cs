using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Blocks
{
    public static Dictionary<string, Block> BlockRegistry = new Dictionary<string, Block>(); 
    public static Block AIR = new Block();
    public static Block GRASS = new Block();
    public static Block DIRT = new Block();
    public static Block STONE = new Block();
    public static Block LOG = new Block();

    public static void Initialize()
    {
        GRASS.Shape = new BoxShape[] { new BoxShape("Atlas", Vector3.zero, new Vector3(1f, -1f, 1f), Vector2.zero, 16f) };
        GRASS.Shape[0].setSides(new Vector2(32f, 0f), new Vector2(16f, 16f));
        GRASS.Shape[0].setTop(new Vector2(16f, 0f), new Vector2(16f, 16f));
        DIRT.Shape = new BoxShape[] { new BoxShape("Atlas", Vector3.zero, new Vector3(1f, -1f, 1f), Vector2.zero, 16f) };
        STONE.Shape = new BoxShape[] { new BoxShape("Atlas", Vector3.zero, new Vector3(1f, -1f, 1f), new Vector2(48f,0f), 16f) };
        LOG.Shape = new BoxShape[] { new BoxShape("Atlas", Vector3.zero, new Vector3(1f, -1f, 1f), new Vector2(0f, 16f), 16f) };
        LOG.Shape[0].setTop(new Vector2(16f, 16f), new Vector2(16f, 16f));
        Register("air", AIR);
        Register("grass", GRASS);
        Register("dirt", DIRT);
        Register("stone", STONE);
        Register("log", LOG);
    }

    static void Register(string id, Block block)
    {
        BlockRegistry[id] = block;
    }
}
