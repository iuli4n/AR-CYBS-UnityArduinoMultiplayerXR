using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class ImageDownloader : MonoBehaviourPun
{
    // BUG:HIGH: imagemenu should refresh after each download
    // BUG:MEDHI: should check if image already exists on local drive
    // THESE TWO BUGS WILL CANCEL EACH OTHER OUT
    //      BUGS:NETWORK: this downloads a file on everyone's machine via rpc; it should only download on the server and then everyone should sync their files
    //      BUGS:FTP: because this downloads into the FileDrive path of the local machine, that will be updated when the machine syncs with server

    public AudioSource soundGo, soundGood, soundBad;

    public void DownloadOneImageAndOpen(string downloadURL)
    {
        PhotonView.Get(this).RPC("RPC_DownloadOneImageAndOpen", RpcTarget.AllViaServer, downloadURL);
    }

    [PunRPC]
    void RPC_DownloadOneImageAndOpen(string downloadURL) 
    {
        StartCoroutine(GetTexture(downloadURL.Substring(downloadURL.LastIndexOf("/") + 1), downloadURL, true));
    }


    IEnumerator GetTexture(string name, string url, bool andOpen = false)
    {
        Debug.Log("Downloading " + name + " from " + url);

        

        soundGo.Play();

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            soundBad.Play();

        }
        else
        {
            soundGood.Play();
                
            Debug.Log("Got texture for " + name);
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            byte[] bytes = texture.EncodeToPNG();
            var dirPath = FileDriveManager.Instance.StreamingDrivePath() + "/DownloadedFromWeb/";
            File.WriteAllBytes(dirPath + name + ".png", bytes);
            Debug.Log("Saved to " + dirPath + name + ".png");


            if (andOpen)
            {
                ThumbnailManager.Instance.ThumbnailPressPath("/DownloadedFromWeb/" + name+".png");
                //ThumbnailManager.Instance.ThumbnailPress(texture);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }




}
