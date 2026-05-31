using UnityEditor;
using UnityEngine;

#if UNITY_6000_5_OR_NEWER
using Unity.Hierarchy;
using Unity.Hierarchy.Editor;
using UnityEngine.UIElements;
#endif

namespace NKStudio
{
    [InitializeOnLoad]
    public class HierarchyGUI : Editor
    {
        static HierarchyGUI()
        {
            if (EditorApplication.isPlaying)
                return;
            
#if !UNITY_6000_4_OR_NEWER
            // Unity 6.3 이하: 기존 IMGUI 방식 사용
            EditorApplication.hierarchyWindowItemOnGUI += (id, _) => HierarchyWindowItemOnGUI(id);
#elif UNITY_6000_5_OR_NEWER
            // Unity 6.5 이상: UI Toolkit BindViewItem 구독
            HierarchyWindow.BindViewItem += OnBindViewItem;
#else
            // Unity 6.4: 비활성화
#endif
        }

#if !UNITY_6000_4_OR_NEWER
        /// <summary>
        /// 게임 오브젝트의 태그가 Folder 맞다면 폴더 아이콘을 그려냅니다.
        /// </summary>
        private static void HierarchyWindowItemOnGUI(int instanceID)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if(go == null)
                return;

            if (GroupObjectsEditor.AutoAddTag("Folder"))
            {
                if (go.CompareTag("Folder"))
                    DrawIcon(go); 
            }
        }
        
        private static void DrawIcon(GameObject gameObject)
        {
            if (gameObject.activeInHierarchy)
                ChangeFolderIconActive(gameObject.GetInstanceID(), gameObject);
            else
                ChangeFolderIconDeActive(gameObject.GetInstanceID(), gameObject);
        }

        private static void ChangeFolderIconActive(int instanceId, GameObject obj)
        {
            int childCount = obj.transform.childCount;
            bool isExtended = UFolderUtility.IsExpanded(obj);
            string iconName;
            bool hasChild = childCount > 0;
            if (hasChild)
            {
                if (EditorGUIUtility.isProSkin)
                {
                    if (isExtended)
                        iconName = "FolderOpened On Icon";
                    else
                        iconName = "Folder On Icon";
                }
                else
                {
                    if (isExtended)
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
            Texture2D icon = folderIconContent.image as Texture2D;

            HierarchyWindowAdapter.ApplyIconByInstanceId(instanceId, icon);
        }

        private static void ChangeFolderIconDeActive(int instanceId, GameObject obj)
        {
            int childCount = obj.transform.childCount;
            bool isExtended = UFolderUtility.IsExpanded(obj);
            string iconName;
            bool hasChild = childCount > 0;
            if (hasChild)
            {
                if (EditorGUIUtility.isProSkin)
                {
                    if (isExtended)
                        iconName = "FolderOpened Icon";
                    else
                        iconName = "Folder Icon";
                }
                else
                {
                    if (isExtended)
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
            Texture2D icon = folderIconContent.image as Texture2D;

            HierarchyWindowAdapter.ApplyIconByInstanceId(instanceId, icon);
        }
#endif

#if UNITY_6000_5_OR_NEWER
        /// <summary>
        /// Unity 6.5+ BindViewItem 콜백 함수
        /// </summary>
        private static void OnBindViewItem(HierarchyWindow window, HierarchyView view, HierarchyViewItem viewItem)
        {
            // 아이콘 초기화
            if (viewItem.Icon != null) 
                viewItem.Icon.style.backgroundImage = new StyleBackground(StyleKeyword.Null);

            if (viewItem.Handler is not HierarchyGameObjectHandler gameObjectHandler)
                return;

            ref readonly var node = ref viewItem.Node;
            var go = gameObjectHandler.GetGameObject(node);

            if (go == null)
                return;

            if (GroupObjectsEditor.AutoAddTag("Folder"))
            {
                if (go.CompareTag("Folder"))
                {
                    if (viewItem.Icon != null)
                    {
                        bool isExpanded = view.IsExpanded(node);
                        Texture2D icon = UFolderUtility.GetFolderIcon(go, isExpanded);
                        viewItem.Icon.style.backgroundImage = new StyleBackground(icon);
                    }
                }
            }
        }
#endif
    }
}
