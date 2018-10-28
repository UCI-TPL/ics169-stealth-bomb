using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XInputDotNetPure;

public class Controller
{
    public UnityEvent A_Pressed = new UnityEvent();
    public UnityEvent B_Pressed = new UnityEvent();
    public UnityEvent X_Pressed = new UnityEvent();
    public UnityEvent Y_Pressed = new UnityEvent();
    public UnityEvent LB_Pressed = new UnityEvent();
    public UnityEvent RB_Pressed = new UnityEvent();
    public UnityEvent LT = new UnityEvent();
    public UnityEvent RT = new UnityEvent();
    public UnityEvent DPadL = new UnityEvent();
    public UnityEvent DPadR = new UnityEvent();
    public UnityEvent DPadU = new UnityEvent();
    public UnityEvent DPadD = new UnityEvent();
    public UnityEvent Start = new UnityEvent();
    public UnityEvent Back = new UnityEvent();
    public UnityEvent LStick = new UnityEvent();
    public UnityEvent RStick = new UnityEvent();

    public Controller()
    {
        // A Controller is born!
    }
}

public class InputManager : MonoBehaviour {
    [Header("Script Settings")]
    [Tooltip("If true, this allows the XInputManager to exist across scenes. Should be false at all times, unless it isn't a part of a GameController prefab.")]
    public bool enableSingletonBehavior = false;
    [Tooltip("If true, allow for debug-related info that checks this variable to be displayed/printed etc. Debug-related code in the script should check this variable.")]
    public bool enableDebug = false;

    public static InputManager im_Instance = null;

    // GAMEPAD STATES - Enough for a maximum of 4 players.
    private GamePadState p1_currentState;
    private GamePadState p1_prevState;
    private GamePadState p2_currentState;
    private GamePadState p2_prevState;
    private GamePadState p3_currentState;
    private GamePadState p3_prevState;
    private GamePadState p4_currentState;
    private GamePadState p4_prevState;

    // Array of Controller objects called
    public Controller[] connectedControllers = new Controller[4];

    void Awake()
    {
        if (enableSingletonBehavior)
        {
            if (im_Instance == null)
                im_Instance = this;
            else if (im_Instance != this)
                Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }

        for (int i = 0; i < 4; i++)
        {
            connectedControllers[i] = new Controller();
        }
    }

    void Update () {
        updateGamePadStates();
        processP1Input();
        processP2Input();
        processP3Input();
        processP4Input();
    }

    //  
    //  PUBLIC FUNCTIONS
    //

