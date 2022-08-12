using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugUI_ReloadLevel : MonoBehaviour
{
    public string keyCode = "r";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ReloadLevel()
    {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            ReloadLevel();
        }
    }
}
