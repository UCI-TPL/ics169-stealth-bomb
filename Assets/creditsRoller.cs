using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class creditsRoller : MonoBehaviour
{
    // a list credit text
    // created by; special thanks; titles; thirdparty
    public GameObject[] credits;

    private bool rolling;
    private int creditCounter;
    private int numOfCredit;

    private GameObject nextCredit;
    private float startY;
    private float endY;
    private RectTransform rt;

    public float rollSpeed;
    private float rollTimer;

    private void Start()
    {
        rolling = false;
        creditCounter = 0;
        numOfCredit = credits.Length;

    }


    private void Update()
    {
        if (creditCounter != numOfCredit)
        {
            print(creditCounter);
        }

        if (!rolling && creditCounter != numOfCredit)
        {
            // print(creditCounter);
            nextCredit = credits[creditCounter];
            nextCredit.SetActive(true);
            creditCounter++;

            rt = nextCredit.GetComponent<RectTransform>();
            startY = -(Screen.height)/2f;
            endY = -(startY*2.5f) * (rt.localScale.y);

            rollTimer = endY/rollSpeed;
            rolling = true;
            
            Vector3 initialPos = new Vector3(rt.transform.position.x, startY, rt.transform.position.z);
            rt.transform.position = initialPos;

            rollUpCall();
        }
        else
        {
            if (!nextCredit.activeSelf)
            {
                rolling = false;
            }
        }
        // if there is next credit AND rolling flag is OFF
        //     set rolling flag ON
        //     set the next credit initial position
        //     roll the next credit
    }

    
    public void rollUpCall()
    {
        StartCoroutine(rollUp());
    }

    private IEnumerator rollUp()
    {
        float origY = rt.transform.position.y;
        for (float t=0; t<rollTimer; t+=Time.deltaTime)
        {
            float newY = Mathf.Lerp(origY, endY, Mathf.Min(1, t/rollTimer));
            rt.transform.position = new Vector3(rt.transform.position.x, newY, rt.transform.position.x);
            yield return null;
        } 
        nextCredit.SetActive(false);
    }
}