    public Vector2 getLeftStickData(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0:
                return new Vector2(p1_currentState.ThumbSticks.Left.X, p1_currentState.ThumbSticks.Left.Y);
            case 1:
                return new Vector2(p2_currentState.ThumbSticks.Left.X, p2_currentState.ThumbSticks.Left.Y);
            case 2:
                return new Vector2(p3_currentState.ThumbSticks.Left.X, p3_currentState.ThumbSticks.Left.Y);
            case 3:
                return new Vector2(p4_currentState.ThumbSticks.Left.X, p4_currentState.ThumbSticks.Left.Y);
            default:
                return Vector2.zero;

        }
    }
    public Vector2 getRightStickData(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0:
                return new Vector2(p1_currentState.ThumbSticks.Right.X, p1_currentState.ThumbSticks.Right.Y);
            case 1:
                return new Vector2(p2_currentState.ThumbSticks.Right.X, p2_currentState.ThumbSticks.Right.Y);
            case 2:
                return new Vector2(p3_currentState.ThumbSticks.Right.X, p3_currentState.ThumbSticks.Right.Y);
            case 3:
                return new Vector2(p4_currentState.ThumbSticks.Right.X, p4_currentState.ThumbSticks.Right.Y);
            default:
                return Vector2.zero;

        }
    }
    public float getLeftTriggerValue(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0:
                return p1_currentState.Triggers.Left;
            case 1:
                return p2_currentState.Triggers.Left;
            case 2:
                return p3_currentState.Triggers.Left;
            case 3:
                return p4_currentState.Triggers.Left;
            default:
                return 0f;
        }
    }
    public float getRightTriggerValue(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0:
                return p1_currentState.Triggers.Right;
            case 1:
                return p2_currentState.Triggers.Right;
            case 2:
                return p3_currentState.Triggers.Right;
            case 3:
                return p4_currentState.Triggers.Right;
            default:
                return 0f;
        }
    }

    //
    //  PRIVATE FUNCTIONS
    //

    private void updateGamePadStates()
    {
        p1_prevState = p1_currentState;
        p1_currentState = GamePad.GetState(PlayerIndex.One);
        p2_prevState = p2_currentState;
        p2_currentState = GamePad.GetState(PlayerIndex.Two);
        p3_prevState = p3_currentState;
        p3_currentState = GamePad.GetState(PlayerIndex.Three);
        p4_prevState = p4_currentState;
        p4_currentState = GamePad.GetState(PlayerIndex.Four);
    }
    private void processP1Input()
    {
        // Sticks and D-Pad
        if (p1_currentState.ThumbSticks.Left.X != 0f || p1_currentState.ThumbSticks.Left.Y != 0f)
            connectedControllers[0].LStick.Invoke();
        if (p1_currentState.ThumbSticks.Right.X != 0f || p1_currentState.ThumbSticks.Right.Y != 0f)
            connectedControllers[0].RStick.Invoke();
        if (p1_currentState.DPad.Up == ButtonState.Pressed)
            connectedControllers[0].DPadU.Invoke();
        if (p1_currentState.DPad.Down == ButtonState.Pressed)
            connectedControllers[0].DPadD.Invoke();
        if (p1_currentState.DPad.Left == ButtonState.Pressed)
            connectedControllers[0].DPadL.Invoke();
        if (p1_currentState.DPad.Right == ButtonState.Pressed)
            connectedControllers[0].DPadR.Invoke();
        // Triggers
        if (p1_currentState.Triggers.Left != 0f)
            connectedControllers[0].LT.Invoke();
        if (p1_currentState.Triggers.Right != 0f)
            connectedControllers[0].RT.Invoke();
        // Bumpers
        if (p1_currentState.Buttons.LeftShoulder == ButtonState.Pressed && p1_prevState.Buttons.LeftShoulder != ButtonState.Pressed)
            connectedControllers[0].LB_Pressed.Invoke();
        if (p1_currentState.Buttons.RightShoulder == ButtonState.Pressed && p1_prevState.Buttons.RightShoulder != ButtonState.Pressed)
            connectedControllers[0].RB_Pressed.Invoke();
        // Face Buttons
        if (p1_currentState.Buttons.A == ButtonState.Pressed && p1_prevState.Buttons.A != ButtonState.Pressed)
            connectedControllers[0].A_Pressed.Invoke();
        if (p1_currentState.Buttons.B == ButtonState.Pressed && p1_prevState.Buttons.B != ButtonState.Pressed)
            connectedControllers[0].B_Pressed.Invoke();
        if (p1_currentState.Buttons.X == ButtonState.Pressed && p1_prevState.Buttons.X != ButtonState.Pressed)
            connectedControllers[0].X_Pressed.Invoke();
        if (p1_currentState.Buttons.Y == ButtonState.Pressed && p1_prevState.Buttons.Y != ButtonState.Pressed)
            connectedControllers[0].Y_Pressed.Invoke();
        // Start and Back
        if (p1_currentState.Buttons.Start == ButtonState.Pressed && p1_prevState.Buttons.Start != ButtonState.Pressed)
            connectedControllers[0].Start.Invoke();
        if (p1_currentState.Buttons.Back == ButtonState.Pressed && p1_prevState.Buttons.Back != ButtonState.Pressed)
            connectedControllers[0].Back.Invoke();
    }
    private void processP2Input()
    {
        // Sticks and D-Pad
        if (p2_currentState.ThumbSticks.Left.X != 0f || p2_currentState.ThumbSticks.Left.Y != 0f)
            connectedControllers[1].LStick.Invoke();
        if (p2_currentState.ThumbSticks.Right.X != 0f || p2_currentState.ThumbSticks.Right.Y != 0f)
            connectedControllers[1].RStick.Invoke();
        if (p2_currentState.DPad.Up == ButtonState.Pressed)
            connectedControllers[1].DPadU.Invoke();
        if (p2_currentState.DPad.Down == ButtonState.Pressed)
            connectedControllers[1].DPadD.Invoke();
        if (p2_currentState.DPad.Left == ButtonState.Pressed)
            connectedControllers[1].DPadL.Invoke();
        if (p2_currentState.DPad.Right == ButtonState.Pressed)
            connectedControllers[1].DPadR.Invoke();
        // Triggers
        if (p2_currentState.Triggers.Left != 0f)
            connectedControllers[1].LT.Invoke();
        if (p2_currentState.Triggers.Right != 0f)
            connectedControllers[1].RT.Invoke();
        // Bumpers
        if (p2_currentState.Buttons.LeftShoulder == ButtonState.Pressed && p2_prevState.Buttons.LeftShoulder != ButtonState.Pressed)
            connectedControllers[1].LB_Pressed.Invoke();
        if (p2_currentState.Buttons.RightShoulder == ButtonState.Pressed && p2_prevState.Buttons.RightShoulder != ButtonState.Pressed)
            connectedControllers[1].RB_Pressed.Invoke();
        // Face Buttons
        if (p2_currentState.Buttons.A == ButtonState.Pressed && p2_prevState.Buttons.A != ButtonState.Pressed)
            connectedControllers[1].A_Pressed.Invoke();
        if (p2_currentState.Buttons.B == ButtonState.Pressed && p2_prevState.Buttons.B != ButtonState.Pressed)
            connectedControllers[1].B_Pressed.Invoke();
        if (p2_currentState.Buttons.X == ButtonState.Pressed && p2_prevState.Buttons.X != ButtonState.Pressed)
            connectedControllers[1].X_Pressed.Invoke();
        if (p2_currentState.Buttons.Y == ButtonState.Pressed && p2_prevState.Buttons.Y != ButtonState.Pressed)
            connectedControllers[1].Y_Pressed.Invoke();
        // Start and Back
        if (p2_currentState.Buttons.Start == ButtonState.Pressed && p2_prevState.Buttons.Start != ButtonState.Pressed)
            connectedControllers[1].Start.Invoke();
        if (p2_currentState.Buttons.Back == ButtonState.Pressed && p2_prevState.Buttons.Back != ButtonState.Pressed)
            connectedControllers[1].Back.Invoke();
    }
    private void processP3Input()
    {
        // Sticks and D-Pad
        if (p3_currentState.ThumbSticks.Left.X != 0f || p3_currentState.ThumbSticks.Left.Y != 0f)
            connectedControllers[2].LStick.Invoke();
        if (p3_currentState.ThumbSticks.Right.X != 0f || p3_currentState.ThumbSticks.Right.Y != 0f)
            connectedControllers[2].RStick.Invoke();
        if (p3_currentState.DPad.Up == ButtonState.Pressed)
            connectedControllers[2].DPadU.Invoke();
        if (p3_currentState.DPad.Down == ButtonState.Pressed)
            connectedControllers[2].DPadD.Invoke();
        if (p3_currentState.DPad.Left == ButtonState.Pressed)
            connectedControllers[2].DPadL.Invoke();
        if (p3_currentState.DPad.Right == ButtonState.Pressed)
            connectedControllers[2].DPadR.Invoke();
        // Triggers
        if (p3_currentState.Triggers.Left != 0f)
            connectedControllers[2].LT.Invoke();
        if (p3_currentState.Triggers.Right != 0f)
            connectedControllers[2].RT.Invoke();
        // Bumpers
        if (p3_currentState.Buttons.LeftShoulder == ButtonState.Pressed && p3_prevState.Buttons.LeftShoulder != ButtonState.Pressed)
            connectedControllers[2].LB_Pressed.Invoke();
        if (p3_currentState.Buttons.RightShoulder == ButtonState.Pressed && p3_prevState.Buttons.RightShoulder != ButtonState.Pressed)
            connectedControllers[2].RB_Pressed.Invoke();
        // Face Buttons
        if (p3_currentState.Buttons.A == ButtonState.Pressed && p3_prevState.Buttons.A != ButtonState.Pressed)
            connectedControllers[2].A_Pressed.Invoke();
        if (p3_currentState.Buttons.B == ButtonState.Pressed && p3_prevState.Buttons.B != ButtonState.Pressed)
            connectedControllers[2].B_Pressed.Invoke();
        if (p3_currentState.Buttons.X == ButtonState.Pressed && p3_prevState.Buttons.X != ButtonState.Pressed)
            connectedControllers[2].X_Pressed.Invoke();
        if (p3_currentState.Buttons.Y == ButtonState.Pressed && p3_prevState.Buttons.Y != ButtonState.Pressed)
            connectedControllers[2].Y_Pressed.Invoke();
        // Start and Back
        if (p3_currentState.Buttons.Start == ButtonState.Pressed && p3_prevState.Buttons.Start != ButtonState.Pressed)
            connectedControllers[2].Start.Invoke();
        if (p3_currentState.Buttons.Back == ButtonState.Pressed && p3_prevState.Buttons.Back != ButtonState.Pressed)
            connectedControllers[2].Back.Invoke();
    }
    private void processP4Input()
    {
        // Sticks and D-Pad
        if (p4_currentState.ThumbSticks.Left.X != 0f || p4_currentState.ThumbSticks.Left.Y != 0f)
            connectedControllers[3].LStick.Invoke();
        if (p4_currentState.ThumbSticks.Right.X != 0f || p4_currentState.ThumbSticks.Right.Y != 0f)
            connectedControllers[3].RStick.Invoke();
        if (p4_currentState.DPad.Up == ButtonState.Pressed)
            connectedControllers[3].DPadU.Invoke();
        if (p4_currentState.DPad.Down == ButtonState.Pressed)
            connectedControllers[3].DPadD.Invoke();
        if (p4_currentState.DPad.Left == ButtonState.Pressed)
            connectedControllers[3].DPadL.Invoke();
        if (p4_currentState.DPad.Right == ButtonState.Pressed)
            connectedControllers[3].DPadR.Invoke();
        // Triggers
        if (p4_currentState.Triggers.Left != 0f)
            connectedControllers[3].LT.Invoke();
        if (p4_currentState.Triggers.Right != 0f)
            connectedControllers[3].RT.Invoke();
        // Bumpers
        if (p4_currentState.Buttons.LeftShoulder == ButtonState.Pressed && p4_prevState.Buttons.LeftShoulder != ButtonState.Pressed)
            connectedControllers[3].LB_Pressed.Invoke();
        if (p4_currentState.Buttons.RightShoulder == ButtonState.Pressed && p4_prevState.Buttons.RightShoulder != ButtonState.Pressed)
            connectedControllers[3].RB_Pressed.Invoke();
        // Face Buttons
        if (p4_currentState.Buttons.A == ButtonState.Pressed && p4_prevState.Buttons.A != ButtonState.Pressed)
            connectedControllers[3].A_Pressed.Invoke();
        if (p4_currentState.Buttons.B == ButtonState.Pressed && p4_prevState.Buttons.B != ButtonState.Pressed)
            connectedControllers[3].B_Pressed.Invoke();
        if (p4_currentState.Buttons.X == ButtonState.Pressed && p4_prevState.Buttons.X != ButtonState.Pressed)
            connectedControllers[3].X_Pressed.Invoke();
        if (p4_currentState.Buttons.Y == ButtonState.Pressed && p4_prevState.Buttons.Y != ButtonState.Pressed)
            connectedControllers[3].Y_Pressed.Invoke();
        // Start and Back
        if (p4_currentState.Buttons.Start == ButtonState.Pressed && p4_prevState.Buttons.Start != ButtonState.Pressed)
            connectedControllers[3].Start.Invoke();
        if (p4_currentState.Buttons.Back == ButtonState.Pressed && p4_prevState.Buttons.Back != ButtonState.Pressed)
            connectedControllers[3].Back.Invoke();
    }

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
