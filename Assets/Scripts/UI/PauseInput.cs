using UnityEngine;
using UnityEngine.InputSystem;

public class PauseInput : MonoBehaviour
{
    [SerializeField] private PauseMenuUI pauseMenu;
    private PlayerInputActions input;

    private void Awake()
    {
        input = new PlayerInputActions();
        input.UI.Pause.performed += _ => pauseMenu.TogglePause();
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();
}