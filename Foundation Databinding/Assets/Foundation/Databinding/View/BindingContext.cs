// -------------------------------------
//  Domain		: Avariceonline.com
//  Author		: Nicholas Ventimiglia
//  Product		: Unity3d Foundation
//  Published		: 2015
//  -------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Databinding.Model;
using UnityEngine;

namespace Foundation.Databinding.View
{
    /// <summary>
    /// Mediates between the model and the data-binders. Place this on the root of your visual tree.
    /// </summary>
    /// <remarks> 
    /// Use the MonoBinding to bind to a monobehaviour.
    /// Use the PropertyBinding to bind to a inner property of a parent model
    /// Use the MockBinding to bind against a model template (usefull for item controls)
    /// </remarks>
    [AddComponentMenu("Foundation/Databinding/BindingContext")]
    [ExecuteInEditMode]
    public class BindingContext : MonoBehaviour, IBindingElement
    {
        #region Editor stuff

        private static Assembly[] _assemblies;
        public static Assembly[] Assemblies
        {
            get
            {
                RefreshAssembly();
                return _assemblies;
            }
        }

        private static Type[] _modelTypes;
        public static Type[] ModelTypes
        {
            get
            {
                RefreshAssembly();
                return _modelTypes;
            }
        }

        private static string[] _namespaces;
        public static string[] NameSpaces
        {
            get
            {
                RefreshAssembly();
                return _namespaces;
            }
        }

        static void RefreshAssembly()
        {
            if (_assemblies == null)
            {

                var a = AppDomain.CurrentDomain.GetAssemblies().Where(o => !o.Location.Contains("Editor"));
                var b = a.OrderBy(o => o.FullName).ToArray();

                // Ignore UnityEngine types
                _assemblies = b;
                _modelTypes = _assemblies.SelectMany(o => o.GetTypes()).Where(o => o.IsPublic).OrderBy(o => o.Name).ToArray();
                _namespaces = _modelTypes.Select(o => o.Namespace).OrderBy(o => o).Distinct().ToArray();
            }
        }

        /// <summary>
        /// Ways to discover a view model
        /// </summary>
        public enum BindingContextMode
        {
            None,
            MonoBinding,
            MockBinding,
            PropBinding,
        }


        [HideInInspector]
        public BindingContextMode ContextMode;

        [HideInInspector]
        public bool ModelIsMock;

        /// <summary>
        /// MockBinding field
        /// </summary>
        [HideInInspector]
        public string ModelAssembly;

        /// <summary>
        /// MockBinding field
        /// </summary>
        [HideInInspector]
        public string ModelNamespace;

        /// <summary>
        /// MockBinding field
        /// </summary>
        [HideInInspector]
        public string ModelFullName;

        /// <summary>
        /// MockBinding field
        /// </summary>
        [HideInInspector]
        public string ModelType;

        /// <summary>
        /// MonoBinding Field
        /// </summary>
        [HideInInspector]
        public MonoBehaviour ViewModel;

        /// <summary>
        /// Gets Data ValueType from backing values
        /// </summary>
        /// <returns></returns>
        public Type GetDataType()
        {
            return ModelTypes.FirstOrDefault(o => o.FullName == ModelFullName);
        }

        /// <summary>
        /// Has a data type (from backing values)
        /// </summary>
        /// <returns></returns>
        public bool HasDataType()
        {
            return !string.IsNullOrEmpty(ModelFullName) && !string.IsNullOrEmpty(ModelNamespace);
        }

        #endregion
        private Type _dataType;
        /// <summary>
        /// The current model type. Used by editors.
        /// </summary>
        public Type DataType
        {
            get { return _dataType; }
            set
            {
                if (_dataType == value)
                    return;

                _dataType = value;

                if (DebugMode)
                {
                    Debug.Log(GetInstanceID() + ":" + name + ": " + " DataType " + value);
                }

                if (value != null)
                {
                    ModelType = value.Name;
                    ModelFullName = value.FullName;
                    ModelNamespace = value.Namespace;
                    ModelAssembly = value.Assembly.FullName;
                }
            }
        }

