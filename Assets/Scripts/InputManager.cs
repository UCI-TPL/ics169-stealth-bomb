using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XInputDotNetPure;

public class Controller
{
    public float vibrationValue;
    public UnityEvent A_Pressed = new UnityEvent();
    public UnityEvent B_Pressed = new UnityEvent();
    public UnityEvent X_Pressed = new UnityEvent();
    public UnityEvent Y_Pressed = new UnityEvent();
    public UnityEvent LB_Pressed = new UnityEvent();
    public UnityEvent RB_Pressed = new UnityEvent();
    public UnityEvent LT_Pressed = new UnityEvent();
    public UnityEvent LT_Released = new UnityEvent();
    public UnityEvent RT_Pressed = new UnityEvent();
    public UnityEvent RT_Released = new UnityEvent();
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
        vibrationValue = 0;
        // A Controller is born!
    }
}

public class InputManager : MonoBehaviour {

    // Returns the current inputManager
    private static InputManager _inputManager;
    public static InputManager inputManager {
        get {
            if (_inputManager != null)
                return _inputManager;
            _inputManager = FindObjectOfType<InputManager>();
            if (_inputManager == null) {
                Debug.LogError("Input Manager not found, Created new Input Manager.");
                _inputManager = new GameObject("Input Manager").AddComponent<InputManager>();
                GameObject g = GameObject.Find("Managers");
                _inputManager.transform.SetParent(g != null ? g.transform : new GameObject("Managers").transform);
            }
            return _inputManager;
        }
    }

    public Controller[] controllers = new Controller[4];

    private void Start() {
        for (int i = 0; i < 4; ++i)
            controllers[i] = new XboxController(i);
    }

    private void Update() {
        for (int i = 0; i < 4; ++i)
            controllers[i].UpdateController();
    }

    // Controller object for Mouse and Keyboard, This is not implemented yet
    public class MouseKeyboard : Controller {

        public override Vector2 MoveVector() {
            throw new System.NotImplementedException();
        }

        public override Vector2 AimVector() {
            throw new System.NotImplementedException();
        }

        public override void UpdateController() {
            throw new System.NotImplementedException();
        }

        // Do nothing, because a keyboard can't vibrate. Atleast mine can't!
        public override void Vibrate(float strength) { }

        public MouseKeyboard() {
            type = Type.MouseKeyboard;
        }
    }

    // Controller object for Xbox controllers
    public class XboxController : Controller {
        private PlayerIndex playerIndex;
        private GamePadState state;
        private GamePadState prevState;
        public bool isActive {
            get { return state.IsConnected && prevState.IsConnected; }
        }
        private delegate void Action();
        private delegate void EventCall(Action a);
        private delegate bool TestEvent();
        private delegate Vector2 TestJoyStick();

        private Dictionary<ButtonCode, ButtonTest> ButtonMap = new Dictionary<ButtonCode, ButtonTest>();
        private Dictionary<JoyStickCode, TestJoyStick> JoyStickMap = new Dictionary<JoyStickCode, TestJoyStick>();

        private ButtonTest attackTest = new ButtonTest();
        private ButtonTest jumpTest = new ButtonTest();
        private ButtonTest dodgeTest = new ButtonTest();

        public override Vector2 MoveVector() {
            return JoyStickMap[JoyStickCode.Left]();
        }

        public override Vector2 AimVector() {
            return JoyStickMap[JoyStickCode.Right]();
        }

        private void MapButtons() {
            attackTest += ButtonMap[ButtonCode.RightBumper] += ButtonMap[ButtonCode.RightTrigger];
            jumpTest += ButtonMap[ButtonCode.LeftBumper];
            dodgeTest += ButtonMap[ButtonCode.LeftTrigger];
        }

        public void UpdateEvents() {
            attackTest.Down(delegate { attack.OnDown.Invoke(); });
            attackTest.Up(delegate { attack.OnUp.Invoke(); });
            attack.Pressed = attackTest.Pressed();

            jumpTest.Down(delegate { jump.OnDown.Invoke(); });
            jumpTest.Up(delegate { jump.OnUp.Invoke(); });
            jump.Pressed = jumpTest.Pressed();

            dodgeTest.Down(delegate { dodge.OnDown.Invoke(); });
            dodgeTest.Up(delegate { dodge.OnUp.Invoke(); });
            dodge.Pressed = dodgeTest.Pressed();
        }

        public void UpdateJoySticks() {

        }

