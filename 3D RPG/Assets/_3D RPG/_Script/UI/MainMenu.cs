using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    Button newGameButton;
    Button continueGameButton;
    Button quitGameButton;

    private void Awake()
    {
        newGameButton = transform.GetChild(1).GetComponent<Button>();
        continueGameButton = transform.GetChild(2).GetComponent<Button>();
        quitGameButton = transform.GetChild(3).GetComponent<Button>();

        quitGameButton.onClick.AddListener(QuitGame);
        newGameButton.onClick.AddListener(NewGame);
        continueGameButton.onClick.AddListener(ContinueGame);
    }

    void NewGame()
    {
        PlayerPrefs.DeleteAll();

        //ת������
        SceneController.Instance.TransitionToFirstLevel();
    }

    void ContinueGame()
    {
        //ת����������ȡ����
        SceneController.Instance.TransitionToLoadGame();
    }

    void QuitGame()
    {
        Application.Quit();
        Debug.Log("�˳���Ϸ");
    }
}
