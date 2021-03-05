
using MagicUnion.Fun;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicUnion.Fun
{    
    public class TestError
    {

        public static void Test()
        {
            string msg;
            CallStack trace;
           // Functional.Maybe<Error> inner;

            //var error = Error.Create(Msg: msg, Callstack: trace, InnerError: inner);
        }
    }

    /// <summary>
    /// Record type.
    /// Record is readonly and by field value equatable class,
    /// which implements <see cref="Create"/>, <see cref="With"/>, <see cref="Deconstruct"/> and  implicit conversion to <see cref="ValueTuple"/> methods.    
    /// </summary>
    public partial class Error : IEquatable<Error>
    {
        public readonly string Msg;
        public readonly CallStack Callstack;
        public readonly Maybe<Error> InnerError;

        public static Error Create(Exception exc)
        {
            if (exc == null) return null;

            var callstack = CallStack.Create(exc.StackTrace) ?? CallStack.Create(new StackTrace().ToString());

            return new Error(exc.Message, callstack, exc.InnerException==null ? null : Create(exc.InnerException));

        }

        public static Error Create(string Msg, CallStack Callstack=null, Error InnerError=null)
        {
            if (Msg == null) return null;

            var callstack = Callstack ?? CallStack.Create(new StackTrace().ToString());

            return new Error(Msg, callstack, InnerError);
        }

        public static Error Create(string Msg, CallStack Callstack, Maybe<Error> InnerError )
        {
            if (Msg == null) return null;

            var callstack = Callstack ?? CallStack.Create(new StackTrace().ToString());

            return new Error(Msg, callstack, InnerError);
        }

        public static Error InvalidArgument(string ArgumentName, CallStack Callstack = null)
        {
            var stack = Callstack ?? CallStack.Create(new StackTrace().ToString());

            return Create(String.Format("Invalid argument '{0}.", ArgumentName), stack, null);
        }
    }

    /// <summary>
    ///  Record type.    
    /// </summary>
    public partial class Error : IEquatable<Error>
    {
        private Error(string Msg, CallStack Callstack, Maybe<Error> InnerError)
        {
            this.Msg = Msg;
            this.Callstack = Callstack;
            this.InnerError = InnerError;
        }

        public Error With(string Msg = null, CallStack Callstack = null, Error InnerError = null)
        {
            return Create(Msg == null ? this.Msg : Msg,
                          Callstack == null ? this.Callstack : Callstack,
                          InnerError == null ? this.InnerError : InnerError);
        }

        public void Desconstruct(out string Msg, out CallStack Callstack, out Maybe<Error> InnerError)
        {
            Msg = this.Msg;
            Callstack = this.Callstack;
            InnerError = this.InnerError;
        }

        public static implicit operator ValueTuple<string, CallStack, Maybe<Error>>(Error o)
        {
            return ValueTuple.Create(o.Msg, o.Callstack, o.InnerError);
        }

        public static bool operator ==(Error one, Error other)
        {
            return one.Equals(other);
        }

        public static bool operator !=(Error one, Error other)
        {
            return !one.Equals(other);
        }

        public static bool operator ==(Error one, ValueTuple<string, CallStack, Maybe<Error>> tuple)
        {
            return one.Equals(tuple);
        }

        public static bool operator !=(Error one, ValueTuple<string, CallStack, Maybe<Error>> tuple)
        {
            return !one.Equals(tuple);
        }

        public override bool Equals(object obj)
        {
            return Equals((Error)obj);
        }

        public bool Equals(Error other)
        {
            if (ReferenceEquals(other, null)) return false;
            bool equal = Msg.Equals(other.Msg)
                         && Callstack.Equals(other.Callstack)
                         && InnerError.Equals(other.InnerError)
;
            return equal;
        }

        public bool Equals(ValueTuple<string, CallStack, Maybe<Error>> tuple)
        {
            bool equal = Msg.Equals(tuple.Item1)
                         && Callstack.Equals(tuple.Item2)
                         && InnerError.Equals(tuple.Item3)
;
            return equal;
        }

        public override int GetHashCode()
        {
            var hash = 77723;
            hash = hash * 77773 + Msg.GetHashCode();
            hash = hash * 77773 + Callstack.GetHashCode();
            hash = hash * 77773 + InnerError.GetHashCode();

            return hash;
        }
    }
}