        private object _dataInstance;
        /// <summary>
        /// The current model instance. May be set remotely
        /// </summary>
        public object DataInstance
        {
            get { return _dataInstance; }
            set
            {
                if (_dataInstance == value)
                    return;

                if (DebugMode)
                {
                    Debug.Log(GetInstanceID() + ":" + name + ": " + " DataInstance " + value);
                }

                OnRemoveInstance();

                _dataInstance = value;

                if (DataInstance != null)
                    OnAddInstance();
            }
        }

        protected bool IsWrappedBinder;

        // private type casts of the Data Instance
        protected IObservableModel BindableContext { get; set; }

        /// <summary>
        /// All subscribes child binders
        /// </summary>
        protected List<IBindingElement> Binders = new List<IBindingElement>();

        /// <summary>
        /// Prints debug messages. It can get spammy
        /// </summary>
        public bool DebugMode;

        // ReSharper disable once UnusedMember.Resource
        protected void OnEnable()
        {
            FindModel();
          //  SubscribeBinder(this);
        }

        protected void OnDisable()
        {
            if (Application.isPlaying)
                Context = null;
        }


        [ContextMenu("Find Model")]
        public void FindModel()
        {
            switch (ContextMode)
            {
                case BindingContextMode.MonoBinding:
                    Context = null;
                    DataInstance = ViewModel;
                    break;

                case BindingContextMode.MockBinding:
                    Context = null;
                    if (DataType == null)
                        DataType = GetDataType();
                    break;

                case BindingContextMode.PropBinding:
                    Context = transform.parent.GetComponentInParent<BindingContext>();
                    if (!Application.isPlaying)
                        SetPropertyTypeData();
                    break;
                case BindingContextMode.None:
                    ClearTypeData();
                    DataInstance = null;
                    break;
            }
        }

        void OnRemoveInstance()
        {
            if (!Application.isPlaying)
                return;

            // unsubscribe
            if (BindableContext != null)
            {
                BindableContext.OnBindingUpdate -= RelayBindingUpdate;
                if (IsWrappedBinder)
                    ((ModelBinder)BindableContext).Dispose();
            }

            // remove type casts
            BindableContext = null;
            IsWrappedBinder = false;

            // set Binder DataInstance
            var array = Binders.ToArray();

            for (int i = 0;i < array.Length;i++)
            {
                //was removed
                if (array[i].Context != this)
                    continue;

                array[i].Model = null;
            }
        }

        void OnAddInstance()
        {
            if (DebugMode)
            {
                Debug.Log(GetInstanceID() + ":" + name + ": " + " OnAddInstance");
            }


            // set the data type if not null (Used By Editor Inspector).
            DataType = DataInstance.GetType();

            if (!Application.isPlaying)
                return;

            // set type casts
            BindableContext = DataInstance as IObservableModel;

            if (BindableContext == null)
            {
                BindableContext = new ModelBinder(DataInstance);
                IsWrappedBinder = true;
            }

            // subscribe to down messages

            BindableContext.OnBindingUpdate += RelayBindingUpdate;

            // Tell the binders of the new data instance
            var array = Binders.ToArray();
            for (int i = 0;i < array.Length;i++)
            {
                //was removed
                if (array[i].Context != this)
                    continue;

                array[i].Model = BindableContext;
            }
        }

        [ContextMenu("Clear TypeData")]
        public void ClearTypeData()
        {
            DataType = null;
            Context = null;
            Model = null;
            ModelType = null;
            ModelFullName = null;
            ModelNamespace = null;
            ModelAssembly = null;
        }

