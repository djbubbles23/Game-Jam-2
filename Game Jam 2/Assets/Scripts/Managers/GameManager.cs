using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public enum EndScreen { None, GameOver, Win }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private float startTime;
    private bool isGameOver = false;
    private EndScreen currentEndScreen = EndScreen.None;

    [Header("UI Canvases")]
    public Canvas pauseMenuCanvas;
    public Canvas gameOverCanvas;
    public Canvas winCanvas;
    public Canvas hudCanvas;  // For timer and in-game HUD

    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI winText;
    public Image lossTintOverlay; // Full-screen red tint for loss
    public Image winTintOverlay;  // Full-screen white tint for win

    [Header("Audio")]
    public AudioSource lossSoundEffect; // Placeholder loss sound

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Optionally persist: DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        startTime = Time.time;
        // At game start, only show the HUD.
        SetUIState(hud: true, pause: false, gameOver: false, win: false);
    }

    private void Update()
    {
        // Allow pausing regardless of game over.
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();

        // Update timer if game is active.
        if (!isPaused && timerText != null)
        {
            float currentTime = Time.time - startTime;
            timerText.text = "Time: " + currentTime.ToString("F1") + "s";
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            // When pausing, hide HUD and any end screens, show pause menu.
            SetUIState(hud: false, pause: true, gameOver: false, win: false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else // Unpausing.
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // If game is over, re-enable the appropriate end screen; otherwise, show HUD.
            if (isGameOver)
            {
                if (currentEndScreen == EndScreen.GameOver)
                    SetUIState(hud: false, pause: false, gameOver: true, win: false);
                else if (currentEndScreen == EndScreen.Win)
                    SetUIState(hud: false, pause: false, gameOver: false, win: true);
            }
            else
            {
                SetUIState(hud: true, pause: false, gameOver: false, win: false);
            }
        }

        // Disable/Enable player movement. (Assumes FirstPersonController handles camera movement.)
        FirstPersonController fpc = FindFirstObjectByType<FirstPersonController>();
        if (fpc != null)
            fpc.enabled = !isPaused;
    }

    /// <summary>
    /// Sets which UI canvases should be active.
    /// </summary>
    /// <param name="hud">Show the in-game HUD?</param>
    /// <param name="pause">Show the pause menu?</param>
    /// <param name="gameOver">Show the game over screen?</param>
    /// <param name="win">Show the win screen?</param>
    private void SetUIState(bool hud, bool pause, bool gameOver, bool win)
    {
        if (hudCanvas) hudCanvas.enabled = hud;
        if (pauseMenuCanvas) pauseMenuCanvas.enabled = pause;
        if (gameOverCanvas) gameOverCanvas.enabled = gameOver;
        if (winCanvas) winCanvas.enabled = win;
        if (lossTintOverlay) lossTintOverlay.gameObject.SetActive(gameOver);
        if (winTintOverlay) winTintOverlay.gameObject.SetActive(win);
    }

    public void RestartGame()
    {
        Debug.Log("RestartGame called");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;

        SetUIState(hud: false, pause: false, gameOver: true, win: false);
        if (gameOverText != null)
            gameOverText.text = "You Lost!";
        if (lossSoundEffect != null)
            lossSoundEffect.Play();

        currentEndScreen = EndScreen.GameOver;

        // Trigger screen shake if available.
        CameraShake cs = FindFirstObjectByType<CameraShake>();
        if (cs != null)
            cs.Shake();
    }

    public void WinGame()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;

        SetUIState(hud: false, pause: false, gameOver: false, win: true);
        if (winText != null)
        {
            float finalTime = Time.time - startTime;
            winText.text = "You Escaped!\nTime: " + finalTime.ToString("F1") + "s";
        }

        currentEndScreen = EndScreen.Win;
    }
}