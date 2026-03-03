using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishFlag : MonoBehaviour, Finish
{

    public void ChangeScene(string scene)
    {
       SceneManager.LoadScene(scene);
    }


}
