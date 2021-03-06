﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{


    private static PauseMenuUI instance;

    private bool paused = false;
    private int playerWhoPaused = -1; //only the player who paused can unpause
 

    public static PauseMenuUI Instance
    {
        get
        {
            if (instance != null)
                return instance;
            instance = FindObjectOfType<PauseMenuUI>();
            if (instance == null)
                Debug.LogError("PauseMenu not found");
            return instance;
        }
    }

    private Canvas canvas;

    [SerializeField]
    private RectTransform PauseMenuRect;

    [SerializeField]
    private ButtonController GoMenuButton;

    void Awake()
    {
        instance = this;
        canvas = GetComponentInParent<Canvas>();
    }

    public void Pause(int p_number)
    {

        if(GameManager.instance.CanPause()) //checks to see if we are in the game or not
        {
            if (paused == false)
            {
                PauseMenuRect.gameObject.SetActive(true);
                playerWhoPaused = p_number;
                Time.timeScale = 0f;
                paused = true;
            }
            else if (p_number == playerWhoPaused) //the player who paused gets to unpause
            {
                paused = false;
                Time.timeScale = 1f;
                playerWhoPaused = -1;
                PauseMenuRect.gameObject.SetActive(false);
            }
        }

    }

    public void Disable()
    {
        PauseMenuRect.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void PressGoMenuButton(int p_number) {
        if (PauseMenuRect.gameObject.activeSelf == true && GoMenuButton.gameObject.activeSelf == true && p_number == playerWhoPaused) {
            GoMenuButton.PressButton();
        }
    }

    public void ReleaseGoMenuButton(int p_number) {
        if (PauseMenuRect.gameObject.activeSelf == true && GoMenuButton.gameObject.activeSelf == true && p_number == playerWhoPaused) {
            GoMenuButton.ReleaseButton();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (paused == true) {
            _ButtonSelect();
        }
    }

    private void _ButtonSelect() {
        if (GoMenuButton.IsButtonInNormalState())
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(GoMenuButton.gameObject);
    }
}
