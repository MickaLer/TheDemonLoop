using System.Collections.Generic;
using System.IO;
using System.Linq;
using Editor.Utils;
using ScriptableObjects;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

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
        private Vector2 _scrollPositionMiddle;
        private Vector2 _scrollPositionRight;
        private int _selectedBoss;
        private int _selectedPhase;
        private int _selectPattern;
        private Color _baseColor;

        public int SelectedBoss
        {
            get => _selectedBoss;
            set
            {
                if (value != _selectedBoss) LoadSelectedBoss(value);
                _selectedBoss = value;
            }
        }

        private readonly string _mainPath = "Assets/ScriptableObjects/BossPhases/";
        private readonly DirectoryInfo _mainDir = new("Assets/ScriptableObjects/BossPhases/");

        private void OnEnable()
        {
            DirectoryInfo[] info = _mainDir.GetDirectories();

            foreach (DirectoryInfo f in info) _bossNames.Add(f.Name);

            LoadSelectedBoss(SelectedBoss);
            _baseColor = GUI.color;
        }

        private void OnDisable()
        {
            _bossNames.Clear();
        }

        private void OnGUI()
        {
            LeftPanel();
            MiddlePanel();
            RightPanel();
        }

        private void LeftPanel()
        {
            //Rename, select, create or remove bosses folders
            GUILayout.BeginArea(new Rect(30, 100, 100, 300));
            GUILayout.BeginVertical();
            _scrollPositionLeft = GUILayout.BeginScrollView(_scrollPositionLeft);
            SelectedBoss = GUILayout.SelectionGrid(SelectedBoss, _bossNames.ToArray(), 1);
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            GUI.color = Color.green;
            if (GUILayout.Button("Add"))
            {
                string newName = "newBoss" + _bossNames.Count;
                _bossNames.Add(newName);
                SelectedBoss = _bossNames.Count - 1;
                _mainDir.CreateSubdirectory(newName);
                AssetDatabase.Refresh();
            }

            GUI.color = _baseColor;

            GUI.color = Color.red;
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

            GUI.color = _baseColor;
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

        private void MiddlePanel()
        {
            //Select, rename, add or remove Phases
            GUILayout.BeginArea(new Rect(150, 100, 100, 300));
            GUILayout.BeginVertical();
            _scrollPositionMiddle = GUILayout.BeginScrollView(_scrollPositionMiddle);
            _selectedPhase = GUILayout.SelectionGrid(_selectedPhase, _bossPhases.Select(o => o.name).ToArray(), 1);
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            GUI.color = Color.green;
            if (GUILayout.Button("Add"))
            {
                string newName = AssetDatabase.GenerateUniqueAssetPath(_mainPath + _bossNames[SelectedBoss] +
                                                                       "/newPhase" + _bossPhases.Count + ".asset");
                BossPhase asset = CreateInstance<BossPhase>();

                AssetDatabase.CreateAsset(asset, newName);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                _bossPhases.Add(asset);
            }

            GUI.color = _baseColor;

            GUI.color = Color.red;
            if (GUILayout.Button("Remove"))
            {
                Rect buttonRect = GUILayoutUtility.GetLastRect();
                PopupWindow.Show(
                    new Rect(buttonRect.x, buttonRect.yMax + 20, 0, 0),
                    new ValidatePopup(_bossPhases[_selectedPhase].name, () =>
                    {
                        AssetDatabase.DeleteAsset(_mainPath + _bossNames[SelectedBoss] + "/" +
                                                  _bossPhases[_selectedPhase].name + ".asset");
                        _bossPhases.RemoveAt(_selectedPhase);
                        AssetDatabase.Refresh();
                        _selectedPhase = _bossPhases.Count - 1;
                    })
                );
            }

            GUI.color = _baseColor;
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Rename"))
            {
                string oldName = _bossPhases[_selectedPhase].name;
                Rect buttonRect = GUILayoutUtility.GetLastRect();
                PopupWindow.Show(
                    new Rect(buttonRect.x, buttonRect.yMax + 20, 0, 0),
                    new StringEditorPopup(_bossPhases[_selectedPhase].name, (newValue) =>
                    {
                        AssetDatabase.MoveAsset(_mainPath + _bossNames[SelectedBoss] + "/" + oldName + ".asset",
                            _mainPath + _bossNames[SelectedBoss] + "/" + newValue + ".asset");
                        Repaint();
                        AssetDatabase.Refresh();
                    })
                );
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void RightPanel()
        {
            //Modify Phases
            if (0 >= _bossPhases.Count) return;
            GUILayout.BeginArea(new Rect(270, 50, position.width - 300, position.height - 100));
            GUILayout.BeginArea(new Rect(0, 0,320, position.height - 100));

            GUILayout.BeginVertical();
            if (GUILayout.Button(_bossPhases[_selectedPhase].bossSprite, GUILayout.Width(320), GUILayout.Height(180)))
            {
                string tempValue = EditorUtility.OpenFilePanel("Select a texture", "", "png");
                if(tempValue != "") EditorCoroutineUtility.StartCoroutine(TextureUtils.LoadTextureFromPath(tempValue, _bossPhases[_selectedPhase]), this);
            }
            _bossPhases[_selectedPhase].maxLife = EditorGUILayout.FloatField("Max Life", _bossPhases[_selectedPhase].maxLife);
            _scrollPositionRight = GUILayout.BeginScrollView(_scrollPositionRight);
            _selectedPhase = GUILayout.SelectionGrid(_selectedPhase, _bossPhases[_selectedPhase].BossPatterns.Select(o => o.PatternName).ToArray(), 1);
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            GUI.color = Color.green;
            if (GUILayout.Button("Add"))
            {
                
            }

            GUI.color = _baseColor;

            GUI.color = Color.red;
            if (GUILayout.Button("Remove"))
            {
               
            }

            GUI.color = _baseColor;
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Rename"))
            {
                
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
            GUILayout.EndArea();
        }

        private void LoadSelectedBoss(int selected)
            {
                _bossPhases.Clear();
                DirectoryInfo info = new DirectoryInfo(_mainPath + "/" + _bossNames[selected]);
                ;
                foreach (var file in info.GetFiles("*.asset"))
                {
                    var temp = AssetDatabase.LoadAssetAtPath<BossPhase>(_mainPath + "/" + _bossNames[selected] + "/" +
                                                                        file.Name);
                    _bossPhases.Add(temp);
                }
            }
        }
    }