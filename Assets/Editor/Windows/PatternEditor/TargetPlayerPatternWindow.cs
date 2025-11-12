using System;
using System.Collections.Generic;
using System.Linq;
using Patterns;
using UnityEditor;
using UnityEngine;

namespace Editor.Windows.PatternEditor
{
    public class TargetPlayerPatternWindow : EditorWindow
    {
        private static TargetPlayerPattern _currentPattern;
        
        public static void ShowWindow(Pattern currentPattern)
        {
            _currentPattern = currentPattern as TargetPlayerPattern;
            TargetPlayerPatternWindow wnd = GetWindow<TargetPlayerPatternWindow>();
            wnd.titleContent = new GUIContent("TargetPlayerPattern" + currentPattern.name);
        }
        
        private Vector2 _scrollPositionLeft;
        private Color _baseColor;
        private int _selectedStep;

        private void OnEnable()
        {
            _baseColor = GUI.color;
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
            _selectedStep = GUILayout.SelectionGrid(_selectedStep, _currentPattern.steps.Select(o => o.order.ToString()).ToArray(), 1);
            GUILayout.EndScrollView();
            
            //Create and delete steps
            GUILayout.BeginHorizontal();
            GUI.color = Color.green;
            if (GUILayout.Button("Add"))
            {
                TargetPlayerPattern.TargetInfos newStep = new TargetPlayerPattern.TargetInfos {
                    order = _currentPattern.steps.Count,
                };
                _currentPattern.steps.Add(newStep);
                EditorUtility.SetDirty(_currentPattern);
                _selectedStep = _currentPattern.steps.Count - 1;
            }
            GUI.color = _baseColor;
            GUI.color = Color.red;
            if (GUILayout.Button("Remove"))
            {
                _currentPattern.steps.RemoveAt(_selectedStep);
                _selectedStep = 0;
                ReorderSteps();
            }
            GUI.color = _baseColor;
            GUILayout.EndHorizontal();

            //Edit step order
            if (GUILayout.Button("EditOrder"))
            {
                int oldName = _currentPattern.steps[_selectedStep].order;
                Rect buttonRect = GUILayoutUtility.GetLastRect();
                PopupWindow.Show(
                    new Rect(buttonRect.x, buttonRect.yMax + 20, 0, 0),
                    new IntEditorPopup(oldName, (newValue) =>
                    {
                        TargetPlayerPattern.TargetInfos temp = _currentPattern.steps[_selectedStep];
                        temp.order = newValue;
                        _currentPattern.steps[_selectedStep] = temp;
                        Repaint();
                        ReorderSteps();
                    })
                );
            }
            GUILayout.EndVertical();
        }
        
        void StepModifierPanel()
        {
            if (_selectedStep >= _currentPattern.steps.Count) return;
            TargetPlayerPattern.TargetInfos currentStep = _currentPattern.steps[_selectedStep];
            
            currentStep.targetType = (TargetPlayerPattern.TargetType)EditorGUILayout.EnumFlagsField("TargetType",currentStep.targetType);
            
            //Modify step properties
            GUILayout.BeginVertical();
            switch (currentStep.targetType)
            {
                case TargetPlayerPattern.TargetType.Ball:
                    currentStep.ballNumbers = EditorGUILayout.IntField("BallNumbers",currentStep.ballNumbers);
                    currentStep.timeBetweenBalls = EditorGUILayout.FloatField("TimeBetweenBalls", currentStep.timeBetweenBalls);
                    currentStep.ballPrefab = EditorGUILayout.ObjectField("BallPrefab",currentStep.ballPrefab,typeof(GameObject),false) as GameObject;
                    break;
                case TargetPlayerPattern.TargetType.Laser:
                    currentStep.duration = EditorGUILayout.FloatField("Duration",currentStep.duration);
                    currentStep.followAfterSpawn = EditorGUILayout.Toggle("FollowAfterSpawn",currentStep.followAfterSpawn);
                    currentStep.laserPrefab = EditorGUILayout.ObjectField("LaserPrefab",currentStep.laserPrefab,typeof(GameObject),false) as GameObject;
                    break;
                case TargetPlayerPattern.TargetType.Movement:
                    currentStep.speed = EditorGUILayout.FloatField("Speed",currentStep.speed);
                    currentStep.chargingTime = EditorGUILayout.FloatField("ChargingTime",currentStep.chargingTime);
                    break;
            }
            GUILayout.EndVertical();
            
            //Save and apply changes
            _currentPattern.steps[_selectedStep] = currentStep;
            EditorUtility.SetDirty(_currentPattern);
        }
        
        private void ReorderSteps()
        {
            if (_currentPattern.steps.Count <= 0) return;
            _currentPattern.steps = _currentPattern.steps.OrderBy(o => o.order).ToList();
            EditorUtility.SetDirty(_currentPattern);
        }
    }
}