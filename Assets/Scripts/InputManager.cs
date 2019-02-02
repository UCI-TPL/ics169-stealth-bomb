using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

public class InputManager : MonoBehaviour {

    // Returns the current inputManager
    private static InputManager _inputManager;
    private bool keyboardEnabled;
    Thread UpdateXInputStateThread = null;


    public static InputManager inputManager {
        get {
            if (_inputManager != null)
                return _inputManager;
            _inputManager = FindObjectOfType<InputManager>();
            if (_inputManager == null) {
                Debug.LogWarning("Input Manager not found, Created new Input Manager.");
                _inputManager = new GameObject("Input Manager").AddComponent<InputManager>();
                GameObject g = GameObject.Find("Managers");
                _inputManager.transform.SetParent(g != null ? g.transform : new GameObject("Managers").transform);
            }
            return _inputManager;
        }
    }

    public Controller[] controllers = new Controller[4];

    // Change the specified player's type of controller. Either xbox or mouse-and-keyboard
    public void ChangeControllerType(int playerIndex, Controller.Type controllerType) {
        switch (controllerType) {
            case Controller.Type.Xbox:
                controllers[playerIndex] = new XboxController(playerIndex);
                break;
            case Controller.Type.MouseKeyboard:
                controllers[playerIndex] = new MouseKeyboard(playerIndex);
                break;
            case Controller.Type.TouchScreen:
                controllers[playerIndex] = new TouchScreenController(playerIndex);
                break;
        }
    }

    // OLD(temporary method for turning on mouse and keyboard controls for player 1)
    // Current: if turnOn is true, turns on mouse and keyboard for first player in controller index without an active controller.
    //          if turnOn is false, turns off mouse and keyboard for all players.
    public void UseMouseAndKeyboardForFirstDisconnectedPlayer(bool turnOn) {
        if (turnOn) {
            for (int i = 0; i < controllers.Length; ++i) {
                if (controllers[i].type == Controller.Type.Xbox && !((XboxController) controllers[i]).isActive) {
                    ChangeControllerType(i, Controller.Type.MouseKeyboard);
                    break;
                }
            }
        }
        else {
            for (int i = 0; i < controllers.Length; ++i) {
                if (controllers[i].type == Controller.Type.MouseKeyboard) {
                    ChangeControllerType(i, Controller.Type.Xbox);
                }
            }
        }

        // if (turnOn) {
        //     ChangeControllerType(3, Controller.Type.MouseKeyboard);
        // }
        // else {
        //     ChangeControllerType(3, Controller.Type.Xbox);
        // }

        keyboardEnabled = turnOn;
    }

    // Used to scale controller joystick inputs to camera angle
    protected Vector2 cameraScale;

    // Set up controllers
    private void Awake() {
        if (inputManager != this) {
            Destroy(this);
            return;
        }
        // only temporary, need to make going from keyboard to controller more formal and streamlined!!!!!!!!!!
        // controllers[0] = new MouseKeyboard(0);
        keyboardEnabled = false;
        // for (int i = 1; i < 4; ++i)
        for (int i = 0; i < 4; ++i)
            controllers[i] = new XboxController(i);
        // ChangeControllerType(0, Controller.Type.MouseKeyboard);
#if UNITY_ANDROID || UNITY_IOS
        Debug.Log("Mobile Device Detected");
        ChangeControllerType(0, Controller.Type.TouchScreen);
        ChangeControllerType(1, Controller.Type.TouchScreen);
#endif

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        UpdateXInputStateThread = new Thread(UpdateXInputStateThreadLoop);
        UpdateXInputStateThread.Start();
#endif
    }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    private void UpdateXInputStateThreadLoop() {
        while (true) {
            foreach (Controller controller in controllers) {
                if (controller.type == Controller.Type.Xbox) {
                    XboxController c = (XboxController)controller;
                    if (c.stateUpdated) {
                        c.gettingState = true;
                        c.stateUpdated = false;
                        c.UpdateState(GamePad.GetState(c.playerIndex));
                    }
                }
            }
            Thread.Sleep(10);
        }
    }
#endif

