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
        private static StyleSheet _cachedStyleSheet;

        private static readonly string[] UFolderClassNames = new string[]
        {
            "ufolder-dark-active-child-expanded",
            "ufolder-dark-active-child-collapsed",
            "ufolder-dark-active-empty",
            "ufolder-light-active-child-expanded",
            "ufolder-light-active-child-collapsed",
            "ufolder-light-active-empty",
            "ufolder-dark-deactive-child-expanded",
            "ufolder-dark-deactive-child-collapsed",
            "ufolder-dark-deactive-empty",
            "ufolder-light-deactive-child-expanded",
            "ufolder-light-deactive-child-collapsed",
            "ufolder-light-deactive-empty"
        };

        static HierarchyGUI()
        {
            if (EditorApplication.isPlaying)
                return;
            
#if !UNITY_6000_4_OR_NEWER
            // Unity 6.3 이하: 기존 IMGUI 방식 사용
            EditorApplication.hierarchyWindowItemOnGUI += (id, _) => HierarchyWindowItemOnGUI(id);
#elif UNITY_6000_5_OR_NEWER
            // Unity 6.5 이상: UI Toolkit 신규 API 사용
            HierarchyWindow.BindView += OnBindView;
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
        /// Unity 6.5+ BindView 콜백 함수 (스타일시트 주입)
        /// </summary>
        private static void OnBindView(HierarchyWindow window, HierarchyView view)
        {
            LoadAndApplyStyleSheet(view);
        }

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
                    string styleClass = GetFolderStyleClass(viewItem, go);
                    if (!string.IsNullOrEmpty(styleClass) && viewItem.Icon != null)
                    {
                        viewItem.Icon.AddToClassList(styleClass);
                    }
                }
            }
        }

        /// <summary>
        /// ViewItem의 이전 스타일(USS 클래스 오버라이드)을 초기화합니다.
        /// </summary>
        private static void ResetViewItemStyle(HierarchyViewItem viewItem)
        {
            if (viewItem.Icon != null)
            {
                foreach (var className in UFolderClassNames)
                {
                    viewItem.Icon.RemoveFromClassList(className);
                }
            }
        }

        /// <summary>
        /// 스타일시트를 로드하고 뷰에 적용합니다.
        /// </summary>
        private static void LoadAndApplyStyleSheet(HierarchyView view)
        {
            if (_cachedStyleSheet == null)
            {
                // 1순위: 패키지 공식 경로 기준 로드
                _cachedStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.nkstudio.ufolder/Editor/UFolderStyle.uss");
                if (_cachedStyleSheet == null)
                {
                    // 2순위: 로컬 에셋 폴더 경로 기준 로드
                    _cachedStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/com.nkstudio.ufolder@18ffc1b77594/Editor/UFolderStyle.uss");
                }
            }

            if (_cachedStyleSheet != null && !view.styleSheets.Contains(_cachedStyleSheet))
            {
                view.styleSheets.Add(_cachedStyleSheet);
            }
        }

        /// <summary>
        /// Unity 6.5+ 뷰 아이템 정보를 기반으로 매칭할 USS 클래스 결정
        /// </summary>
        private static string GetFolderStyleClass(HierarchyViewItem viewItem, GameObject obj)
        {
            int childCount = obj.transform.childCount;
            bool isExpanded = viewItem.View != null ? viewItem.View.IsExpanded(viewItem.Node) : false;
            bool hasChild = childCount > 0;
            bool isPro = EditorGUIUtility.isProSkin;

            string theme = isPro ? "dark" : "light";
            string activeState = obj.activeInHierarchy ? "active" : "deactive";
            string childState = hasChild ? (isExpanded ? "child-expanded" : "child-collapsed") : "empty";

            // ufolder-{theme}-{activeState}-{childState} 형식의 클래스명 빌드
            return $"ufolder-{theme}-{activeState}-{childState}";
        }
#endif
    }
}
