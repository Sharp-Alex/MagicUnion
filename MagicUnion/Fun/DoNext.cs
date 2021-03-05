using System;
using System.Collections.Generic;
using System.Text;

namespace MagicUnion.Fun
{
    public static class Next
    {
        public static Result<R> Do<T, R>(this T t, Func<T, R> f)
        {
            return Try.Fun(()=>f(t));
        }

        public static Result<Empty> Do<T>(this T t, Action<T> act)
        {
            return Try.Action(() => act(t));
        }

        public static Result<Empty> Do<T>(this Result<T> t, Action<T> act)
        {
            return t.Match(
                fromBottom: _ => Result<Empty>.None,
                fromValue:  v => Try.Action(() => act(v)),
                fromErrors: e => Result<Empty>.Create(e));
        }

        public static Result<R> Do<T,R>(this Result<T> t, Func<T,R> f)
        {
            return t.Match(
                fromBottom: _ => Result<R>.None,
                fromValue: v => Try.Fun(() => f(v)),
                fromErrors: e => Result<R>.Create(e));
        }
    }
    
}
