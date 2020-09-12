using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Input
{
    public Input()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    public void Update()
    {
        Main.client.localPlayer.yaw += UnityEngine.Input.GetAxisRaw("Mouse X") * 6f;
        Main.client.localPlayer.pitch -= UnityEngine.Input.GetAxisRaw("Mouse Y") * 6f;
        if (UnityEngine.Input.GetButton("Sprint"))
            Main.client.localPlayer.sprint = true;
        else
            Main.client.localPlayer.sprint = false;
        if (UnityEngine.Input.GetButtonDown("Jump"))
            Main.client.localPlayer.jump = true;
            //Main.client.localPlayer.rigidBody.velocity += Vector3.up * 5f;
        if (UnityEngine.Input.GetButtonDown("Fire1"))
            Main.instance.RemoveBlockAt(Main.client.targetBlock, false);
        if (UnityEngine.Input.GetButtonDown("Grenade"))
        {
            var gren = GameObject.Instantiate(Main.instance.grenadePrefab,Main.client.localPlayer.transform.position + 0.6f * Vector3.up, Main.client.localPlayer.getEyeRotation());
            gren.GetComponent<LiveGrenade>().Throw(Main.client.localPlayer.getEyeRotation() * Vector3.forward * 15f + Main.client.localPlayer.rigidBody.velocity);
            
        }
        if (UnityEngine.Input.GetButton("Sneak"))
            Main.client.localPlayer.sneaking = true;
        else
            Main.client.localPlayer.sneaking = false;
        if (UnityEngine.Input.GetButtonDown("Fire2"))
        {
            var pos = Main.client.targetBlock + Main.client.targetBlockNormal;
            var ppos = new Vector3(Mathf.Floor(Main.client.localPlayer.transform.position.x), Mathf.Floor(Main.client.localPlayer.transform.position.y) + 1, Mathf.Floor(Main.client.localPlayer.transform.position.z));
            //Debug.Log(pos.ToString() + " != " + ppos.ToString());
            if (ppos != pos)
                Main.instance.PlaceBlockAt(pos, Main.client.currentBlock, false);
        }

        if (UnityEngine.Input.GetButtonDown("Fire3"))
            Main.client.currentBlock = Main.instance.GetBlockAtAbsolute(Main.client.targetBlock, false);
            //Main.instance.PlaceBlockAt(Main.client.targetBlock + Main.client.targetBlockNormal, Blocks.GRASS, false);
    }

    public void FixedUpdate()
    {
        Main.client.localPlayer.movementAxis = UnityEngine.Input.GetAxisRaw("Vertical") * (Main.client.localPlayer.getRotation() * Vector3.forward) + UnityEngine.Input.GetAxisRaw("Horizontal") * (Main.client.localPlayer.getRotation() * Vector3.right);
        //Main.client.localPlayer.rigidBody.velocity += UnityEngine.Input.GetAxisRaw("Vertical") * (Main.client.localPlayer.getRotation() * Vector3.forward) * 10f * Time.deltaTime + UnityEngine.Input.GetAxisRaw("Horizontal") * (Main.client.localPlayer.getRotation() * Vector3.right) * 10f * Time.deltaTime;

    }
}
