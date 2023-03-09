
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMngr : MonoBehaviour
{
    private void OnEnable() {
        SingletonManager.Register(this);
    }

    private void OnDisable() {
        SingletonManager.Remove<SceneMngr>();
    }

    public void LoadGameScene() {
        SceneManager.LoadScene("GameScene");
    }

    public void LoadMainMenu() {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitGame() {
        Application.Quit();
    }
}