    // Update every controller every frame
    private void Update() {
        if (SceneManager.GetActiveScene().name.Equals("mainMenu")) {
            bool keyboardAlreadyAssigned = false;
            for (int i = 0; i < controllers.Length; ++i) {
                if (keyboardEnabled) {
                    GamePadState testState = GamePad.GetState((PlayerIndex) i);
                    if (!testState.IsConnected /* && controllers[i].type != Controller.Type.MouseKeyboard */) {
                        if (/*controllers[i].type != Controller.Type.MouseKeyboard &&*/ !keyboardAlreadyAssigned) {
                            ChangeControllerType(i, Controller.Type.MouseKeyboard);
                            keyboardAlreadyAssigned = true;
                        }
                        else if (/* controllers[i].type == Controller.Type.MouseKeyboard && */ keyboardAlreadyAssigned) {
                            ChangeControllerType(i, Controller.Type.Xbox);
                        }
                        continue;
                    }
                    else {
                        if (testState.IsConnected && controllers[i].type == Controller.Type.MouseKeyboard) {
                            ChangeControllerType(i, Controller.Type.Xbox);
                        }
                    }
                }
                else {
                    if (controllers[i].type == Controller.Type.MouseKeyboard) {
                        ChangeControllerType(i, Controller.Type.Xbox);
                    }
                }
            }
        }

        // Debug.Log("players' controller types: " + controllers[0].type + ", " + controllers[1].type + ", " + controllers[2].type + ", " + controllers[3].type);

        cameraScale = new Vector2(Mathf.Sin(Mathf.Deg2Rad * Camera.main.transform.eulerAngles.x), 1);
        for (int i = 0; i < 4; ++i)
            controllers[i].UpdateController();
    }

    private void OnApplicationQuit() {
        for (int i = 0; i < 4; ++i)
            controllers[i].OnApplicationQuit();
    }

    private void OnDisable() {
        for (int i = 0; i < 4; ++i)
            if (controllers[i] != null)
                controllers[i].OnApplicationQuit();
    }

    #region mapping interface for UI
    /*
    - Kyle
    public method to update input mapping
     */
    public void mapInput(string action, string input, int player)
    {
        controllers[player].setMapping(action, input);
    }

    public void removeInputMapping( string action, string input, int player)
    {
        controllers[player].removeMapping(action, input);
    }

    
#endregion


    // Controller object for Mouse and Keyboard, This is not implemented yet
    public class MouseKeyboard : Controller {

        private int playerIndex;

        public override Vector2 MoveVector() {
            return new Vector2((Input.GetKey(KeyCode.D) ? 1 : 0) + (Input.GetKey(KeyCode.A) ? -1 : 0), (Input.GetKey(KeyCode.W) ? 1 : 0) + (Input.GetKey(KeyCode.S) ? -1 : 0)).normalized;
        }

        public override Vector2 AimVector() {
            Vector2 aim = Vector2.zero;
            Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(GameManager.instance.players[playerIndex].controller.transform.position);
            // Debug.Log("player screen position: " + playerScreenPosition + ", mouse position: " + Input.mousePosition);
            aim = new Vector2(Input.mousePosition.x - playerScreenPosition.x, Input.mousePosition.y - playerScreenPosition.y);
            // Debug.Log(aim);
            aim = (aim * inputManager.cameraScale).normalized;

            // old method of computing mouse aim. broken! dont use it!!!
            // Vector2 aim = Vector2.zero;
            // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // RaycastHit hit;
            // if (Physics.Raycast(ray, out hit)) {
            //     // aim vector using mouse and keyboard can not be found without the player's transform/position.
            //     Vector3 playerPosition = GameManager.instance.players[playerIndex].controller.transform.position;
            //     float aimX = hit.transform.position.x - playerPosition.x;
            //     float aimZ = hit.transform.position.z - playerPosition.z;
            //     aim = (new Vector2(aimX, aimZ)).normalized;
            //     Debug.Log(aim);
            //     // transform.LookAt(new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.z));
            // }
            // Debug.Log("mouse position: " + Input.mousePosition);
            return aim;
            // return (new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * inputManager.cameraScale).normalized;
            // return MoveVector();
        }

        public override void UpdateController() {
            if (Input.GetKeyDown(KeyCode.Space))
                jump.OnDown.Invoke();
            if (Input.GetKeyUp(KeyCode.Space))
                jump.OnUp.Invoke();
            jump.Pressed = Input.GetKey(KeyCode.Space);
            if (Input.GetKeyDown(KeyCode.Mouse0))
                attack.OnDown.Invoke();
            if (Input.GetKeyUp(KeyCode.Mouse0))
                attack.OnUp.Invoke();
            attack.Pressed = Input.GetKey(KeyCode.Mouse0);
            if (Input.GetKeyDown(KeyCode.LeftShift))
                dodge.OnDown.Invoke();
            if (Input.GetKeyUp(KeyCode.LeftShift))
                dodge.OnUp.Invoke();
            dodge.Pressed = Input.GetKey(KeyCode.LeftShift);
        }

