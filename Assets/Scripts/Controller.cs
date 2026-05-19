using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [SerializeField] protected KeyCode debugInput;
    [SerializeField] public Controller controllerToSwitch;

    #region KeyboardControl
    [Header("Keyboard :", order = 1)]
    [Header("Set Param :", order = 1)]
    [SerializeField] public bool isKeyboard;
    [SerializeField] protected int indexKeyboard;
    protected Keyboard keyboard;

    [Header("Horizontal Y", order = 1)]
    [SerializeField] protected KeyCode positiveDirectionX;
    [SerializeField] protected KeyCode negativeDirectionX;

    [Header("Horizontal X", order = 1)]
    [SerializeField] protected KeyCode positiveDirectionY;
    [SerializeField] protected KeyCode negativeDirectionY;

    [Header("Force", order = 1)]
    [SerializeField] protected KeyCode PositiveForceInput;
    [SerializeField] protected KeyCode negativeForceInput;

    [Header("Interact", order = 1)]
    [SerializeField] protected KeyCode interactInput;
    [SerializeField] protected KeyCode SwitchControllerInput;
    #endregion

    #region GamepadControl
    [Header("Gamepad :", order = 3)]
    [Header("Set Param :", order = 1)]
    [SerializeField] public int indexGamepad;
    protected Gamepad gamepad;
    #endregion

    public virtual void Start()
    {
        RefreshInputDevice();
    }

    public virtual void Update()
    {
        if (isKeyboard)
            KeyboardManager();
        else
            GamepadManager();
    }

    public void RefreshInputDevice()
    {
        if (isKeyboard)
        {
            keyboard = Keyboard.current;
            return;
        }

        gamepad = GetGamepadByIndex(indexGamepad);
    }

    protected Gamepad GetGamepadByIndex(int index)
    {
        if (Gamepad.all.Count == 0)
            return null;

        if (index < 0 || index >= Gamepad.all.Count)
            return null;

        return Gamepad.all[index];
    }

    #region Keyboard
    public virtual void KeyboardManager()
    {
        Horizontal(new Vector2(XAxisInputK(), YAxisInputK()));

        if (Input.GetKeyDown(interactInput))
            InteractInput();

        if (Input.GetKey(negativeForceInput))
            NegativeForce();

        if (Input.GetKey(PositiveForceInput))
            PositiveForce();

        if (Input.GetKeyDown(SwitchControllerInput))
            SwitchController();
    }

    protected float XAxisInputK()
    {
        float value = 0f;

        if (Input.GetKey(negativeDirectionX))
            value--;

        if (Input.GetKey(positiveDirectionX))
            value++;

        return value;
    }

    protected float YAxisInputK()
    {
        float value = 0f;

        if (Input.GetKey(negativeDirectionY))
            value--;

        if (Input.GetKey(positiveDirectionY))
            value++;

        return value;
    }
    #endregion

    #region Gamepad
    public virtual void GamepadManager()
    {
        if (gamepad == null)
            gamepad = GetGamepadByIndex(indexGamepad);

        if (gamepad == null)
            return;

        Horizontal(gamepad.leftStick.value);

        if (gamepad.buttonWest.wasPressedThisFrame)
            InteractInput();

        if (gamepad.leftTrigger.isPressed)
            NegativeForce();

        if (gamepad.rightTrigger.isPressed)
            PositiveForce();

        if (gamepad.buttonNorth.wasPressedThisFrame)
            SwitchController();
    }
    #endregion

    #region Common Functions
    public virtual void SwitchController()
    {
        if (controllerToSwitch != null)
            controllerToSwitch.enabled = true;

        enabled = false;
    }

    public virtual void Horizontal(Vector2 direction)
    {
    }

    public virtual void InteractInput()
    {
        print("InteractInput");
    }

    public virtual void PositiveForce()
    {
    }

    public virtual void NegativeForce()
    {
    }
    #endregion
}