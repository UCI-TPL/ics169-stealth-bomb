using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class XInputManager : MonoBehaviour {
    [Header("Script Settings")]
    [Tooltip("If true, this allows the XInputManager to exist across scenes. Should be false at all times, unless it isn't a part of a GameController prefab.")]
    public bool enableSingletonBehavior = false;
    [Tooltip("If true, allow for debug-related info that checks this variable to be displayed/printed etc. Debug-related code in the script should check this variable.")]
    public bool enableDebug = false;

    public static XInputManager xim_Instance = null;

    // GAMEPAD STATES - Enough for a maximum of 4 players.
    private GamePadState p1_currentState;
    private GamePadState p1_prevState;
    private GamePadState p2_currentState;
    private GamePadState p2_prevState;
    private GamePadState p3_currentState;
    private GamePadState p3_prevState;
    private GamePadState p4_currentState;
    private GamePadState p4_prevState;

    void Awake()
    {
        if (enableSingletonBehavior)
        {
            if (xim_Instance == null)
                xim_Instance = this;
            else if (xim_Instance != this)
                Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update () {
        p1_prevState = p1_currentState;
        p1_currentState = GamePad.GetState(PlayerIndex.One);
        p2_prevState = p2_currentState;
        p2_currentState = GamePad.GetState(PlayerIndex.Two);
        p3_prevState = p3_currentState;
        p3_currentState = GamePad.GetState(PlayerIndex.Three);
        p4_prevState = p4_currentState;
        p4_currentState = GamePad.GetState(PlayerIndex.Four);
	}

    //  
    //  PUBLIC FUNCTIONS
    //


    //
    //  PRIVATE FUNCTIONS
    //


    //
    //  "ON" FUNCTIONS
    //

    void OnGUI()
    {
        if (enableDebug)
        {
            string text = "--- Player 1 ---\n";
            text += string.Format("Connected: {0}\n", p1_currentState.IsConnected);
            text += string.Format("D-Pad Left: {0} | D-Pad Right: {1} \nD-Pad Up: {2} | D-Pad Down: {3}\n", p1_currentState.DPad.Left, p1_currentState.DPad.Right, p1_currentState.DPad.Up, p1_currentState.DPad.Down);
            text += string.Format("L-Stick X: {0} | L-Stick Y: {1} \nR-Stick X: {2} | R-Stick Y: {3}\n", p1_currentState.ThumbSticks.Left.X, p1_currentState.ThumbSticks.Left.Y, p1_currentState.ThumbSticks.Right.X, p1_currentState.ThumbSticks.Right.Y);
            text += string.Format("L-Stick Button: {0} | R-Stick Button: {1}\n", p1_currentState.Buttons.LeftStick, p1_currentState.Buttons.RightStick);
            text += string.Format("A: {0} | B: {1} | X: {2} | Y: {3}\n", p1_currentState.Buttons.A, p1_currentState.Buttons.B, p1_currentState.Buttons.X, p1_currentState.Buttons.Y);
            text += string.Format("L-Bumper: {0} | L-Trigger: {1} \nR-Bumper: {2} | R-Trigger: {3}\n", p1_currentState.Buttons.LeftShoulder, p1_currentState.Triggers.Left, p1_currentState.Buttons.RightShoulder, p1_currentState.Triggers.Right);
            text += string.Format("Start: {0} | Back: {1}", p1_currentState.Buttons.Start, p1_currentState.Buttons.Back);
            GUI.Label(new Rect(0, 0, Screen.width/2, Screen.height/2), text);

            text = "--- Player 2 ---\n";
            text += string.Format("Connected: {0}\n", p2_currentState.IsConnected);
            text += string.Format("D-Pad Left: {0} | D-Pad Right: {1} \nD-Pad Up: {2} | D-Pad Down: {3}\n", p2_currentState.DPad.Left, p2_currentState.DPad.Right, p2_currentState.DPad.Up, p2_currentState.DPad.Down);
            text += string.Format("L-Stick X: {0} | L-Stick Y: {1} \nR-Stick X: {2} | R-Stick Y: {3}\n", p2_currentState.ThumbSticks.Left.X, p2_currentState.ThumbSticks.Left.Y, p2_currentState.ThumbSticks.Right.X, p2_currentState.ThumbSticks.Right.Y);
            text += string.Format("L-Stick Button: {0} | R-Stick Button: {1}\n", p2_currentState.Buttons.LeftStick, p2_currentState.Buttons.RightStick);
            text += string.Format("A: {0} | B: {1} | X: {2} | Y: {3}\n", p2_currentState.Buttons.A, p2_currentState.Buttons.B, p2_currentState.Buttons.X, p2_currentState.Buttons.Y);
            text += string.Format("L-Bumper: {0} | L-Trigger: {1} \nR-Bumper: {2} | R-Trigger: {3}\n", p2_currentState.Buttons.LeftShoulder, p2_currentState.Triggers.Left, p2_currentState.Buttons.RightShoulder, p2_currentState.Triggers.Right);
            text += string.Format("Start: {0} | Back: {1}", p2_currentState.Buttons.Start, p2_currentState.Buttons.Back);
            GUI.Label(new Rect(Screen.width / 2, 0, Screen.width, Screen.height / 2), text);

            text = "--- Player 3 ---\n";
            text += string.Format("Connected: {0}\n", p3_currentState.IsConnected);
            text += string.Format("D-Pad Left: {0} | D-Pad Right: {1} \nD-Pad Up: {2} | D-Pad Down: {3}\n", p3_currentState.DPad.Left, p3_currentState.DPad.Right, p3_currentState.DPad.Up, p3_currentState.DPad.Down);
            text += string.Format("L-Stick X: {0} | L-Stick Y: {1} \nR-Stick X: {2} | R-Stick Y: {3}\n", p3_currentState.ThumbSticks.Left.X, p3_currentState.ThumbSticks.Left.Y, p3_currentState.ThumbSticks.Right.X, p3_currentState.ThumbSticks.Right.Y);
            text += string.Format("L-Stick Button: {0} | R-Stick Button: {1}\n", p3_currentState.Buttons.LeftStick, p3_currentState.Buttons.RightStick);
            text += string.Format("A: {0} | B: {1} | X: {2} | Y: {3}\n", p3_currentState.Buttons.A, p3_currentState.Buttons.B, p3_currentState.Buttons.X, p3_currentState.Buttons.Y);
            text += string.Format("L-Bumper: {0} | L-Trigger: {1} \nR-Bumper: {2} | R-Trigger: {3}\n", p3_currentState.Buttons.LeftShoulder, p3_currentState.Triggers.Left, p3_currentState.Buttons.RightShoulder, p3_currentState.Triggers.Right);
            text += string.Format("Start: {0} | Back: {1}", p3_currentState.Buttons.Start, p3_currentState.Buttons.Back);
            GUI.Label(new Rect(0, Screen.height / 2, Screen.width/2, Screen.height), text);

            text = "--- Player 4 ---\n";
            text += string.Format("Connected: {0}\n", p4_currentState.IsConnected);
            text += string.Format("D-Pad Left: {0} | D-Pad Right: {1} \nD-Pad Up: {2} | D-Pad Down: {3}\n", p4_currentState.DPad.Left, p4_currentState.DPad.Right, p4_currentState.DPad.Up, p4_currentState.DPad.Down);
            text += string.Format("L-Stick X: {0} | L-Stick Y: {1} \nR-Stick X: {2} | R-Stick Y: {3}\n", p4_currentState.ThumbSticks.Left.X, p4_currentState.ThumbSticks.Left.Y, p4_currentState.ThumbSticks.Right.X, p4_currentState.ThumbSticks.Right.Y);
            text += string.Format("L-Stick Button: {0} | R-Stick Button: {1}\n", p4_currentState.Buttons.LeftStick, p4_currentState.Buttons.RightStick);
            text += string.Format("A: {0} | B: {1} | X: {2} | Y: {3}\n", p4_currentState.Buttons.A, p4_currentState.Buttons.B, p4_currentState.Buttons.X, p4_currentState.Buttons.Y);
            text += string.Format("L-Bumper: {0} | L-Trigger: {1} \nR-Bumper: {2} | R-Trigger: {3}\n", p4_currentState.Buttons.LeftShoulder, p4_currentState.Triggers.Left, p4_currentState.Buttons.RightShoulder, p4_currentState.Triggers.Right);
            text += string.Format("Start: {0} | Back: {1}", p4_currentState.Buttons.Start, p4_currentState.Buttons.Back);
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, Screen.width, Screen.height), text);
        }
    }
}
