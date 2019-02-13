using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostController : PlayerController {   //this inherits from PlayerController so the movement remains the same 



    public GameObject Cursor;

    //_AlbedoColor

    [SerializeField]
    Image CursorImage;
    public GameObject Point; //the cursor that the player moves around

    public GameObject GhostPrefab; //the ghost that hangs out on the curve on the side of the map

    [HideInInspector]
    public GameObject GhostBody;


    public ParabolaController GhostParabola1; //maps can have either 1 or 2 parabolas    
  
    public ParabolaController GhostParabola2;

    Vector3 startPosition; //used for lerping
    Vector3 endPosition;

    private bool switchSides = true; //lerp if true

    public bool leftSide;

    Vector3 latestPosition; //the position from the previous update, where ghost is lerping from

    private float travelTime = 0.3f; //how long it takes to lerp all across

  
    private float startTravelTime; //used to see how much travel has gone on 

    public float cooldown = 0.1f;

    private Vector3 smoothVel; //used by smoothDamp don't worry about it 

    private void Start()
    {
       
        smoothVel = Vector3.zero;
        forward = Camera.main.transform.forward;
        forward.Scale(new Vector3(1, 0, 1));
        forward.Normalize();
        right = Camera.main.transform.right;
        right.Scale(new Vector3(1, 0, 1));
        right.Normalize();
        CursorImage = GetComponent<Image>(); //used to change to player color
        CursorImage.color = playerColor;
        CursorImage.enabled = false;
        //this.gameObject.SetActive(false);
        //this.rend.material.color = Color.clear;
        //Cursor.GetComponent<Renderer>().material.SetColor("_AlbedoColor", playerColor);
        //Cursor.transform.Find("Cube").GetComponent<Renderer>().material.SetColor("Color_52FADAA",playerColor);

        leftSide = transform.position.z >= transform.position.x; //true for left, false for right   (could work for up & down as well)

        Quaternion rotation = (transform.position.z >= transform.position.x) ? Quaternion.Euler(0f, 135, 0f) : Quaternion.Euler(0f, 315f, 0f);
        GhostBody = Instantiate(GhostPrefab, transform.position, rotation);
        
        lastPosition = GhostBody.transform.position;
        //rend.material.SetColor("Color_91A455EE", playerColor);
        GhostBody.GetComponentsInChildren<Renderer>()[1].material.color = playerColor;
        GhostBody.GetComponentsInChildren<Renderer>()[1].material.SetColor("Color_998F7755", playerColor);


        //GameObject Curves = GameObject.FindGameObjectWithTag("ghost-curve");
        GameObject Curves = GameObject.Find("GhostCurves");


        GhostParabola1 = Curves.transform.Find("Curve 1").gameObject.GetComponent<ParabolaController>();
        GhostParabola2 = Curves.transform.Find("Curve 2").gameObject.GetComponent<ParabolaController>();

        input.controllers[player.playerNumber].attack.OnDown.AddListener(Activate);


        StartCoroutine("SpawnCursor");

        startTravelTime = Time.time; //this is included so when the ghost spawns, it spawns where thep player died and then floats to its proper spot real quick
        latestPosition = GhostBody.transform.position;
        travelTime = travelTime * 2f;


    }

    public IEnumerator SpawnCursor() //so the ghost player can spawn and fly away a bit before the image
    {
        yield return new WaitForSeconds(1f);
        CursorImage.enabled = true;
        travelTime = travelTime / 2f;
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

        if (((startTravelTime + travelTime) <= Time.time) && switchSides) //check to see if the side switching has ended
            switchSides = false;

        if (leftSide != (transform.position.z >= transform.position.x)) //if this is true a side has been switched
        {
            leftSide = !leftSide;
            switchSides = true;
            startTravelTime = Time.time;
            latestPosition = GhostBody.transform.position;
        }

        ParabolaController Ghost = leftSide ? GhostParabola1 : GhostParabola2;
        Vector3 offset = leftSide ? GameManager.instance.GhostOffset : -GameManager.instance.GhostOffset;
        Quaternion rotation = (transform.position.z >= transform.position.x) ? Quaternion.Euler(0f, 135, 0f) : Quaternion.Euler(0f, 315f, 0f);
        Vector3 currentPosition = Ghost.UpdatePosition((transform.position.z + transform.position.x) / 2) + offset;
       
        if (!switchSides) //this is the default case, business as usual no switching
        {
            pos = currentPosition;
            GhostBody.transform.rotation = rotation;
            latestPosition = GhostBody.transform.position;
        }
        else //if sides have switches then continue the lerp 
        {
            float lerpPosition = (Time.time - startTravelTime) / travelTime; //how far along the lerp should it be
            pos = Vector3.Lerp(latestPosition, currentPosition, lerpPosition);
            //pos = Vector3.SmoothDamp(latestPosition, currentPosition, ref smoothVel, 10f);
        } 

        if (pos != Vector3.zero)
            GhostBody.transform.position = pos;
        
       
    }




}
