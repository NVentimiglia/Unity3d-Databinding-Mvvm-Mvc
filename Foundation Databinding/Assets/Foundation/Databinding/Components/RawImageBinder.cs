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
    /// UI.RawImage to Texture2d or Color
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    [AddComponentMenu("Foundation/Databinding/RawImageBinder")]
    public class RawImageBinder : BindingBase
    {
        protected RawImage Target;

        [HideInInspector]
        public BindingInfo SpriteBinding = new BindingInfo { BindingName = "Sprite" };
        [HideInInspector]
        public BindingInfo ColorBinding = new BindingInfo { BindingName = "Color" };

        protected bool IsInit;

        public bool SetNativeSize;

        protected void Awake()
        {
            Init();
        }

        public override void Init()
        {
            if (IsInit)
                return;
            IsInit = true;

            Target = GetComponent<RawImage>();

            SpriteBinding.Action = UpdateLabel;
            SpriteBinding.Filters = BindingFilter.Properties;
            SpriteBinding.FilterTypes = new[] { typeof(Texture2D) };


            ColorBinding.Action = UpdateColor;
            ColorBinding.Filters = BindingFilter.Properties;
            ColorBinding.FilterTypes = new[] { typeof(Color) };
        }


        private void UpdateLabel(object arg)
        {
            if(arg == null)
                return;
            if (Target)
            {
                var texture = (Texture2D)arg;

                Target.texture = texture;

                if(SetNativeSize)
                    Target.SetNativeSize();
            }
        }

        private void UpdateColor(object arg)
        {
            if (Target)
            {
                Target.color = ((Color)arg);
            }
        }
    }
}