        public MouseKeyboard(int playerIndex) {
            type = Type.MouseKeyboard;
            this.playerIndex = playerIndex;
        }
    }
    
    // Controller object for Xbox controllers
    public class XboxController : Controller {
        public PlayerIndex playerIndex;
        private GamePadState state;
        private GamePadState prevState;
        public bool stateUpdated = true;
        public bool gettingState = false;
        public bool isActive {
            get { return state.IsConnected && prevState.IsConnected; }
        }
        private delegate void Action();
        private delegate void EventCall(Action a);
        private delegate bool TestEvent();
        private delegate Vector2 TestJoyStick();

        // LOTS OF ORGANIZING CODE so that things can be tested with just the enum values
        private readonly Dictionary<ButtonCode, ButtonTest> ButtonMap = new Dictionary<ButtonCode, ButtonTest>();
        private readonly Dictionary<JoyStickCode, TestJoyStick> JoyStickMap = new Dictionary<JoyStickCode, TestJoyStick>();
        private readonly ButtonTest AttackTest = new ButtonTest();
        private readonly ButtonTest JumpTest = new ButtonTest();
        private readonly ButtonTest DodgeTest = new ButtonTest();
        private readonly ButtonTest StartTest = new ButtonTest();
        private readonly ButtonTest SwitchTest = new ButtonTest();

        // Public variables showing what the controlls are currently mapped to
        public readonly Dictionary<ActionCode, HashSet<ButtonCode>> ButtonMaps = new Dictionary<ActionCode, HashSet<ButtonCode>>();
        public JoyStickCode moveJoyStick { get; private set; }
        public JoyStickCode aimJoyStick { get; private set; }

        public override Vector2 MoveVector() {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return (JoyStickMap[moveJoyStick]() * inputManager.cameraScale).normalized * JoyStickMap[moveJoyStick]().magnitude;
#else
            return Vector3.zero;
#endif
        }

        public override Vector2 AimVector() {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return (JoyStickMap[aimJoyStick]() * inputManager.cameraScale).normalized;
#else
            return Vector3.zero;
#endif
        }

        // Add a button mapping to the specified action
        public void AddButtonMapping(ActionCode action, ButtonCode button) {
            if (ButtonMaps[action].Add(button))
                TestFromActionCode(action).AddTest(ButtonMap[button]);
        }

        // Remove a button mapping from the specified action
        public void RemoveButtonMapping(ActionCode action, ButtonCode button) {
            if (ButtonMaps[action].Remove(button))
                TestFromActionCode(action).RemoveTest(ButtonMap[button]);
        }

        // Remove all button mappings related to an action
        public void ClearButtonMapping(ActionCode action) {
            try {
                ButtonMaps[action].Clear();
                TestFromActionCode(action).Clear();
            }
            catch (KeyNotFoundException e) {
                Debug.LogError("Action Code not set up correctly");
            }
        }

        // Remove all button mappings for all actions
        public void ClearAllButtonMapping() {
            foreach (ActionCode action in System.Enum.GetValues(typeof(ActionCode))) {
                ClearButtonMapping(action);
            }
        }

        private ButtonTest TestFromActionCode(ActionCode code) {
            switch(code) {
                case ActionCode.Attack:
                    return AttackTest;
                case ActionCode.Dodge:
                    return DodgeTest;
                case ActionCode.Jump:
                    return JumpTest;
                case ActionCode.Start:
                    return StartTest;
                case ActionCode.Switch:
                    return SwitchTest;
                default:
                    return AttackTest;
            }
        }

        // Set the Movement controls to the specified joystick
        public void SetMoveJoyStick(JoyStickCode joyStickCode) {
            moveJoyStick = joyStickCode;
        }

        // Set the Aiming controls to the specified joystick
        public void SetAimJoyStick(JoyStickCode joyStickCode) {
            aimJoyStick = joyStickCode;
        }

        // Set controller to the default button mapping
        private void SetDefaultMapping() {
            ClearAllButtonMapping();
            AddButtonMapping(ActionCode.Attack, ButtonCode.RightBumper);
            AddButtonMapping(ActionCode.Attack, ButtonCode.RightTrigger);
            AddButtonMapping(ActionCode.Jump, ButtonCode.LeftBumper);
            AddButtonMapping(ActionCode.Dodge, ButtonCode.LeftTrigger);
            AddButtonMapping(ActionCode.Start, ButtonCode.Start);
            AddButtonMapping(ActionCode.Switch, ButtonCode.Y);
            SetMoveJoyStick(JoyStickCode.Left);
            SetAimJoyStick(JoyStickCode.Right);
        }

