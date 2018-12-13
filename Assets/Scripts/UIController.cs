using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [SerializeField]
    private Text score;
    [SerializeField]
    private Text highScore;
    [SerializeField]
    private InputField alias;
    [SerializeField]
    private Image mainMenu;
    [SerializeField]
    private GameObject inGameUI;

    [SerializeField]
    private Image previousResults;
    private bool isResultsVisible;
    [SerializeField]
    private Image previousResultsContent;
    [SerializeField]
    private Text previousResultsText;


    private void Start()
    {
        Instance = this;
        inGameUI.SetActive(false);
        previousResults.gameObject.SetActive(false);

        UpdateHighScore();
        GetAlias();
    }

    private void Update()
    {
        if (GameController.Instance.IsGameStarted)
            UpdateScore();
    }


    private void UpdateScore()
    {
        score.text = "Score: " + GameController.Instance.CoinsCollected.ToString();
    }

    private void UpdateHighScore()
    {
        if (PlayerPrefs.GetInt("highScore") < GameController.Instance.CoinsCollected)
        {
            PlayerPrefs.SetInt("highScore", GameController.Instance.CoinsCollected);
        }
        highScore.text = "High Score: " + PlayerPrefs.GetInt("highScore", 0);
    }

    public void StartGame()
    {
        mainMenu.gameObject.SetActive(false);
        inGameUI.SetActive(true);
    }

    public void EndGame()
    {
        mainMenu.gameObject.SetActive(true);
        inGameUI.SetActive(false);

        UpdateHighScore();
    }

    public void SetAlias()
    {
        PlayerPrefs.SetString("alias", alias.text);
    }

    public void GetAlias()
    {
        alias.text = PlayerPrefs.GetString("alias", "");
    }

    public void ShowPreviousResults()
    {
        isResultsVisible = !isResultsVisible;
        previousResults.gameObject.SetActive(isResultsVisible);

        UpdatePreviousResults();
    }

    private void UpdatePreviousResults()
    {
        foreach (Transform child in previousResultsContent.transform)
        {
            Destroy(child.gameObject);
        }

        var resContainer = XmlController.GetResults();
        resContainer.Results.Reverse();

        foreach (var res in resContainer.Results)
        {
            var resText = Instantiate(previousResultsText);
            resText.text = "Alias: " + res.alias + 
                "\nCollected " + res.coinsCollected + " coins" + 
                "\nLifetime: " + res.lifetime + " seconds" + 
                "\nStart date: " + res.startDate.ToString("dd/MM/yyyy HH:mm:ss") + 
                "\nEnd reason: " + res.endReason;
            resText.transform.SetParent(previousResultsContent.transform);
            resText.transform.localScale = Vector3.one;
        }
    }
}
