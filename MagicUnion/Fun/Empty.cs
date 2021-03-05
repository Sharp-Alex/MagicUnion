using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicUnion.Fun
{
    class EmptyTest
    {
        static void Test()
        {
            byte emptyVal = 1;

            //var empty = Empty.Create(Value: emptyVal);
        }
    }

    /// <summary>
    /// Empty box.
    /// Rreadonly and by field value equatable class,
    /// which implements <see cref="Create"/>, <see cref="With"/>, <see cref="Deconstruct"/> and  implicit conversion to <see cref="ValueTuple"/> methods.    
    /// </summary>
    public partial struct Empty : IEquatable<Empty>
    {        
        public static Empty Create()
        {
            return _empty;
        }

        /// <summary>
        /// Void.
        /// </summary>
        public static Empty One => _empty;
    }

    /// <summary>
    /// Empty box.  
    /// </summary>
    public partial struct Empty : IEquatable<Empty>
    {
        private readonly byte _value;

        private static Empty _empty = new Empty(1);

        private Empty(byte Value)
        {
            this._value = Value;
        }
       

        public static bool operator ==(Empty one, Empty other)
        {
            return true;
        }

        public static bool operator !=(Empty one, Empty other)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            return Equals((Empty)obj);
        }

        public bool Equals(Empty other)
        {
            if (ReferenceEquals(other, null))
                return false;

            return true;

        }

        public bool Equals(ValueTuple<byte> tuple)
        {
            bool equal = _value.Equals(tuple.Item1)
;
            return equal;
        }

        public override int GetHashCode()
        {
            var hash = 77723;
            hash = hash * 77773 + _value.GetHashCode();

            return hash;
        }
    }
}
