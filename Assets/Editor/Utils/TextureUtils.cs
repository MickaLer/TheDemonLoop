using System;
using System.Collections;
using System.IO;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace Editor.Utils
{
    public static class TextureUtils
    {
        
        public static IEnumerator LoadTextureFromPath(string path, BossPhase bossPhase)
        {
            Texture2D temp = null;
            using UnityWebRequest request = UnityWebRequestTexture.GetTexture(path);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) temp = DownloadHandlerTexture.GetContent(request);

            if (temp != null)
            {
                string assetsPath = Application.dataPath + "/AddedSprite";
                if (!Directory.Exists(assetsPath)) Directory.CreateDirectory(assetsPath);
                AssetDatabase.Refresh();
                DirectoryInfo dirInfo = new DirectoryInfo("Assets/AddedSprite");
                FileInfo originalInfo = new FileInfo(path);
                FileUtil.CopyFileOrDirectory(path, assetsPath + "/newDataImage" + Random.Range(0,Int32.MaxValue) + originalInfo.Extension);;
                var files = dirInfo.GetFiles();
                AssetDatabase.ImportAsset("Assets/AddedSprite/" + files[files.Length - 1].Name);
                AssetDatabase.Refresh();
                temp = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AddedSprite/" + files[files.Length - 1].Name);
                bossPhase.bossSprite = temp;
            }
            
            yield return null;
        }
    }
}