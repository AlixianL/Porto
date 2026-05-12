using UnityEngine;
using UnityEngine.InputSystem;

public class CarExitInputBridge : MonoBehaviour
{
    [SerializeField] private Controller controller;
    [SerializeField] private KeyCode keyboardExitKey = KeyCode.E;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<Controller>();
    }

    private void Update()
    {
        if (controller == null)
            return;

        if (!controller.enabled)
            return;

        if (controller.isKeyboard)
        {
            if (Input.GetKeyDown(keyboardExitKey))
                controller.SwitchController();
        }
        else
        {
            if (controller.indexGamepad >= 0 && controller.indexGamepad < Gamepad.all.Count)
            {
                Gamepad gamepad = Gamepad.all[controller.indexGamepad];

                if (gamepad.buttonWest.wasPressedThisFrame || gamepad.buttonEast.wasPressedThisFrame)
                    controller.SwitchController();
            }
        }
    }
}