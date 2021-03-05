//----------------------------------------------------------------------------------------
// Module name: CheckNull.cs
//
// Author:      Oleksandr Patalakha
//
// Created at:  2017.07.10
//
// Description:  Checks objects and value types on null
//             
//----------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Reflection;

namespace MagicUnion.Fun
{
    /* //Net  since Oktober 2016
     * //https://msdn.microsoft.com/en-us/library/system.reflection.introspectionextensions.gettypeinfo(v=vs.110).aspx
     * //
    static class Check<T>
        {
            static readonly bool IsValueType;
            static readonly bool IsNullable;

            static Check()
            {
                IsNullable = Nullable.GetUnderlyingType(typeof(T)) != null;
                IsValueType = typeof(T).GetTypeInfo().IsValueType;   
                            
            }
        //IsValueType
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsNull(T value) =>
                IsNullable
                    ? value.Equals(default(T))
                    : !IsValueType && EqualityComparer<T>.Default.Equals(value, default(T));
    }
    */

    #region Both classes move to other file
    /// <summary>
    /// File logging
    /// </summary>
    public static class LogConfig
    {
        /// <summary>
        /// Logger
        /// </summary>
        public static Action<Exception> ErrorLogger = ex => { };
    }

    /// <summary>
    /// Monad Disposable interface
    /// </summary>
    public interface IMonadDisposable : IDisposable
    {
    }
    #endregion

    /// <summary>
    /// Type safe Null check of structures and objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Check<T>
    {
        static readonly bool IsValueType;
        static readonly bool IsNullable;

        static Check()
        {
            IsNullable = Nullable.GetUnderlyingType(typeof(T)) != null;
            IsValueType = typeof(T).IsValueType;
        }

        
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// IsNull
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNull(T value)
        {
            return Check<T>.IsNullable
                ? value.Equals(default(T))
                : !IsValueType && EqualityComparer<T>.Default.Equals(value, default(T));
        }
        /// <summary>
        /// ThrowIfNull
        /// </summary>
        /// <param name="value"></param>
        /// <param name="location"></param>
        public static void ThrowIfNull(T value, string location = "Check")
        {
            if (IsNull(value))
                throw new ArgumentNullException("'" + location + "' result is null.  Not allowed.");
        }

        /// <summary>
        /// IsNullReturn
        /// </summary>
        /// <param name="value"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static T IsNullReturn(T value, string location)
        {
            if (IsNull(value))
                throw new ArgumentNullException("'" + location + "' result is null.  Not allowed.");

            return value;
        }
    }

    /// <summary>
    /// Lambda extension
    /// </summary>
    public static class Ext
    {
        /// <summary>
        /// Projects values into a lambda
        /// Useful when one needs to declare a local variable which breaks your expression.  
        /// This allows you to keep the expression going.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="projector"></param>
        /// <returns></returns>
        public static R map<T1, T2, R>(T1 value1, T2 value2, Func<T1, T2, R> projector)
        {
            return projector(value1, value2);
        }
    }
}
