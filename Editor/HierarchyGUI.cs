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
            // Unity 6.5 이상: UI Toolkit 신규 API 사용
            HierarchyWindow.BindViewItem += OnBindViewItem;
#else
            // Unity 6.4: 아무 작업도 수행하지 않아 폴더 기능 비활성화
#endif
        }

#if !UNITY_6000_4_OR_NEWER
        /// <summary>
        /// 게임 오브젝트의 태그가 Folder 맞다면 폴더 아이콘을 그려냅니다.
        /// </summary>
        /// <param name="instanceID"></param>
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
        
        /// <summary>
        /// 게임 오브젝트의 activeInHierarchy에 따라 아이콘을 그려냅니다.
        /// </summary>
        /// <param name="gameObject">타겟 게임 오브젝트</param>
        private static void DrawIcon(GameObject gameObject)
        {
            if (gameObject.activeInHierarchy)
                ChangeFolderIconActive(gameObject.GetInstanceID(), gameObject);
            else
                ChangeFolderIconDeActive(gameObject.GetInstanceID(), gameObject);
        }

        /// <summary>
        /// 활성 모드에서 폴더 아이콘 변경
        /// </summary>
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

        /// <summary>
        /// 비활성화 모드에서 폴더 아이콘 변경
        /// </summary>
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
                //Theme Color
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
            ResetViewItemStyle(viewItem);

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
                    Texture2D icon = GetFolderIcon(viewItem, go);
                    if (icon != null && viewItem.Icon != null)
                    {
                        viewItem.Icon.style.backgroundImage = new StyleBackground(icon);
                    }
                }
            }
        }

        /// <summary>
        /// ViewItem의 이전 스타일(배경 이미지 오버라이드)을 초기화합니다.
        /// </summary>
        private static void ResetViewItemStyle(HierarchyViewItem viewItem)
        {
            if (viewItem.Icon != null)
            {
                viewItem.Icon.style.backgroundImage = null;
            }
        }

        /// <summary>
        /// Unity 6.5+ 뷰 아이템 정보를 기반으로 폴더 아이콘 텍스쳐 결정
        /// </summary>
        private static Texture2D GetFolderIcon(HierarchyViewItem viewItem, GameObject obj)
        {
            int childCount = obj.transform.childCount;
            bool isExpanded = viewItem.View != null ? viewItem.View.IsExpanded(viewItem.Node) : false;
            
            string iconName;
            bool hasChild = childCount > 0;
            
            if (obj.activeInHierarchy)
            {
                if (hasChild)
                {
                    if (EditorGUIUtility.isProSkin)
                        iconName = isExpanded ? "FolderOpened On Icon" : "Folder On Icon";
                    else
                        iconName = isExpanded ? "FolderOpened Icon" : "Folder Icon";
                }
                else
                {
                    iconName = EditorGUIUtility.isProSkin ? "FolderEmpty On Icon" : "FolderEmpty Icon";
                }
            }
            else
            {
                if (hasChild)
                {
                    if (EditorGUIUtility.isProSkin)
                        iconName = isExpanded ? "FolderOpened Icon" : "Folder Icon";
                    else
                        iconName = isExpanded ? "FolderOpened On Icon" : "Folder On Icon";
                }
                else
                {
                    iconName = EditorGUIUtility.isProSkin ? "FolderEmpty Icon" : "FolderEmpty On Icon";
                }
            }

            GUIContent folderIconContent = EditorGUIUtility.IconContent(iconName);
            return folderIconContent.image as Texture2D;
        }
#endif
    }
}