        // Update button events, invoking them if condition is met
        private void UpdateEvents() {
            AttackTest.Down(delegate { attack.OnDown.Invoke(); });
            AttackTest.Up(delegate { attack.OnUp.Invoke(); });
            bool testAll = false;
            foreach (TestEvent del in AttackTest.Pressed)
                testAll = testAll || del();
            attack.Pressed = testAll;

            JumpTest.Down(delegate { jump.OnDown.Invoke(); });
            JumpTest.Up(delegate { jump.OnUp.Invoke(); });
            testAll = false;
            foreach (TestEvent del in JumpTest.Pressed)
                testAll = testAll || del();
            jump.Pressed = testAll;

            DodgeTest.Down(delegate { dodge.OnDown.Invoke(); });
            DodgeTest.Up(delegate { dodge.OnUp.Invoke(); });
            testAll = false;
            foreach (TestEvent del in DodgeTest.Pressed)
                testAll = testAll || del();
            dodge.Pressed = testAll;

            StartTest.Down(delegate { start.OnDown.Invoke(); });
            StartTest.Up(delegate { start.OnUp.Invoke(); });
            testAll = false;
            foreach (TestEvent del in StartTest.Pressed)
                testAll = testAll || del();
            start.Pressed = testAll;

            SwitchTest.Down(delegate { Switch.OnDown.Invoke(); });
            SwitchTest.Up(delegate { start.OnUp.Invoke(); });
            testAll = false;
            foreach (TestEvent del in StartTest.Pressed)
                testAll = testAll || del();
            Switch.Pressed = testAll;

        }

        // Create a new Xbox controller with the specified player number
        public XboxController(int playerIndex) {
            type = Type.Xbox;
            this.playerIndex = (PlayerIndex)playerIndex;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            GamePad.GetState(this.playerIndex, GamePadDeadZone.None);
#region Button Defenitions
            ButtonMap.Add(ButtonCode.A, new ButtonTest(ADown, AUp, APressed));
            ButtonMap.Add(ButtonCode.B, new ButtonTest(BDown, BUp, BPressed));
            ButtonMap.Add(ButtonCode.X, new ButtonTest(XDown, XUp, XPressed));
            ButtonMap.Add(ButtonCode.Y, new ButtonTest(YDown, YUp, YPressed));
            ButtonMap.Add(ButtonCode.Up, new ButtonTest(UpDown, UpUp, UpPressed));
            ButtonMap.Add(ButtonCode.Down, new ButtonTest(DownDown, DownUp, DownPressed));
            ButtonMap.Add(ButtonCode.Left, new ButtonTest(LeftDown, LeftUp, LeftPressed));
            ButtonMap.Add(ButtonCode.Right, new ButtonTest(RightDown, RightUp, RightPressed));
            ButtonMap.Add(ButtonCode.Back, new ButtonTest(BackDown, BackUp, BackPressed));
            ButtonMap.Add(ButtonCode.Start, new ButtonTest(StartDown, StartUp, StartPressed));
            ButtonMap.Add(ButtonCode.Guide, new ButtonTest(GuideDown, GuideUp, GuidePressed));
            ButtonMap.Add(ButtonCode.LeftStick, new ButtonTest(LeftStickDown, LeftStickUp, LeftStickPressed));
            ButtonMap.Add(ButtonCode.RightStick, new ButtonTest(RightStickDown, RightStickUp, RightStickPressed));
            ButtonMap.Add(ButtonCode.RightBumper, new ButtonTest(RightBumperDown, RightBumperUp, RightBumperPressed));
            ButtonMap.Add(ButtonCode.LeftBumper, new ButtonTest(LeftBumperDown, LeftBumperUp, LeftBumperPressed));
            ButtonMap.Add(ButtonCode.RightTrigger, new ButtonTest(RightTriggerDown, RightTriggerUp, RightTriggerPressed));
            ButtonMap.Add(ButtonCode.LeftTrigger, new ButtonTest(LeftTriggerDown, LeftTriggerUp, LeftTriggerPressed));
#endregion
            JoyStickMap.Add(JoyStickCode.Left, LeftJoyStickTest);
            JoyStickMap.Add(JoyStickCode.Right, RightJoyStickTest);
            JoyStickMap.Add(JoyStickCode.DPad, DPadJoyStickTest);

            ButtonMaps.Add(ActionCode.Attack, new HashSet<ButtonCode>());
            ButtonMaps.Add(ActionCode.Jump, new HashSet<ButtonCode>());
            ButtonMaps.Add(ActionCode.Dodge, new HashSet<ButtonCode>());
            ButtonMaps.Add(ActionCode.Start, new HashSet<ButtonCode>());
            ButtonMaps.Add(ActionCode.Switch, new HashSet<ButtonCode>());
            SetDefaultMapping();
#endif
        }



