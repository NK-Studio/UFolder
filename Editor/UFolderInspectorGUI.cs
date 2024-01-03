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

            // UFolderUtility.HideAllComponent(gameObject);
            
            // ApplyIcon(gameObject);
        }


    }
}
