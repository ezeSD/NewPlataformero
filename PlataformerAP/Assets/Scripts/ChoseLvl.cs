using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChoseLvl : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dropdown;
    string sceneToLoad;

    // Update is called once per frame
    void Update()
    {
        LvlSelect();
    }


    void LvlSelect()
    {
        if (dropdown != null)
        {

            if (dropdown.value == 0)
            {
                sceneToLoad = "Lvl1";
            }
            else if (dropdown.value == 1)
            {
                sceneToLoad = "Lvl2";
            }
            else if (dropdown.value == 2)
            {
                sceneToLoad = "Lvl3";
            }
    
        }
    
    }


    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }


}
