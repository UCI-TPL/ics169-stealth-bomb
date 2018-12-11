using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostController : PlayerController {   //this inherits from PlayerController so the movement remains the same 


    Image CursorImage;

    public GameObject GhostPrefab; //the ghost that hangs out on the curve on the side of the map

    [HideInInspector]
    public GameObject GhostBody;

    public ParabolaController ParabolaController;
    public GameObject ParabolaRoot;

    public GameObject Point;

    public float cooldown = 0.1f;

  //  Camera camera = Ge

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
        //rend.material.color = playerColor; //setting the player color based on playeNum 
        lastPosition = transform.position;
        //Vector3 ghostLocation = new Vector3(transform.position.x - 13f, transform.position.y, transform.position.z + 6f);
        Vector3 ghostLocation = new Vector3(1f, 8f, 20f);

        GhostBody = Instantiate(GhostPrefab, ghostLocation, transform.rotation);
        GhostBody.GetComponentsInChildren<Renderer>()[1].material.color = playerColor;
        GhostBody.GetComponent<ParabolaController>().Begin(GameObject.FindGameObjectWithTag("ghost-curve").gameObject); //this is like a  constructor used to attach the parabola root (which is pre-existing in the map)
        GhostBody.GetComponent<ParabolaController>().ParabolaRoot = GameObject.FindGameObjectWithTag("ghost-curve"); //The GhostBody is created and placed upon the paraboloa

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
        GhostBody.GetComponent<ParabolaController>().UpdatePosition((transform.position.z + transform.position.x)/2);
    }




}
