using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    public void MoveToScene(string sceneStr)
    {
        string[] s  = sceneStr.Split(".");
        string scene = s[0];
        string menu = s[1];
        if (menu == "pause" || menu == "win" || menu == "lose")
        {
            GameObject girl = GameObject.Find("Girl");
            if (menu == "pause")
            {
                girl.GetComponent<PlayerController>().OnPause();
                girl.GetComponent<AttributeController>().EndRun();
            }
            else if (menu == "win") 
            {
                girl.GetComponent<PlayerController>().Win();
                if (scene == "MainMenu") girl.GetComponent<AttributeController>().EndRun();
            }
            else if (menu == "lose")
            {
                girl.GetComponent<PlayerController>().Lose();
                girl.GetComponent<AttributeController>().EndRun();
            }
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