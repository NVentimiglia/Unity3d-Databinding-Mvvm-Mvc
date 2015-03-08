// -------------------------------------
//  Domain		: Avariceonline.com
//  Author		: Nicholas Ventimiglia
//  Product		: Unity3d Foundation
//  Published		: 2015
//  -------------------------------------
using System;
using System.Collections;
using Foundation.Tasks;
using UnityEngine;

namespace Foundation.Databinding.Model
{

    /// <summary>
    /// Implements the IObservableModel for clr objects
    /// </summary>
    public abstract class ObservableObject : IObservableModel
    {
        public event Action<ObservableMessage> OnBindingUpdate;

        private ModelBinder _binder;

        private ObservableMessage _bindingMessage;

        protected ObservableObject()
        {
            _bindingMessage = new ObservableMessage { Sender = this };
            _binder = new ModelBinder(this);
        }

        public void RaiseBindingUpdate(string memberName, object paramater)
        {
            if (OnBindingUpdate != null)
            {
                _bindingMessage.Name = memberName;
                _bindingMessage.Value = paramater;
                OnBindingUpdate(_bindingMessage);
            }

            _binder.RaiseBindingUpdate(memberName, paramater);
        }

        public void SetValue(string memberName, object paramater)
        {
            _binder.RaiseBindingUpdate(memberName, paramater);
        }

        public void Command(string memberName)
        {
            _binder.Command(memberName);
        }

        public void Command(string memberName, object paramater)
        {
            _binder.Command(memberName, paramater);
        }

        [HideInInspector]
        public object GetValue(string memberName)
        {
            return _binder.GetValue(memberName);
        }

        public object GetValue(string memberName, object paramater)
        {
            return _binder.GetValue(memberName, paramater);
        }

        [HideInInspector]
        public virtual void Dispose()
        {
            if (_binder != null)
            {
                _binder.Dispose();
            }

            if (_bindingMessage != null)
            {
                _bindingMessage.Dispose();
            }

            _bindingMessage = null;
            _binder = null;
        }

        public void NotifyProperty(string memberName, object paramater)
        {
            RaiseBindingUpdate(memberName, paramater);
        }

        /// <summary>
        /// Via CoroutineHandler
        /// </summary>
        /// <param name="routine"></param>
        public Coroutine StartCoroutine(IEnumerator routine)
        {
           return TaskManager.StartRoutine(routine);
        }

        /// <summary>
        /// Via CoroutineHandler
        /// </summary>
        /// <param name="routine"></param>
        public void StopCoroutine(IEnumerator routine)
        {
            TaskManager.StopRoutine(routine);
        }
    }
}