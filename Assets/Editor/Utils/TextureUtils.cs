using System.Collections;
using System.IO;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

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
                DirectoryInfo info = new DirectoryInfo("Assets/AddedSprite");
                FileUtil.CopyFileOrDirectory(path, assetsPath + "/newDataImage" + info.GetFiles().Length + ".png");
                var files = info.GetFiles();
                AssetDatabase.ImportAsset("Assets/AddedSprite/" + files[files.Length - 1].Name);
                temp = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/AddedSprite/" + files[files.Length - 1].Name);
                bossPhase.bossSprite = temp;
                AssetDatabase.Refresh();
            }
            
            yield return null;
        }
    }
}