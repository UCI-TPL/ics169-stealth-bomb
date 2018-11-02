using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XInputDotNetPure;

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

    // Set up controllers
    private void Awake() {
        for (int i = 0; i < 4; ++i)
            controllers[i] = new XboxController(i);
    }

    // Update every controller every frame
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
        public override void Vibrate(float strength, float duration) { }

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

        // LOTS OF ORGANIZING CODE so that things can be tested with just the enum values
        private Dictionary<ButtonCode, ButtonTest> ButtonMap = new Dictionary<ButtonCode, ButtonTest>();
        private Dictionary<JoyStickCode, TestJoyStick> JoyStickMap = new Dictionary<JoyStickCode, TestJoyStick>();
        private Dictionary<ActionCode, ButtonTest> ActionTests = new Dictionary<ActionCode, ButtonTest>();
        private Dictionary<ActionCode, ButtonEvent> ActionEvents = new Dictionary<ActionCode, ButtonEvent>();

        // Public variables showing what the controlls are currently mapped to
        public readonly Dictionary<ActionCode, HashSet<ButtonCode>> ButtonMaps = new Dictionary<ActionCode, HashSet<ButtonCode>>();
        public JoyStickCode moveJoyStick { get; private set; }
        public JoyStickCode aimJoyStick { get; private set; }

        public override Vector2 MoveVector() {
            return JoyStickMap[moveJoyStick]();
        }

        public override Vector2 AimVector() {
            return JoyStickMap[aimJoyStick]();
        }

        // Add a button mapping to the specified action
        public void AddButtonMapping(ActionCode action, ButtonCode button) {
            if (ButtonMaps[action].Add(button))
                ActionTests[action] += ButtonMap[button];
        }

        // Remove a button mapping from the specified action
        public void RemoveButtonMapping(ActionCode action, ButtonCode button) {
            if (ButtonMaps[action].Remove(button))
                ActionTests[action] -= ButtonMap[button];
        }

        // Remove all button mappings related to an action
        public void ClearButtonMapping(ActionCode action) {
            ButtonMaps[action].Clear();
            ActionTests[action].Clear();
        }

        // Remove all button mappings for all actions
        public void ClearAllButtonMapping() {
            foreach (ActionCode action in System.Enum.GetValues(typeof(ActionCode))) {
                ClearButtonMapping(action);
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
            SetMoveJoyStick(JoyStickCode.Left);
            SetAimJoyStick(JoyStickCode.Right);
        }

        // Update button events, invoking them if condition is met
        private void UpdateEvents() {
            foreach (ActionCode action in System.Enum.GetValues(typeof(ActionCode))) {
                ActionTests[action].Down(delegate { ActionEvents[action].OnDown.Invoke(); });
                ActionTests[action].Up(delegate { ActionEvents[action].OnUp.Invoke(); });
                bool testAll = false;
                foreach (TestEvent del in ActionTests[action].Pressed.GetInvocationList())
                    testAll = testAll || del();
                ActionEvents[action].Pressed = testAll;
            }
        }

        // Create a new Xbox controller with the specified player number
        public XboxController(int playerIndex) {
            type = Type.Xbox;
            this.playerIndex = (PlayerIndex)playerIndex;
            #region Button Defenitions
            ButtonMap.Add(ButtonCode.A, new ButtonTest(ADown, AUp, APressed));
            ButtonMap.Add(ButtonCode.RightBumper, new ButtonTest(RightBumperDown, RightBumperUp, RightBumperPressed));
            ButtonMap.Add(ButtonCode.LeftBumper, new ButtonTest(LeftBumperDown, LeftBumperUp, LeftBumperPressed));
            ButtonMap.Add(ButtonCode.RightTrigger, new ButtonTest(RightTriggerDown, RightTriggerUp, RightTriggerPressed));
            ButtonMap.Add(ButtonCode.LeftTrigger, new ButtonTest(LeftTriggerDown, LeftTriggerUp, LeftTriggerPressed));
            #endregion
            JoyStickMap.Add(JoyStickCode.Left, LeftJoyStickTest);
            JoyStickMap.Add(JoyStickCode.Right, RightJoyStickTest);

            ActionTests.Add(ActionCode.Attack, new ButtonTest());
            ActionTests.Add(ActionCode.Jump, new ButtonTest());
            ActionTests.Add(ActionCode.Dodge, new ButtonTest());

            ActionEvents.Add(ActionCode.Attack, attack);
            ActionEvents.Add(ActionCode.Jump, jump);
            ActionEvents.Add(ActionCode.Dodge, dodge);

            ButtonMaps.Add(ActionCode.Attack, new HashSet<ButtonCode>());
            ButtonMaps.Add(ActionCode.Jump, new HashSet<ButtonCode>());
            ButtonMaps.Add(ActionCode.Dodge, new HashSet<ButtonCode>());
            SetDefaultMapping();
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

        public override void Vibrate(float strength, float duration) {
            inputManager.StartCoroutine(VibrateDuration(strength, duration));
        }

        private IEnumerator VibrateDuration(float strength, float duration) {
            float endTime = Time.time + duration;
            GamePad.SetVibration(playerIndex, strength, strength);
            while (endTime >= Time.time) {
                yield return null;
            }
            GamePad.SetVibration(playerIndex, 0, 0);
        }

        // Update This controller's events
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

        // Container for test delegates coresponding to a events on a certain button
        private class ButtonTest {
            public EventCall Down;
            public EventCall Up;
            public TestEvent Pressed;
            public ButtonTest() {
                Clear();
            }

            public ButtonTest(EventCall Down, EventCall Up, TestEvent Pressed) {
                this.Down = Down;
                this.Up = Up;
                this.Pressed = Pressed;
            }

            // Clear all tests
            public void Clear() {
                Down = delegate { };
                Up = delegate { };
                Pressed = delegate { return false; };
            }

            // Add a button map
            public static ButtonTest operator +(ButtonTest l, ButtonTest r) {
                ButtonTest b = new ButtonTest(l.Down, l.Up, l.Pressed);
                b.Down += r.Down;
                b.Up += r.Up;
                b.Pressed += r.Pressed;
                return b;
            }

            // Remove a button map
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
        public abstract void Vibrate(float strength, float duration);
        public abstract void UpdateController();

        // Public Methods to get controller states
        public readonly ButtonEvent attack = new ButtonEvent();
        public readonly ButtonEvent jump = new ButtonEvent();
        public readonly ButtonEvent dodge = new ButtonEvent();
        public abstract Vector2 MoveVector();
        public abstract Vector2 AimVector();

        // List of every PlayerAction available
        public enum ActionCode {
            Attack, Jump, Dodge
        }

        // Type of Controller
        public enum Type {
            Xbox, MouseKeyboard
        }

        // Container for events of a specific action, such as attack or jump
        public class ButtonEvent {
            public readonly UnityEvent OnDown = new UnityEvent();
            public readonly UnityEvent OnUp = new UnityEvent();
            public bool Pressed;
        }
    }
}
