using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockRow
{
    public List<RowBlocks> blocks = new List<RowBlocks>();

    public Block getBlockAt(int height)
    {
        //Remove dumb restrictions
        /*
        //Please don't uncomment the debug logs unless you want +6gb text files on your pc
        if (height > 256)
        {
            //Logger.DevWarn("Tried to get a block from a chunk row at a height greater than 256, returning air (" + height.ToString() + ")");
            return Blocks.AIR;
        }
        if (height < 0)
        {
            //Logger.DevWarn("Tried to get a block from a chunk row at a height below 0, returning air (" + height.ToString() + ")");
            return Blocks.AIR;
        }*/
        foreach (var element in blocks)
        {
            if (element.yMin <= height && element.yMax >= height)
                return element.block;
        }
        //Logger.DevWarn("Found an empty spot inside of a chunk row. Not even an air block. Returning one. ("+height.ToString()+")");
        return Blocks.AIR;
    }

    public bool PlaceBlockAt(int height, Block block, Chunk chunk)
    {
        if (this.getBlockAt(height) == Blocks.AIR)
        {
            this.blocks.Add(new RowBlocks(height, height, block));
            chunk.AsyncRegen(true);
            return true;
        }
        return false;
    }

    public void RemoveAt(int height, Chunk chunk)
    {
        List<RowBlocks> toRemove = new List<RowBlocks>();
        List<RowBlocks> toAdd = new List<RowBlocks>();
        for(var i=0; i < blocks.Count; i++)
        {
            /*
            if (i >= blocks.Count)
                break;*/
            var element = blocks[i];
            if (element.yMin <= height && element.yMax >= height)
            {
                if (element.yMin == height)
                {
                    element.yMin += 1;
                }
                else
                {
                    if (element.yMax == height)
                    {
                        element.yMax -= 1;
                    }
                    else
                    {
                        //var splitRow = new BlockRow();
                        if ((element.yMax) - (height + 1) >= 0 )
                            toAdd.Add(new RowBlocks(height + 1, element.yMax, element.block));
                        element.yMax = height - 1;
                        //Fuck the air
                        //toAdd.Add(new RowBlocks(height, height, Blocks.AIR));
                        //Shit
                        /*
                        if (element.yMax - element.yMin < 0)
                        {
                            toRemove.Add(element);
                            //Logger.Log("THIS SHOULD FUCKING DO SOMETHING");
                        }*/
                    }
                }
            }
        }
        //stupidest hack ever, why doesnt it work in the above loop
        foreach(var element in blocks)
        {
            if (element.yMax - element.yMin < 0)
            {
                toRemove.Add(element);
                //Logger.Log("THIS SHOULD FUCKING DO SOMETHING");
            }
        }
        foreach(var element in toAdd)
        {
            this.blocks.Add(element);
        }
        foreach(var element in toRemove)
        {
            this.blocks.Remove(element);
        }
        chunk.dirty = true;
        //chunk.AsyncRegen(true);
    }

    public static BlockRow getAt(Vector2 pos)
    {
        if (pos.x != (int)pos.x || pos.y != (int)pos.y)
        {
            Logger.Error("Tried to load a chunk at an invalid position! (" + pos.ToString() + ")");
            return null;
        }
        var chunk = Main.instance.getChunkAt(new Vector3(pos.x,0f,pos.y), false);
        if (chunk != null && chunk.IsGenerated())
        {
            var row2 = chunk.getRowAt((int)(pos.x - (chunk.position.x * 16)), (int)(pos.y - (chunk.position.y * 16)));
            return row2;
        }
        var row = new BlockRow();
        /*
        var x = i - (Mathf.Floor(i / 16) * 16) + id.x * 16;
        var z = Mathf.Floor(i / 16) + id.y * 16;*/
        var y = 60 + (int)(Perlin.Noise(Main.instance.perlinX + pos.x * 0.1f, Main.instance.perlinY + pos.y * 0.1f) * 3);
        var stoneHeight = Mathf.FloorToInt(y * 0.85f);
        row.blocks.Add(new RowBlocks(0, stoneHeight-1, Blocks.STONE));
        row.blocks.Add(new RowBlocks(stoneHeight, y-1, Blocks.DIRT));
        row.blocks.Add(new RowBlocks(y, y, Blocks.GRASS));
        //blocks of air... that's fucking stupid. Just handle null references properly.
        //row.blocks.Add(new RowBlocks(y + 1, 256, Blocks.AIR));
        return row;
    }

    public RowBlocks getTopMost()
    {
        RowBlocks topMost = null;
        foreach(var element in blocks)
        {
            if (topMost == null && element.block != Blocks.AIR)
                topMost = element;
            else
            {
                if (element.yMax > topMost.yMax && element.block != Blocks.AIR)
                    topMost = element;
            }
        }
        return topMost;
    }
}
