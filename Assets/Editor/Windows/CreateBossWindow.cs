using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScriptableObjects;
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
        
        private List<string> _bossNames = new();
        private List<BossPhase> _bossPhases = new();
        private Vector2 _scrollPositionLeft;
        private Vector2 _scrollPositionRight;
        private int _selectedBoss;
        private int _selectedPhase;

        public int SelectedBoss
        {
            get => _selectedBoss;
            set
            {
                if(value != _selectedBoss)LoadSelectedBoss(value);
                _selectedBoss = value;
            }
        }

        private readonly string _mainPath = "Assets/ScriptableObjects/BossPhases/";
        private readonly DirectoryInfo _mainDir = new("Assets/ScriptableObjects/BossPhases/");

        private void OnEnable()
        {
            DirectoryInfo[] info = _mainDir.GetDirectories();

            foreach (DirectoryInfo f in info) _bossNames.Add(f.Name);
            
        }

        private void OnDisable()
        {
            _bossNames.Clear();
        }

        private void OnGUI()
        {
            Color baseColor = GUI.color;
            LeftPanel();
            
            //Select Phase
            GUILayout.BeginArea(new Rect(150,100,100,200));
            _scrollPositionRight = GUILayout.BeginScrollView(_scrollPositionRight);
            _selectedPhase = GUILayout.SelectionGrid(_selectedPhase, _bossPhases.Select(o => o.name).ToArray(), 1);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            
            //Modify Phases
            GUI.color = Color.green;
            GUILayout.BeginArea(new Rect(270,50,position.width - 300,position.height - 100));
            GUILayout.Button("oui", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            GUILayout.EndArea();
            GUI.color = baseColor;
        }

        private void LeftPanel()
        {
            //Rename, select, create or remove bosses 
            GUILayout.BeginArea(new Rect(30,100,100,200));
            GUILayout.BeginVertical();
            _scrollPositionLeft = GUILayout.BeginScrollView(_scrollPositionLeft);
            SelectedBoss = GUILayout.SelectionGrid(SelectedBoss, _bossNames.ToArray(), 1);
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                string newName = "newBoss" + _bossNames.Count;
                _bossNames.Add(newName);
                SelectedBoss = _bossNames.Count - 1;
                _mainDir.CreateSubdirectory(newName);
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("Remove"))
            {
                Rect buttonRect = GUILayoutUtility.GetLastRect();
                PopupWindow.Show(
                    new Rect(buttonRect.x, buttonRect.yMax + 20, 0, 0),
                    new ValidatePopup(_bossNames[SelectedBoss], () =>
                    {
                        AssetDatabase.DeleteAsset("Assets/ScriptableObjects/BossPhases/" + _bossNames[SelectedBoss]);
                        _bossNames.RemoveAt(SelectedBoss);
                        AssetDatabase.Refresh();
                        SelectedBoss = _bossNames.Count - 1;
                    })
                );
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Rename"))
            {
                string oldName = _bossNames[SelectedBoss];
                Rect buttonRect = GUILayoutUtility.GetLastRect();
                PopupWindow.Show(
                    new Rect(buttonRect.x, buttonRect.yMax + 20, 0, 0),
                    new StringEditorPopup(_bossNames[SelectedBoss], (newValue) =>
                    {
                        _bossNames[SelectedBoss] = newValue;
                        AssetDatabase.MoveAsset(_mainPath + oldName, _mainPath + _bossNames[SelectedBoss]);
                        Repaint();
                        AssetDatabase.Refresh();
                    })
                );
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void LoadSelectedBoss(int selected)
        {
            _bossPhases.Clear();
            DirectoryInfo info = new DirectoryInfo(_mainPath + "/"+ _bossNames[selected]);;
            foreach (var file in info.GetFiles("*.asset"))
            {
                var temp = AssetDatabase.LoadAssetAtPath<BossPhase>(_mainPath + "/"+ _bossNames[selected] + "/" + file.Name);
                _bossPhases.Add(temp);
            }
        }
    }
    
}
