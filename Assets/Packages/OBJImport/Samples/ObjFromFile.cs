using Dummiesman;
using System.IO;
using UnityEngine;

public class ObjFromFile : MonoBehaviour
{
    string objPath = string.Empty;
    string error = string.Empty;
    //GameObject loadedObject;

    void OnGUI() {

        /***
        objPath = GUI.TextField(new Rect(0, 0, 256, 32), objPath);
        GUI.Label(new Rect(0, 0, 256, 32), "Obj Path:");
        if(GUI.Button(new Rect(256, 32, 64, 32), "Load File"))
        {
            LoadFile(objPath);
        }
        ***/

        if (!string.IsNullOrWhiteSpace(error))
        {
            GUI.color = Color.red;
            GUI.Box(new Rect(0, 64, 256 + 64, 32), error);
            GUI.color = Color.white;
        }
    }

    void Start()
    {
        //LoadFile(Application.streamingAssetsPath + "/dank_lego_man/tinker.obj");
    }

    public GameObject LoadFile(string objPath)
    {
        objPath = Application.streamingAssetsPath + "/DriveSync" + objPath;

        Debug.Log("Loading OBJ file: " + objPath);
        
        //file path
        if (!File.Exists(objPath))
        {
            error = "File doesn't exist: "+objPath;
            Debug.LogError(error);
            return null;
        }
        else
        {
            //if (loadedObject != null)
            //    Destroy(loadedObject);

            GameObject loadedObject = new OBJLoader().Load(objPath);

            loadedObject.transform.localScale *= 0.01f;// new Vector3(.1f, .1f, .1f);
            error = string.Empty;

            return loadedObject;
        }
    }
}
