using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using XInputDotNetPure;

public class AudioMenuManager : MonoBehaviour
{
    public MainMenuManager mainMenuManager;
    public float sliderStepSize = 0.5f;
    public string selectedButtonSound = "Bow";
    public Selectable[] AudioMenuInteractables;
    
    private bool audioPanelActive;

    GamePadState[] currentStates;
	GamePadState[] prevStates;
	PlayerIndex[] players;
	private int currentAudioMenuInteractable;
	private int prevAudioMenuInteractable;

    private float timer;
    private float buttonTimer;

    private bool hasMoved;

    private Selectable selectable;

    private InputManager input;
    // Start is called before the first frame update
    void Start()
    {
        input = InputManager.inputManager;
        audioPanelActive = false;
        currentStates = new GamePadState[4];
		prevStates = new GamePadState[4];
		players = new PlayerIndex[4];
        ResetAudioPanel();
        timer = mainMenuManager.cooldown;
        hasMoved = false;
        buttonTimer = 0.0f;

		for (int i = 0; i < 4; i++) {
			players[i] = (PlayerIndex) i;
            AssignControllerEvents(i);
		}
    }

    public void ResetAudioPanel() {
        currentAudioMenuInteractable = 0;
        prevAudioMenuInteractable = currentAudioMenuInteractable;
        // audioMenuButtons(currentAudioMenuInteractable);
    }

    // Update is called once per frame
    void Update()
    {
        audioPanelActive = (mainMenuManager.getCurrentPanel() == 5);
        if (audioPanelActive) {
            prevAudioMenuInteractable = currentAudioMenuInteractable;

            for (int i = 0; i < players.Length; i++) {
                prevStates[i] = currentStates[i];
                currentStates[i] = GamePad.GetState(players[i]);

                if (Input.GetAxis("Mouse X") == 0 || Input.GetAxis("Mouse Y") == 0) {
                    if (timer >= mainMenuManager.cooldown) {
                        // Debug.Log("Test 2");
                        hasMoved = false;
                        // makes sure controller is connected and there is no other input since cooldown finished
                        if (currentStates[i].IsConnected && !hasMoved) {
                                // Debug.Log("Test 3");
                                // Debug.Log("Left Thumbstick input = " + currentStates[i].ThumbSticks.Left.Y);
                            if (currentStates[i].ThumbSticks.Left.Y > mainMenuManager.controllerStickDeadZone /*&& prevState.ThumbSticks.Left.Y <= 0.0f*/) {
                                currentAudioMenuInteractable--;
                                CheckAndMoveCursorToBottom();
                                // CheckForAndSkipDeactivatedButtons(true);
                                // audioMenuButtons(currentAudioMenuInteractable);
                                // Debug.Log("cursor moved up to " + currentAudioMenuInteractable);
                                hasMoved = true;
                            }
                            else if (currentStates[i].ThumbSticks.Left.Y < -mainMenuManager.controllerStickDeadZone /*&& prevState.ThumbSticks.Left.Y >= 0.0f*/) {
                                currentAudioMenuInteractable++;
                                CheckAndMoveCursorToTop();
                                // CheckForAndSkipDeactivatedButtons(false);
                                // audioMenuButtons(currentAudioMenuInteractable);
                                // Debug.Log("cursor moved down to " + currentAudioMenuInteractable);
                                hasMoved = true;
                            }

                            audioMenuButtons(currentAudioMenuInteractable);

                            if (currentAudioMenuInteractable < AudioMenuInteractables.Length - 1) {
                                if (currentStates[i].ThumbSticks.Left.X > mainMenuManager.controllerStickDeadZone) {
                                    ((Slider) AudioMenuInteractables[currentAudioMenuInteractable]).value += sliderStepSize;
                                }
                                else if (currentStates[i].ThumbSticks.Left.X < -mainMenuManager.controllerStickDeadZone) {
                                    ((Slider) AudioMenuInteractables[currentAudioMenuInteractable]).value -= sliderStepSize;
                                }
                            }

                            // if (AudioPanel.activeSelf == true)

                            if (hasMoved) 
                            {
                                timer = 0.0f;
                                timer += 1.0f * Time.deltaTime;
                            }
                        }
                    }

                    // NOTE: this button may have to change later. Most likely will conflict with PlayerJoinManager.cs controls!!
                    // getCurrentPanel() (or menu) needs to be <= 1, indicating that we are currently in the main menu, or we pressed quit while in the Unity editor.
                    // if (currentStates[i].Buttons.A == ButtonState.Pressed /* && prevState.Buttons.A == ButtonState.Released */ && getCurrentPanel() == 1 && btn.transform.GetSiblingIndex() == currentAudioMenuInteractable) {
                    // 	// Debug.Log("clicking");
                    // 	// b.onClick.Invoke();
                    // 	ActivateButton();
                    // }
                    // Debug.Log("current main menu button: " + currentAudioMenuInteractable);
                }
			}
		}

		if (hasMoved && timer < mainMenuManager.cooldown) 
			timer += 1.0f * Time.deltaTime;

        if (audioPanelActive) {
			buttonTimer += 1.0f * Time.deltaTime;
		}
		else {
			buttonTimer = 0.0f;
		}
    }

