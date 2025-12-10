using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public List<GameObject> targets;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI gameOverText;
    public Button restartButton;
    public GameObject titleScreen;

    private float spawnRate = 1.0f;
    private int score = 0;
    public bool isGameActive;
    public int totalLives = 3;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateVolume(0.5f);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = $"Score: {score}";
    }

    private System.Collections.IEnumerator SpawnTarget()
    {
        while(isGameActive)
        {
            yield return new WaitForSeconds(spawnRate);
            int index = Random.Range(0, targets.Count);
            Instantiate(targets[index]);
        }

    }

    public void GameOver()
    {
        isGameActive = false;
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    public bool IsGameOver()
    {
        return gameOverText.gameObject.active;
    }

    // In the Button’s inspector, click + to add a new On Click event, drag it in the "Game Manager" object
    // from Hierarchy and select the GameManager.RestartGame function
    public void RestartGame()
    {
        Debug.Log("Restarting Game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame(Difficulty difficulty)
    {
        spawnRate /= (int)difficulty;

        isGameActive = true;
        StartCoroutine(SpawnTarget());
        UpdateScore(0);

        titleScreen.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(true);

        UpdateLives();
        livesText.gameObject.SetActive(true);
    }

    private void UpdateLives()
    {
        livesText.text = $"Lives: {totalLives}";
    }

    public void ReduceLife()
    {
        --totalLives;

        UpdateLives();

        if(totalLives <= 0)
        {
            GameOver();
        }
    }

    public void UpdateVolume(float intensity)
    {
        audioSource.volume = intensity;
    }
}
