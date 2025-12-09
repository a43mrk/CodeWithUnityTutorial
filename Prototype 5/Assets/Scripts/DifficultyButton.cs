using UnityEngine;
using UnityEngine.UI;
public enum Difficulty
{
    Easy = 1,
    Medium,
    Hard
}

public class DifficultyButton : MonoBehaviour
{
    private Button button;
    private GameManager gameManager;
    public Difficulty difficulty;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        button = GetComponent<Button>();
        button.onClick.AddListener(SetDifficulty);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetDifficulty()
    {
        Debug.Log(button.gameObject.name + " was clicked");
        gameManager.StartGame(difficulty);
    }
}
