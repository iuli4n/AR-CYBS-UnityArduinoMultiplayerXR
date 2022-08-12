using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;

public class FileDriveManager : MonoBehaviour
{
    public bool debug_gui = true;
    public static FileDriveManager Instance;

    public string ueserverAddress = "ftp://127.0.0.1:10021";

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Assert(!Instance, "Should only have one type of this object");

        Instance = this;
    }

    private void OnEnable()
    {
        RefreshOBJFiles();
    }

    public string StreamingDrivePath() {
        return Application.streamingAssetsPath + "/DriveSync";
    }

    #region SimpleHelpers

    private String GetFilelistText(String wholeAddress)
    {
        // gets the list of folders & files as a text

        String dirlist;

        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(wholeAddress);
        request.Credentials = new NetworkCredential("anonymous", "");
        request.Timeout = 5000;
        request.KeepAlive = false;

        Debug.Log(" ************* Getting file list from [" + request.RequestUri + "]     [" + wholeAddress + "]");

        request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        Debug.Log("Got response: " + response.StatusDescription);

        // Parse the response
        Stream responseStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(responseStream);
        dirlist = StringHack(reader.ReadToEnd());
        reader.Close();
        request.Abort();
        response.Close();

        return dirlist;
    }

    HashSet<String> getDirsFromDirlist(String dirlist)
    {
        HashSet<String> list = new HashSet<String>();
        foreach (String f in dirlist.Split('\n'))
            if (f.StartsWith("d"))
                list.Add(f.Substring(50).Trim()) ;
        return list;
    }
    HashSet<String> getFilesFromDirlist(String dirlist)
    {
        HashSet<String> list = new HashSet<String>();
        foreach (String f in dirlist.Split('\n'))
            if (f.StartsWith("-"))
                list.Add(f.Substring(50).Trim());
        return list;
    }


    String CombinePath(String parentFolders, String child)
    {
        // always returns someting like this: / or /this or /this/that

        if (parentFolders == "") parentFolders = "/";
        if (!parentFolders.EndsWith("/")) parentFolders += "/";

        return (parentFolders + child).Trim();
    }

    private String StringHack(String utf8String)
    {
        // tries to clean up the string in case it has weird characters
        byte[] utf8array = System.Text.Encoding.UTF8.GetBytes(utf8String);
        String result = "";
        foreach (byte b in utf8array)
        {
            result += (char)b;
        }
        return result;
    }


    private void IndicateWorkingStart()
    {
        DebugUI_WorkingStatus.Instance.OpenProgressIndicator();
    }
    private void IndicateWorkingEnd()
    {
        DebugUI_WorkingStatus.Instance.CloseProgressIndicator();
    }


    IEnumerator DownloadFile_FTP(String ftpAddress, String ftpPath, String localPath)
    {
        // Get the object used to communicate with the server.
        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpAddress + ftpPath);
        request.Method = WebRequestMethods.Ftp.DownloadFile;
        request.Credentials = new NetworkCredential("anonymous", "");

        Debug.Log("Downloading " + request.RequestUri);

        FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        Stream responseStream = response.GetResponseStream();

        Debug.Log("File path: " + localPath);
        var fileStream = File.Create(localPath);
        responseStream.CopyTo(fileStream);
        
        fileStream.Close();
        response.Close();

        Debug.Log("Done storing file in " + localPath);

        yield break;
    }

    #endregion



    

    IEnumerator MirrorFiles(String ftpAddress, String ftpBaseRelPath, String localBaseRelPath, int debug_depth = 0)
    {
        Debug.Assert(ftpBaseRelPath.StartsWith("/"));
        if (debug_depth > 10)
        {
            Debug.LogError("EXITING DUE TO BIG DEPTH");
            yield break;
        }

        string localBasePath = StreamingDrivePath() + ftpBaseRelPath;

        String dirlist = GetFilelistText(StringHack(ftpAddress + ftpBaseRelPath));

        Debug.Log("---\n" + dirlist + "\n-----\n");

        HashSet<String> ftpDirs = getDirsFromDirlist(dirlist);
        HashSet<String> ftpFiles = getFilesFromDirlist(dirlist);


        // CALCULATE DELTAS

        HashSet<String> dirsNew = new HashSet<string>();
        HashSet<String> dirsDeleted = new HashSet<string>();

        foreach (string name in ftpDirs)
        {
            String localPath = CombinePath(localBasePath, name);
            if (!Directory.Exists(localPath))
            {
                // local directory doesn't exist, meaning it's new
                dirsNew.Add(name);
            }
        }
        foreach (string name in Directory.GetDirectories(localBasePath))
        {
            string name2 = name.Substring(localBasePath.Length);
            //Debug.Log("GETDIRECTORIES: " + name);
            if (!ftpDirs.Contains(name2))
            {
                // ftp directory doesn't exist, meaning it's been deleted
                dirsDeleted.Add(name2);
            }
        }

        HashSet<String> filesNew = new HashSet<string>();
        HashSet<String> filesDeleted = new HashSet<string>();

        foreach (string name in ftpFiles)
        {
            //Debug.Log("GETFTPFILES: " + name);
            String localPath = CombinePath(localBasePath, name);
            if (!File.Exists(localPath))
            {
                // local directory doesn't exist, meaning it's new
                filesNew.Add(name);
            }
        }
        foreach (string name in Directory.GetFiles(localBasePath))
        {
            string name2 = name.Substring(localBasePath.Length);
            //Debug.Log("GETLOCALFILES: " + name);
            if (!ftpFiles.Contains(name2))
            {
                // ftp directory doesn't exist, meaning it's been deleted
                filesDeleted.Add(name2);
            }
        }



        // PROCESS DELTAS

        const bool skipSync = false;
        foreach (string n in dirsNew)
        {
            Debug.Log("DIR NEW: " + n);
            //if (!skipSync) 
                Directory.CreateDirectory(CombinePath(localBasePath, n));
            // will be populated when we recurse into it
        }
        foreach (string n in dirsDeleted)
        {
            Debug.Log("DIR REMOVED: " + n);
            if (!skipSync) Directory.Delete(CombinePath(localBasePath, n), true);
        }
        foreach (string n in filesDeleted)
        {
            Debug.Log("FILE REMOVED: " + n);
            if (!skipSync) File.Delete(CombinePath(localBasePath, n));
        }
        foreach (string n in filesNew)
        {
            Debug.Log("FILE NEW: " + n);
            String ftpFullPath = CombinePath(ftpBaseRelPath, n);
            String localFullPath = CombinePath(localBasePath, n);
            if (!skipSync) yield return DownloadFile_FTP(ftpAddress, ftpFullPath, localFullPath);
        }


        // now recursively go into all the directories found on the ftp

        foreach (string n in ftpDirs)
        {
            /// BUG: I think we need to close previous connection
            /// also add coroutine waiting. 
            yield return MirrorFiles(ftpAddress, 
                CombinePath(ftpBaseRelPath, n + "/"), 
                CombinePath(localBasePath, n), 
                debug_depth + 1);
        }

        yield break;


        // OLDER STUFF

        foreach (String n in ftpDirs)
        {
            Debug.Log("DIR: " + n);
            
            String localPath = StreamingDrivePath() + CombinePath(localBasePath, n);
            Debug.Log("DIR2: " + localPath);
            Directory.CreateDirectory(localPath);

            /// BUG: I think we need to close previous connection
            /// also add coroutine waiting. 
            yield return MirrorFiles(ftpAddress, CombinePath(ftpBaseRelPath, n+"/"), CombinePath(localBasePath, n), debug_depth+1);
            
        }
        foreach (String n in ftpFiles)
        {
            Debug.Log("FILE: " + n);

            String ftpPath = CombinePath(ftpBaseRelPath, n);
            String localPath = StreamingDrivePath() + CombinePath(localBasePath, n);
            
            yield return DownloadFile_FTP(ftpAddress, ftpPath, localPath);
        }

        //Debug.Log($"Directory List Complete, status {response.StatusDescription}");

        
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    List<string> objFileNames = new List<string>();

    void RefreshOBJFiles()
    {
        objFileNames.Clear();
        string path = StreamingDrivePath();
        string[] entries = Directory.GetFileSystemEntries(path, "*.obj", SearchOption.AllDirectories);
        foreach (string s in entries)
        {
            string r = s.Replace("\\", "/");
            //Debug.Log(r);
            objFileNames.Add(r.Substring(path.Length)); //r.IndexOf("/DriveSync/")));
        }
    }

    IEnumerator RefreshDriveCoroutine(string serverAddress)
    {
        IndicateWorkingStart();

        yield return MirrorFiles(serverAddress, "/", "/");
        RefreshOBJFiles();

        IndicateWorkingEnd();
    }



    private void GenerateModel(string path)
    {
        PhotonView.Get(this).RPC("RPC_GenerateModel", RpcTarget.AllViaServer, path);
    }

    [PunRPC]
    void RPC_GenerateModel(string path)
    {

#if UNITY_EDITOR
        SceneStepsManager.Instance.CreateAndConfigureNewObjectForScene_ModelFrom3DFile(Vector3.zero, Quaternion.identity, Vector3.one, path, Vector3.zero);
#endif
    }

    public GameObject SpawnModelFromFile(string relDrivePath) { 
        GameObject o = this.GetComponent<ObjFromFile>().LoadFile(relDrivePath);
        return o;
    }

    public void RefreshDriveNow()
    {
        PhotonView.Get(this).RPC("RPC_RefreshDrive", RpcTarget.AllViaServer, ueserverAddress);
    }

    [PunRPC]
    void RPC_RefreshDrive(string serverAddress)
    {
        StartCoroutine(RefreshDriveCoroutine(serverAddress));
    }


#if UNITY_EDITOR

    public ImageDownloader imageDownloader;

    string downloadURL;
    private void OnGUI()
    {
        if (!debug_gui)
            return;

        downloadURL = GUILayout.TextField(downloadURL);
        if (GUILayout.Button("GET IMAGE"))
        {
            imageDownloader.DownloadOneImageAndOpen(downloadURL);
            downloadURL = "";
        }
        

        if (GUILayout.Button("** REFRESH SYNC **"))
        {
            RefreshDriveNow();
        }

        if (GUILayout.Button("** REFRESH LOCAL **"))
        {
            RefreshOBJFiles();
        }

        foreach (string o in objFileNames)
        {
            if (GUILayout.Button("*** LOAD: " + o))
            {
                GenerateModel(o);
            }
        }

        /***
        if (GUILayout.Button("*** OPEN FILE ****"))
        {
            string rootpath = StreamingDrivePath();
            string path = EditorUtility.OpenFilePanel("Load", rootpath, "obj");
            if (path.Length != 0)
            {
                path = path.Substring(rootpath.Length); // only take the stuff after /DriveSync
                GenerateModel(path);
            }
        }
        ****/
    }
#endif
}
