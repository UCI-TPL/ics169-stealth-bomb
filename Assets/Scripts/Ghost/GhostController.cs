﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostController : PlayerController {   //this inherits from PlayerController so the movement remains the same 


    Image CursorImage;
    public GameObject Point; //the cursor that the player moves around

    public GameObject GhostPrefab; //the ghost that hangs out on the curve on the side of the map

    [HideInInspector]
    public GameObject GhostBody;

    [HideInInspector]
    public ParabolaController GhostParabola1; //maps can have either 1 or 2 parabolas    
    [HideInInspector]
    public ParabolaController GhostParabola2;
    [HideInInspector]


    public float cooldown = 0.1f;

    private void Start()
    {   
        forward = Camera.main.transform.forward;
        forward.Scale(new Vector3(1, 0, 1));
        forward.Normalize();
        right = Camera.main.transform.right;
        right.Scale(new Vector3(1, 0, 1));
        right.Normalize();
        CursorImage = GetComponent<Image>(); //used to change to player color
        CursorImage.color = playerColor;

        GhostBody = Instantiate(GhostPrefab, transform.position, transform.rotation);
        GhostBody.GetComponentsInChildren<Renderer>()[1].material.color = playerColor;


        GameObject Curves = GameObject.FindGameObjectWithTag("ghost-curve");

        GhostParabola1 = Curves.transform.Find("Curve 1").gameObject.GetComponent<ParabolaController>();
        GhostParabola2 = Curves.transform.Find("Curve 2").gameObject.GetComponent<ParabolaController>();

        input.controllers[player.playerNumber].attack.OnDown.AddListener(Activate);

    }

    private float destoryTileTime = 0.0f;

    public void Activate()
    {
        if (destoryTileTime <= Time.time)
        {
            destoryTileTime = Time.time + cooldown;
            int layerMask = 1 << 11; //this makes sure that it can only detect the Ground layer
            RaycastHit hit;
            if (Physics.Raycast(Point.transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask))
            {
                Tile temp = hit.transform.GetComponent<Tile>();
                if (temp)
                    TileManager.tileManager.DestroyTiles(temp.position);
            }
        }
    }

    private void FixedUpdate()
    {
        Move(player.stats.moveSpeed);

        Vector3 pos;

        if (transform.position.z >= transform.position.x) //depending on the position, go to either the first or second parabola and rotate accordingly
        {
            pos = GhostParabola1.UpdatePosition((transform.position.z + transform.position.x) / 2) - GameManager.instance.GhostOffset;
            GhostBody.transform.rotation = Quaternion.Euler(0f, 135, 0f);
        }
        else
        {
            pos = GhostParabola2.UpdatePosition((transform.position.z + transform.position.x) / 2) + GameManager.instance.GhostOffset;
            GhostBody.transform.rotation = Quaternion.Euler(0f, 315, 0f);
        }
        
        if (pos != Vector3.zero)
            GhostBody.transform.position = pos;
       
    }




}