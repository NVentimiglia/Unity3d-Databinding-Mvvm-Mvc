// -------------------------------------
//  Domain		: Avariceonline.com
//  Author		: Nicholas Ventimiglia
//  Product		: Unity3d Foundation
//  Published		: 2015
//  -------------------------------------
using Foundation.Databinding.View;
using UnityEngine;
using UnityEngine.UI;

namespace Foundation.Databinding.Components
{
    /// <summary>
    /// For binding text or submit commands to your UI.InputField.
    /// </summary>
    [RequireComponent(typeof(InputField))]
    [AddComponentMenu("Foundation/Databinding/InputFieldBinder")]
    public class InputFieldBinder : BindingBase
    {
        protected InputField Target;

        [HideInInspector]
        public BindingInfo TextBinding = new BindingInfo { BindingName = "Text" };

        [HideInInspector]
        public BindingInfo SubmitBinding = new BindingInfo { BindingName = "Submit" };

        [HideInInspector]
        public BindingInfo EnabledBinding = new BindingInfo { BindingName = "Enabled" };

        protected bool IsInit;

        public AudioClip TypeSound;

        public float SoundLag = .5f;
        protected float NextSound;
        protected string oldText;


        
        protected void Awake()
        {
            Init();
        }

        public override void Init()
        {
            if (IsInit)
                return;
            IsInit = true;

            NextSound = Time.time + SoundLag;

            Target = GetComponent<InputField>();
            Target.onEndEdit.AddListener(SubmitText);

            TextBinding.Action = UpdateText;
            TextBinding.Filters = BindingFilter.Properties;
            TextBinding.FilterTypes = new[] { typeof(string) };

            SubmitBinding.Filters = BindingFilter.Commands;

            EnabledBinding.Action = UpdateState;
            EnabledBinding.Filters = BindingFilter.Properties;
            EnabledBinding.FilterTypes = new[] { typeof(bool) };
        }

        private void SubmitText(string text)
        {
            if(string.IsNullOrEmpty(text))
                return;

            SetValue(SubmitBinding.MemberName, null);
        }

        private void UpdateState(object arg)
        {
            Target.interactable = (bool)arg;
        }
      
        private void UpdateText(string text)
        {
            oldText = text;
            SetValue(TextBinding.MemberName, text);
        }

        private void UpdateText(object arg)
        {
            if (Target)
            {
                if (arg!= null)
                {
                    Target.text = oldText = arg.ToString();
                }
                else
                {
                    Target.text = oldText = string.Empty;
                }
            }
        }


        void Update()
        {
            if (Target.text != oldText)
            {
                if (Target.text.Contains("\t"))
                {
                    Target.text = Target.text.Replace("\t", string.Empty);


                    return;
                }


                if (TypeSound)
                {
                    if (NextSound < Time.time)
                    {
                        Audio2DListener.PlayUI(TypeSound, 1);
                        NextSound = Time.time + SoundLag;
                    }
                }

                UpdateText(Target.text);
            }
        }

    }
}
