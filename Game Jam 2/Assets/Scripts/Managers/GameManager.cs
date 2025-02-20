using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public enum EndScreen { None, GameOver, Win }
public enum Difficulty { Easy = 6, Normal = 9, Hard = 12 }

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
    public Canvas hudCanvas;

    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI winText;
    public Image lossTintOverlay;
    public Image winTintOverlay;

    [Header("Hiding Overlay")]
    public Image hidingOverlay;

    [Header("Audio")]
    public AudioSource lossSoundEffect;

    [Header("Difficulty Settings")]
    public Difficulty currentDifficulty = Difficulty.Normal;
    [Tooltip("Reference to the Doll's SphereCollider used for hearing detection.")]
    public SphereCollider dollSphereCollider;

    [Header("Difficulty Buttons")]
    public Button difficultyEasyButton;
    public Button difficultyNormalButton;
    public Button difficultyHardButton;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        startTime = Time.time;
        SetUIState(true, false, false, false);
        SetDifficulty(currentDifficulty);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();

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
            SetUIState(false, true, false, false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (isGameOver)
            {
                if (currentEndScreen == EndScreen.GameOver)
                    SetUIState(false, false, true, false);
                else if (currentEndScreen == EndScreen.Win)
                    SetUIState(false, false, false, true);
            }
            else
            {
                SetUIState(true, false, false, false);
            }
        }

        FirstPersonController fpc = FindFirstObjectByType<FirstPersonController>();
        if (fpc != null)
        {
            if (!isGameOver)
                fpc.enabled = !isPaused;
            else
                fpc.enabled = false;
        }
    }

    private void SetUIState(bool hud, bool pause, bool gameOver, bool win)
    {
        if (hudCanvas) hudCanvas.enabled = hud;
        if (pauseMenuCanvas) pauseMenuCanvas.enabled = pause;
        if (gameOverCanvas) gameOverCanvas.enabled = gameOver;
        if (winCanvas) winCanvas.enabled = win;
        if (lossTintOverlay) lossTintOverlay.gameObject.SetActive(gameOver);
        if (winTintOverlay) winTintOverlay.gameObject.SetActive(win);
        if (hidingOverlay != null)
            hidingOverlay.gameObject.SetActive(hidingOverlay.gameObject.activeSelf);
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
        SetUIState(false, false, true, false);
        if (gameOverText != null)
            gameOverText.text = "You Lost!";
        if (lossSoundEffect != null)
            lossSoundEffect.Play();
        currentEndScreen = EndScreen.GameOver;
        CameraShake cs = FindFirstObjectByType<CameraShake>();
        if (cs != null)
            cs.Shake();
    }

    public void WinGame()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;
        SetUIState(false, false, false, true);
        if (winText != null)
        {
            float finalTime = Time.time - startTime;
            winText.text = "You Escaped!\nTime: " + finalTime.ToString("F1") + "s";
        }
        currentEndScreen = EndScreen.Win;
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        if (dollSphereCollider != null)
            dollSphereCollider.radius = (int)currentDifficulty;

        UpdateDifficultyButtonOutlines();
    }

    public void SetDifficultyEasy()
    {
        SetDifficulty(Difficulty.Easy);
    }

    public void SetDifficultyNormal()
    {
        SetDifficulty(Difficulty.Normal);
    }

    public void SetDifficultyHard()
    {
        SetDifficulty(Difficulty.Hard);
    }

    private void UpdateDifficultyButtonOutlines()
    {
        UpdateButtonOutline(difficultyEasyButton, currentDifficulty == Difficulty.Easy);
        UpdateButtonOutline(difficultyNormalButton, currentDifficulty == Difficulty.Normal);
        UpdateButtonOutline(difficultyHardButton, currentDifficulty == Difficulty.Hard);
    }

    private void UpdateButtonOutline(Button button, bool isSelected)
    {
        if (!button.TryGetComponent<Outline>(out var outline))
            outline = button.gameObject.AddComponent<Outline>();
        outline.enabled = isSelected;
    }
}