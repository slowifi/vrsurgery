using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour
{
    public void SceneReset()
    {
        Scene scene = SceneManager.GetActiveScene();
        // SceneManager.LoadScene("SampleScene");
        SceneManager.LoadScene(scene.name);
    }
}