    // helper method to check if the controller cursor/highlighter has gone too high in the buttons index and needs to be needs moved to the bottom of the menu.
	private void CheckAndMoveCursorToBottom() {
		if (currentAudioMenuInteractable < 0) {
			currentAudioMenuInteractable = AudioMenuInteractables.Length - 1;
		}
	}

	// helper method to check if the controller cursor/highlighter has gone too low in the buttons index and needs to be moved to the top of the menu.
	private void CheckAndMoveCursorToTop() {
		if (currentAudioMenuInteractable >= AudioMenuInteractables.Length) {
			currentAudioMenuInteractable = 0;
		}
	}

    private void ActivateButton(int controllerIdx) {
        if (PlayerCanPressButton(controllerIdx)) {
            ((ButtonController) AudioMenuInteractables[currentAudioMenuInteractable]).onClick.Invoke();
        }
    }

    private bool PlayerCanPressButton(int controllerIdx) {
        return currentStates[controllerIdx].IsConnected && audioPanelActive && buttonTimer >= mainMenuManager.buttonCoolDown && currentAudioMenuInteractable >= 3;
    }

    // helper method that assigns what functions should be called by what input/event.
	private void AssignControllerEvents(int controllerIdx) {
		// A button, B button, and Start button has been assigned. Still need to assign Y button.
		switch (controllerIdx) {
			case 0:
				input.controllers[controllerIdx].confirm.OnDown.AddListener( () => ActivateButton(0) );
				break;
			case 1:
				input.controllers[controllerIdx].confirm.OnDown.AddListener( () => ActivateButton(1) );
				break;
			case 2:
				input.controllers[controllerIdx].confirm.OnDown.AddListener( () => ActivateButton(2) );
				break;
			case 3:
				input.controllers[controllerIdx].confirm.OnDown.AddListener( () => ActivateButton(3) );
				break;
			default:
				Debug.Log("variable playerIdx out of range!");
				break;
		}
	}

    private void audioMenuButtons( int i )
	{
		// btn = mainMenuPanel.transform.GetChild(i).gameObject;
		// if (prevAudioMenuInteractable != currentAudioMenuInteractable)
		// 	Debug.Log("audio menu interactable selected: " + btn.name);
		_buttonSelect(i);
	}

    private void _buttonSelect(int i)
	{
		selectable = AudioMenuInteractables[i];
        if (selectedButtonSound != null && selectedButtonSound != "")
            GameManager.instance.audioManager.Play(selectedButtonSound);
		selectable.Select();
	}
}
