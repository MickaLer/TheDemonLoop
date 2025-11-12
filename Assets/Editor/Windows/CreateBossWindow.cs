using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Editor.Utils;
using Editor.Windows.PatternEditor;
using Patterns;
using ScriptableObjects;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

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

        private readonly List<string> _bossNames = new();
        private readonly List<BossPhase> _bossPhases = new();
        private Vector2 _scrollPositionLeft;
        private Vector2 _scrollPositionMiddle;
        private Vector2 _scrollPositionRight;
        private int _selectedBoss;
        private int _selectedPhase;
        private int _selectPattern;
        private Color _baseColor;

        private int SelectedBoss
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
            
            //Load all boss names for SelectionGrid
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
            MiddleLeftPanel();
            MiddleRightPanel();
            RightPanel();
        }

        private void LeftPanel()
        {
            //Rename, select, create or remove bosses folders
            GUILayout.BeginArea(new Rect(30, 100, 100, 300));
            GUILayout.BeginVertical();
            GUILayout.Label("List of bosses :");
            _scrollPositionLeft = GUILayout.BeginScrollView(_scrollPositionLeft);
            SelectedBoss = GUILayout.SelectionGrid(SelectedBoss, _bossNames.ToArray(), 1);
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            GUI.color = Color.green;
            if (GUILayout.Button("Add"))
            {
                string newName = "newBoss" + _bossNames.Count;
                _bossNames.Add(newName);
                DirectoryInfo newDir = _mainDir.CreateSubdirectory(newName);
                AssetDatabase.Refresh();
                newDir.CreateSubdirectory("Patterns");
                AssetDatabase.Refresh();
                SelectedBoss = _bossNames.Count - 1;
            }
            GUI.color = _baseColor;
            GUI.color = Color.red;
            if (GUILayout.Button("Remove"))
            {
                Rect buttonRect = GUILayoutUtility.GetLastRect();
                //Show a validation popup to prevent miss click
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
        
        private void MiddleLeftPanel()
        {
            //Select, rename, add or remove Phases
            GUILayout.BeginArea(new Rect(150, 100, 100, 300));
            GUILayout.BeginVertical();
            GUILayout.Label("List of phases :");
            _scrollPositionMiddle = GUILayout.BeginScrollView(_scrollPositionMiddle);
            _selectedPhase = GUILayout.SelectionGrid(_selectedPhase, _bossPhases.Select(o => o.name).ToArray(), 1);
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            GUI.color = Color.green;
            if (GUILayout.Button("Add"))
            {
                string newName = AssetDatabase.GenerateUniqueAssetPath(_mainPath + _bossNames[SelectedBoss] + "/newPhase" + _bossPhases.Count + ".asset");
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
                    //Show a validation popup to prevent miss click
                    new ValidatePopup(_bossPhases[_selectedPhase].name, () =>
                    {
                        AssetDatabase.DeleteAsset(_mainPath + _bossNames[SelectedBoss] + "/" + _bossPhases[_selectedPhase].name + ".asset");
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
                        AssetDatabase.MoveAsset(_mainPath + _bossNames[SelectedBoss] + "/" + oldName + ".asset", _mainPath + _bossNames[SelectedBoss] + "/" + newValue + ".asset");
                        Repaint();
                        AssetDatabase.Refresh();
                    })
                );
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void MiddleRightPanel()
        {
            if (_bossPhases.Count == 0) return;
            
            BossPhase current = _bossPhases[_selectedPhase];
            if (0 >= _bossPhases.Count) return;
            GUILayout.BeginArea(new Rect(270, 50,320, position.height - 100));
            GUILayout.BeginVertical();
            
            //Add a new texture in unity's file and use it for the phase sprite
            if (GUILayout.Button(current.bossSprite, GUILayout.Width(320), GUILayout.Height(180)))
            {
                string tempValue = EditorUtility.OpenFilePanel("Select a texture", "", "png,jpg");
                if(tempValue != "")
                {
                    EditorCoroutineUtility.StartCoroutine( TextureUtils.LoadTextureFromPath(tempValue, current), this);
                    EditorUtility.SetDirty(current);
                }
            }
            
            //Modify Phases Stats
            current.maxLife = EditorGUILayout.FloatField("Max Life", current.maxLife);
            current.patternCooldown = EditorGUILayout.FloatField("Pattern cooldown", current.patternCooldown);
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        
        private void RightPanel()
        {
            if (_bossPhases.Count == 0) return;
            BossPhase current = _bossPhases[_selectedPhase];
            GUILayout.BeginArea(new Rect(590, 50,position.width - 620, position.height - 100));
            GUILayout.BeginVertical();
            GUILayout.Label("List of patterns :");
            _scrollPositionRight = GUILayout.BeginScrollView(_scrollPositionRight);
            
            // Loop through every list of patterns
            var temp = current.bossPatterns;
            for (var index = 0; index < current.bossPatterns.Count; index++)
            {
                GUILayout.BeginHorizontal();
                var patternList = current.bossPatterns[index];
                for (var i = 0; i < temp[index].patterns.Count; i++)
                {
                    var patternPiece = patternList.patterns[i];
                    if (GUILayout.Button(patternPiece.name)) OpenPatternWindow(patternPiece);
                    CreatePatternButton(patternList, patternPiece); // Remove or Rename buttons
                    if(i >= patternList.patterns.Count) break;
                }

                // Security
                if (current.bossPatterns.Count <= index || current.bossPatterns[index].patterns != patternList.patterns)
                {
                    GUILayout.EndHorizontal();
                    break;
                }
                
                //Add patterns
                GUI.color = Color.green;
                if (GUILayout.Button("+"))
                {
                    GenericMenu contextMenu = new GenericMenu();
                    contextMenu.AddItem(new GUIContent("SpawnLaser"), false, () => {
                            patternList.patterns.Add(CreatePattern<SpawnLaserPattern>("newSpawnLaserPattern"));
                            EditorUtility.SetDirty(_bossPhases[_selectedPhase]);
                        });
                    contextMenu.AddItem(new GUIContent("Movement"), false, () => {
                            patternList.patterns.Add(CreatePattern<MovingPattern>("newMovingPattern"));
                            EditorUtility.SetDirty(_bossPhases[_selectedPhase]);
                        });
                    contextMenu.AddItem(new GUIContent("TargetPlayer"), false, () => {
                        patternList.patterns.Add(CreatePattern<TargetPlayerPattern>("newTargetPlayerPattern"));
                        EditorUtility.SetDirty(_bossPhases[_selectedPhase]);
                    });
                    contextMenu.ShowAsContext();
                }

                GUI.color = _baseColor;
                GUILayout.EndHorizontal();
            }

            // Create a list of patterns
            if (GUILayout.Button("+", GUILayout.Width(20),GUILayout.Height(20)))
            {
                current.bossPatterns.Add(new BossPhaseData(){patterns = new()});
                EditorUtility.SetDirty(_bossPhases[_selectedPhase]);
            }
            
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        
        private T CreatePattern<T>(string patternName) where T : Pattern
        {
            //Create a pattern T in the assets 
            string newName = AssetDatabase.GenerateUniqueAssetPath(_mainPath + _bossNames[SelectedBoss] + "/Patterns/" + patternName + Random.Range(0, Int32.MaxValue) + ".asset");
            T asset = CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, newName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }

        private void OpenPatternWindow(Pattern pattern)
        {
            //Open a window specific to a pattern
            switch (pattern)
            {
                case SpawnLaserPattern:
                    LaserPatternWindow.ShowWindow(pattern);
                    break;
                case MovingPattern:
                    MovingPatternWindow.ShowWindow(pattern);
                    break;
                case TargetPlayerPattern:
                    TargetPlayerPatternWindow.ShowWindow(pattern);
                    break;
            }
        }
        
        private void CreatePatternButton(BossPhaseData currentList, Pattern currentPattern)
        {
            // For lists of patterns, create rename and remove buttons, and the followingPatternDelay value
            if (GUILayout.Button("Rename"))
            {
                string oldName = currentPattern.name;
                Rect buttonRect = GUILayoutUtility.GetLastRect();
                PopupWindow.Show(
                    new Rect(buttonRect.x, buttonRect.yMax + 20, 0, 0),
                    new StringEditorPopup(oldName, (newValue) =>
                    {
                        AssetDatabase.MoveAsset(_mainPath + _bossNames[SelectedBoss] + "/Patterns/" + oldName + ".asset", _mainPath + _bossNames[SelectedBoss] + "/Patterns/" + newValue + ".asset");
                        AssetDatabase.Refresh();
                        Repaint();
                        EditorUtility.SetDirty(_bossPhases[_selectedPhase]);
                    })
                    
                );
            }
            GUI.color = Color.red;
            if (GUILayout.Button("-"))
            {
                currentList.patterns.Remove(currentPattern);
                AssetDatabase.DeleteAsset(_mainPath + _bossNames[SelectedBoss] + "/Patterns/" + currentPattern.name + ".asset");
                AssetDatabase.Refresh();
                if(currentList.patterns.Count == 0) _bossPhases[_selectedPhase].bossPatterns.Remove(currentList);
                EditorUtility.SetDirty(_bossPhases[_selectedPhase]);
            }

            GUI.color = _baseColor;
            //If put at -1, it will play the next pattern along this one
            currentPattern.followingPatternDelay = EditorGUILayout.FloatField(currentPattern.followingPatternDelay);
        }

        private void LoadSelectedBoss(int selected)
            {
                // Load every boss phase in the folder using the selected boss name 
                _bossPhases.Clear();
                DirectoryInfo info = new DirectoryInfo(_mainPath + "/" + _bossNames[selected]);
                foreach (var file in info.GetFiles("*.asset"))
                {
                    var temp = AssetDatabase.LoadAssetAtPath<BossPhase>(_mainPath + "/" + _bossNames[selected] + "/" +
                                                                        file.Name);
                    _bossPhases.Add(temp);
                }
            }
        }
    }