        public XboxController(int playerIndex) {
            type = Type.Xbox;
            this.playerIndex = (PlayerIndex)playerIndex;
            #region Button Defenitions
            ButtonMap.Add(ButtonCode.A, new ButtonTest(ADown, AUp, APressed));
            ButtonMap.Add(ButtonCode.RightBumper, new ButtonTest(RightBumperDown, RightBumperUp, RightBumperPressed));
            ButtonMap.Add(ButtonCode.LeftBumper, new ButtonTest(LeftBumperDown, LeftBumperUp, LeftBumperPressed));
            ButtonMap.Add(ButtonCode.LeftTrigger, new ButtonTest(LeftTriggerDown, LeftTriggerUp, LeftTriggerPressed));
            ButtonMap.Add(ButtonCode.RightTrigger, new ButtonTest(RightTriggerDown, RightTriggerUp, RightTriggerPressed));
            #endregion
            JoyStickMap.Add(JoyStickCode.Left, LeftJoyStickTest);
            JoyStickMap.Add(JoyStickCode.Right, RightJoyStickTest);
            MapButtons();
        }

        #region Button Defenitions
        private void ADown(Action action) {
            if (ButtonDown(state.Buttons.A, prevState.Buttons.A))
                action();
        }

        private void AUp(Action action) {
            if (ButtonUp(state.Buttons.A, prevState.Buttons.A))
                action();
        }

        private bool APressed() {
            return ButtonPressed(state.Buttons.A);
        }

        private void RightBumperDown(Action action) {
            if (ButtonDown(state.Buttons.RightShoulder, prevState.Buttons.RightShoulder))
                action();
        }

        private void RightBumperUp(Action action) {
            if (ButtonUp(state.Buttons.RightShoulder, prevState.Buttons.RightShoulder))
                action();
        }

        private bool RightBumperPressed() {
            return ButtonPressed(state.Buttons.RightShoulder);
        }

        private void LeftBumperDown(Action action) {
            if (ButtonDown(state.Buttons.LeftShoulder, prevState.Buttons.LeftShoulder))
                action();
        }

        private void LeftBumperUp(Action action) {
            if (ButtonUp(state.Buttons.LeftShoulder, prevState.Buttons.LeftShoulder))
                action();
        }

        private bool LeftBumperPressed() {
            return ButtonPressed(state.Buttons.LeftShoulder);
        }

        private void LeftTriggerDown(Action action) {
            if (state.Triggers.Left > 0 && prevState.Triggers.Left <= 0)
                action();
        }

        private void LeftTriggerUp(Action action) {
            if (state.Triggers.Left <= 0 && prevState.Triggers.Left > 0)
                action();
        }

        private bool LeftTriggerPressed() {
            return state.Triggers.Left > 0;
        }

        private void RightTriggerDown(Action action) {
            if (state.Triggers.Right > 0 && prevState.Triggers.Right <= 0)
                action();
        }

        private void RightTriggerUp(Action action) {
            if (state.Triggers.Right <= 0 && prevState.Triggers.Right > 0)
                action();
        }

        private bool RightTriggerPressed() {
            return state.Triggers.Right > 0;
        }
        #endregion

        private Vector2 LeftJoyStickTest() {
            return new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
        }

        private Vector2 RightJoyStickTest() {
            return new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
        }

        public override void Vibrate(float strength) {
            GamePad.SetVibration(playerIndex, strength, strength);
        }

        public override void UpdateController() {
            prevState = state;
            state = GamePad.GetState(playerIndex);
            if (isActive)
                UpdateEvents();
        }

        // Compare ButtonStates and return whether the button had just been pressed
        private bool ButtonDown(ButtonState current, ButtonState previous) {
            return current == ButtonState.Pressed && previous != ButtonState.Pressed;
        }

        // Compare ButtonStates and return whether the button had just been released
        private bool ButtonUp(ButtonState current, ButtonState previous) {
            return current != ButtonState.Pressed && previous == ButtonState.Pressed;
        }

        // Check if a button state is pressed
        private bool ButtonPressed(ButtonState current) {
            return current == ButtonState.Pressed;
        }

        // List of every button available
        public enum ButtonCode {
            A, B, X, Y, Up, Down, Left, Right, LeftBumper, RightBumper, Back, Start, LeftStick, RightStick, LeftTrigger, RightTrigger, Home
        }

        // List of every JoyStick available
        public enum JoyStickCode {
            Left, Right, DPad
        }

