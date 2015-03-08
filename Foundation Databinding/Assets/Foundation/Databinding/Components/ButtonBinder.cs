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
    /// Button Click to Command. Invoke a method or run a coroutine !
    /// </summary>
    [RequireComponent(typeof(Button))]
    [AddComponentMenu("Foundation/Databinding/ButtonBinder")]
    public class ButtonBinder : BindingBase
    {

        protected Button Button;
        protected ButtonParamater Paramater;

        [HideInInspector]
        public BindingInfo EnabledBinding = new BindingInfo { BindingName = "Enabled" };

        [HideInInspector]
        public BindingInfo OnClickBinding = new BindingInfo { BindingName = "OnClick" };

        protected bool IsInit;

    //    public AudioClip ClickSound;

        void Awake()
        {
            Init();
        }

        public void Call()
        {
            if(!Button.IsInteractable())
                return;

           // if (ClickSound)
          //  {
           //     Audio2DListener.PlayUI(ClickSound, 1);
          //  }

            SetValue(OnClickBinding.MemberName, Paramater == null? null : Paramater.GetValue());
        }

        private void UpdateState(object arg)
        {
            Button.interactable = (bool)arg;
        }

      
        public override void Init()
        {
            if (IsInit)
                return;
            IsInit = true;

            Paramater = GetComponent<ButtonParamater>();
            Button = GetComponent<Button>();

            OnClickBinding.Filters = BindingFilter.Commands;
            
            EnabledBinding.Action = UpdateState;
            EnabledBinding.Filters = BindingFilter.Properties;
            EnabledBinding.FilterTypes = new[] { typeof(bool) };
            
            Button.onClick.AddListener(Call);
        }
    }
}