        /// <summary>
        /// Registers the binder with the this context.
        /// </summary>
        /// <param name="child"></param>
        public void SubscribeBinder(IBindingElement child)
        {

            if (!Binders.Contains(child))
                Binders.Add(child);

            child.Model = BindableContext;
        }

        /// <summary>
        /// Unregisters the binder with this context
        /// </summary>
        /// <param name="child"></param>
        public void UnsubscribeBinder(IBindingElement child)
        {
            child.Model = null;

            Binders.Remove(child);
        }

        /// <summary>
        /// Clears all children
        /// </summary>
        public void ClearBinders()
        {
            Binders.Clear();
            SubscribeBinder(this);
        }

        /// <summary>
        /// Posts a message down to data binders
        /// </summary>
        /// <param name="message"></param>
        void RelayBindingUpdate(ObservableMessage message)
        {
            var array = Binders.ToArray();

            for (int i = 0;i < array.Length;i++)
            {
                array[i].OnBindingMessage(message);
            }

            OnBindingMessage(message);
        }

        [ContextMenu("Debug Info")]
        public void DebugInfo()
        {
            Debug.Log("Binders : " + Binders.Count);
            Debug.Log("ValueType : " + ModelType);
            Debug.Log("Instance : " + DataInstance);
        }



        // For Hierarchical Binding
        #region IBindingElment

        private BindingContext _parentContext;
        /// <summary>
        /// For hierarchal binding.
        /// </summary>
        public BindingContext Context
        {
            get { return _parentContext; }
            set
            {
                if (_parentContext == value)
                    return;

                if (_parentContext != null)
                {
                    _parentContext.UnsubscribeBinder(this);
                }

                _parentContext = value;

                if (_parentContext != null)
                {
                    _parentContext.SubscribeBinder(this);
                }
            }
        }

        [SerializeField]
        private IObservableModel _model;
        public IObservableModel Model
        {
            get { return _model; }
            set
            {
                if (_model == value)
                    return;

                _model = value;

                InitialValue();
            }
        }

        /// <summary>
        /// For hierarchal binding.
        /// </summary>
        [HideInInspector]
        [SerializeField]
        private string _propertyName;
        public string PropertyName
        {
            get { return _propertyName; }
            set
            {
                if (_propertyName == value)
                    return;

                _propertyName = value;
                SetPropertyTypeData();
                InitialValue();
            }
        }

        public void OnBindingMessage(ObservableMessage message)
        {
            if (message.Name == PropertyName)
            {
                DataInstance = message.Value;

                var array = Binders.ToArray();

                for (int i = 0;i < array.Length;i++)
                {
                    array[i].OnBindingRefresh();
                }
            }
        }

        public void OnBindingRefresh()
        {
        }

        /// <summary>
        /// For hierarchal binding.
        /// </summary>
        void InitialValue()
        {
            // ignore if editor
            if (!Application.isPlaying)
                return;

            // ignore if empty
            if (string.IsNullOrEmpty(PropertyName))
                return;

            // ignore if null
            if (Model == null)
                return;

            if (DebugMode && Application.isPlaying)
            {
                Debug.Log(GetInstanceID() + ":" + name + ": " + "InitialValue : " + Model.GetValue(PropertyName));
            }

            // Set the local Instance
            DataInstance = Model.GetValue(PropertyName);

        }

        void SetPropertyTypeData()
        {
            if (Application.isPlaying)
                return;

            if (String.IsNullOrEmpty(PropertyName))
                return;

            if (Context == null)
                return;

            if (Context.DataType == null)
                return;

            // Use reflection to set local type (Value will be null, no instance
            var member = Context.DataType.GetMember(PropertyName).FirstOrDefault();

            if (member == null)
                return;

            if (member is FieldInfo)
            {
                DataType = ((FieldInfo)member).FieldType;
            }

            if (member is PropertyInfo)
            {
                DataType = ((PropertyInfo)member).PropertyType;
            }
        }
        #endregion



    }
}

