using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StudentProjectSceneManager : MonoBehaviour
{
    // DESCRIPTION: This keeps track of what student project we're currently in, and what scene step we're currently on
    // Most of this information is used by SceneStepsManager

    public static StudentProjectSceneManager Instance;

    //public /*readonly*/ bool isInProject = false;
    public string CurrentProjectName { get; private set; }  // relative path within any Resources folder, pointing to a folder containing scene prefabs
    public int CurrentSceneIndex { get; private set; }
    public string CurrentScenePrefabName { get; private set; }

    //public int TempSceneIndex { get { return pc_prefabScenes.Length - 1;  } }

    public string CurrentScenePrefabLocationFull { get {
            return GetFullPathForScene(CurrentScenePrefabName);
        }
    }

    private string GetFullPathForScene(string scenePrefabName)
    {
        return resourcesRootFolder + "/" + CONST_studentProjectsFolderName + "/" + CurrentProjectName + "/" + scenePrefabName + ".prefab";
    }
    /*
    public string CurrentScenePrefabLocationShort
    {
        get
        {
            return CONST_studentProjectsFolderName + "/" + CurrentProjectName + "/" + CurrentScenePrefabName;
        }
    }
    */

    public GameObject CurrentScenePrefabFromDisk { get; private set; }

    public GameObject[] pc_prefabScenes;

    string CONST_studentProjectsFolderName = "StudentProjects";
    string resourcesRootFolder = "Assets/Iulian_Temp/Resources";

    // list of student project paths on PC
    DirectoryInfo[] projectFolders;


    private void Awake()
    {
        Debug.Assert(Instance == null, "Should not have multiple instances of this object!");
        Instance = this;

        Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        DirectoryInfo levelDirectoryPath = new DirectoryInfo(Application.dataPath + "\\Iulian_Temp\\Resources\\StudentProjects");
        projectFolders = levelDirectoryPath.GetDirectories();
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string[] GetProjectNames()
    {

        // TODO:PERF: cache this and update only if the projects change

        string[] names = new string[projectFolders.Length];
        for (int i = 0; i < names.Length; i++)
        {
            names[i] = projectFolders[i].Name;
        }
        return names;
    }


    public void Clear()
    {
        // means we're unloading the current project
        pc_prefabScenes = null;
        CurrentProjectName = "";
        CurrentSceneIndex = -1;
        CurrentScenePrefabName = "";
    }
    public void SwitchToProject(string name)
    {
        // updates the project name and loads the pc_prefabScenes, but doesn't switch to a scene

        CurrentProjectName = name;
        CurrentSceneIndex = -1;

#if UNITY_EDITOR
        pc_prefabScenes = Resources.LoadAll<GameObject>(CONST_studentProjectsFolderName + "/" + CurrentProjectName);
#endif
    }
    public void SwitchToScene(int i)
    {
        // Switches to point to a scene in the current project, and updates the prefab gameobject that has that scene info
        // This doesn't load the actual scene objects into the current project (that's done somewhere else, like SceneStepsManager)

        CurrentSceneIndex = i;

#if UNITY_EDITOR
        GameObject prefa = pc_prefabScenes[i];
        CurrentScenePrefabName = prefa.name;
        CurrentScenePrefabFromDisk = prefa;
#endif
    }

    /**
    public string GetTempSceneDiskLocation()
    {
        GameObject prefa = pc_prefabScenes[TempSceneIndex];
        return GetFullPathForScene(prefa.name);
    }
    public GameObject GetTempScenePrefab()
    {
        return pc_prefabScenes[TempSceneIndex]; ;
    }
    **/ 
}
