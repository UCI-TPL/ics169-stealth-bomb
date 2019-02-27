using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonController : Button, ISelectHandler, IDeselectHandler
{
    public float selectedScale = 1.2f;

    BaseEventData m_BaseEvent;

    private bool buttonPressed = false;

    private bool playSelectedButtonSFX = true;

    [Tooltip("Determines whether or not the button will play the pressedButtonSound.")]
    [SerializeField]
    private bool playPressedButtonSFX;

    [Tooltip("The name of the sound in AudioManager that you want to play when the button is selected.")]
    public string selectedButtonSound = "Bow";

    [Tooltip("The name of the sound in AudioManager that you want to play when the button is pressed and activated.")]
    public string pressedButtonSound = "Bow";

    // [SerializeField]
    // private Button b;

    // void Awake() {
    //     b = this.gameObject.GetComponent<Button>();
    // }

    // // Start is called before the first frame update
    // void Start()
    // {
    //     if (pressedSound != null) {
    //         b.onClick.AddListener(() => GameManager.instance.audioManager.Play("Bow"));
    //     }
    //     // b.DoStateTransition(SelectionState.Pressed, true);
    // }

    // Update is called once per frame
    void Update()
    {
        if (base.IsHighlighted(m_BaseEvent) || base.IsPressed()) {
            // Debug.Log(this.gameObject.name + " transition state = " + base.currentSelectionState);
            this.transform.localScale = new Vector3(selectedScale, selectedScale, this.transform.localScale.z);

            // if (base.IsPressed()) buttonPressed = true;
            // else buttonPressed = false;
        }
        else {
            this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    public void EnlargeButton() {
        this.transform.localScale = new Vector3(selectedScale, selectedScale, this.transform.localScale.z);
    }

    public void UnenlargeButton() {
        this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public void PlayPressedButtonSound() {
        // playSelectedButtonSFX = !playPressedButtonSFX;
        if (pressedButtonSound != null && pressedButtonSound != "" && playPressedButtonSFX) {
            GameManager.instance.audioManager.Play(pressedButtonSound);
        }
    }

    public void PlaySelectedButtonSound() {
        // Debug.Log("PlaySelectedButtonSound() called.");
        // Debug.Log("selectedButtonSound variable = " + selectedButtonSound);
        if (selectedButtonSound != null && selectedButtonSound != "") {
            // Debug.Log("selectedButtonSound variable is not null or empty string.");
            GameManager.instance.audioManager.Play(selectedButtonSound);
        } 
    }

    public void SelectButton() {
        // this.transform.localScale = new Vector3(selectedScale, selectedScale, this.transform.localScale.z);
        base.DoStateTransition(SelectionState.Highlighted, true);
    }

    public void PressButton() {
        base.DoStateTransition(SelectionState.Pressed, true);
        buttonPressed = true;
        // base.onClick.Invoke();
    }

    public void ReleaseButton() {
        if (buttonPressed) {
            base.onClick.Invoke();
            buttonPressed = false;
            base.DoStateTransition(SelectionState.Highlighted, true);
        }
    }

    public bool IsButtonInNormalState() {
        return !(base.IsHighlighted(m_BaseEvent) || base.IsPressed());
    }

    public override void OnSelect(BaseEventData eventData) {
        // this.transform.localScale = new Vector3(selectedScale, selectedScale, this.transform.localScale.z);
        // Debug.Log(gameObject.name + ": OnSelect() called; currentSelectedGameobject = " + EventSystem.current.currentSelectedGameObject.name);
        base.OnSelect(eventData);
    }

    public override void OnDeselect(BaseEventData eventData) {
        // this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        base.OnDeselect(eventData);
        // Debug.Log(gameObject.name + ": OnDeselect() called; currentSelectedGameobject = " + eventData.selectedObject.name /*EventSystem.current.currentSelectedGameObject.name*/);
    }

    // protected override void OnDisable() {
    //     Debug.Log(gameObject.name + ": OnDisable() called.");
    //     base.OnDisable();
    // }

    // protected override void OnEnable() {
    //     // playSelectedButtonSFX = true;  // should reenable selected button sound after the menu that the button belongs is turned back on.
    //     Debug.Log(gameObject.name + ": OnEnable() called.");
    //     base.OnEnable();
    // }


}
