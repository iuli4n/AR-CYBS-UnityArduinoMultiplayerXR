using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachToSceneStep : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AttachToSceneWhenReady());
    }

    IEnumerator AttachToSceneWhenReady()
    {
        // it takes a bit for the scene to start, so this object only attaches when the scene is ready

        bool isDone = false;
        while (!isDone)
        {
            if (SceneStepsManager.Instance == null || SceneStepsManager.Instance.currentSceneRoot == null)
            {
                yield return new WaitForSeconds(.5f);
            }
            else
            {
                this.transform.parent = SceneStepsManager.Instance.currentSceneRoot.transform;
                isDone = true;
            }
        }
        Destroy(this);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
