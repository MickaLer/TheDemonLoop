using UnityEditor;
using UnityEngine;

namespace Editor.Windows
{
    public class StringEditorPopup : PopupWindowContent
    {
        private string _tempValue;
        private readonly System.Action<string> _onClose;

        public StringEditorPopup(string initialValue, System.Action<string> onClose)
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
            EditorGUILayout.LabelField("Edit value:", EditorStyles.boldLabel);
            _tempValue = EditorGUILayout.TextField(_tempValue);

            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK"))
            {
                _onClose?.Invoke(_tempValue);
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