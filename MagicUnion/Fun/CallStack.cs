//********************************************************************************************************************
//* Module name: CallStack.cs
//* Author     : Oleksandr Patalakha
//* Created    : 2021.03.05
//*
//* Description: CallStack .
//*
//********************************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicUnion.Fun
{
    public class CallStackTest
    {
        public static void Test()
        {
            string callStack;

            //var r = CallStack.Create(Text: callStack);
        }
    }

    /// <summary>
    /// Record type.
    /// Record is readonly and by field value equatable class,
    /// which implements <see cref="Create"/>, <see cref="With"/>, <see cref="Deconstruct"/> and  implicit conversion to <see cref="ValueTuple"/> methods.    
    /// </summary>
    public partial class CallStack : IEquatable<CallStack>
    {
        public readonly string Text;

        public static CallStack Create(string Text)
        {
            if (Text == null) return null;

            return new CallStack(Text);
        }
    }

    /// <summary>
    ///  Record type.    
    /// </summary>
    public partial class CallStack : IEquatable<CallStack>
    {
        private CallStack(string Text)
        {
            this.Text = Text;
        }

        public CallStack With(string Text = null)
        {
            return Create(Text == null ? this.Text : Text);
        }

        public void Desconstruct(out string Text)
        {
            Text = this.Text;
        }
        
        public static implicit operator string(CallStack obj)
        {
            return obj.Text;
        }

        public static implicit operator ValueTuple<string>(CallStack o)
        {
            return ValueTuple.Create(o.Text);
        }

        public static bool operator ==(CallStack one, CallStack other)
        {
            return one.Equals(other);
        }

        public static bool operator !=(CallStack one, CallStack other)
        {
            return !one.Equals(other);
        }

        public static bool operator ==(CallStack one, ValueTuple<string> tuple)
        {
            return one.Equals(tuple);
        }

        public static bool operator !=(CallStack one, ValueTuple<string> tuple)
        {
            return !one.Equals(tuple);
        }

        public override bool Equals(object obj)
        {
            return Equals((CallStack)obj);
        }

        public bool Equals(CallStack other)
        {
            if (ReferenceEquals(other, null)) return false;
            bool equal = Text.Equals(other.Text)
;
            return equal;
        }

        public bool Equals(ValueTuple<string> tuple)
        {
            bool equal = Text.Equals(tuple.Item1)
;
            return equal;
        }

        public override int GetHashCode()
        {
            var hash = 77723;
            hash = hash * 77773 + Text.GetHashCode();

            return hash;
        }
    }
}
