using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NKStudio
{
    public static class UFolderUtility
    {
        internal static IEnumerable<EditorWindow> GetAllWindowsByType(string type) => Resources.FindObjectsOfTypeAll(typeof(EditorWindow)).Where(obj => obj.GetType().ToString() == type).Select(obj => (EditorWindow)obj);
        
#if !UNITY_6000_4_OR_NEWER
        /// <summary>
        /// Hierarchy 뷰에서 대상 GameObject가 열려있는지 확인합니다.
        /// </summary>
        /// <param name="gameObject">열려있는지 확인할 게임 오브젝트</param>
        internal static bool IsExpanded(GameObject gameObject)
        {
            var expandedGameObjects = GetExpandedGameObjects();
 
            if (expandedGameObjects != null)
                return expandedGameObjects.Contains(gameObject);
 
            return false;
        }

        /// <summary>
        /// Hierarchy 뷰에서 확장된(즉, 펼쳐진) 모든 GameObject의 목록을 가져옵니다.
        /// </summary>
        private static List<GameObject> GetExpandedGameObjects()
        {
            object sceneHierarchy = HierarchyWindowAdapter.GetFirstHierarchy();
 
            MethodInfo methodInfo = sceneHierarchy
                .GetType()
                .GetMethod("GetExpandedGameObjects");
 
            if (methodInfo != null)
            {
                object result = methodInfo.Invoke(sceneHierarchy, Array.Empty<object>());
 
                return (List<GameObject>)result;
            }
 
            return null;
        }
#endif

#if UNITY_6000_5_OR_NEWER
        /// <summary>
        /// 게임 오브젝트 및 확장 상태에 맞춰 알맞은 폴더 아이콘을 반환합니다.
        /// </summary>
        public static Texture2D GetFolderIcon(GameObject obj, bool isExpanded)
        {
            if (obj.activeInHierarchy)
            {
                return GetFolderIconActive(obj, isExpanded);
            }
            else
            {
                return GetFolderIconDeActive(obj, isExpanded);
            }
        }

        private static Texture2D GetFolderIconActive(GameObject obj, bool isExpanded)
        {
            int childCount = obj.transform.childCount;
            string iconName;
            bool hasChild = childCount > 0;
            if (hasChild)
            {
                if (EditorGUIUtility.isProSkin)
                {
                    if (isExpanded)
                        iconName = "FolderOpened On Icon";
                    else
                        iconName = "Folder On Icon";
                }
                else
                {
                    if (isExpanded)
                        iconName = "FolderOpened Icon";
                    else
                        iconName = "Folder Icon";
                }
            }
            else
            {
                if (EditorGUIUtility.isProSkin)
                    iconName = "FolderEmpty On Icon";
                else
                    iconName = "FolderEmpty Icon";
            }

            GUIContent folderIconContent = EditorGUIUtility.IconContent(iconName);
            return folderIconContent.image as Texture2D;
        }

        private static Texture2D GetFolderIconDeActive(GameObject obj, bool isExpanded)
        {
            int childCount = obj.transform.childCount;
            string iconName;
            bool hasChild = childCount > 0;
            if (hasChild)
            {
                if (EditorGUIUtility.isProSkin)
                {
                    if (isExpanded)
                        iconName = "FolderOpened Icon";
                    else
                        iconName = "Folder Icon";
                }
                else
                {
                    if (isExpanded)
                        iconName = "FolderOpened On Icon";
                    else
                        iconName = "Folder On Icon";
                }
            }
            else
            {
                if (EditorGUIUtility.isProSkin)
                    iconName = "FolderEmpty Icon";
                else
                    iconName = "FolderEmpty On Icon";
            }

            GUIContent folderIconContent = EditorGUIUtility.IconContent(iconName);
            return folderIconContent.image as Texture2D;
        }
#endif

        [MenuItem("Tools/UFolder/About", priority = int.MaxValue)]
        private static void About()
        {
            string path = AssetDatabase.GUIDToAssetPath("8fee9135ba53440da42f4924bb4012ed");
            ;
            TextAsset packageJson = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            PackageInfo info = JsonUtility.FromJson<PackageInfo>(packageJson.text);
 
            Debug.Log($"UFolder v{info.version}");
        }
        
        [Serializable]
        internal class PackageInfo
        {
            public string name;
            public string displayName;
            public string version;
            public string unity;
            public string description;
            public List<string> keywords;
            public string type;
        }
    }
}
