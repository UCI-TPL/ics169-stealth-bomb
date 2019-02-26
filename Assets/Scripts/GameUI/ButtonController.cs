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

    public string pressedSound = "Bow";

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

    public void PlayPressedButtonSound(string sound) {
        // base.onClick.AddListener(() => GameManager.instance.audioManager.Play(pressedSound));
        GameManager.instance.audioManager.Play(pressedSound);
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
        // Debug.Log(this.gameObject.name + " actually selected.");
        base.OnSelect(eventData);
    }

    // public override void OnDeselect(BaseEventData eventData) {
    //     // this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    //     base.OnDeselect(eventData);
    // }


}
