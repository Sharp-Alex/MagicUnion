//----------------------------------------------------------------------------------------
// Module name: Maybe.cs
//
// Author:      Oleksandr Patalakha
//
// Created at:  2017.07.10
//
// Description:  Maybe(or Optional) aka Haskel Maybe monad
//             
//----------------------------------------------------------------------------------------
// Third party code. No warnings
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;

namespace MagicUnion.Fun
{
   
    /// <summary>
    /// Maybe monad
    /// http://www.codefugue.com/haskell-in-c-sharp-functors/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("Maybe.Some={IsSome} Value={Value} Type={typeof(T).Name}")]
    public struct Maybe<T>: IEquatable<Maybe<T>>, IEnumerable<T>
    {
        readonly T _value;

        readonly bool _hasValue;

        /// <summary>
        /// Protected constroctor.
        /// 1.Interface instead of object.
        /// Monads laws are implemented as extension functions(e.g. FMap, LiftA, LiftM) on the Maybe interface, not a Maybe class.
        /// Since these function signatures will be automatically resolved by c# compiler, 
        /// only when we are using Maybe interface and not a Maybe class signature,
        /// we forbid construction of Maybe class objects and allow only Maybe interface.
        /// Thus for all created objects all extension function will be automatically resolved by c# compiler.
        /// 
        /// 2.Limit number of Nothings. Hide constructor and allow only Factory. Only Constructor can produe new nothings.
        /// 
        /// 
        /// </summary>
        /// <param name="t"></param>
        private Maybe(T t)
        {            
            if (Check<T>.IsNull(t))
            {
                _value = t;
                _hasValue = false;
                return;
            }

            _value = t;
            _hasValue = true;
        }

        /// <summary>
        /// Identity function to return Maybe monad from any value
        /// Null values are converted to Maybe.Nothing value
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Maybe<T> Return(T t)
        {
            //null into nothing
            if (Check<T>.IsNull(t))
                return Nothing;

            return new Maybe<T>(t);
        }

        /// <summary>
        /// Monad lifting.
        /// Binds (converts) Maybe(T) to Maybe(R).
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="lift"></param>
        /// <returns></returns>
        public Maybe<R> LiftM<R>(Func<T, Maybe<R>> lift )
        {
            return IsSome
                ? lift(Value)
                : Maybe<R>.Nothing;
        }

        /// <summary>
        /// Gets true if monad has no value
        /// </summary>
        public bool IsNothing
        {
            get { return !IsSome; }
        }

        /// <summary>
        /// Gets true, when monad has some value
        /// </summary>
        public bool IsSome 
        { 
            get 
            { 
                return _hasValue; 
            } 
        }

        /// <summary>
        /// Gets true, when monad has some value
        /// </summary>
        public bool HasValue
        {
            get
            {
                return _hasValue;
            }
        }

        /// <summary>
        /// Gets value
        /// </summary>
        public T Value { get { return _value; } }

        /// <summary>
        /// Nothing - empty or no value
        /// </summary>
        public readonly static Maybe<T> Nothing = new Maybe<T>(); //because of Unit - it has always value and it has only single value.

        /// <summary>
        /// Implicit conversion to Maybe monad
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static implicit operator Maybe<T>(T t)
        {
            return Maybe<T>.Return(t);
        }

        /// <summary>
        /// Overwritten - gets hash code of the containing value if it has one or zero
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return IsSome
                ? Value.GetHashCode()
                : 0;
        }

        /// <summary>
        /// Overwritten - gets compares the containing value if it has one.
        /// Two Nothings are equal.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Maybe<T> other)
        {
            return IsSome && other.IsSome
                ? EqualityComparer<T>.Default.Equals(Value, other.Value)
                : IsNothing && other.IsNothing
                    ? true
                    : false;
        }