        /*
        - Kyle
        input mapping customization
        */
#region  mapping customization
        public override void setMapping(string a, string b)
        {
            //  Debug.Log("add new mapping: button: " + b + " action: " + a);
            if (a == "move")
            {
                if (b == "leftStick")
                    SetMoveJoyStick(JoyStickCode.Left);
                else if (b == "rightStick")
                    SetMoveJoyStick(JoyStickCode.Right);
            }
            else if ( a == "aim")
            {
                if (b == "leftStick")
                    SetAimJoyStick(JoyStickCode.Left);
                else if (b == "rightStick")
                    SetAimJoyStick(JoyStickCode.Right);
            }
            else
            {
                Controller.ActionCode newAction = getAction(a);
                XboxController.ButtonCode newButton = GetButton(b);
                AddButtonMapping(newAction, newButton);
            }
        }

        public override void removeMapping(string a, string b)
        {
            // if (a == "move")
            // {
            //     if (b == "leftStick")
            //         SetMoveJoyStick(JoyStickCode.Left);
            //     else if (b == "rightStick")
            //         SetMoveJoyStick(JoyStickCode.Right);
            // }
            // else if ( a == "aim")
            // {
            //     if (b == "leftStick")
            //         SetAimJoyStick(JoyStickCode.Left);
            //     else if (b == "rightStick")
            //         SetAimJoyStick(JoyStickCode.Right);
            // }
            // else
            // {
                if (b == "leftStick" || b == "rightSTick")
                    return;
                Controller.ActionCode oldAction = getAction(a);
                XboxController.ButtonCode oldButton = GetButton(b);
                RemoveButtonMapping(oldAction, oldButton);
            // }
        }

        private Controller.ActionCode getAction(string a)
        {
         
            Debug.Log("Here we are with "+a);
            if ( a == "attack")
                return Controller.ActionCode.Attack;
            else if ( a=="dodge")
                return Controller.ActionCode.Dodge;
            else if ( a=="jump")
                return Controller.ActionCode.Jump;
            
            return Controller.ActionCode.Attack;
        }

        private XboxController.ButtonCode GetButton(string b)
        {
            if ( b == "leftBumper")
                return XboxController.ButtonCode.LeftBumper;
            else if (b == "leftTrigger")
                return XboxController.ButtonCode.LeftTrigger;
            else if (b == "rightBumper")
                return XboxController.ButtonCode.RightBumper;
            else if (b == "rightTrigger")
                return XboxController.ButtonCode.RightTrigger;
            else if (b == "A")
                return XboxController.ButtonCode.A;
            else if (b == "B")
                return XboxController.ButtonCode.B;
            else if (b == "X")
                return XboxController.ButtonCode.X;
            else if (b == "Y")
                return XboxController.ButtonCode.Y;

            return XboxController.ButtonCode.A;
        }
#endregion

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

        private void BDown(Action action) {
            if (ButtonDown(state.Buttons.B, prevState.Buttons.B))
                action();
        }

        private void BUp(Action action) {
            if (ButtonUp(state.Buttons.B, prevState.Buttons.B))
                action();
        }

        private bool BPressed() {
            return ButtonPressed(state.Buttons.B);
        }

        private void XDown(Action action) {
            if (ButtonDown(state.Buttons.X, prevState.Buttons.X))
                action();
        }

        private void XUp(Action action) {
            if (ButtonUp(state.Buttons.X, prevState.Buttons.X))
                action();
        }

        private bool XPressed() {
            return ButtonPressed(state.Buttons.X);
        }

        private void YDown(Action action) {
            if (ButtonDown(state.Buttons.Y, prevState.Buttons.Y))
                action();
        }

        private void YUp(Action action) {
            if (ButtonUp(state.Buttons.Y, prevState.Buttons.Y))
                action();
        }

        private bool YPressed() {
            return ButtonPressed(state.Buttons.Y);
        }

        private void UpDown(Action action) {
            if (ButtonDown(state.DPad.Up, prevState.DPad.Up))
                action();
        }

        private void UpUp(Action action) {
            if (ButtonUp(state.DPad.Up, prevState.DPad.Up))
                action();
        }

