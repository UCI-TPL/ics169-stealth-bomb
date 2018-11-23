using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControlsUI : MonoBehaviour {

    [SerializeField]
    private RectTransform CanvasRect;
    [SerializeField]
    private RectTransform RightJoyStickArea;
    [SerializeField]
    private RectTransform RightJoyStick;
    [SerializeField]
    private RectTransform LeftJoyStickArea;
    [SerializeField]
    private RectTransform LeftJoyStick;
    [SerializeField]
    private float JoystickRadiusScale;
    public Vector2 JoystickRadiusRatio { get { return RightJoyStickArea.rect.size / 2 / CanvasRect.rect.size * JoystickRadiusScale; } }

    /// <summary>
    /// Set the right joystick's initial position on the screen
    /// </summary>
    /// <param name="screenPosition"> Screen position as a ratio between 0 and 1 </param>
    public void SetRightJoystickPosition(Vector2 screenPosition) {
        RightJoyStickArea.anchoredPosition = CanvasRect.rect.size * screenPosition;
        ShowRightJoystick();
    }

    /// <summary>
    /// Set the right joystick's initial position on the screen
    /// </summary>
    /// <param name="screenPosition"> Screen position as a ratio between 0 and 1 </param>
    public void SetRightJoystickDirection(Vector2 direction) {
        RightJoyStick.anchoredPosition = new Vector2(RightJoyStickArea.anchoredPosition.x, RightJoyStickArea.anchoredPosition.y) + RightJoyStickArea.rect.size / 2 * direction * JoystickRadiusScale;
    }

    public void HideRightJoystick() {
        RightJoyStickArea.gameObject.SetActive(false);
        RightJoyStick.gameObject.SetActive(false);
    }

    public void ShowRightJoystick() {
        RightJoyStickArea.gameObject.SetActive(true);
        RightJoyStick.gameObject.SetActive(true);
    }

    /// <summary>
    /// Set the left joystick's initial position on the screen
    /// </summary>
    /// <param name="screenPosition"> Screen position as a ratio between 0 and 1 </param>
    public void SetLeftJoystickPosition(Vector2 screenPosition) {
        LeftJoyStickArea.anchoredPosition = CanvasRect.rect.size * screenPosition;
        ShowLeftJoystick();
    }

    /// <summary>
    /// Set the Left joystick's initial position on the screen
    /// </summary>
    /// <param name="screenPosition"> Screen position as a ratio between 0 and 1 </param>
    public void SetLeftJoystickDirection(Vector2 direction) {
        LeftJoyStick.anchoredPosition = new Vector2(LeftJoyStickArea.anchoredPosition.x, LeftJoyStickArea.anchoredPosition.y) + LeftJoyStickArea.rect.size / 2 * direction * JoystickRadiusScale;
    }

    public void HideLeftJoystick() {
        LeftJoyStickArea.gameObject.SetActive(false);
        LeftJoyStick.gameObject.SetActive(false);
    }

    public void ShowLeftJoystick() {
        LeftJoyStickArea.gameObject.SetActive(true);
        LeftJoyStick.gameObject.SetActive(true);
    }

}
