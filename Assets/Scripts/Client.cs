using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Client
{
    public Block currentBlock = Blocks.LOG;
    public PlayerEntity localPlayer;
    public Input input = new Input();
    public bool targeting = false;
    public Vector3 targetBlock = Vector3.zero;
    public Vector3 targetBlockNormal = Vector3.up;
    DepthOfField dofEffect;
    
    public void Start()
    {
        dofEffect = Main.instance.fxProfile.GetSetting<DepthOfField>();
    }
    // Update is called once per frame
    public void Update()
    {
        input.Update();
        targeting = false;
        RaycastHit hit;
        if (Physics.Raycast(localPlayer.getEyePos(), localPlayer.getEyeRotation() * Vector3.forward, out hit, Mathf.Infinity, 1 << 8))
        {
            targeting = true;
            Vector3 newNorm = new Vector3(Mathf.Round(hit.normal.x), Mathf.Round(hit.normal.y), Mathf.Round(hit.normal.z));
            targetBlockNormal = newNorm;
            //Hack? Push it a lil bit inside to avoid spazzing out because of inaccuracy
            var blockPos = hit.point - newNorm * 0.01f;
            blockPos = new Vector3(Mathf.Floor(blockPos.x), Mathf.Ceil(blockPos.y), Mathf.Floor(blockPos.z));
            /*
            if (newNorm != Vector3.up && newNorm != -Vector3.right && newNorm != -Vector3.forward)
                blockPos -= hit.normal;*/
            var top = Main.instance.GetRowAt(blockPos, false).getTopMost();
            /*
            if (top.yMax <= blockPos.y)
                blockPos.y = top.yMax;*/
            targetBlock = blockPos;
            dofEffect.active = true;
            dofEffect.focusDistance.value = Mathf.Lerp(dofEffect.focusDistance.value,Vector3.Distance(localPlayer.getEyePos(), hit.point),10f*Time.deltaTime);
        }
        else
        {
            dofEffect.active = false;
        }
        
    }

    public void breakBlock()
    {

    }

    public void FixedUpdate()
    {
        input.FixedUpdate();
    }
}
