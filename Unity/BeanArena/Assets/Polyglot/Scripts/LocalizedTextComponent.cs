using System.Collections.Generic;
#if UNITY_5 || UNITY_2017_1_OR_NEWER
using JetBrains.Annotations;
#endif
using UnityEngine;

namespace Polyglot
{
    public abstract class LocalizedTextComponent<T> : MonoBehaviour, ILocalize where T : Component
    {
        [Tooltip("The text component to localize")]
        [SerializeField]
        private T text;
        
        [Tooltip("Maintain original text alignment. If set to false, localization will determine whether text is left or right aligned")]
        [SerializeField]
        private bool maintainTextAlignment;
        public bool MaintainTextAlignment
        {
            get
            {
                return maintainTextAlignment;
            }
            set
            {
                maintainTextAlignment = value;
            }
        }

        [Tooltip("The key to localize with")]
        [SerializeField]
        private string key;

        public string Key
        {
            get { return key; }
            set
            {
                key = value;
                OnLocalize();
            }
        }

        public int maxSymbolsLength = -1;

        public List<object> Parameters { get { return parameters; } }

        private readonly List<object> parameters = new List<object>();

#if UNITY_5 || UNITY_2017_1_OR_NEWER
        [UsedImplicitly]
#endif
        public void Reset()
        {
            text = GetComponent<T>();
        }

#if UNITY_5 || UNITY_2017_1_OR_NEWER
        [UsedImplicitly]
#endif
        public void OnEnable()
        {
            Localization.Instance.AddOnLocalizeEvent(this);
        }

        public void OnDisable()
        {
            Localization.Instance.RemoveOnLocalizeEvent(this);
        }

        protected abstract void SetText(T component, string value);

        protected abstract void UpdateAlignment(T component, LanguageDirection direction);

        public void OnLocalize()
        {
#if UNITY_EDITOR
            var flags = text != null ? text.hideFlags : HideFlags.None;
            if(text != null) text.hideFlags = HideFlags.DontSave;
#endif
            string translationStr = null;

            if (parameters != null && parameters.Count > 0)
            {
                translationStr = Localization.GetFormat(key, parameters.ToArray());
            }
            else
            {
                translationStr = Localization.Get(key);
            }

            if (maxSymbolsLength > 0) {
                if (translationStr.Length > maxSymbolsLength) {
                    translationStr = translationStr.Substring(0, maxSymbolsLength) + ".";
                }
            }

            SetText(text, translationStr);

            var direction = Localization.Instance.SelectedLanguageDirection;

            if (text != null && !maintainTextAlignment) UpdateAlignment(text, direction);

#if UNITY_EDITOR
            if (text != null) text.hideFlags = flags;
#endif
        }

        public void ClearParameters()
        {
            parameters.Clear();
        }

        public void AddParameter(object parameter)
        {
            parameters.Add(parameter);
            OnLocalize();
        }
        public void AddParameter(int parameter)
        {
            AddParameter((object)parameter);
        }
        public void AddParameter(float parameter)
        {
            AddParameter((object)parameter);
        }
        public void AddParameter(string parameter)
        {
            AddParameter((object)parameter);
        }
        public void SetParameters(params object[] parameters) 
        {
            ClearParameters();
            this.parameters.AddRange(parameters);
            OnLocalize();
        }
    }
}