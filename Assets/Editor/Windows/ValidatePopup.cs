using UnityEditor;
using UnityEngine;

namespace Editor.Windows
{
    public class ValidatePopup : PopupWindowContent
    {
        private string _tempValue;
        private readonly System.Action _onClose;

        public ValidatePopup(string initialValue, System.Action onClose)
        {
            _tempValue = initialValue;
            _onClose = onClose;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(250, 75);
        }

        public override void OnGUI(Rect rect)
        {
            EditorGUILayout.LabelField("Are you sure u want to delete :", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(_tempValue);

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK"))
            {
                _onClose?.Invoke();
                editorWindow.Close();
            }
            if (GUILayout.Button("Cancel"))
            {
                editorWindow.Close();
            }
            GUILayout.EndHorizontal();
        }
    }
}