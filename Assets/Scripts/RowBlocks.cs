using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowBlocks
{
    public int yMin = 0;
    public int yMax = 1;
    public Block block;
    public RowBlocks(int yMin, int yMax, Block block)
    {
        this.yMin = yMin;
        this.yMax = yMax;
        this.block = block;
    }
}
