using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor.Windows
{
    public class CreateBossWindow : EditorWindow
    {
        [MenuItem("Tools/CreateBossWindow")]
        public static void ShowWindow()
        {
            CreateBossWindow wnd = GetWindow<CreateBossWindow>();
            wnd.titleContent = new GUIContent("CreateBoss");
        }

        private void OnEnable()
        {
            DirectoryInfo dir = new DirectoryInfo("Assets/ScriptableObjects/BossPhases/");
            FileInfo[] info = dir.GetFiles("*.*");

            foreach (FileInfo f in info)
            {
                Debug.Log(f.Name);
            }
        }

        private void CreateGUI()
        {
            
        }
    }
    
}
