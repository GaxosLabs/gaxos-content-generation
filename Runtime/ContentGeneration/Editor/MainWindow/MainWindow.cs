using System.Linq;
using ContentGeneration.Editor.MainWindow.Components;
using ContentGeneration.Editor.MainWindow.Components.BasicExamples;
using ContentGeneration.Editor.MainWindow.Components.DallE;
using ContentGeneration.Editor.MainWindow.Components.Meshy;
using ContentGeneration.Editor.MainWindow.Components.RequestsList;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow
{
    public class MainWindow : EditorWindow
    {
        [SerializeField] VisualTreeAsset _root;
        SubWindowToggle[] _allToggles;

        [MenuItem("AI Content Generation/Main window")]
        public static void ShowMainWindow()
        {
            var wnd = GetWindow<MainWindow>();
            wnd.minSize = new Vector2(500, 300);
            wnd.titleContent = new GUIContent("Content Generation");
        }

        public void CreateGUI()
        {
            // rootVisualElement.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            var rootInstance = _root.Instantiate();
            rootInstance.style.height = new StyleLength(new Length(100, LengthUnit.Percent));

            var dallE = rootInstance.Q<DallETab>();
            var stabilityAI = rootInstance.Q<Components.StabilityAI.StabilityTab>();
            var meshy = rootInstance.Q<MeshyTab>();
            var requestsList = rootInstance.Q<RequestsListTab>();
            var basicGeneration = rootInstance.Q<BasicGenerationTab>();
            var imageToImage = rootInstance.Q<ImageToImageTab>();
            var maskGeneration = rootInstance.Q<MaskGenerationTab>();

            var subWindowsContainer = rootInstance.Q<VisualElement>("subWindowsContainer");
            var subWindows = subWindowsContainer.Children().ToArray();
            foreach (var visualElement in subWindows)
            {
                subWindowsContainer.Remove(visualElement);
            }

            var sideMenu = rootInstance.Q<VisualElement>("sideMenuItemsContainer");
            _allToggles = sideMenu.Children().
                Where(c => c is SubWindowToggle).Cast<SubWindowToggle>().ToArray();
            

            rootInstance.Q<SubWindowToggle>("subWindowToggleDallE").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, dallE);
            };
            rootInstance.Q<SubWindowToggle>("subWindowToggleStabilityAI").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, stabilityAI);
            };
            rootInstance.Q<SubWindowToggle>("subWindowToggleMeshy").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, meshy);
            };
            rootInstance.Q<SubWindowToggle>("subWindowToggleRequestsList").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, requestsList);
            };
            rootInstance.Q<SubWindowToggle>("subWindowToggleBasicGeneration").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, basicGeneration);
            };
            rootInstance.Q<SubWindowToggle>("subWindowToggleImageToImage").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, imageToImage);
            };
            rootInstance.Q<SubWindowToggle>("subWindowToggleMaskGeneration").OnToggled += (sender, v) =>
            {
                ToggleSubWindow(sender, v, subWindowsContainer, maskGeneration);
            };


            rootVisualElement.Add(rootInstance);
        }
        
        void ToggleSubWindow(SubWindowToggle sender, bool v, VisualElement subWindowsContainer,
            VisualElement subWindow)
        {
            if (v)
            {
                subWindowsContainer.Add(subWindow);
                subWindow.style.display = DisplayStyle.Flex;

                foreach (var subWindowToggle in _allToggles)
                {
                    if (sender != subWindowToggle)
                    {
                        subWindowToggle.ToggleOff();
                    }
                }
            }
            else
            {
                subWindowsContainer.Remove(subWindow);
            }
        }
    }
}