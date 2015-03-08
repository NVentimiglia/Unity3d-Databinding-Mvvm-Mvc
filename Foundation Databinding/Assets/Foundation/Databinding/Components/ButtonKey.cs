// -------------------------------------
//  Domain		: Avariceonline.com
//  Author		: Nicholas Ventimiglia
//  Product		: Unity3d Foundation
//  Published		: 2015
//  -------------------------------------
using UnityEngine;
using UnityEngine.UI;

namespace Foundation.Databinding.Components
{
    /// <summary>
    /// Click a button via Keyboard input
    /// </summary>
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(ButtonBinder))]
    [AddComponentMenu("Foundation/Databinding/ButtonKey")]
    public class ButtonKey : MonoBehaviour
    {

        protected Button Button;
        protected ButtonBinder Binder;

        public KeyCode Key;

        public bool RequireDouble = false;
        protected float lastHit;
        
        void Awake()
        {
            Binder = GetComponent<ButtonBinder>();
            Button = GetComponent<Button>();
        }
        
        void Update()
        {
            if(!Button.enabled || !Binder.enabled || !Button.IsInteractable())
                return;

            if (Input.GetKeyUp(Key))
            {
                if (RequireDouble)
                {
                    if (lastHit + .2f > Time.time)
                    {
                        Binder.Call();
                        lastHit = 0;
                    }
                    lastHit = Time.time;
                }
                else
                {
                    Binder.Call();
                }
            }
        }
    }
}