        private bool UpPressed() {
            return ButtonPressed(state.DPad.Up);
        }

        private void DownDown(Action action) {
            if (ButtonDown(state.DPad.Down, prevState.DPad.Down))
                action();
        }

        private void DownUp(Action action) {
            if (ButtonUp(state.DPad.Down, prevState.DPad.Down))
                action();
        }

        private bool DownPressed() {
            return ButtonPressed(state.DPad.Down);
        }

        private void LeftDown(Action action) {
            if (ButtonDown(state.DPad.Left, prevState.DPad.Left))
                action();
        }

        private void LeftUp(Action action) {
            if (ButtonUp(state.DPad.Left, prevState.DPad.Left))
                action();
        }

        private bool LeftPressed() {
            return ButtonPressed(state.DPad.Left);
        }

        private void RightDown(Action action) {
            if (ButtonDown(state.DPad.Right, prevState.DPad.Right))
                action();
        }

        private void RightUp(Action action) {
            if (ButtonUp(state.DPad.Right, prevState.DPad.Right))
                action();
        }

        private bool RightPressed() {
            return ButtonPressed(state.DPad.Right);
        }

        private void BackDown(Action action) {
            if (ButtonDown(state.Buttons.Back, prevState.Buttons.Back))
                action();
        }

        private void BackUp(Action action) {
            if (ButtonUp(state.Buttons.Back, prevState.Buttons.Back))
                action();
        }

        private bool BackPressed() {
            return ButtonPressed(state.Buttons.Back);
        }

        private void GuideDown(Action action) {
            if (ButtonDown(state.Buttons.Guide, prevState.Buttons.Guide))
                action();
        }

        private void GuideUp(Action action) {
            if (ButtonUp(state.Buttons.Guide, prevState.Buttons.Guide))
                action();
        }

        private bool GuidePressed() {
            return ButtonPressed(state.Buttons.Guide);
        }

        private void StartDown(Action action) {
            if (ButtonDown(state.Buttons.Start, prevState.Buttons.Start))
                action();
        }

        private void StartUp(Action action) {
            if (ButtonUp(state.Buttons.Start, prevState.Buttons.Start))
                action();
        }

        private bool StartPressed() {
            return ButtonPressed(state.Buttons.Start);
        }

        private void LeftStickDown(Action action) {
            if (ButtonDown(state.Buttons.LeftStick, prevState.Buttons.LeftStick))
                action();
        }

        private void LeftStickUp(Action action) {
            if (ButtonUp(state.Buttons.LeftStick, prevState.Buttons.LeftStick))
                action();
        }

        private bool LeftStickPressed() {
            return ButtonPressed(state.Buttons.LeftStick);
        }

        private void RightStickDown(Action action) {
            if (ButtonDown(state.Buttons.RightStick, prevState.Buttons.RightStick))
                action();
        }

        private void RightStickUp(Action action) {
            if (ButtonUp(state.Buttons.RightStick, prevState.Buttons.RightStick))
                action();
        }

