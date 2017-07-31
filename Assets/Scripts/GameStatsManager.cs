using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStatsManager : MonoBehaviour {

    public Text scoreText;
    public RectTransform newHighscore;
    public int score;

    public RectTransform menuUI, endGameUI, exitConfirmationUI;


    bool exitConfirmaion;

    void Start() {
        Reset();
    }

    void UpdateUI() {
        scoreText.text = "" + score;
    }

    public void Reset() {
        menuUI.gameObject.SetActive(true);
        endGameUI.gameObject.SetActive(false);
        exitConfirmationUI.gameObject.SetActive(false);
        score = GetBestScore();
        UpdateUI();
    }

    public void GameStarted() {
        menuUI.gameObject.SetActive(false);
        score = 0;
        UpdateUI();
    }

    public void AddScore() {
        score++;
        UpdateUI();
    }

    public int GetBestScore() {
        return PlayerPrefs.GetInt("score_kills");
    }

    public void Exit() {
        if (!Application.isMobilePlatform && !Application.isWebPlayer) {
            if (!exitConfirmaion) {
                exitConfirmaion = true;
                exitConfirmationUI.gameObject.SetActive(true);
            }
            else {
                Application.Quit();
            }
        }
    }

    public void ExitCancle() {
        exitConfirmaion = false;
        exitConfirmationUI.gameObject.SetActive(false);
    }

    public void Save() {
        if (score > GetBestScore()) {
            PlayerPrefs.SetInt("score_kills", score);
            newHighscore.gameObject.SetActive(true);
        }
        else {
            newHighscore.gameObject.SetActive(false);
        }
        endGameUI.gameObject.SetActive(true);
    }
}
