using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Settings")]
    public string gameOverSceneName = "GameOver";

    private int notesCount = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public int GetNotesCount()
    {
        return notesCount;
    }

    public void AddNote()
    {
        notesCount++;

        if (notesCount >= 5)
            TriggerGameOver();
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Invoke(nameof(LoadGameOverScene), 2.5f);
    }

    private void LoadGameOverScene()
    {
        notesCount = 0;
        isGameOver = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(gameOverSceneName);
    }
}