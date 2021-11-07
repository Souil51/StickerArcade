using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public Text TimerText;
    public Slider TimerSlider;

    public GameObject PanelEndText;
    public Text EndText;

    public GameObject PanelStartText;
    public Text StartText;

    public Text ScoreText;

    public GameObject PanelNewLevelText;
    public Text NewLevelText;

    public GameObject PanelGameOverText;
    public Text GameOverText;

    public GameObject PanelObjectif;
    public Text ObjectifText;
    public GameObject PanelScore;
    public Button ButtonStop;
    public GameObject PanelMenu;
    public Image GreenCross;
    public Image ImageAudio;

    public Text LevelText;

    public Sprite SpriteAudioON;
    public Sprite SpriteAudioOFF;

    public void UpdateTimer(float fTime, float fTimerDuration, bool bDirection)
    {
        if (fTime == 0)
        {
            TimerText.text = "0";

            if (bDirection)
                TimerSlider.value = 1;
            else
                TimerSlider.value = 0;
        }
        else
        {
            TimerText.text = (((int)fTime) + 1).ToString();

            float fValue = fTime / fTimerDuration;

            if (bDirection)
                TimerSlider.value = 1 - fValue;
            else
                TimerSlider.value = fValue;
        }
    }

    public void ShowStartMessage(string szText)
    {
        PanelStartText.SetActive(true);
        StartText.text = szText;
    }

    public void HideStartMessage()
    {
        PanelStartText.SetActive(false);
    }

    public string GetStartMessage()
    {
        return StartText.text;
    }

    public void ShowEndMessage(string szText)
    {
        PanelEndText.SetActive(true);
        EndText.text = szText;
    }

    public void HideEndMessage()
    {
        PanelEndText.SetActive(false);
    }

    public void UpdateScore(int nbPoints)
    {
        ScoreText.text = nbPoints + " points";
    }

    public void ShowNewLevelText(bool bDisplay, int nObjectif, int nLevel)
    {
        NewLevelText.text = "Round " + nLevel + " !\n\n Objectif : " + nObjectif;

        PanelNewLevelText.SetActive(bDisplay);
    }

    public void ShowGameOverText(bool bDisplay, int nPoints)
    {
        GameOverText.text = "Tu n'as pas atteint l'objectif !\n\nGAME OVER\n\nTu as fait " + nPoints + " points";

        PanelGameOverText.SetActive(bDisplay);
    }

    public void ShowPanelObjectif(bool bDisplay)
    {
        PanelObjectif.SetActive(bDisplay);
    }

    public void UpdateObjectif(int nObjectif, int nScore)
    {
        ObjectifText.text = "Objectif : \n" + nScore + " / " + nObjectif;
    }

    public void ShowPermanentUI(bool bDisplay)
    {
        TimerText.gameObject.SetActive(bDisplay);
        TimerSlider.gameObject.SetActive(bDisplay);
        PanelScore.gameObject.SetActive(bDisplay);
        ButtonStop.gameObject.SetActive(bDisplay);
        LevelText.gameObject.SetActive(bDisplay);
    }
    
    public void SetLevelText(int nLevel)
    {
        LevelText.text = "Round " + nLevel.ToString();
    }

    public void ShowMenu(bool bDisplay)
    {
        PanelMenu.SetActive(bDisplay);
    }

    public void DisplayGreenCross(bool bDisplay)
    {
        GreenCross.gameObject.SetActive(bDisplay);
    }

    public void SetAudioEnable(bool bEnabled)
    {
        ImageAudio.sprite = bEnabled ? SpriteAudioON : SpriteAudioOFF;
    }
}