        private bool RightStickPressed() {
            return ButtonPressed(state.Buttons.RightStick);
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

        private Vector2 DPadJoyStickTest() {
            return new Vector2((ButtonPressed(state.DPad.Right) ? 1 : 0) + (ButtonPressed(state.DPad.Left) ? -1 : 0), (ButtonPressed(state.DPad.Up) ? 1 : 0) + (ButtonPressed(state.DPad.Down) ? -1 : 0)).normalized;
        }

        public override void Vibrate(float strength, float duration, VibrationMode vibrationMode) {
            switch (vibrationMode) {
                case VibrationMode.Flat:
                    inputManager.StartCoroutine(VibrateDuration(strength, duration));
                    break;
                case VibrationMode.Diminish:
                    inputManager.StartCoroutine(VibrateDiminish(strength, duration));
                    break;
            }
        }

        private IEnumerator VibrateDuration(float strength, float duration) {
            float endTime = Time.unscaledTime + duration;
            while (endTime >= Time.unscaledTime) {
                GamePad.SetVibration(playerIndex, strength, strength);
                yield return null;
            }
            GamePad.SetVibration(playerIndex, 0, 0);
        }

        private IEnumerator VibrateDiminish(float strength, float duration) {
            float endTime = Time.unscaledTime + duration;
            float newStrength;
            while (endTime >= Time.unscaledTime) {
                newStrength = strength * (-Mathf.Pow(1 - (endTime - Time.unscaledTime) / duration, 2) + 1);
                GamePad.SetVibration(playerIndex, newStrength, newStrength);
                yield return null;
            }
            GamePad.SetVibration(playerIndex, 0, 0);
        }

        public override void OnApplicationQuit() {
            GamePad.SetVibration(playerIndex, 0, 0);
        }

        // Update This controller's events
        public override void UpdateController() {
            //State is updated in a different thread
            if (stateUpdated)
                UpdateState(state);
            if (isActive)
                UpdateEvents();
            stateUpdated = true;
        }

        public void UpdateState(GamePadState state) {
            prevState = this.state;
            this.state = state;
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
            A, B, X, Y, Up, Down, Left, Right, LeftBumper, RightBumper, Back, Start, LeftStick, RightStick, LeftTrigger, RightTrigger, Guide
        }

        // List of every JoyStick available
        public enum JoyStickCode {
            Left, Right, DPad
        }

        // Container for test delegates coresponding to a events on a certain button
        private class ButtonTest {
            public EventCall Down;
            public EventCall Up;
            public HashSet<TestEvent> Pressed = new HashSet<TestEvent>();
            public ButtonTest() {
                Clear();
            }

            public ButtonTest(EventCall Down, EventCall Up, TestEvent Pressed) {
                this.Down = Down;
                this.Up = Up;
                this.Pressed.Add(Pressed);
            }

            // Clear all tests
            public void Clear() {
                Down = delegate { };
                Up = delegate { };
                Pressed.Clear();
            }

            public void AddTest(ButtonTest other) {
                Down += other.Down;
                Up += other.Up;
                Pressed.UnionWith(other.Pressed);
            }

            public void RemoveTest(ButtonTest other) {
                Down -= other.Down;
                Up -= other.Up;
                Pressed.ExceptWith(other.Pressed);
            }

            //// Add a button map
            //public static ButtonTest operator +(ButtonTest l, ButtonTest r) {
            //    ButtonTest b = new ButtonTest(l.Down, l.Up, l.Pressed);
            //    b.Down += r.Down;
            //    b.Up += r.Up;
            //    b.Pressed += r.Pressed;
            //    return b;
            //}

            //// Remove a button map
            //public static ButtonTest operator -(ButtonTest l, ButtonTest r) {
            //    ButtonTest b = new ButtonTest(l.Down, l.Up, l.Pressed);
            //    b.Down -= r.Down;
            //    b.Up -= r.Up;
            //    b.Pressed -= r.Pressed;
            //    return b;
            //}
        }
    }    // Controller object for Mouse and Keyboard, This is not implemented yet
    public class TouchScreenController : Controller {
        private int playerNumber;
        private TouchControlsUI touchControlsUI;
        private TouchControlsUI TouchControlsUI {
            get {
                if (touchControlsUI != null)
                    return touchControlsUI;
                touchControlsUI = FindObjectOfType<TouchControlsUI>();
                if (touchControlsUI == null)
                    Debug.LogError("touchControlsUI not found");
                return touchControlsUI;
            }
        }
        private int RightTouchID;
        private Vector2 RightTouchStart = -Vector2.one;
        private Vector2 RightTouchEnd = -Vector2.one;
        private int LeftTouchID;
        private Vector2 LeftTouchStart = -Vector2.one;
        private Vector2 LeftTouchEnd = -Vector2.one;

        public override Vector2 MoveVector() {
            Vector2 direction = RightTouchEnd - RightTouchStart;
            return RightTouchStart.x >= 0 ? ( direction.magnitude > TouchControlsUI.JoystickRadiusRatio.x * Camera.main.pixelWidth ? direction.normalized : direction / Camera.main.pixelRect.size / TouchControlsUI.JoystickRadiusRatio ) : Vector2.zero;
        }

        public override Vector2 AimVector() {
            Vector2 direction = LeftTouchEnd - LeftTouchStart;
            return LeftTouchStart.x >= 0 ? (direction.magnitude > TouchControlsUI.JoystickRadiusRatio.x * Camera.main.pixelWidth ? direction.normalized : direction / Camera.main.pixelRect.size / TouchControlsUI.JoystickRadiusRatio ) : Vector2.zero;
        }

