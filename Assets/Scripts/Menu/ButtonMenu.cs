using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class ButtonMenu : MonoBehaviour
    {
        public void OnPlayButtonClickedToGame()
        {
            SceneManager.LoadScene("Game");
        }
        
        public void OnPlayButtonClickedToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}