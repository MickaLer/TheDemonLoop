using System.Collections.Generic;
using System.Linq;
using Patterns;
using UnityEditor;
using UnityEngine;

namespace Editor.Windows.PatternEditor
{
    public class LaserPatternWindow : EditorWindow
    {
        private static SpawnLaserPattern _currentPattern;
        
        public static void ShowWindow(Pattern currentPattern)
        {
            _currentPattern = currentPattern as SpawnLaserPattern;
            LaserPatternWindow wnd = GetWindow<LaserPatternWindow>();
            wnd.titleContent = new GUIContent("LaserPattern_" + currentPattern.name);
        }
        
        private Vector2 _scrollPositionLeft;
        private Vector2 _scrollPositionRight;
        private Color _baseColor;
        private int _selectedStep;
        private int _selectedLaser;
        private readonly List<string> _laserCounts = new();
        private int SelectedStep
        {
            get => _selectedStep;
            set
            {
                if(value != _selectedStep) MakeNewLasersList(value);
                _selectedStep = value;
            }
        }


        private void OnEnable()
        {
            _baseColor = GUI.color;
            MakeNewLasersList(SelectedStep);
            ReorderSteps();
        }

        private void ReorderSteps()
        {
            if (_currentPattern.steps.Count <= 0) return;
            _currentPattern.steps = _currentPattern.steps.OrderBy(o => o.order).ToList();
            EditorUtility.SetDirty(_currentPattern);
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            StepsPanel();
            StepModifierPanel();
            GUILayout.EndHorizontal();
        }

        void StepsPanel()
        {
            GUILayout.BeginVertical();
            //Show steps
            GUILayout.Label("List of steps :");
            _scrollPositionLeft = GUILayout.BeginScrollView(_scrollPositionLeft);
            SelectedStep = GUILayout.SelectionGrid(SelectedStep, _currentPattern.steps.Select(o => o.order.ToString()).ToArray(), 1);
            GUILayout.EndScrollView();
            
            //Create and delete steps
            GUILayout.BeginHorizontal();
            GUI.color = Color.green;
            if (GUILayout.Button("Add"))
            {
                SpawnLaserPattern.Step newStep = new SpawnLaserPattern.Step {
                    order = _currentPattern.steps.Count,
                    spawnedLasers = new()
                };
                _currentPattern.steps.Add(newStep);
                EditorUtility.SetDirty(_currentPattern);
                SelectedStep = _currentPattern.steps.Count - 1;
            }
            GUI.color = _baseColor;
            GUI.color = Color.red;
            if (GUILayout.Button("Remove"))
            {
                _currentPattern.steps.RemoveAt(SelectedStep);
                SelectedStep = 0;
                ReorderSteps();
            }
            GUI.color = _baseColor;
            GUILayout.EndHorizontal();

            //Edit step order
            if (GUILayout.Button("EditOrder"))
            {
                int oldName = _currentPattern.steps[SelectedStep].order;
                Rect buttonRect = GUILayoutUtility.GetLastRect();
                PopupWindow.Show(
                    new Rect(buttonRect.x, buttonRect.yMax + 20, 0, 0),
                    new IntEditorPopup(oldName, (newValue) =>
                    {
                        SpawnLaserPattern.Step temp = _currentPattern.steps[SelectedStep];
                        temp.order = newValue;
                        _currentPattern.steps[SelectedStep] = temp;
                        Repaint();
                        ReorderSteps();
                    })
                );
            }
            GUILayout.EndVertical();
        }

        void StepModifierPanel()
        {
            if (SelectedStep >= _currentPattern.steps.Count) return;
            SpawnLaserPattern.Step currentStep = _currentPattern.steps[SelectedStep];
            
            //Modify step properties
            GUILayout.BeginVertical();
            currentStep.speed = EditorGUILayout.FloatField("Speed",currentStep.speed);
            currentStep.duration = EditorGUILayout.FloatField("Duration",currentStep.duration);
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical();
            //Show lasers
            GUILayout.Label("List of lasers :");
            _scrollPositionRight = GUILayout.BeginScrollView(_scrollPositionRight);
            _selectedLaser = GUILayout.SelectionGrid(_selectedLaser, _laserCounts.ToArray(), 1);
            GUILayout.EndScrollView();
            
            //Add or remove lasers
            GUILayout.BeginHorizontal();
            GUI.color = Color.green;
            if (GUILayout.Button("Add"))
            {
                SpawnLaserPattern.Laser newLaser = new SpawnLaserPattern.Laser();
                currentStep.spawnedLasers.Add(newLaser);
                _selectedLaser = currentStep.spawnedLasers.Count - 1;
                MakeNewLasersList(SelectedStep);
            }
            GUI.color = _baseColor;
            GUI.color = Color.red;
            if (GUILayout.Button("Remove"))
            {
                currentStep.spawnedLasers.RemoveAt(_selectedLaser);
                _selectedLaser = 0;
                MakeNewLasersList(SelectedStep);
            }
            GUI.color = _baseColor;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            
            // Show current laser properties 
            if (currentStep.spawnedLasers.Count > 0) currentStep.spawnedLasers[_selectedLaser] = LaserModifierPanel(currentStep.spawnedLasers[_selectedLaser]);
            
            //Save and apply changes
            _currentPattern.steps[SelectedStep] = currentStep;
            EditorUtility.SetDirty(_currentPattern);
        }

        SpawnLaserPattern.Laser LaserModifierPanel(SpawnLaserPattern.Laser currentState)
        {
            //Modify lasers properties
            SpawnLaserPattern.Laser temp = currentState;
            GUILayout.BeginVertical();
            temp.angle = EditorGUILayout.FloatField("Angle",temp.angle);
            temp.maxAngle = EditorGUILayout.FloatField("MaxAngle",temp.maxAngle);
            temp.direction = EditorGUILayout.FloatField("Direction",temp.direction);
            temp.spawned = EditorGUILayout.ObjectField("Spawned",temp.spawned,typeof(GameObject),false) as GameObject;
            GUILayout.EndVertical();
            return temp;
        }

        void MakeNewLasersList(int newIndex)
        {
            if (newIndex >= _currentPattern.steps.Count) return;
            // Create a laser index list so that it is usable with SelectionGrid
            _laserCounts.Clear();
            for (int i = 0; i < _currentPattern.steps[newIndex].spawnedLasers.Count; i++)
            {
                _laserCounts.Add(i.ToString());
            }
        }
    }
}