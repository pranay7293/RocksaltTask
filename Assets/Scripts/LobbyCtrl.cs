using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyCtrl : MonoBehaviour
{
    [SerializeField]
    private Button[] levelButtons;
    public GameObject LevelMenu;
    public GameObject QuiteGamepage;
    public Button Play;
    public Button Quite;
    public Button Close;
    public Button Music;
    public Button Sound;

    private bool isMusicOn = true;
    private bool isSoundOn = true;

    void Start()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            levelButtons[i].onClick.AddListener(() => SceneManager.LoadScene(levelIndex));
            SoundManager.Instance.PlaySound(Sounds.LevelClick);
        }
        Music.onClick.AddListener(MuteMusic);
        Sound.onClick.AddListener(ChangeSound);
        Play.onClick.AddListener(LoadLevels);
        Quite.onClick.AddListener(QuiteGame);
        Close.onClick.AddListener(CloseLevelMenu);
    }

    private void CloseLevelMenu()
    {
        SoundManager.Instance.PlaySound(Sounds.BackExit);

        LevelMenu.SetActive(false);
    }

    private void QuiteGame()
    {
        SoundManager.Instance.PlaySound(Sounds.BackExit);
        QuiteGamepage.SetActive(true);
    }

    private void LoadLevels()
    {
        SoundManager.Instance.PlaySound(Sounds.PlayPause);
        LevelMenu.SetActive(true);
    }

    private void ChangeSound()
    {

        if (isSoundOn)
        {
            SoundManager.Instance.PlaySound(Sounds.Menu);
            SoundManager.Instance.StopSfx();
        }
        else
        {
            SoundManager.Instance.StartSfx();
            SoundManager.Instance.PlaySound(Sounds.Menu);
        }
        isSoundOn = !isSoundOn;
    }

    private void MuteMusic()
    {

        if (isMusicOn)
        {
            SoundManager.Instance.PlaySound(Sounds.Menu);
            SoundManager.Instance.StopMusic();
        }
        else
        {
            SoundManager.Instance.StartMusic();
            SoundManager.Instance.PlaySound(Sounds.Menu);
        }
        isMusicOn = !isMusicOn;
    }

}
