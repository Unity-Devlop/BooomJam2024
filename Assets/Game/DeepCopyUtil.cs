using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace UnityToolkit
{
    public static class DeepCopyUtil
    {
        public static T DeepCopyByJson<T>(T obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json);
        }


        // public static T DeepCopyByReflect<T>(T obj)
        // {
        //     //如果是字符串或值类型则直接返回
        //     if (obj is string || obj.GetType().IsValueType) return obj;
        //     object retval = Activator.CreateInstance(obj.GetType());
        //     FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic |
        //                                                  BindingFlags.Instance | BindingFlags.Static);
        //     foreach (FieldInfo field in fields)
        //     {
        //         try
        //         {
        //             field.SetValue(retval, DeepCopyByReflect(field.GetValue(obj)));
        //         }
        //         catch
        //         {
        //         }
        //     }
        //
        //     return (T)retval;
        // }


        //         /// <summary>
        // /// 提供对象或集合的深拷贝（拷贝private/public：实例成员、属性、静态成员）
        // /// </summary>
        // /// <typeparam name="T">对象类型或集合元素类型</typeparam>
        // public class DeepCopyHelper<T> where T : class, new() // 需要无参构造函数，构造表达式树的时候需要利用无参构造函数创建对象
        // {
        //     /// <summary>
        //     /// 映射表达式，泛型缓存每个类型存一份
        //     /// </summary>
        //     private static readonly Func<T, T> s_CopyFunc = null;
        //
        //     /// <summary>
        //     /// 静态构造函数，每个泛型类型会且只会执行一次
        //     /// </summary>
        //     static DeepCopyHelper()
        //     {
        //         BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        //         ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "m"); // 参数m :m => 
        //         List<MemberBinding> memberBindingList = new List<MemberBinding>();
        //         foreach (var item in typeof(T).GetProperties(bindingFlags))
        //         {
        //             if (!item.CanWrite) // 只读属性不拷贝
        //             {
        //                 continue;
        //             }
        //             MemberExpression property = Expression.Property(parameterExpression, item); // m.Name
        //             MemberBinding memberBinding = Expression.Bind(item, property); // Name = m.Name
        //             memberBindingList.Add(memberBinding);
        //         }
        //         foreach (var item in typeof(T).GetFields(bindingFlags))
        //         {
        //             MemberExpression property = Expression.Field(parameterExpression, item);
        //             MemberBinding memberBinding = Expression.Bind(item, property);
        //             memberBindingList.Add(memberBinding);
        //         }
        //         MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(T)), memberBindingList.ToArray());// new T() {Name = m.Name}
        //         Expression<Func<T, T>> lambda = Expression.Lambda<Func<T, T>>(memberInitExpression, new ParameterExpression[]
        //         {
        //                 parameterExpression
        //         }); // m => new T() {Name = m.Name}
        //
        //         s_CopyFunc = lambda.Compile();
        //     }
        //
        //     /// <summary>
        //     /// 对象拷贝（拷贝private/public：实例成员、属性、静态成员）
        //     /// </summary>
        //     /// <param name="data">源</param>
        //     /// <returns></returns>
        //     public static T DeepCopy(T data)
        //     {
        //         return s_CopyFunc(data);
        //     }
        //
        //     /// <summary>
        //     /// 集合拷贝（拷贝private/public：实例成员、属性、静态成员）
        //     /// </summary>
        //     /// <param name="data">源</param>
        //     /// <returns></returns>
        //     public static List<T> DeepCopyList(List<T> data)
        //     {
        //         if (data == null)
        //         {
        //             return null;
        //         }
        //         if (data.Count == 0)
        //         {
        //             return new List<T>();
        //         }
        //         return data.Select(a => DeepCopy(a)).ToList();
        //     }
        // }
    }
}