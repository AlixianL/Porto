using NUnit.Framework;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class Controller : MonoBehaviour
{
    [SerializeField] protected KeyCode DebugInput;
    [SerializeField] public Controller otherController;

    #region KeyboardControl
    [Header("Keyboard :", order = 1)]
    [Header("Set Param :", order = 1)]
    [SerializeField] protected bool isKeyboard;
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
    #endregion

    #region GamepadControl
    [Header("Gamepad :", order = 3)]
    [Header("Set Param :", order = 1)]
    [SerializeField] public int indexGamepad;
    protected Gamepad gamepad;
    #endregion



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        if (isKeyboard)
        {
            keyboard = Keyboard.current;
        }
        else
        {
            gamepad = Gamepad.all[0];
        }
    }
    
    // Update is called once per frame
    public virtual void Update()
    {
        if (isKeyboard)
        {
            KeyboardManager();
        }
        else
        {
            GamepadManager();
        }
    }

    #region Keyboard
    public virtual void KeyboardManager()
    {
        Horizontal(new Vector2 (XAxisInputK(), YAxisInputK()));
        if (Input.GetKeyDown(interactInput) )
        {
            InteractInput();
        }
        if (Input.GetKey(negativeForceInput))
        {
            NegativeForce();
        }
        if (Input.GetKey(PositiveForceInput))
        {
            PositiveForce();
        }
    }

    protected float XAxisInputK()
    {
        float value = 0f;
        if (Input.GetKey(negativeDirectionX))
        {
            value--;
        }
        if (Input.GetKey(positiveDirectionX))
        {
            value++;
        }
        return value;
    }

    protected float YAxisInputK()
    {
        float value = 0f;
        if (Input.GetKey(negativeDirectionY))
        {
            value--;
        }
        if (Input.GetKey(positiveDirectionY))
        {
            value++;
        }
        return value;
    }
    #endregion

    #region Gamepad
    public virtual void GamepadManager()
    {
        Horizontal(gamepad.leftStick.value);
        if (gamepad.buttonWest.wasPressedThisFrame)
        {
            InteractInput();
        }
        if (gamepad.leftShoulder.isPressed)
        {
            NegativeForce();
        }
        if (gamepad.rightShoulder.isPressed)
        {
            PositiveForce();
        }
        if (gamepad.buttonNorth.wasPressedThisFrame)
        {
            SwitchController();
        }
    }
    #endregion

    #region Commun Functions
    public virtual void SwitchController()
    {
        otherController.enabled = true;
        this.enabled = false;
    }

    public virtual void Horizontal(Vector2 direction)
    {

    }

    public virtual void InteractInput()
    {

    }

    public virtual void PositiveForce()
    {

    }

    public virtual void NegativeForce()
    {

    }
    #endregion
}
