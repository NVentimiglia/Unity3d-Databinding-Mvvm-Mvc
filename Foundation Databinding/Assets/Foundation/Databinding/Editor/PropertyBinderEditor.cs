//// --------------------------------------
////  Unity3d Mvvm Toolkit and Lobby
////  PropertyBinderEditor.cs
////  copyright (c) 2014 Nicholas Ventimiglia, http://avariceonline.com
////  All rights reserved.
////  -------------------------------------
//// 

//using System;
//using System.Linq;
//using Foundation.Core;
//using Foundation.Core.Databinding;
//using Foundation.Core.Editor;
//using UnityEditor;
//using UnityEngine;

//namespace Assets.Foundation.Core.Editor
//{
//    /// <summary>
//    /// Handles the finding of the Context
//    /// </summary>
//    [CustomEditor(typeof(PropertyBinderBase), true)]
//    public class PropertyBinderEditor : UnityEditor.Editor
//    {
//        protected PropertyBinderBase Target;

//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();

//            Target = target as PropertyBinderBase;

//            if (!Application.isPlaying)
//                Target.FindContext();


//            if (Target.OverrideEditor)
//                return;

//            if (Target.Context == null)
//            {
//                EditorGUILayout.LabelField("BindingContext not found.");
//                PropertyTextInput();
//            }
//            else if (Target.Context.DataType == null)
//            {
//                EditorGUILayout.LabelField("BindingContext.DataType not found.");
//                PropertyTextInput();
//            }
//            else
//            {
//                PropertyDropDown();
//            }
//        }

//        void PropertyDropDown()
//        {
//            var useUnity = EditorGUILayout.Toggle("Show UnityEngine Fields", Target.ShowUnityNames);

//            if (useUnity != Target.ShowUnityNames)
//            {
//                Target.ShowUnityNames = useUnity;
//                // Save the changes back to the object
//                EditorUtility.SetDirty(target);
//                return;
//            }

//            var type = Target.Context.DataType;

//            //var flags = BindingFlags.Public;

//            //// Fields, Properties and Methods without arguments
//            //var members = type.GetMembers(flags)
//            //       .Where(o => o is FieldInfo || o is PropertyInfo)
//            //       .Where(o => !o.HasAttribute<HideInInspector>()).ToArray();

//            var members = EditorMembersHelper.GetMembers(type);

//            if (members.Length == 0)
//            {
//                EditorGUILayout.LabelField(string.Format("{0} has no fields or properties.", type));
//                return;
//            }

//            var choices = Target.ShowUnityNames
//                ? members.OrderBy(o => o.Name)
//                : members.Where(o => !o.Module.Assembly.FullName.Contains("UnityEngine")).OrderBy(o => o.Name);

//            var labels = choices.Select(o =>
//                string.Format("{0} : {1}",
//                o.Name,
//                o.GetMemberType().Name
//                )).ToArray();

//            var names = choices.Select(o => o.Name).ToArray();

//            var index = Array.IndexOf(names, Target.PropertyName);

//            var i = EditorGUILayout.Popup("Property / Field", index, labels.ToArray());

//            if (i != index)
//            {
//                Target.PropertyName = names[i];
//                EditorUtility.SetDirty(target);
//            }
//        }

//        void PropertyTextInput()
//        {
//            var p = EditorGUILayout.TextField("Property", Target.PropertyName);

//            if (p != Target.PropertyName)
//            {
//                Target.PropertyName = p;
//                EditorUtility.SetDirty(target);
//            }
//        }
//    }
//}

