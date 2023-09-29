using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    public GameObject LevelWinPage;
    public GameObject LevelLosePage;
    public GameObject PauseMenu;
    public Button Pause;
    public Button NextLevel;
    public Button Replay1;
    public Button Replay2;
    public Button Resume;
    public Button Exit1;
    public Button Exit2;
    public Button Exit3;

    void Start()
    {
        Pause.onClick.AddListener(PauseGame);
        NextLevel.onClick.AddListener(NextLevelLoad);
        Replay1.onClick.AddListener(ReplayLevel);
        Replay2.onClick.AddListener(ReplayLevel);
        Resume.onClick.AddListener(ResumeGame);
        Exit1.onClick.AddListener(ExitToLobby);
        Exit2.onClick.AddListener(ExitToLobby);
        Exit3.onClick.AddListener(ExitToLobby);
    }

    private void PauseGame()
    {
        SoundManager.Instance.PlaySound(Sounds.PlayPause);
        PauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
    }

    private void NextLevelLoad()
    {
        SoundManager.Instance.PlaySound(Sounds.LevelClick);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        currentSceneIndex += 1;
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void ReplayLevel()
    {
        SoundManager.Instance.PlaySound(Sounds.PlayPause);
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    private void ResumeGame()
    {
        SoundManager.Instance.PlaySound(Sounds.PlayPause);
        PauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
    }

    private void ExitToLobby()
    {
        SoundManager.Instance.PlaySound(Sounds.BackExit);
        SceneManager.LoadScene(0);
    }
    public void LoadLevelWinPage()
    {
        LevelWinPage.SetActive(true);
    }
    public void LoadLevelLosePage()
    {
        LevelLosePage.SetActive(true);
    }

}
