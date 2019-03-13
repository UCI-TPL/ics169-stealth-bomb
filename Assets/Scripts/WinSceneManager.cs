using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class WinSceneManager : MonoBehaviour
{
    public Text winText;

    GamePadState[] currentStates;
	GamePadState[] prevStates;
	PlayerIndex[] players;

    private InputManager input;

    [SerializeField]
    private ButtonController b;

    // Start is called before the first frame update
    void Start()
    {
        input = InputManager.inputManager;
        currentStates = new GamePadState[4];
		prevStates = new GamePadState[4];
		players = new PlayerIndex[4];

        for (int i = 0; i < currentStates.Length; i++) {
            players[i] = (PlayerIndex) i;
            AssignControllerEvents(i);
        }

        string winners = "";
        foreach (Player p in GameManager.instance.Winners) {
            // winText.color = p.Color;
            winners += "<color=#" + ColorUtility.ToHtmlStringRGBA(p.Color) + ">Player " + (p.playerNumber + 1) + " Wins!</color>\n";
        }
        // ColorUtility.
        winText.text = winners;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateControllersAndButtons();
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
        return currentStates[controllerIdx].IsConnected;
    }

    // helper method that assigns what functions should be called by what input/event.
	private void AssignControllerEvents(int controllerIdx) {
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
        if (b.IsButtonInNormalState())
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(b.gameObject);
    }
}
