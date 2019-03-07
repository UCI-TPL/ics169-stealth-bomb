using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using XInputDotNetPure;

public class CreditsManager : MonoBehaviour
{
    public MainMenuManager mainMenuManager;
    public GameObject audioManagerPrefab;
    private AudioManager audioManager;

    public GameObject backBtn;
    private ButtonController b;

    private InputManager input;

    private bool creditsPanelActive;

    GamePadState[] currentStates;
	GamePadState[] prevStates;
	PlayerIndex[] players;

    private float timer;
    private float buttonTimer;

    private bool playing;

    void Awake() {
        // b = backBtn.GetComponent<ButtonController>();
        playing = false;
        audioManager = audioManagerPrefab.GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        input = InputManager.inputManager;
        currentStates = new GamePadState[4];
		prevStates = new GamePadState[4];
		players = new PlayerIndex[4];

        timer = mainMenuManager.cooldown;
        buttonTimer = 0.0f;

        for (int i = 0; i < currentStates.Length; i++) {
            players[i] = (PlayerIndex) i;
            AssignControllerEvents(i);
        }

        creditsPanelActive = false;
    }

    // Update is called once per frame
    // barebones xbox controller navigation for now, since there is only 1 button
    void Update()
    {
        creditsPanelActive = (mainMenuManager.getCurrentPanel() == 6);

        if (creditsPanelActive) {
            UpdateControllersAndButtons();

            // put any you want to update while credits panel is open in this bracket
            

            if (!playing)
            {
                //stop all other music/
                GameManager.instance.audioManager.Stop("Main Menu");
                // audioManager.Stop("Main Menu");
                //play credit music
                GameManager.instance.audioManager.Play("Credit");
                playing = true;
            }
        }
        
		else {
			buttonTimer = 0.0f;

            if (playing)
            {
                // stop credit music
                GameManager.instance.audioManager.Stop("Credit");
                // // play mainMenumusic
                GameManager.instance.audioManager.Play("Main Menu");
                playing = false;
            }
        }
    }

    private void UpdateControllersAndButtons() {
        for (int i = 0; i < players.Length; i++) {
            prevStates[i] = currentStates[i];
            currentStates[i] = GamePad.GetState(players[i]);

            // if (Input.GetAxis("Mouse X") == 0 || Input.GetAxis("Mouse Y") == 0) {
            //     if (currentStates[i].IsConnected) {
            //         _ButtonSelect();
            //     }
            // }
            // if (b == null) 
            //     b = backBtn.GetComponent<ButtonController>();
            
            // if (!b.IsButtonInNormalState())
                _ButtonSelect();
        }

        buttonTimer += 1.0f * Time.deltaTime;
    }

    private void PressSelectedMenuOption(int controllerIdx) {
        if (PlayerCanPressButton(controllerIdx)) {
            b.PressButton();
            // b.onClick.Invoke();
        }
    }

    private void ReleaseSelectedMenuOption(int controllerIdx) {
		if (PlayerCanPressButton(controllerIdx))
			b.ReleaseButton();
	}

    private bool PlayerCanPressButton(int controllerIdx) {
        return currentStates[controllerIdx].IsConnected && creditsPanelActive && buttonTimer >= mainMenuManager.buttonCoolDown;
    }

    // helper method that assigns what functions should be called by what input/event.
	private void AssignControllerEvents(int controllerIdx) {
		// A button, B button, and Start button has been assigned. Still need to assign Y button.
		switch (controllerIdx) {
			case 0:
				input.controllers[controllerIdx].confirm.OnDown.AddListener( () => PressSelectedMenuOption(0) );
				input.controllers[controllerIdx].confirm.OnUp.AddListener( () => ReleaseSelectedMenuOption(0) );
				break;
			case 1:
				input.controllers[controllerIdx].confirm.OnDown.AddListener( () => PressSelectedMenuOption(1) );
				input.controllers[controllerIdx].confirm.OnUp.AddListener( () => ReleaseSelectedMenuOption(1) );
				break;
			case 2:
				input.controllers[controllerIdx].confirm.OnDown.AddListener( () => PressSelectedMenuOption(2) );
				input.controllers[controllerIdx].confirm.OnUp.AddListener( () => ReleaseSelectedMenuOption(2) );
				break;
			case 3:
				input.controllers[controllerIdx].confirm.OnDown.AddListener( () => PressSelectedMenuOption(3) );
				input.controllers[controllerIdx].confirm.OnUp.AddListener( () => ReleaseSelectedMenuOption(3) );
				break;
			default:
				Debug.Log("variable playerIdx out of range!");
				break;
		}
	}

    private void _ButtonSelect() {
        b = backBtn.GetComponent<ButtonController>();
        if (b.IsButtonInNormalState())
            EventSystem.current.SetSelectedGameObject(backBtn);
            // b.Select();
    }
}
