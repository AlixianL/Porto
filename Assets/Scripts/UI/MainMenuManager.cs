using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string tutorialSceneName = "TutorialScene";
    [SerializeField] private GameObject optionsPanel;

    public void PlayTutorial()
    {
        SceneManager.LoadScene(tutorialSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }
}