        private class ButtonTest {
            public EventCall Down;
            public EventCall Up;
            public TestEvent Pressed;
            public ButtonTest() {
                this.Down = delegate {};
                this.Up = delegate {};
                this.Pressed = delegate { return false; };
            }
            public ButtonTest(EventCall Down, EventCall Up, TestEvent Pressed) {
                this.Down = Down;
                this.Up = Up;
                this.Pressed = Pressed;
            }

            public static ButtonTest operator +(ButtonTest l, ButtonTest r) {
                ButtonTest b = new ButtonTest(l.Down, l.Up, l.Pressed);
                b.Down += r.Down;
                b.Up += r.Up;
                b.Pressed += r.Pressed;
                return b;
            }

            public static ButtonTest operator -(ButtonTest l, ButtonTest r) {
                ButtonTest b = new ButtonTest(l.Down, l.Up, l.Pressed);
                b.Down -= r.Down;
                b.Up -= r.Up;
                b.Pressed -= r.Pressed;
                return b;
            }
        }
    }

    public abstract class Controller {
        public Type type;
        public abstract void Vibrate(float strength);
        public abstract void UpdateController();

        // Public Methods to get controller states
        public ButtonEvent attack = new ButtonEvent();
        public ButtonEvent jump = new ButtonEvent();
        public ButtonEvent dodge = new ButtonEvent();
        public abstract Vector2 MoveVector();
        public abstract Vector2 AimVector();

        public enum Type {
            Xbox, MouseKeyboard
        }
        public class ButtonEvent {
            public UnityEvent OnDown = new UnityEvent();
            public UnityEvent OnUp = new UnityEvent();
            public bool Pressed;
        }
    }
    //[Header("Script Settings")]
    //[Tooltip("If true, this allows the XInputManager to exist across scenes. Should be false at all times, unless it isn't a part of a GameController prefab.")]
    //public bool enableSingletonBehavior = false;
    //[Tooltip("If true, allow for debug-related info that checks this variable to be displayed/printed etc. Debug-related code in the script should check this variable.")]
    //public bool enableDebug = false;

    //public static InputManager im_Instance = null;

    //// States: (Previous)-(Current)
    //// P1 States: 0-1 | P2 States: 2-3 | P3 States: 4-5 | P4 States: 6-7
    //private GamePadState[] playerStates = new GamePadState[8];

    //// Array of Controller objects called connectedControllers
    //public Controller[] connectedControllers = new Controller[4];

    //void Awake()
    //{
    //    if (enableSingletonBehavior)
    //    {
    //        if (im_Instance == null)
    //            im_Instance = this;
    //        else if (im_Instance != this)
    //            Destroy(gameObject);
    //        DontDestroyOnLoad(gameObject);
    //    }

    //    for (int i = 0; i < 8; i++)
    //    {
    //        playerStates[i] = new GamePadState();
    //    }

    //    for (int i = 0; i < 4; i++)
    //    {
    //        connectedControllers[i] = new Controller();
    //    }
    //}

    //void Update () {
    //    updateGamePadStates();
    //    processAllControllerInput();
    //}

    //void FixedUpdate() {
    //    for (int i = 0; i < 4; i++) {
    //        GamePad.SetVibration((PlayerIndex) i, connectedControllers[i].vibrationValue, connectedControllers[i].vibrationValue);
    //        connectedControllers[i].vibrationValue = 0;
    //    }
    //}

    ////  
    ////  PUBLIC FUNCTIONS
    ////

    //public Vector2 getLeftStickData(int playerIndex)
    //{
    //    switch (playerIndex)
    //    {
    //        case 0:
    //            return new Vector2(playerStates[1].ThumbSticks.Left.X, playerStates[1].ThumbSticks.Left.Y);
    //        case 1:
    //            return new Vector2(playerStates[3].ThumbSticks.Left.X, playerStates[3].ThumbSticks.Left.Y);
    //        case 2:
    //            return new Vector2(playerStates[5].ThumbSticks.Left.X, playerStates[5].ThumbSticks.Left.Y);
    //        case 3:
    //            return new Vector2(playerStates[7].ThumbSticks.Left.X, playerStates[7].ThumbSticks.Left.Y);
    //        default:
    //            return Vector2.zero;

