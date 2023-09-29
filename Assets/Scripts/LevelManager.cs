using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public enum GameLevel
    {
        Level1,
        Level2,
        Level3,
        Level4,
    }

    public GameLevel currentLevel;
    public int targetScore; 
    public int movesAllowed;
    public float timeLimit;
    public int targetRedElements;
    public int targetBlueElements;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI blueCount;
    public TextMeshProUGUI redCount;
    public TextMeshProUGUI timeLeft;
    public Slider scoreSlider;
    public LevelUI levelUI;

    private bool isTargetReached = false;
    private bool isBlueCleared = false;
    private bool isRedCleared = false;

    private int totalScore = 0;
    

    private void Start()
    {
        UpdateMovesText(movesAllowed); 
        UpdateScoreText(totalScore);
        UpdateBlueCount(targetBlueElements);
        UpdateRedCount(targetRedElements);
    }

    private void Update()
    {
        if(currentLevel == GameLevel.Level3)
        {
            timeLimit -= Time.deltaTime;
            if (timeLimit <= 0)
            {
                UpdateTimer(0);
                if (!isTargetReached)
                {
                    GameOver(false);
                }
            }
            else
            {
                UpdateTimer(timeLimit);
            }
        }        
    }

    public void MakeMove()
    {
        movesAllowed--;

        if (movesAllowed <= 0 && !isTargetReached)
        {
            UpdateMovesText(0);
            GameOver(false); 
        }
        else
        {
            UpdateMovesText(movesAllowed);
        }
    }

    public void UpdateScore(int newScore)
    {
        totalScore += newScore;

        UpdateScoreText(totalScore);
        if (totalScore >= targetScore && !isTargetReached)
        {
            GameOver(true);
            isTargetReached = true;
        }
    }

    public void BlueElementsCleared(int count)
    {
        targetBlueElements -= count;
        if (targetBlueElements <= 0)
        {
            isBlueCleared = true;
            UpdateBlueCount(0);
            CheckLevelCompletion();
        }
        else
        {
            UpdateBlueCount(targetBlueElements);

        }
    }
    public void RedElementsCleared(int count)
    {
        targetRedElements -= count;
        if (targetRedElements <= 0)
        {
            isRedCleared = true;
            UpdateRedCount(0);
            CheckLevelCompletion();
        }
        else
        {
            UpdateRedCount(targetRedElements);
        }
    }
    public void CheckLevelCompletion()
    {
        if (isBlueCleared && currentLevel == GameLevel.Level2)
        {
            GameOver(true );
        }
        else if (isBlueCleared && isRedCleared && currentLevel == GameLevel.Level4)
        {
            GameOver(true);
        }
    }

    private void UpdateMovesText(int moves)
    {
        movesText.text = "Moves: " + moves;
    }

    private void UpdateBlueCount(int count)
    {
        blueCount.text = count.ToString();
    }

    private void UpdateRedCount(int count)
    {
        redCount.text = count.ToString();
    }

    public void UpdateTimer(float timer)
    {
        int remainingtime = (int)timer;
        timeLeft.text = "Time Left: " + remainingtime;
    }

    private void UpdateScoreText(int currentScore)
    {
        scoreText.text = "Score: " + currentScore;
        scoreSlider.value = currentScore;
    }

    private void GameOver(bool win)
    {
        if (win)
        {
            SoundManager.Instance.PlaySound(Sounds.LevelCompleted);
            Invoke(nameof(levelUI.LevelWinPage), 3f);
        }
        else
        {

            SoundManager.Instance.PlaySound(Sounds.LevelFailed);
            Invoke(nameof(levelUI.LevelLosePage), 3f);
        }
    }
}
