// -------------------------------------
//  Domain		: Avariceonline.com
//  Author		: Nicholas Ventimiglia
//  Product		: Unity3d Foundation
//  Published		: 2015
//  -------------------------------------
using System;
using Foundation.Databinding.View;
using UnityEngine;

namespace Foundation.Databinding.Components
{
    /// <summary>
    /// Sets a GameObject's Visual State (SetActive) by comparing to a property's string value
    /// </summary>
    [AddComponentMenu("Foundation/Databinding/VisualStatesBinder")]
    public class VisualStatesBinder : BindingBase
    {

    [Serializable]
        public struct StateValue
        {
            public GameObject Target;
            public string Value;
        }
        public StateValue[] Targets;

        public bool IntConvert;
        
        [HideInInspector]
        public BindingInfo ValueBinding = new BindingInfo { BindingName = "State" };

        protected bool IsInit;


        void Awake()
        {
            Init();
        }

        private void UpdateState(object arg)
        {
            foreach (var target in Targets)
            {
                if (IntConvert)
                {
                    var value = arg == null ? 0 : (int) arg;
                    target.Target.SetActive(value == int.Parse(target.Value));
                }
                else
                {
                    var valid = arg != null && target.Value.ToLower() == arg.ToString().ToLower();
                    target.Target.SetActive(valid);

                }
            }
        }


        public override void Init()
        {
            if (IsInit)
                return;
            IsInit = true;

            ValueBinding.Action = UpdateState;
            ValueBinding.Filters = BindingFilter.Properties;
            ValueBinding.FilterTypes = new[] { typeof(bool), typeof(string), typeof(int), typeof(Enum) };
        }
    }
}