    //    }
    //}
    //public Vector2 getRightStickData(int playerIndex)
    //{
    //    switch (playerIndex)
    //    {
    //        case 0:
    //            return new Vector2(playerStates[1].ThumbSticks.Right.X, playerStates[1].ThumbSticks.Right.Y);
    //        case 1:
    //            return new Vector2(playerStates[3].ThumbSticks.Right.X, playerStates[3].ThumbSticks.Right.Y);
    //        case 2:
    //            return new Vector2(playerStates[5].ThumbSticks.Right.X, playerStates[5].ThumbSticks.Right.Y);
    //        case 3:
    //            return new Vector2(playerStates[7].ThumbSticks.Right.X, playerStates[7].ThumbSticks.Right.Y);
    //        default:
    //            return Vector2.zero;

    //    }
    //}
    //public float getLeftTriggerValue(int playerIndex)
    //{
    //    switch (playerIndex)
    //    {
    //        case 0:
    //            return playerStates[1].Triggers.Left;
    //        case 1:
    //            return playerStates[3].Triggers.Left;
    //        case 2:
    //            return playerStates[5].Triggers.Left;
    //        case 3:
    //            return playerStates[7].Triggers.Left;
    //        default:
    //            return 0f;
    //    }
    //}
    //public float getRightTriggerValue(int playerIndex)
    //{
    //    switch (playerIndex)
    //    {
    //        case 0:
    //            return playerStates[1].Triggers.Right;
    //        case 1:
    //            return playerStates[3].Triggers.Right;
    //        case 2:
    //            return playerStates[5].Triggers.Right;
    //        case 3:
    //            return playerStates[7].Triggers.Right;
    //        default:
    //            return 0f;
    //    }
    //}

    ////
    ////  PRIVATE FUNCTIONS
    ////

    //private void updateGamePadStates()
    //{
    //    for (int i = 0; i < 4; i++)
    //    {
    //        playerStates[i * 2] = playerStates[i * 2 + 1];
    //        playerStates[i * 2 + 1] = GamePad.GetState((PlayerIndex)i);
    //    }
    //}

    //private void processAllControllerStickInput() {
    //    for (int i = 0; i < 4; i++) 
    //    {
    //        if (playerStates[i * 2 + 1].ThumbSticks.Left.X != 0f || playerStates[i * 2 + 1].ThumbSticks.Left.Y != 0f)
    //            connectedControllers[i].LStick.Invoke();
    //        if (playerStates[i * 2 + 1].ThumbSticks.Right.X != 0f || playerStates[i * 2 + 1].ThumbSticks.Right.Y != 0f)
    //            connectedControllers[i].RStick.Invoke();
    //    }
    //}

    //private void processAllControllerInput()
    //{
    //    for (int i = 0; i < 4; i++)
    //    {
    //        // Sticks and D-Pad
    //        if (playerStates[i * 2 + 1].ThumbSticks.Left.X != 0f || playerStates[i * 2 + 1].ThumbSticks.Left.Y != 0f)
    //            connectedControllers[i].LStick.Invoke();
    //        if (playerStates[i * 2 + 1].ThumbSticks.Right.X != 0f || playerStates[i * 2 + 1].ThumbSticks.Right.Y != 0f)
    //            connectedControllers[i].RStick.Invoke();
    //        if (playerStates[i * 2 + 1].DPad.Up == ButtonState.Pressed)
    //            connectedControllers[i].DPadU.Invoke();
    //        if (playerStates[i * 2 + 1].DPad.Down == ButtonState.Pressed)
    //            connectedControllers[i].DPadD.Invoke();
    //        if (playerStates[i * 2 + 1].DPad.Left == ButtonState.Pressed)
    //            connectedControllers[i].DPadL.Invoke();
    //        if (playerStates[i * 2 + 1].DPad.Right == ButtonState.Pressed)
    //            connectedControllers[i].DPadR.Invoke();
    //        // Triggers
    //        if (playerStates[i * 2 + 1].Triggers.Left != 0f)
    //            connectedControllers[i].LT_Pressed.Invoke();
    //        else 
    //            connectedControllers[i].LT_Released.Invoke();
    //        if (playerStates[i * 2 + 1].Triggers.Right != 0f)
    //            connectedControllers[i].RT_Pressed.Invoke();
    //        else
    //            connectedControllers[i].RT_Released.Invoke();
    //        // Bumpers
    //        if (playerStates[i * 2 + 1].Buttons.LeftShoulder == ButtonState.Pressed && playerStates[i * 2].Buttons.LeftShoulder != ButtonState.Pressed)
    //            connectedControllers[i].LB_Pressed.Invoke();
    //        if (playerStates[i * 2 + 1].Buttons.RightShoulder == ButtonState.Pressed && playerStates[i * 2].Buttons.RightShoulder != ButtonState.Pressed)
    //            connectedControllers[i].RB_Pressed.Invoke();
    //        // Face Buttons
    //        if (playerStates[i * 2 + 1].Buttons.A == ButtonState.Pressed && playerStates[i * 2].Buttons.A != ButtonState.Pressed)
    //            connectedControllers[i].A_Pressed.Invoke();
    //        if (playerStates[i * 2 + 1].Buttons.B == ButtonState.Pressed && playerStates[i * 2].Buttons.B != ButtonState.Pressed)
    //            connectedControllers[i].B_Pressed.Invoke();
    //        if (playerStates[i * 2 + 1].Buttons.X == ButtonState.Pressed && playerStates[i * 2].Buttons.X != ButtonState.Pressed)
    //            connectedControllers[i].X_Pressed.Invoke();
    //        if (playerStates[i * 2 + 1].Buttons.Y == ButtonState.Pressed && playerStates[i * 2].Buttons.Y != ButtonState.Pressed)
    //            connectedControllers[i].Y_Pressed.Invoke();
    //        // Start and Back
    //        if (playerStates[i * 2 + 1].Buttons.Start == ButtonState.Pressed && playerStates[i * 2].Buttons.Start != ButtonState.Pressed)
    //            connectedControllers[i].Start.Invoke();
    //        if (playerStates[i * 2 + 1].Buttons.Back == ButtonState.Pressed && playerStates[i * 2].Buttons.Back != ButtonState.Pressed)
    //            connectedControllers[i].Back.Invoke();
    //    }
    //}

