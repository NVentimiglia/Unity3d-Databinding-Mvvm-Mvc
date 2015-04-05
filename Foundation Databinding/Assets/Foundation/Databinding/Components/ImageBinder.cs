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
    /// UI.Image to Texture2d or Color
    /// </summary>
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("Foundation/Databinding/ImageBinder")]
    public class ImageBinder : BindingBase
    {
        protected Image Target;

        [HideInInspector]
        public BindingInfo SpriteBinding = new BindingInfo { BindingName = "Sprite" };
        [HideInInspector]
        public BindingInfo ColorBinding = new BindingInfo { BindingName = "Color" };

        protected bool IsInit;
        protected Sprite original = null;

        protected void Awake()
        {
            Init();
        }

        public override void Init()
        {
            if (IsInit)
                return;
            IsInit = true;

            Target = GetComponent<Image>();

            original = Target.sprite;

            SpriteBinding.Action = UpdateLabel;
            SpriteBinding.Filters = BindingFilter.Properties;
            SpriteBinding.FilterTypes = new[] { typeof(Texture2D), typeof(Sprite) };


            ColorBinding.Action = UpdateColor;
            ColorBinding.Filters = BindingFilter.Properties;
            ColorBinding.FilterTypes = new[] { typeof(Color) };
        }


        private void UpdateLabel(object arg)
        {
            if (Target)
            {
                var texture = (Texture2D)arg;
                var sprite = (Sprite) arg;

                if(sprite != null)
                {
                    Target.sprite = sprite;
                }
                else if (texture != null)
                {
                    Target.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);;
                }
                else
                {
                    Target.sprite = original;
                }
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
