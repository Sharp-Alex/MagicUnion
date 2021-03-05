using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicUnion.Fun
{

    public static class Try
    {
        /// <summary>
        /// Executes given function inside of try-catch block and returns TryResult
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Result<T> Fun<T>(Func<T> func)
        {
            try
            {
                return Result<T>.Create(func());
            }
            catch (Exception e)
            {
                return Result<T>.Create(Error.Create(e.Message));
            }

        }

        public static Result<Empty> Action(Action act)
        {
            try
            {
                act();
                return Result<Empty>.Create(Empty.One);
            }
            catch (Exception e)
            {
                return Result<Empty>.Create(Error.Create(e));
            }

        }
    }

    public static class PipeExt
    {
        public static R Do<T, R>(T rt, Func<T, R> f)
        {
            return f(rt);
        }
    }

    public static class ResultExt
    {
        public static Result<T> ToResult<T>(this Error error)
        {
            return Result<T>.Create(error);
        }

        public static Result<T> ToResult<T>(this T t)
        {
            return Result<T>.Create(t);
        }        


        /// <summary>
        /// Monadic join operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mmt"></param>
        /// <returns></returns>
        public static Result<T> Join<T>(this Result<Result<T>> mmt)
        {
            return mmt.LiftM<T>(t => t);
        }

        /// <summary>
        /// Functor operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="mt"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Result<R> FMap<T, R>(this Result<T> mt, Func<T, R> func)
        {
            return mt.LiftM<R>(t => Result<R>.Create(func(t)));
        }

        /// <summary>
        /// Convert exception into new exception, e.g. into "user friendly" one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mt"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Result<T> FMapErr<T>(this Result<T> mt, Func<List<Error>, List<Error>> func)
        {
            return mt.Match(
                fromBottom: _ => Result<T>.None,
                fromValue: t => mt,
                fromErrors: e => Result<T>.Create(func(e)));
        }

        // applicative - maybe applies a maybe<f[m]> maybe<x[n]>
        /// <summary>
        /// Applicative lifting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="self"></param>
        /// <param name="applicative"></param>
        /// <returns></returns>
        public static Result<R> LiftA<T, R>(this Result<T> self, Result<Func<T, R>> applicative)
        {
            return applicative.LiftM<R>(func => self.FMap<T, R>(func));
        }

        /// <summary>
        /// Act like LiftM, func will be executed in try catch block
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="self"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Result<R> TrySelect<T, R>(this Result<T> self, Func<T, R> func)
        {
            return self.LiftM(t => Try.Fun(() => func(t)));
        }


        /// <summary>
        /// Unlocks LINQ for TryResult monad.
        /// See LINQ Select for more info.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="self"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Result<R> Select<T, R>(this Result<T> self, Func<T, R> func)
        {
            return self.FMap(func);
        }

        /// <summary>
        /// Unlocks LINQ for TryResult monad.
        /// See LINQ SelectMany for more info.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="self"></param>
        /// <param name="monad"></param>
        /// <returns></returns>
        public static Result<R> SelectMany<T, R>(this Result<T> self, Func<T, Result<R>> monad)
        {
            return self.LiftM(monad);
        }

    }

}