    ////
    ////  "ON" FUNCTIONS
    ////

    //void OnGUI()
    //{
    //    if (enableDebug)
    //    {
    //        string text = "--- Player 1 ---\n";
    //        text += string.Format("Connected: {0}\n", playerStates[1].IsConnected);
    //        text += string.Format("D-Pad Left: {0} | D-Pad Right: {1} \nD-Pad Up: {2} | D-Pad Down: {3}\n", playerStates[1].DPad.Left, playerStates[1].DPad.Right, playerStates[1].DPad.Up, playerStates[1].DPad.Down);
    //        text += string.Format("L-Stick X: {0} | L-Stick Y: {1} \nR-Stick X: {2} | R-Stick Y: {3}\n", playerStates[1].ThumbSticks.Left.X, playerStates[1].ThumbSticks.Left.Y, playerStates[1].ThumbSticks.Right.X, playerStates[1].ThumbSticks.Right.Y);
    //        text += string.Format("L-Stick Button: {0} | R-Stick Button: {1}\n", playerStates[1].Buttons.LeftStick, playerStates[1].Buttons.RightStick);
    //        text += string.Format("A: {0} | B: {1} | X: {2} | Y: {3}\n", playerStates[1].Buttons.A, playerStates[1].Buttons.B, playerStates[1].Buttons.X, playerStates[1].Buttons.Y);
    //        text += string.Format("L-Bumper: {0} | L-Trigger: {1} \nR-Bumper: {2} | R-Trigger: {3}\n", playerStates[1].Buttons.LeftShoulder, playerStates[1].Triggers.Left, playerStates[1].Buttons.RightShoulder, playerStates[1].Triggers.Right);
    //        text += string.Format("Start: {0} | Back: {1}", playerStates[1].Buttons.Start, playerStates[1].Buttons.Back);
    //        GUI.Label(new Rect(0, 0, Screen.width/2, Screen.height/2), text);

