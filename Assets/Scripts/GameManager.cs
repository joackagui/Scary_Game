using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private int notesCount = 0;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        
    }
    public int GetNotesCount()
    {
        return notesCount;
    }
    public void AddNote()
    {
        notesCount++;
    }
}