using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    public void MoveToScene(string scene)
    {
        if (SceneManager.GetActiveScene().name == "RunScene" && scene == "MainMenu") 
        {
            GameObject girl = GameObject.Find("Girl");
            girl.GetComponent<PlayerController>().DisableUIControls();
            girl.GetComponent<AttributeController>().EndRun();
        }
        SceneManager.LoadScene(scene);
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void UnPause() {GameObject.Find("Girl").GetComponent<PlayerController>().OnPause();}
}