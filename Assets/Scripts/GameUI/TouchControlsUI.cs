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
    private RectTransform RightJoyStickArea2;
    [SerializeField]
    private RectTransform RightJoyStick2;
    [SerializeField]
    private RectTransform LeftJoyStickArea2;
    [SerializeField]
    private RectTransform LeftJoyStick2;
    [SerializeField]
    private float JoystickRadiusScale;
    public Vector2 JoystickRadiusRatio { get { return RightJoyStickArea.rect.size / 2 / CanvasRect.rect.size * JoystickRadiusScale; } }

    /// <summary>
    /// Set the right joystick's initial position on the screen
    /// </summary>
    /// <param name="screenPosition"> Screen position as a ratio between 0 and 1 </param>
    public void SetRightJoystickPosition(Vector2 screenPosition, int playerNumber) {
        if (playerNumber == 0)
            RightJoyStickArea.anchoredPosition = CanvasRect.rect.size * screenPosition;
        else
            RightJoyStickArea2.anchoredPosition = CanvasRect.rect.size * screenPosition;
        ShowRightJoystick(playerNumber);
    }

    /// <summary>
    /// Set the right joystick's initial position on the screen
    /// </summary>
    /// <param name="screenPosition"> Screen position as a ratio between 0 and 1 </param>
    public void SetRightJoystickDirection(Vector2 direction, int playerNumber) {
        if (playerNumber == 0)
            RightJoyStick.anchoredPosition = new Vector2(RightJoyStickArea.anchoredPosition.x, RightJoyStickArea.anchoredPosition.y) + RightJoyStickArea.rect.size / 2 * direction * JoystickRadiusScale;
        else
            RightJoyStick2.anchoredPosition = new Vector2(RightJoyStickArea2.anchoredPosition.x, RightJoyStickArea2.anchoredPosition.y) + RightJoyStickArea2.rect.size / 2 * direction * JoystickRadiusScale;
    }

    public void HideRightJoystick(int playerNumber) {
        if (playerNumber == 0) {
            RightJoyStickArea.gameObject.SetActive(false);
            RightJoyStick.gameObject.SetActive(false);
        }
        else {
            RightJoyStickArea2.gameObject.SetActive(false);
            RightJoyStick2.gameObject.SetActive(false);
        }
    }

    public void ShowRightJoystick(int playerNumber) {
        if (playerNumber == 0) {
            RightJoyStickArea.gameObject.SetActive(true);
            RightJoyStick.gameObject.SetActive(true);
        }
        else {
            RightJoyStickArea2.gameObject.SetActive(true);
            RightJoyStick2.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Set the left joystick's initial position on the screen
    /// </summary>
    /// <param name="screenPosition"> Screen position as a ratio between 0 and 1 </param>
    public void SetLeftJoystickPosition(Vector2 screenPosition, int playerNumber) {
        if (playerNumber == 0)
            LeftJoyStickArea.anchoredPosition = CanvasRect.rect.size * screenPosition;
        else
            LeftJoyStickArea2.anchoredPosition = CanvasRect.rect.size * screenPosition;
        ShowLeftJoystick(playerNumber);
    }

    /// <summary>
    /// Set the Left joystick's initial position on the screen
    /// </summary>
    /// <param name="screenPosition"> Screen position as a ratio between 0 and 1 </param>
    public void SetLeftJoystickDirection(Vector2 direction, int playerNumber) {
        if (playerNumber == 0)
            LeftJoyStick.anchoredPosition = new Vector2(LeftJoyStickArea.anchoredPosition.x, LeftJoyStickArea.anchoredPosition.y) + LeftJoyStickArea.rect.size / 2 * direction * JoystickRadiusScale;
        else
            LeftJoyStick2.anchoredPosition = new Vector2(LeftJoyStickArea2.anchoredPosition.x, LeftJoyStickArea2.anchoredPosition.y) + LeftJoyStickArea2.rect.size / 2 * direction * JoystickRadiusScale;
    }

    public void HideLeftJoystick(int playerNumber) {
        if (playerNumber == 0) {
            LeftJoyStickArea.gameObject.SetActive(false);
            LeftJoyStick.gameObject.SetActive(false);
        }
        else {
            LeftJoyStickArea2.gameObject.SetActive(false);
            LeftJoyStick2.gameObject.SetActive(false);
        }

    }

    public void ShowLeftJoystick(int playerNumber) {
        if (playerNumber == 0) {
            LeftJoyStickArea.gameObject.SetActive(true);
            LeftJoyStick.gameObject.SetActive(true);
        }
        else {
            LeftJoyStickArea2.gameObject.SetActive(true);
            LeftJoyStick2.gameObject.SetActive(true);
        }
    }

}
