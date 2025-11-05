using System.Linq;
using Patterns;
using UnityEditor;
using UnityEngine;

namespace Editor.Windows.PatternEditor
{
    public class MovingPatternWindow : EditorWindow
    {
        private static MovingPattern _currentPattern;
        
        public static void ShowWindow(Pattern currentPattern)
        {
            _currentPattern = currentPattern as MovingPattern;
            MovingPatternWindow wnd = GetWindow<MovingPatternWindow>();
            wnd.titleContent = new GUIContent("MovingPattern_" + currentPattern.name);
        }
        
        private Vector2 _scrollPositionLeft;
        private Color _baseColor;
        private int _selectedStep;

        private void OnEnable()
        {
            _baseColor = GUI.color;
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
            _currentPattern.returnBehaviour = (MovingPattern.ReturnBehaviour)EditorGUILayout.EnumFlagsField("ReturnBehaviour",_currentPattern.returnBehaviour);
            _currentPattern.returnDuration = EditorGUILayout.FloatField("ReturnDuration",_currentPattern.returnDuration);
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
                MovingPattern.Step newStep = new MovingPattern.Step {
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
                        MovingPattern.Step temp = _currentPattern.steps[_selectedStep];
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
            MovingPattern.Step currentStep = _currentPattern.steps[_selectedStep];
            
            //Modify step properties
            GUILayout.BeginVertical();
            currentStep.direction = EditorGUILayout.Vector2Field("Direction",currentStep.direction);
            currentStep.speed = EditorGUILayout.FloatField("Speed",currentStep.speed);
            currentStep.duration = EditorGUILayout.FloatField("Duration",currentStep.duration);
            currentStep.collisionBehaviour = (MovingPattern.CollisionBehaviour)EditorGUILayout.EnumFlagsField("CollisionBehaviour",currentStep.collisionBehaviour);
            GUILayout.EndVertical();
            
            //Save and apply changes
            _currentPattern.steps[_selectedStep] = currentStep;
            EditorUtility.SetDirty(_currentPattern);
        }
    }
}