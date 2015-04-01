// -------------------------------------
//  Domain		: Avariceonline.com
//  Author		: Nicholas Ventimiglia
//  Product		: Unity3d Foundation
//  Published		: 2015
//  -------------------------------------
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Foundation.Databinding.Model;
using Foundation.Databinding.View;
using UnityEngine;

namespace Foundation.Databinding.Components
{
    /// <summary>
    /// Presents an array, observable collection or other IEnumerable visually.
    /// </summary>
    /// <remarks>
    /// The Prefab (child template) should have a binding context set to Mock binding
    /// </remarks>
    [AddComponentMenu("Foundation/Databinding/ListBinder")]
    public class ListBinder : BindingBase
    {
        /// <summary>
        /// Child Item Template
        /// </summary>
        /// <remarks>
        /// The Prefab (child template) should have a binding context set to Mock binding
        /// </remarks>
        public GameObject Prefab;

        /// <summary>
        /// Shown when loading
        /// </summary>
        public GameObject LoadingMask;

        /// <summary>
        /// True if this is a static list.
        /// The list will only update once. 
        /// </summary>
        public bool OneTime;

        [HideInInspector]
        public BindingInfo SourceBinding = new BindingInfo { BindingName = "DataSource" };

        protected IObservableCollection DataList;
        protected List<BindingContext> Children = new List<BindingContext>();
        protected bool IsLoaded;
        protected bool IsInit;
        protected int Index = 0;
        protected RectTransform RectTransform;
        protected RectTransform RectTransform2;
        

        /// <summary>
        /// Bottom Up as opposed to Top Down
        /// </summary>
        public bool SetAsFirstSibling = false;
        
        void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            RectTransform2 = Prefab.GetComponent<RectTransform>();

            if (Prefab)
            {
                if (Application.isPlaying)
                    Prefab.transform.SetParent(transform.parent);
                Prefab.SetActive(false);

                if (Prefab.GetComponent<BindingContext>() == null)
                {
                    Debug.LogError("template item must have an Root.");
                    enabled = false;
                }
            }

            if (LoadingMask)
                LoadingMask.SetActive(false);

            Init();

        }

        public override void Init()
        {
            if (IsInit)
                return;
            IsInit = true;

            SourceBinding.Action = UpdateSource;
            SourceBinding.Filters = BindingFilter.Properties;
            SourceBinding.FilterTypes = new[] { typeof(IEnumerable) };
        }


        protected void UpdateSource(object value)
        {
            if (OneTime && IsLoaded)
                return;

            Bind(value);
        }

        /// <summary>
        /// Bind the ObservableCollection
        /// </summary>
        /// <param name="data"></param>
        public void Bind(object data)
        {
            if (DataList != null)
            {
                DataList.OnObjectAdd -= OnAdd;
                DataList.OnObjectInsert -= OnInsert;
                DataList.OnClear -= OnClear;
                DataList.OnObjectRemove -= OnRemove;
            }

            foreach (var item in Children)
            {
                item.DataInstance = null;

                Recycle(item.gameObject);
            }

            DataList = null;
            OnClear();
            StopAllCoroutines();

            if (data is IObservableCollection)
            {
                DataList = data as IObservableCollection;

                StartCoroutine(AddAsync(DataList.GetObjects()));

                DataList.OnObjectAdd += OnAdd;
                DataList.OnObjectInsert += OnInsert;
                DataList.OnClear += OnClear;
                DataList.OnObjectRemove += OnRemove;

                IsLoaded = true;
            }
            else if (data is IEnumerable)
            {
                var a = data as IEnumerable;

                StartCoroutine(AddAsync(a.Cast<object>()));
                IsLoaded = true;
            }
        }

        void OnClear()
        {
            foreach (var item in Children)
            {
                item.DataInstance = null;

                Recycle(item.gameObject);
            }

            Children.Clear();

            IsLoaded = false;
        }

        void OnRemove(object obj)
        {
            var view = Children.FirstOrDefault(o => o.DataInstance == obj);

            if (view != null)
            {
                view.DataInstance = null;

                Children.Remove(view);

                Recycle(view.gameObject);
            }
        }

        void OnAdd(object obj)
        {
            var view = GetNext();

            var root = view.GetComponent<BindingContext>();

            root.DataInstance = obj;

            view.name = "_Item " + Index++;

            Children.Add(root);

            if (SetAsFirstSibling)
                view.transform.SetAsFirstSibling();

        }

        void OnInsert(int location, object obj)
        {
            var view = GetNext();

            var root = view.GetComponent<BindingContext>();

            root.DataInstance = obj;

            view.name = "_Item " + Index++;

            Children.Insert(location, root);


            if (SetAsFirstSibling)
                view.transform.SetSiblingIndex(location);
        }

        IEnumerator AddAsync(IEnumerable<object> objects)
        {
            if (LoadingMask)
                LoadingMask.SetActive(true);

            foreach (var obj in objects)
            {
                OnAdd(obj);
            }

            if (LoadingMask)
                LoadingMask.SetActive(false);

            yield return 1;
        }

        GameObject GetNext()
        {
            if (RectTransform && RectTransform2)
            {
                //var next = Instantiate(Prefab, RectTransform.position, RectTransform.rotation) as GameObject;
                //var rect = next.GetComponent<RectTransform>();
                //rect.parent = RectTransform;
                //// idk, scale sometimes goes wierd
                //rect.localScale = RectTransform2.localScale;
                //next.SetActive(true);
                //return next;

                var next = Instantiate(Prefab, RectTransform.position, RectTransform.rotation) as GameObject;
                next.transform.SetParent(RectTransform);

                // idk, scale sometimes goes wierd
                next.transform.localScale = RectTransform2.localScale;
                next.SetActive(true);
                return next;
            }
            else
            {
                var next = Instantiate(Prefab, transform.position, transform.rotation) as GameObject;
                next.transform.SetParent(RectTransform);

                // idk, scale sometimes goes wierd
                next.transform.localScale = Prefab.transform.localScale;
                next.SetActive(true);
                return next;
            }

        }


        void Recycle(GameObject instance)
        {
            Destroy(instance);
        }

    }
}