        /// <summary>
        /// Overwritten - gets compares the containing value if it has one.
        /// Two Nothings are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(null, obj)
               ? false
               : obj is Maybe<T>
                   ? Equals((Maybe<T>)obj)
                   : IsSome
                       ? Value.Equals(obj)
                       : false;
        }

        /// <summary>
        /// Overwritten - gets compares the containing value if it has one.
        /// Two Nothings are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Maybe<T> left, Maybe<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Overwritten - gets compares the containing value if it has one.
        /// Two Nothings are equal.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Maybe<T> left, Maybe<T> right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// IEnumerator(T) interface
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (IsSome)
            {
                yield return Value;
            }
        }

        /// <summary>
        /// IEnumerator interface
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (IsSome)
            {
                yield return Value;
            }
        }

        /// <summary>
        /// Overwritten - gets: Some(type name)=value.ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return IsSome
                ? "Some<" + typeof(T).Name + ">=" + _value.ToString()
                : "Nothing<" + typeof(T).Name + ">";
        }
    }

    /// <summary>
    /// Maybe extensions provide plain factory methods from any value
    /// </summary>
    public static class Maybe
    {
        /// <summary>
        /// Creates Maybe from the given value.
        /// If value is null creates Maybe.Nothing
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Maybe<T> Some<T>(T t)
        {
            return Maybe<T>.Return(t);
        }

        /// <summary>
        /// Creates Maybe with no value - Nothing
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Maybe<T> Nothing<T>()
        {
            return Maybe<T>.Nothing;
        }
    }

    /// <summary>
    /// A. Why nullables are bad.
    ///  1. Nullable(Nullable(T)) is not allowed - bad. All numbers between 0 and infinity have no special meaning, it is either 0 or any number upto infinity.
    ///     https://stackoverflow.com/questions/12476590/why-is-nullablet-nullable-why-it-cannot-be-reproduced
    ///     And as consequence we can not define monadic Join operation for Nullables.
    ///  2. Objects can not be nullables. We can not pack object into nullable struct.
    ///     As a result we forced to have 2 differnet checks for null for objects and for nullables(struct): obj!=null and val.HasValue, which is crazy.
    ///    
    ///  Summary: Because of above limittions we need to convert Nullable into Maybe, which does not have this synthetic limitations
    /// 
    /// B. Why Struct are bad.
    ///  1. They allow us only check either whole stuct is null.
    ///  2. From other side they do not forbid default parameterless struct constructor.
    ///     So any field of the struct could be invalid(not properly initialized)
    ///     https://social.msdn.microsoft.com/Forums/en-US/16a85f33-f015-447f-9981-879fa7a55e23/how-to-check-struct-is-not-declared?forum=csharplanguage
    ///  Summary: we can not relay on Factory method and create only valid structures, because parameterless constructor is always allowed.
    ///           we have to use HasValue field in every structure - to check either struct was properly initialized (double implementing maybe monad everywhere) 
    /// </summary>
    public static class NullableExt
    {
        public static Nullable<T> ToNullable<T>(this T t) 
            where T:struct
        {
            return new Nullable<T>(t);
        }
        public static Nullable<T> ToNullable<T>(this Maybe<T> t) where T : struct
        {
            return t.HasValue
                ? new Nullable<T>(t.Value)
                : null;
        }

        public static Maybe<T> ToMaybe<T>(this Nullable<T> t) where T : struct
        {
            return t.HasValue
                ? Maybe<T>.Return(t.Value)
                : Maybe<T>.Nothing;
        }

        public static Nullable<T> ReturnNullable<T>(this T t)
            where T : struct
        {
            return new Nullable<T>(t);
        }

        public static Nullable<R> LiftM<T, R>(this Nullable<T> t, Func<T, Nullable<R>> lift)
            where R : struct
            where T : struct
        {
            return t.HasValue
                ? lift(t.Value)
                : null;
        }

        public static Maybe<R> LiftM<T, R>(this Nullable<T> self, Func<T, Maybe<R>> lift)
            where T : struct
        {
            return self.ToMaybe().LiftM(lift);
        }
 
        public static Maybe<R> FMap<T, R>(this Nullable<T> self, Func<T, R> func)
             where T : struct
        {
            return self.ToMaybe().FMap(func);
        }


        public static Maybe<R> Select<T, R>(this Nullable<T> self, Func<T, R> func)
             where T : struct
        {
            return self.FMap(func);
        }

        public static Maybe<R> SelectMany<T, R>(this Nullable<T> self, Func<T, Maybe<R>> monad)
             where T : struct
        {
            return self.LiftM(monad);
        }
        public static Maybe<R> SelectMany<T, V, R>(this Nullable<T> self, Func<T, Maybe<V>> monad, Func<T, V, R> projector)
            where T : struct
        {
            return self.ToMaybe().SelectMany(monad, projector);
        }



        }

    /// <summary>
    /// Maybe extensions provide conversions to Maybe and to other Monads
    /// </summary>
    public static class MaybeExt
    {
        /// <summary>
        /// Convert any value to Maybe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Maybe<T> ToMaybe<T>(this T t)
        {
            return Maybe<T>.Return(t);
        }


        public static Maybe<T> MaybeAs<T>(this object t)
        {
            return Maybe<T>.Return((T)t);
        }

        public static Maybe<T> MaybeIs<T>(this object t)
        {

            return t is T ? Maybe<T>.Return((T)t) : Maybe.Nothing<T>();
        }

        /// <summary>
        /// Return first element as Maybe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Maybe<T> MaybeFirst<T>(this IEnumerable<T> t)
        {
            return t.Count() > 0
                ? t.First()
                : Maybe.Nothing<T>();            
        }

        public static Maybe<T> FindFirst<T>(this IEnumerable<T> t, Func<T, bool> predicate)
        {
            if( t.Count() <1)
                return Maybe.Nothing<T>();

            foreach( var el in t)
            {
                if (predicate(el))
                    return el;
            }
            return Maybe.Nothing<T>();
        }

        public static Maybe<T> FindLast<T>(this IEnumerable<T> t, Func<T, bool> predicate)
        {
            if (t.Count() < 1)
                return Maybe.Nothing<T>();

            for(int i=t.Count()-1; i>=0 ; i--)
            {
                var el = t.ElementAt(i);
                if (predicate(el))
                    return el;
            }
            return Maybe.Nothing<T>();
        }

        public static Maybe<T> MaybeFirst<T>(this IEnumerable<T> t, Func<T,bool> predicate)
        {
            return t.FindFirst(predicate);                
        }

        
        public static Maybe<T> MaybeLast<T>(this IEnumerable<T> t, Func<T, bool> predicate)
        {
            return t.FindLast(predicate);
        }


        /// <summary>
        /// Return ast element as Maybe
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Maybe<T> MaybeLast<T>(this IEnumerable<T> t)
        {
            return t.Count() > 0
                ? t.Last()
                : Maybe.Nothing<T>();
        }        

        //join
        /// <summary>
        /// Monadic join operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mmt"></param>
        /// <returns></returns>
        public static Maybe<T> Join<T>(this Maybe<Maybe<T>> mmt)
        {
            return mmt.LiftM<T>(t => t);
        }

        //functor
        /// <summary>
        /// Functor operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="mt"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Maybe<R> FMap<T, R>(this Maybe<T> mt, Func<T, R> func)
        {
            return mt.LiftM<R>(t =>Maybe<R>.Return(func(t)));
        }

        // applicative - maybe applies a maybe<f[m]> maybe<x[n]>
        /// <summary>
        /// Applicative lift
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="self"></param>
        /// <param name="applicative"></param>
        /// <returns></returns>
        public static Maybe<R> LiftA<T, R>(this Maybe<T> self, Maybe<Func<T, R>> applicative)
        {
            return applicative.LiftM<R>(func => self.FMap<T,R>(func));
        }

        /// <summary>
        /// Unlocks Linq, see Linq info
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="self"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Maybe<R> Select<T, R>(this Maybe<T> self, Func<T, R> func)
        {
            return self.FMap(func);
        }

        /// <summary>
        /// Unlocks linq, see Linq info
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="self"></param>
        /// <param name="monad"></param>
        /// <returns></returns>
        public static Maybe<R> SelectMany<T, R>(this Maybe<T> self, Func<T, Maybe<R>> monad)
        {
            return self.LiftM(monad);
        }

        /// <summary>
        /// Unlocks Linq, see Linq info
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="self"></param>
        /// <param name="monad"></param>
        /// <param name="projector"></param>
        /// <returns></returns>
        public static Maybe<R> SelectMany<T,V, R>(this Maybe<T> self, Func<T, Maybe<V>> monad, Func<T, V, R> projector)
        {
            return self.LiftM(t=> monad(t).FMap(v=> projector(t,v)));
        }


        /// <summary>
        /// Applies function according value matching
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="self"></param>
        /// <param name="Some"></param>
        /// <param name="None"></param>
        /// <returns></returns>
        public static Maybe<R> MatchM<T, R>(this Maybe<T> self, Func<T, R> Some, Func<R> None)
        {
            return self.IsSome
                ? Maybe<R>.Return(Some(self.Value))
                : Maybe<R>.Return(None());
        }


        /// <summary>
        /// Match both states of the Maybe and return a non-null R.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="self"></param>
        /// <param name="Some"></param>
        /// <param name="None"></param>
        /// <returns></returns>
        public static R Match<T, R>(this Maybe<T> self, Func<T, R> Some, Func<R> None)
        {
            return self.IsSome
                ? Check<R>.IsNullReturn(Some(self.Value), "Maybe.Match(Some)")
                : Check<R>.IsNullReturn(None(), "Maybe.Match(None)");
        }

        public static void MatchAction<T, R>(this Maybe<T> self, Action<T> Some, Action None)
        {
            if (self.IsSome)
                Some(self.Value);
            else
                 None();
        }

        /// <summary>
        /// Executes Some action only when Maybe has value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="Some"></param>
        public static void OnValue<T>(this Maybe<T> self, Action<T> Some)
        {
            if (self.HasValue)
                Some(self.Value);
        }

        /// <summary>
        /// Executes user action only when Maybe has no value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="Some"></param>
        public static void OnNoValue<T>(this Maybe<T> self, Action None)
        {
            if (!self.HasValue)
                None();
        }

        public static T GetValue<T>(this Maybe<T> self, T None)
        {
            if (self.HasValue)
                return self.Value;
            return None;
        }

        /// <summary>
        /// Unlocks where operator for Linq
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="pred"></param>
        /// <returns></returns>
        public static Maybe<T> Where<T>(this Maybe<T> self, Func<T, bool> pred)
        {
            return self.IsSome
                     ? pred(self.Value)
                         ? self
                         : Maybe<T>.Nothing
                     : self;

        }
    }

    /// <summary>
    /// Extension for any object
    /// </summary>
    public static class ObjectExt
    {
        /// <summary>
        /// Determines whether the specified object has interface I.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">The self.</param>
        /// <param name="I">The inrterface to check.</param>
        /// <returns>
        ///   <c>Maybe with Value true</c> if the specified object has interface; otherwise, <c>false</c>.
        /// </returns>
        public static Maybe<bool> HasInterface<T>(this T self, Type I)
        {
            return from obj in self.ToMaybe()
                   select self.GetType().GetInterfaces().Contains(I);
        }
    }
}