        public override void UpdateController() {
            bool anyTouch = false;
            foreach (Touch touch in Input.touches)
                if (touch.phase == TouchPhase.Began)
                    anyTouch = true;
            if (anyTouch)
                start.OnDown.Invoke();

            if (RightTouchStart.x < 0) {
                foreach (Touch touch in Input.touches) {
                    if (playerNumber == 0) {
                        if (touch.phase == TouchPhase.Began && touch.position.x < Camera.main.pixelWidth / 2 && touch.position.y > Camera.main.pixelHeight / 2) {
                            RightTouchID = touch.fingerId;
                            RightTouchStart = touch.position;
                            TouchControlsUI.SetRightJoystickPosition(RightTouchStart / Camera.main.pixelRect.size, playerNumber);
                            break;
                        }
                    }
                    else if (playerNumber == 1) {
                        if (touch.phase == TouchPhase.Began && touch.position.x > Camera.main.pixelWidth / 2 && touch.position.y < Camera.main.pixelHeight / 2) {
                            RightTouchID = touch.fingerId;
                            RightTouchStart = touch.position;
                            TouchControlsUI.SetRightJoystickPosition(RightTouchStart / Camera.main.pixelRect.size, playerNumber);
                            break;
                        }
                    }
                }
            }
            foreach (Touch touch in Input.touches) {
                if (touch.fingerId == RightTouchID) {
                    if (touch.phase == TouchPhase.Ended) {
                        RightTouchStart = -Vector2.one;
                        TouchControlsUI.HideRightJoystick(playerNumber);
                        break;
                    }
                    RightTouchEnd = touch.position;
                    TouchControlsUI.SetRightJoystickDirection(MoveVector(), playerNumber);
                    break;
                }
            }

            if (LeftTouchStart.x < 0) {
                foreach (Touch touch in Input.touches) {
                    if (playerNumber == 0) {
                        if (touch.phase == TouchPhase.Began && touch.position.x < Camera.main.pixelWidth / 2 && touch.position.y < Camera.main.pixelHeight / 2) {
                            LeftTouchID = touch.fingerId;
                            LeftTouchStart = touch.position;
                            TouchControlsUI.SetLeftJoystickPosition(LeftTouchStart / Camera.main.pixelRect.size, playerNumber);
                            attack.OnDown.Invoke();
                            break;
                        }
                    }
                    else if (playerNumber == 1) {
                        if (touch.phase == TouchPhase.Began && touch.position.x > Camera.main.pixelWidth / 2 && touch.position.y > Camera.main.pixelHeight / 2) {
                            LeftTouchID = touch.fingerId;
                            LeftTouchStart = touch.position;
                            TouchControlsUI.SetLeftJoystickPosition(LeftTouchStart / Camera.main.pixelRect.size, playerNumber);
                            attack.OnDown.Invoke();
                            break;
                        }
                    }
                }
            }
            foreach (Touch touch in Input.touches) {
                if (touch.fingerId == LeftTouchID) {
                    if (touch.phase == TouchPhase.Ended) {
                        LeftTouchStart = -Vector2.one;
                        TouchControlsUI.HideLeftJoystick(playerNumber);
                        attack.OnUp.Invoke();
                        break;
                    }
                    LeftTouchEnd = touch.position;
                    TouchControlsUI.SetLeftJoystickDirection(AimVector(), playerNumber);
                    break;
                }
            }
            
            attack.Pressed = AimVector() != Vector2.zero;
        }

        public TouchScreenController(int playerNumber) {
            type = Type.TouchScreen;
            this.playerNumber = playerNumber;
        }
    }

    public abstract class Controller {
        public Type type;
        // Public Methods to get controller states
        public readonly ButtonEvent attack = new ButtonEvent();
        public readonly ButtonEvent jump = new ButtonEvent();
        public readonly ButtonEvent dodge = new ButtonEvent();
        public readonly ButtonEvent start = new ButtonEvent();
        public readonly ButtonEvent Switch = new ButtonEvent();
        public abstract Vector2 MoveVector();
        public abstract Vector2 AimVector();

        public virtual void Vibrate(float strength, float duration, VibrationMode vibrationMode = VibrationMode.Flat) { }
        public virtual void OnApplicationQuit() { }
        public abstract void UpdateController();

        public virtual void setMapping( string b, string a) {}
        public virtual void removeMapping( string b, string a) {}

        // List of every PlayerAction available
        public enum ActionCode {
            Attack, Jump, Dodge, Start, Switch, testymode
        }

        // Type of Controller
        public enum Type {
            Xbox, MouseKeyboard, TouchScreen
        }

        public enum VibrationMode {
            Flat, Diminish
        }

        // Container for events of a specific action, such as attack or jump
        public class ButtonEvent {
            public readonly UnityEvent OnDown = new UnityEvent();
            public readonly UnityEvent OnUp = new UnityEvent();
            public bool Pressed;
        }
    }
}
