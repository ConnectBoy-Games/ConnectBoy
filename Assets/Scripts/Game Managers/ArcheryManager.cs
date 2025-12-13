using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArcheryManager : MonoBehaviour
{
    public void GoToHome()
    {
        SceneManager.LoadSceneAsync("Main Scene");
    }
}
