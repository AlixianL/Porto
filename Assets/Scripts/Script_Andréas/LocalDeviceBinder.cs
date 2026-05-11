using UnityEngine;
using UnityEngine.InputSystem;

public class LocalDeviceBinder : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private PlayerInput player1Input;
    [SerializeField] private PlayerInput player2Input;

    private void Start()
    {
        BindPlayer1ToKeyboard();
        BindPlayer2ToGamepad();
    }

    private void BindPlayer1ToKeyboard()
    {
        if (player1Input == null)
            return;

        player1Input.neverAutoSwitchControlSchemes = true;

        if (Keyboard.current != null && Mouse.current != null)
        {
            player1Input.SwitchCurrentControlScheme(
                "Keyboard&Mouse",
                Keyboard.current,
                Mouse.current
            );
        }

        player1Input.SwitchCurrentActionMap("Player-1");

        Debug.Log("Player 1 bind : Keyboard&Mouse");
    }

    private void BindPlayer2ToGamepad()
    {
        if (player2Input == null)
            return;

        player2Input.neverAutoSwitchControlSchemes = true;

        if (Gamepad.all.Count > 0)
        {
            player2Input.SwitchCurrentControlScheme(
                "Gamepad",
                Gamepad.all[0]
            );
        }
        else
        {
            Debug.LogWarning("Aucune manette détectée pour Player 2");
        }

        player2Input.SwitchCurrentActionMap("Player-2");

        Debug.Log("Player 2 bind : Gamepad");
    }
}