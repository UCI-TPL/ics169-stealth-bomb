using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostController : PlayerController {   //this inherits from PlayerController so the movement remains the same 
   
    public GameObject Point; //the cursor that the player moves around

    public LineRenderer lr; //to connect the cursor to the ghost visually 

    public PositionMarker Target;
    public GameObject AOE; //change the color of this

    public GameObject GhostPrefab; //the ghost that hangs out on the curve on the side of the map

    [HideInInspector]
    public GameObject GhostBody;

    [Tooltip("The bomb the player throws to crumble tiles")]
    public GameObject GhostBombPrefab;


    public ParabolaController GhostParabola1; //maps can have either 1 or 2 parabolas    
  
    public ParabolaController GhostParabola2;

    Vector3 startPosition; //used for lerping
    Vector3 endPosition;

    private bool switchSides = true; //lerp if true

    public bool leftSide;

    Vector3 latestPosition; //the position from the previous update, where ghost is lerping from

    private float travelTime = 0.3f; //how long it takes to lerp all across

    private Vector3 targetPosition;
    private float startTravelTime; //used to see how much travel has gone on 

    public float cooldown = 0.1f;

    private Vector3 smoothVel; //used by smoothDamp don't worry about it 

    private float lastGroundDistance = 4f; //the previously height of the crosshair  

    private void Start()
    {
       
        smoothVel = Vector3.zero; //stuff for movement 
        forward = Camera.main.transform.forward;
        forward.Scale(new Vector3(1, 0, 1));
        forward.Normalize();
        right = Camera.main.transform.right;
        right.Scale(new Vector3(1, 0, 1));
        right.Normalize();

        Target.color = playerColor;
        AOE.GetComponent<Renderer>().material.SetColor("Color_52FADAA", playerColor * 2f);
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 1;
        lr.SetPosition(0, transform.position);
        Renderer rend = GetComponent<Renderer>();
        lr.startColor = playerColor;
        lr.endColor = playerColor;

        leftSide = transform.position.z >= transform.position.x; //true for left, false for right   (could work for up & down as well)

        Quaternion rotation = (transform.position.z >= transform.position.x) ? Quaternion.Euler(0f, 135, 0f) : Quaternion.Euler(0f, 315f, 0f);

        GhostBodyInstantiate();
        transform.position = new Vector3(transform.position.x, 10f, transform.position.z);

        GameObject Curves = GameObject.Find("GhostCurves");


        GhostParabola1 = Curves.transform.Find("Curve 1").gameObject.GetComponent<ParabolaController>();
        GhostParabola2 = Curves.transform.Find("Curve 2").gameObject.GetComponent<ParabolaController>();

        input.controllers[player.playerNumber].attack.OnDown.AddListener(Activate);


        StartCoroutine("SpawnCursor");

        startTravelTime = Time.time; //this is included so when the ghost spawns, it spawns where thep player died and then floats to its proper spot real quick
        latestPosition = GhostBody.transform.position;
        travelTime = travelTime * 2f;


    }

    

    public void GhostBodyInstantiate()
    {
        Quaternion rotation = (transform.position.z >= transform.position.x) ? Quaternion.Euler(0f, 135, 0f) : Quaternion.Euler(0f, 315f, 0f);
        GhostBody = Instantiate(GhostPrefab, transform.position, rotation);
        targetPosition = transform.position;

        lastPosition = GhostBody.transform.position;
        rend.material.SetColor("_Color", playerColor);
        //GhostBody.GetComponentsInChildren<Renderer>()[1].material.color = playerColor;
        Renderer[] ghostsRen = GhostBody.GetComponentsInChildren<Renderer>();
        foreach( Renderer ren in ghostsRen)
        {
            ren.material.SetColor("Color_998F7755", playerColor);
        }
        //GhostBody.GetComponentsInChildren<Renderer>()[1].material.SetColor("Color_998F7755", playerColor);
    }

    public IEnumerator SpawnCursor() //so the ghost player can spawn and fly away a bit before the image
    {
        yield return new WaitForSeconds(1f);
        travelTime = travelTime / 2f;
    }

    public IEnumerator  PositionMarkerExpand(float time) //time is half of the bomb travel time. It will expand for one half, and then retract for a second half
    {
        yield return new WaitForSeconds(time);
        float startTime = Time.time;
        float expandTime = Time.time + time;
        float maxSize = 2f;
        while(expandTime >= Time.time)
        {
            float ratio = (Time.time - startTime) / (time);
            float size = Mathf.Lerp(1f,maxSize,ratio); //the grows to 2 over time as the bomb progresses
            Target.SetSize(size);
            yield return null;
        }
        float shrinkTime = Time.time + time;
        startTime = Time.time;
        while (shrinkTime >= Time.time)
        {
            float ratio = (Time.time - startTime) / (time);
            float size = Mathf.Lerp(maxSize, 1f, ratio); //the grows to 2 over time as the bomb progresses
            Target.SetSize(size);
            yield return null;
        }
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
                float distance = this.transform.position.y - hit.transform.position.y;
                GhostBomb ghostBomb = Instantiate(GhostBombPrefab, GhostBody.transform.position, GhostBody.transform.rotation).GetComponent<GhostBomb>();
                ghostBomb.v2 = this.transform.position;
                ghostBomb.v3 = new Vector3(this.transform.position.x, this.transform.position.y - distance, this.transform.position.z);
                StartCoroutine("PositionMarkerExpand",(ghostBomb.travelTime/2)); 
            }
            
        }
    }

    private void FixedUpdate()
    {
        Move(player.stats.moveSpeed * 1f);
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

        pos = currentPosition;
       
        if (!switchSides) //this is the default case, business as usual no switching
        {
            pos = currentPosition;
            GhostBody.transform.rotation = rotation;
            latestPosition = GhostBody.transform.position;
        }
        else //if sides have switches then continue the lerp 
        {
            float lerpPosition = (Time.time - startTravelTime) / travelTime; //how far along the lerp should it be
            //pos = Vector3.Lerp(latestPosition, currentPosition, lerpPosition);
            //pos = Vector3.SmoothDamp(latestPosition, currentPosition, ref smoothVel, 10f);
        }

        if (pos != Vector3.zero)
            targetPosition = pos;
            //GhostBody.transform.position = pos;
    }

    private Vector3 currVelocity;

    private void LateUpdate() {
        UpdateLine();
        transform.forward = new Vector3(1, 0, 1);
        GhostBody.transform.position = Vector3.SmoothDamp(GhostBody.transform.position, targetPosition, ref currVelocity, travelTime);
    }


    public void UpdateLine() //this updates the Line Rendeer between the Ghost and the AOE Cursor thing
    {
        
        int layerMask = 1 << 11; //this makes sure that it can only detect the Ground layer
        lr.positionCount = 3;
        RaycastHit hit;
        if(Physics.Raycast(Point.transform.position, Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
            lastGroundDistance = transform.position.y - hit.transform.position.y;
            DrawLine(lastGroundDistance);
        }
        else
            DrawLine(lastGroundDistance);
    }

    int vertexCount = 12;
    public void DrawLine(float descent)
    {
        List<Vector3> linePoints = new List<Vector3>();
        Vector3 v1 = GhostBody.transform.position; //this is the start point, the ghost body
        Vector3 v2 = this.transform.position; //this is the top of the decal
        float distance = Vector3.Distance(v1, v2);
        Vector3 v3 = new Vector3(v2.x, v2.y - descent + 0.5f, v2.z); //this point where the crosshair is, the ground

        Vector4 v4 = new Vector3(0f, 0f, 0f);
        v4 = v2 + (v1.normalized * (distance/3));
        
        for(float ratio = 0; ratio <= 1; ratio += (1f/(vertexCount )))
        {
            Vector3 tangent1 = Vector3.Lerp(v1, v4, ratio); //this is the line between the ghost body & the top of the decal object
            Vector3 tangent2 = Vector3.Lerp(v4, v3, ratio); //the line between the top of the decal object & the ground
            Vector3 point = Vector3.Lerp(tangent1, tangent2, ratio); //a point on the curve that we want to make
            linePoints.Add(point);
        }

        //Debug.Log("Made it out and the way to go is  " + linePoints);
        lr.positionCount = linePoints.Count;
        linePoints.Reverse();
        lr.SetPositions(linePoints.ToArray());
    }



}
