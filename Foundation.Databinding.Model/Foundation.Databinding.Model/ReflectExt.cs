// -------------------------------------
//  Domain		: Avariceonline.com
//  Author		: Nicholas Ventimiglia
//  Product		: Unity3d Foundation
//  Published		: 2015
//  -------------------------------------
using System;
using System.Linq;
using System.Reflection;

namespace Foundation.Databinding.Model
{


    public static class ReflectionExt
    {
        /// <summary>
        /// return Attribute.IsDefined(m, typeof(T));
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this MemberInfo m) where T : Attribute
        {
#if UNITY_WSA
            return GetAttribute<T>(m) != null;
#else
            return Attribute.IsDefined(m, typeof(T));
#endif
        }

        /// <summary>
        ///  return m.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this MemberInfo m) where T : Attribute
        {
            return m.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
        }

        /// <summary>
        /// return Attribute.IsDefined(m, typeof(T));
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetAttribute<T>(this object m, string memberName) where T : Attribute
        {
#if UNITY_WSA
            var member = m.GetType().GetTypeInfo().DeclaredMembers.FirstOrDefault(o => o.Name == memberName);
#else           
            var member = m.GetType().GetMember(memberName, BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic).FirstOrDefault();
#endif

            if (member == null)
                return null;

            return member.GetAttribute<T>();

        }

        /// <summary>
        /// Returns the Return ValueType of the member
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static Type GetMemberType(this MemberInfo member)
        {
            if (member is MethodInfo)
                return ((MethodInfo)member).ReturnType;

            if (member is PropertyInfo)
                return ((PropertyInfo)member).PropertyType;

            return ((FieldInfo)member).FieldType;
        }

        /// <summary>
        /// Returns the Return ValueType of the member
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public static Type GetParamaterType(this MemberInfo member)
        {
            if (member is MethodInfo)
            {
                var p = ((MethodInfo)member).GetParameters().FirstOrDefault();
                if (p == null)
                    return null;
                return p.ParameterType;
            }

            if (member is PropertyInfo)
                return ((PropertyInfo)member).PropertyType;

            if (member is FieldInfo)
                return ((FieldInfo)member).FieldType;

            return null;
        }


        /// <summary>
        /// Set the member's instances value
        /// </summary>
        /// <returns></returns>
        public static void SetMemberValue(this MemberInfo member, object instance, object value)
        {
            if (member is MethodInfo)
            {
                var method = ((MethodInfo)member);

                if (method.GetParameters().Any())
                {
                    method.Invoke(instance, new[] { value });
                }
                else
                {
                    method.Invoke(instance, null);
                }
            }
            else if (member is PropertyInfo)
            {
                ((PropertyInfo)member).SetValue(instance, value, null);
            }
            else
            {
                ((FieldInfo)member).SetValue(instance, value);
            }
        }



        /// <summary>
        /// Set the member's instances value
        /// </summary>
        /// <returns></returns>
        public static object GetMemberValue(this MemberInfo member, object instance)
        {
            if (member is MethodInfo)
                return ((MethodInfo)member).Invoke(instance, null);
            if (member is PropertyInfo)
                return ((PropertyInfo)member).GetValue(instance, null);


            return ((FieldInfo)member).GetValue(instance);

        }

        /// <summary>
        /// Set the member's instances value
        /// </summary>
        /// <returns></returns>
        public static object GetMemberValue(this object instance, string propertyName)
        {
#if UNITY_WSA
            var member = instance.GetType().GetTypeInfo().DeclaredMembers.FirstOrDefault(o => o.Name == propertyName);
#else
            var member = instance.GetType().GetMember(propertyName).FirstOrDefault();
#endif
            if (member == null)
                return null;

            if (member is MethodInfo)
                return ((MethodInfo)member).Invoke(instance, null);
            if (member is PropertyInfo)
                return ((PropertyInfo)member).GetValue(instance, null);

            return ((FieldInfo)member).GetValue(instance);

        }


        /// <summary>
        /// Set the member's instances value
        /// </summary>
        /// <returns></returns>
        public static T GetMemberValue<T>(this MemberInfo member, object instance)
        {
            return (T)GetMemberValue(member, instance);

        }
    }
}