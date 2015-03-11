// -------------------------------------
//  Domain		: Avariceonline.com
//  Author		: Nicholas Ventimiglia
//  Product		: Unity3d Foundation
//  Published		: 2015
//  -------------------------------------
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Foundation.Tasks;
using UnityEngine;

namespace Foundation.Databinding.Model
{
    /// <summary>
    /// Implements IObservableModel on behalf of other objects.
    /// Change notification requires IObservableModel implementation
    /// </summary>
    public class ModelBinder : IObservableModel, IDisposable
    {
        Type _myType;
        ObservableMessage _bindingMessage = new ObservableMessage();
        object _instance;
        MonoBehaviour _insanceBehaviour;
        IObservableModel _bindableInstance;

        public ModelBinder(object instance)
        {
            _instance = instance;
            _myType = _instance.GetType();

            _insanceBehaviour = instance as MonoBehaviour;
            _bindableInstance = instance as IObservableModel;

            if (_bindableInstance != null)
                _bindableInstance.OnBindingUpdate += _bindableInstance_OnBindingUpdate;

        }
        
        void _bindableInstance_OnBindingUpdate(ObservableMessage obj)
        {
            if (OnBindingUpdate != null)
            {
                OnBindingUpdate(obj);
            }
        }

        /// <summary>
        /// Raises property changed on all listeners
        /// </summary>
        /// <param name="propertyName">property to change</param>
        /// <param name="propValue">value to pass</param>
        public virtual void NotifyProperty(string propertyName, object propValue)
        {
            RaiseBindingUpdate(propertyName, propValue);
        }

        #region interface
        public event Action<ObservableMessage> OnBindingUpdate;

        public void RaiseBindingUpdate(string memberName, object paramater)
        {
            if (OnBindingUpdate != null)
            {
                TaskManager.RunOnMainThread(() =>
                {
                    _bindingMessage.Name = memberName;
                    _bindingMessage.Sender = this;
                    _bindingMessage.Value = paramater;

                    OnBindingUpdate(_bindingMessage);

                });
            }
        }

        [HideInInspector]
        public object GetValue(string memberName)
        {
            var member = _myType.GetMember(memberName).FirstOrDefault();

            if (member == null)
            {
                Debug.LogError("Member not found ! " + memberName + " " + _myType);
                return null;
            }

            return member.GetMemberValue(_instance);
        }

        public object GetValue(string memberName, object paramater)
        {
            var member = _myType.GetMethod(memberName);

            if (member == null)
            {
                Debug.LogError("Member not found ! " + memberName + " " + _myType);
                return null;
            }


            if (paramater != null)
            {
                var p = member.GetParameters().FirstOrDefault();
                if (p == null)
                {
                    return GetValue(memberName);
                }

                var converted = ConverterHelper.ConvertTo(p.GetType(), paramater);
                return member.Invoke(_instance, new[] { converted });
            }

            return member.Invoke(_instance, null);
        }

        [HideInInspector]
        public void Command(string memberName)
        {
            Command(memberName, null);
        }

        public void Command(string memberName, object paramater)
        {
            var member = _myType.GetMember(memberName).FirstOrDefault();

            if (member == null)
            {
                Debug.LogError("Member not found ! " + memberName + " " + _myType);
                return;
            }

            // convert to fit signature
            var converted = ConverterHelper.ConvertTo(member.GetParamaterType(), paramater);

            if (member is MethodInfo)
            {
                var method = member as MethodInfo;
                if (method.ReturnType == typeof(IEnumerator))
                {
                    if (_insanceBehaviour)
                    {
                        if (!_insanceBehaviour.gameObject.activeSelf)
                        {
                            Debug.LogError("Behaviour is inactive ! " + _insanceBehaviour);
                        }

                        // via built in
                        if (converted == null)
                            _insanceBehaviour.StartCoroutine(method.Name);
                        else
                            _insanceBehaviour.StartCoroutine(method.Name, converted);

                    }
                    else
                    {
                        // via helper
                        var routine = method.Invoke(_instance, converted == null ? null : new[] { converted });
                        TaskManager.StartRoutine((IEnumerator)routine);
                    }
                    return;
                }
            }

            member.SetMemberValue(_instance, converted);
        }

        [HideInInspector]
        public void Dispose()
        {
            _bindingMessage.Dispose();

            if (_bindableInstance != null)
            {
                _bindableInstance.OnBindingUpdate -= _bindableInstance_OnBindingUpdate;
            }
            
            _myType = null;
            _instance = null;
            _insanceBehaviour = null;
            _bindableInstance = null;
            _bindingMessage = null;
        }
        #endregion


    }
}