    //        text = "--- Player 2 ---\n";
    //        text += string.Format("Connected: {0}\n", playerStates[3].IsConnected);
    //        text += string.Format("D-Pad Left: {0} | D-Pad Right: {1} \nD-Pad Up: {2} | D-Pad Down: {3}\n", playerStates[3].DPad.Left, playerStates[3].DPad.Right, playerStates[3].DPad.Up, playerStates[3].DPad.Down);
    //        text += string.Format("L-Stick X: {0} | L-Stick Y: {1} \nR-Stick X: {2} | R-Stick Y: {3}\n", playerStates[3].ThumbSticks.Left.X, playerStates[3].ThumbSticks.Left.Y, playerStates[3].ThumbSticks.Right.X, playerStates[3].ThumbSticks.Right.Y);
    //        text += string.Format("L-Stick Button: {0} | R-Stick Button: {1}\n", playerStates[3].Buttons.LeftStick, playerStates[3].Buttons.RightStick);
    //        text += string.Format("A: {0} | B: {1} | X: {2} | Y: {3}\n", playerStates[3].Buttons.A, playerStates[3].Buttons.B, playerStates[3].Buttons.X, playerStates[3].Buttons.Y);
    //        text += string.Format("L-Bumper: {0} | L-Trigger: {1} \nR-Bumper: {2} | R-Trigger: {3}\n", playerStates[3].Buttons.LeftShoulder, playerStates[3].Triggers.Left, playerStates[3].Buttons.RightShoulder, playerStates[3].Triggers.Right);
    //        text += string.Format("Start: {0} | Back: {1}", playerStates[3].Buttons.Start, playerStates[3].Buttons.Back);
    //        GUI.Label(new Rect(Screen.width / 2, 0, Screen.width, Screen.height / 2), text);

    //        text = "--- Player 3 ---\n";
    //        text += string.Format("Connected: {0}\n", playerStates[5].IsConnected);
    //        text += string.Format("D-Pad Left: {0} | D-Pad Right: {1} \nD-Pad Up: {2} | D-Pad Down: {3}\n", playerStates[5].DPad.Left, playerStates[5].DPad.Right, playerStates[5].DPad.Up, playerStates[5].DPad.Down);
    //        text += string.Format("L-Stick X: {0} | L-Stick Y: {1} \nR-Stick X: {2} | R-Stick Y: {3}\n", playerStates[5].ThumbSticks.Left.X, playerStates[5].ThumbSticks.Left.Y, playerStates[5].ThumbSticks.Right.X, playerStates[5].ThumbSticks.Right.Y);
    //        text += string.Format("L-Stick Button: {0} | R-Stick Button: {1}\n", playerStates[5].Buttons.LeftStick, playerStates[5].Buttons.RightStick);
    //        text += string.Format("A: {0} | B: {1} | X: {2} | Y: {3}\n", playerStates[5].Buttons.A, playerStates[5].Buttons.B, playerStates[5].Buttons.X, playerStates[5].Buttons.Y);
    //        text += string.Format("L-Bumper: {0} | L-Trigger: {1} \nR-Bumper: {2} | R-Trigger: {3}\n", playerStates[5].Buttons.LeftShoulder, playerStates[5].Triggers.Left, playerStates[5].Buttons.RightShoulder, playerStates[5].Triggers.Right);
    //        text += string.Format("Start: {0} | Back: {1}", playerStates[5].Buttons.Start, playerStates[5].Buttons.Back);
    //        GUI.Label(new Rect(0, Screen.height / 2, Screen.width/2, Screen.height), text);

    //        text = "--- Player 4 ---\n";
    //        text += string.Format("Connected: {0}\n", playerStates[7].IsConnected);
    //        text += string.Format("D-Pad Left: {0} | D-Pad Right: {1} \nD-Pad Up: {2} | D-Pad Down: {3}\n", playerStates[7].DPad.Left, playerStates[7].DPad.Right, playerStates[7].DPad.Up, playerStates[7].DPad.Down);
    //        text += string.Format("L-Stick X: {0} | L-Stick Y: {1} \nR-Stick X: {2} | R-Stick Y: {3}\n", playerStates[7].ThumbSticks.Left.X, playerStates[7].ThumbSticks.Left.Y, playerStates[7].ThumbSticks.Right.X, playerStates[7].ThumbSticks.Right.Y);
    //        text += string.Format("L-Stick Button: {0} | R-Stick Button: {1}\n", playerStates[7].Buttons.LeftStick, playerStates[7].Buttons.RightStick);
    //        text += string.Format("A: {0} | B: {1} | X: {2} | Y: {3}\n", playerStates[7].Buttons.A, playerStates[7].Buttons.B, playerStates[7].Buttons.X, playerStates[7].Buttons.Y);
    //        text += string.Format("L-Bumper: {0} | L-Trigger: {1} \nR-Bumper: {2} | R-Trigger: {3}\n", playerStates[7].Buttons.LeftShoulder, playerStates[7].Triggers.Left, playerStates[7].Buttons.RightShoulder, playerStates[7].Triggers.Right);
    //        text += string.Format("Start: {0} | Back: {1}", playerStates[7].Buttons.Start, playerStates[7].Buttons.Back);
    //        GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, Screen.width, Screen.height), text);
    //    }
    //}
}
