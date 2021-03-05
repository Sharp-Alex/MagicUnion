using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MagicUnion.Fun
{

    /// <summary>
    /// Discraminated union of following types: (<see cref="Empty"/>|<see cref="int"/>|<see cref="System.Collections.Generic.List<CreateType.Func.Error>"/>).
    /// Discraminated union is readonly and by field value equatable class, which implements <see cref="Create"/> and <see cref="Match"/> methods.    
    /// Any member of the union is accessible via <see cref="Match"/> method.    
    /// </summary>
    public partial struct Result<T> : IEquatable<Result<T>>
    {
        private Empty _bottom => Empty.One;

        public T Value => (T)_obj;

        public List<Error> Errors => (List<Error>)_obj;

        public bool IsBottom => _tag == 0;

        public bool HasValue => _tag == 1;

        public bool HasError => _tag == 2;
    }

    /// <summary>
    ///  Discraminated union of following types: (<see cref="Empty"/>|<see cref="int"/>|<see cref="System.Collections.Generic.List<CreateType.Func.Error>"/>).
    /// </summary>
    public partial struct Result<T> : IEquatable<Result<T>>
    {
        private readonly int _tag;

        private readonly object _obj;

        public static Result<T> None = new Result<T>();

        private Result(Empty Bottom)
        {
            _tag = 0;
            _obj = Bottom;
        }

        private Result(T Value)
        {
            _tag = 1;
            _obj = Value;
        }

        private Result(List<Error> Errors)
        {
            _tag = 2;
            _obj = Errors;
        }

        private Result(Error Error)
        {
            _tag = 2;
            _obj = Lst.New(Error); ;
        }

        public static Result<T> Create(Empty Bottom)
        {
            return None;
        }

        public static Result<T> CreateValue(T Value) //Alias in case if value is of Empty type
        {
            if (Check<T>.IsNull(Value))
            {
                return new Result<T>(Error.InvalidArgument("Value"));
            }
            return new Result<T>(Value);
        }

        public static Result<T> Create(T Value)
        {
            if (Check<T>.IsNull(Value))
            {
                return new Result<T>(Error.InvalidArgument("Value"));
            }
            return new Result<T>(Value);
        }

        public static Result<T> Create(List<Error> Errors)
        {
            if (Errors == null)
                return new Result<T>(Error.InvalidArgument("Errors"));

            return new Result<T>(Errors);
        }

        public static Result<T> Create(Error Error)
        {
            if (Error == null)
            {
                var str = new StackTrace().ToString();
                return new Result<T>(Error.InvalidArgument("Error", CallStack.Create(str)));
            }

            return new Result<T>(Error);
        }

        //op.todo Add With methods to Union
        public Result<T> With(Empty Bottom)
        {
            return Create(Bottom);
        }

        public Result<T> With(T Value)
        {
            return Create(Value);
        }

        public Result<T> With(List<Error> Errors)
        {
            return Create(Errors);
        }

        /// <summary>
        /// Implicit conversion from any value into TryResult monad
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static implicit operator Result<T>(T t)
        {
            return Result<T>.Create(t);
        }

        public Result<R> LiftM<R>(Func<T, Result<R>> lift)
        {
            return Match(
                fromBottom: _ => Result<R>.None,
                fromValue: t => lift(t),
                fromErrors: e => Result<R>.Create(e));
        }

        public R Match<R>(Func<Empty, R> fromBottom, Func<T, R> fromValue, Func<List<Error>, R> fromErrors)
        {
            switch (_tag)
            {
                case 0: return fromBottom(_bottom);
                case 1: return fromValue(Value);
                case 2: return fromErrors(Errors);

                default: return default(R); //unreachable code
            }
        }

        public R Match<R>(ValueTuple<Func<Empty, R>, Func<T, R>, Func<List<Error>, R>> map)
        {
            switch (_tag)
            {
                case 0: return map.Item1(_bottom);
                case 1: return map.Item2(Value);
                case 2: return map.Item3(Errors);

                default: return default(R); //unreachable code
            }
        }


        public void Match(Action<Empty> doBottom, Action<T> doValue, Action<List<Error>> doErrors)
        {
            switch (_tag)
            {
                case 0: doBottom(_bottom); return;
                case 1: doValue(Value); return;
                case 2: doErrors(Errors); return;

                default: return; //unreachable code
            }
        }

        public void Match(ValueTuple<Action<Empty>, Action<T>, Action<List<Error>>> act)
        {
            switch (_tag)
            {
                case 0: act.Item1(_bottom); return;
                case 1: act.Item2(Value); return;
                case 2: act.Item3(Errors); return;

                default: return; //unreachable code
            }
        }

        public static bool operator ==(Result<T> one, Result<T> other)
        {
            return one.Equals(other);
        }

        public static bool operator !=(Result<T> one, Result<T> other)
        {
            return !one.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return Equals((Result<T>)obj);
        }

        public bool Equals(Result<T> other)
        {
            if (ReferenceEquals(other, null)) return false;

            if (_tag != other._tag) return false;

            if (_tag == other._tag && _tag == 0) return true;

            bool equal = _obj.Equals(other._obj);

            return equal;
        }

        public override int GetHashCode()
        {
            var hash = 77723;
            hash = hash * 77773 + _tag.GetHashCode();
            hash = hash * 77773 + _tag == 0
                ? _bottom.GetHashCode()
                : _obj.GetHashCode();

            return hash;
        }
    }

}
