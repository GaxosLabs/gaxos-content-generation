using System.Globalization;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor
{
    [CustomEditor(typeof(Settings))]
    public class SettingsInspector : UnityEditor.Editor
    {
        [SerializeField] VisualTreeAsset _rootAsset;
        Button _refreshButton;
        TextField _credits;

        public override VisualElement CreateInspectorGUI()
        {
            var rootVisualElement = _rootAsset.Instantiate();

            _credits = rootVisualElement.Q<TextField>("credits");
            _credits.value = "";
            _refreshButton = rootVisualElement.Q<Button>("refresh");
            _refreshButton.RegisterCallback<ClickEvent>(_ =>
            {
                Refresh();
            });

            Refresh();
            return rootVisualElement;
        }

        void Refresh()
        {
            if(!_refreshButton.enabledSelf)
                return;
                
            _refreshButton.SetEnabled(false);
            RefreshAsync().ContinueInMainThreadWith(t =>
            {
                if (t.IsFaulted)
                {
                    Debug.LogException(t.Exception);
                }
                    
                _refreshButton.SetEnabled(true);
            });

        }

        async Task RefreshAsync()
        {
            _credits.value = "";
            var credits = await ContentGenerationApi.Instance.GetCredits();
            _credits.value = credits.ToString("F", CultureInfo.InvariantCulture);
        }
    }
}
