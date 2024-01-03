using UnityEditor;
using UnityEngine;

namespace NKStudio.UFolder.Editor
{
    [InitializeOnLoad]
    public class UFolderInspectorGUI
    {
        static UFolderInspectorGUI()
        {
            UnityEditor.Editor.finishedDefaultHeaderGUI -= UFolderGUI;
            UnityEditor.Editor.finishedDefaultHeaderGUI += UFolderGUI;
        }

        private static string Prefix => EditorGUIUtility.isProSkin ? "d_" : "";
        
        private static void UFolderGUI(UnityEditor.Editor editor)
        {
            if(editor == null)
                return;

            Object[] targets = editor.targets;

            if(targets.Length == 0)
                return;

            GameObject gameObject = targets[0] as GameObject;
            if(gameObject == null)
                return;

            if (!gameObject.CompareTag("Folder"))
                return;

            UFolderUtility.HideAllComponent(gameObject);
            
            ApplyIcon(gameObject);
        }

        /// <summary>
        /// Applies an icon to the given GameObject.
        /// </summary>
        /// <param name="target">The GameObject to which the icon will be applied.</param>
        private static void ApplyIcon(GameObject target)
        {
            string iconName = Prefix + "Folder Icon";
            Texture2D icon = EditorGUIUtility.FindTexture(iconName);
            EditorGUIUtility.SetIconForObject(target, icon);
        }
    }
}
