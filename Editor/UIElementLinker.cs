#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIElementLinker
    {
        private static Button s_buttonElement;

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            UIBuilderHook.OnFocusedChanged += OnFocusedChanged;
            UIBuilderHook.OnSelectionChanged += OnSelectionChanged;
        }

        private static void OnFocusedChanged()
        {
            if (s_buttonElement == null)
                AddButtonToUIBuilder(UIBuilderHook.Inspector);
        }

        private static void OnSelectionChanged()
        {
            if (s_buttonElement != null)
                ChangeButtonState();
        }

        private static void AddButtonToUIBuilder(VisualElement root)
        {
            if (root == null)
                return;

            s_buttonElement = new Button();
            s_buttonElement.RegisterCallback<ClickEvent>(evt => InstantiateLinkToVisualElement());
            s_buttonElement.visible = UIBuilderHook.GetSelectedElement() != null;

            root.Add(s_buttonElement);
        }

        private static void ChangeButtonState()
        {
            var element = UIBuilderHook.GetSelectedElement();
            s_buttonElement.visible = element != null;
            if (element != null)
                s_buttonElement.text = "Create Link for " + UIBuilderHookUtilities.GetElementDisplayName(element);
        }

        private static void InstantiateLinkToVisualElement()
        {
            var path = UIBuilderHookUtilities.GetSelectedElementPath(out _);
            var asset = UIBuilderHook.VisualTreeAsset;
            var document = Object.FindObjectsByType<UIDocument>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .Where(d => d.visualTreeAsset == asset)
                .FirstOrDefault();
            if (document == null)
            {
                Debug.LogWarning("No UIDocument found with the matching VisualTreeAsset.");
                return;
            }

            var go = new GameObject();
            go.transform.parent = document.transform;
            go.transform.localPosition = Vector3.zero;
            go.AddComponent<UIElementLink>().SetElementPath(path);

            Selection.activeGameObject = go;
        }
    }
}
#endif