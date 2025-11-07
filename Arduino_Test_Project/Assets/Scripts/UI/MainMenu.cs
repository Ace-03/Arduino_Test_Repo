using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject tutorialMenu;

    public void OnStart()
    {
        SceneManager.LoadScene(1);
    }

    public void OnBack()
    {
        SetScreen(mainMenu);
    }

    public void OnTutorialScreen()
    {
        SetScreen(tutorialMenu);
    }

    private void SetScreen(GameObject screen)
    {
        mainMenu.SetActive(false);
        tutorialMenu.SetActive(false);

        screen.SetActive(true);
    }
}
