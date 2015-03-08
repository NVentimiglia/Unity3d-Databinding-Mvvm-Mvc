//// --------------------------------------
////  Unity3d Mvvm Toolkit and Lobby
////  CommandBinderEditor.cs
////  copyright (c) 2014 Nicholas Ventimiglia, http://avariceonline.com
////  All rights reserved.
////  -------------------------------------
//// 

//using System;
//using System.Collections;
//using System.Linq;
//using System.Reflection;
//using Foundation.Core;
//using UnityEditor;
//using UnityEngine;

//// ReSharper disable once CheckNamespace
//namespace Foundation.Core.Editor
//{
//    /// <summary>
//    /// Handles the finding of the Context
//    /// </summary>
//    [CustomEditor(typeof(CommandBinderBase), true)]
//    public class CommandBinderEditor : UnityEditor.Editor
//    {
//        protected CommandBinderBase Target;

//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();

//            Target = target as CommandBinderBase;

//            if (Application.isPlaying)
//            {
//                MethodNameInput();
//                return;
//            }

//            if (Target.OverrideEditor)
//                return;

//            // ReSharper disable once PossibleNullReferenceException
//            Target.FindContext();

//            if (Target.Context == null)
//            {
//                EditorGUILayout.LabelField("BindingContext not found.");
//                MethodNameInput();
//            }
//            else if (Target.Context.DataType == null)
//            {
//                EditorGUILayout.LabelField("BindingContext.DataType not found.");
//                MethodNameInput();
//            }
//            else 
//            {
//                var useUnity = EditorGUILayout.Toggle("Show UnityEngine Methods", Target.ShowUnityNames);

//                if (useUnity != Target.ShowUnityNames)
//                {
//                    Target.ShowUnityNames = useUnity;
//                    // Save the changes back to the object
//                    EditorUtility.SetDirty(target);
//                    return;
//                }

//                var type = Target.Context.DataType;

//                var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod;

//                var members = type.GetMethods(flags).Where(o =>
//                    o != null
//                    && !o.IsSpecialName
//                    && o.ReturnType == typeof(void) || o.ReturnType == typeof(IEnumerator)
//                    && o.GetParameters().Length < 2
//                    && !o.HasAttribute<HideInInspector>()
//                    ).ToArray();

//                var choices = Target.ShowUnityNames
//                 ? members.OrderBy(o => o.Name).ToArray()
//                 : members.Where(o => !o.Module.Assembly.FullName.Contains("UnityEngine")).OrderBy(o => o.Name).ToArray();


//                var labels = choices.Select(o => GetMethodLabel(o));
//                var index = Array.IndexOf(choices.Select(o => o.Name).ToArray(), Target.CommandName);

//                var i = EditorGUILayout.Popup("Methods", index, labels.ToArray());

//                if (i != index)
//                {
//                    Target.CommandName = choices[i].Name;

//                    EditorUtility.SetDirty(target);
//                }
//            }

//        }

//        string GetMethodLabel(MethodInfo m)
//        {
//            var p = m.GetParameters().FirstOrDefault();

//            if (p == null)
//                return m.Name;
//            return string.Format("{0} : {1}", m.Name, p.ParameterType.Name);
//        }

//        void MethodNameInput()
//        {
//            var p = EditorGUILayout.TextField("Method Name", Target.CommandName);

//            if (p != Target.CommandName)
//            {
//                Target.CommandName = p;
//                EditorUtility.SetDirty(target);
//            }
//        }
//    }
//}


