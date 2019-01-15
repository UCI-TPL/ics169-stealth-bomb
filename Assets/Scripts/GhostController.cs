using System.Collections;
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
    public GameObject GhostParabola1; //maps can have either 1 or 2 parabolas    
    [HideInInspector]
    public GameObject GhostParabola2;
    [HideInInspector]
    public ParabolaController GhostParCont1;  //these are variable so there is not need to use GetComponent or Find multiple times 
    [HideInInspector]
    public ParabolaController GhostParCont2;

    bool MultipleParabolas = false;

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



        //GhostParabola1 = new ParabolaController();
        //GhostParabola2 = new ParabolaController();
        //GameObject[] curves = GameObject.FindGameObjectsWithTag("ghost-curve");

        GameObject Curves = GameObject.FindGameObjectWithTag("ghost-curve");

        GhostParabola1 = Curves.transform.Find("Curve 1").gameObject; //this looks kinda complex but I heard transform.Find is the best way to find children
        GhostParCont1 = GhostParabola1.GetComponent<ParabolaController>();//and keeping all the curves under one parent seemed better

        GhostParabola2 = Curves.transform.Find("Curve 2").gameObject;
        GhostParCont2 = GhostParabola2.GetComponent<ParabolaController>();


        int max = GameManager.instance.GetComponentInChildren<CreateRandomTerrain>().GhostMax;
        int min = GameManager.instance.GetComponentInChildren<CreateRandomTerrain>().GhostMin;


        GhostParabola1.GetComponent<ParabolaController>().Begin(GhostParabola1, max, min);
        GhostParabola2.GetComponent<ParabolaController>().Begin(GhostParabola2, max, min);
        /* no need to check, just make sure each map has two curves somewhere I guess
        if (curves.Length > 1) //start the game with 3 Parabolas if 2 are found, otherwsie start with just one
        {
            MultipleParabolas = true;
            GhostParabola1 = curves[0].GetComponent<ParabolaController>();
            GhostParabola2 = curves[1].GetComponent<ParabolaController>();
            
            GhostParabola1.Begin(curves[0].gameObject,max,min);
            GhostParabola2.Begin(curves[1].gameObject,max,min);
        }
        else
        {
            MultipleParabolas = false;
            GhostParabola1 = curves[0].GetComponent<ParabolaController>();
            GhostParabola1.Begin(curves[0],max,min);
        }
        */

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

        /*
        if(MultipleParabolas != true) //if there is only one Parabola, be go to the first one and face right
        {
            pos = GhostParabola1.UpdatePosition((transform.position.z + transform.position.x) / 2) + GameManager.instance.GhostOffset;
            GhostBody.transform.rotation = Quaternion.Euler(0f, 135f, 0f);
            GhostBody.transform.position = pos;
            return;
        }
        */
        if (transform.position.z >= transform.position.x) //depending on the position, go to either the first or second parabola and rotate accordingly
        {
            pos = GhostParCont1.UpdatePosition((transform.position.z + transform.position.x) / 2) - GameManager.instance.GhostOffset;
            GhostBody.transform.rotation = Quaternion.Euler(0f, 135, 0f);
        }
        else
        {
            pos = GhostParCont2.UpdatePosition((transform.position.z + transform.position.x) / 2) + GameManager.instance.GhostOffset;
            GhostBody.transform.rotation = Quaternion.Euler(0f, 315, 0f);
        }
        
        if (pos != Vector3.zero)
            GhostBody.transform.position = pos;
       
    }




}
