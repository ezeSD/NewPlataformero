using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    [SerializeField] string SceneName;


   public void SceneSwich()
   {
        SceneManager.LoadScene(SceneName);
   }


}
