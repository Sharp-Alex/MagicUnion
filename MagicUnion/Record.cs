//********************************************************************************************************************
//* Module name: Record.cs
//* Author     : Oleksandr Patalakha
//* Created    : 2021.03.05
//*
//* Description: Generates source code for the calss that play role of the record in C#.
//*
//********************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicUnion
{
    internal static class Record
    {
        internal static IEnumerable<string> CreateRecordStr(string className, string factoryMetName, List<CArg> args)
        {
            var fieldFmt = "public readonly {0} {1};";

            var fields = String.Join("\r\n", args.Select(a => String.Format(fieldFmt, a.TypeName, a.Name)));

            var fildInits = String.Join("\r\n", args.Select(a => String.Format("this.{0} = {0};", a.Name)));

            var factoryMetSignature = String.Join(", ", args.Select(a => String.Format("{0} {1}", a.TypeName, a.Name)));

            var factoryMetArgNames = String.Join(", ", args.Select(a => String.Format("{0}", a.Name)));

            var withSignature = String.Join(", ", args.Select(a => a.IsValueType
                                                                       ? String.Format("{0}? {1}=null", a.TypeName, a.Name)
                                                                       : String.Format("{0} {1}=null", a.TypeName, a.Name)));

            var withRefParamFmt = "{0}==null ? this.{0} : {0}";
            var withValParamFmt = "{0}==null?  this.{0} : {0}.Value";
            var withParams = String.Join(",\r\n                          ",
                                         args.Select(a => a.IsValueType
                                            ? String.Format(withValParamFmt, a.Name)
                                            : String.Format(withRefParamFmt, a.Name)));

            var ifNullReturnFmt = "if({0}==null) return null;\r\n";
            var ifNullReturns = String.Join(" ", args.Select(a => a.IsValueType
                                                                       ? ""
                                                                       : String.Format(ifNullReturnFmt, a.Name)));

            var ifNullExcFmt = "if({0}==null) throw new ArgumentNullException(nameof({0}));\r\n";
            var ifNullExc = String.Join(" ", args.Select(a => a.IsValueType
                                                                    ? ""
                                                                    : String.Format(ifNullExcFmt, a.Name)));

            //Hash code with prime numbers  77773, 77723  from  https://primes.utm.edu/lists/small/10000.txt
            var hashCodes = String.Join("", args.Select(a => String.Format("hash = hash * 77773 + {0}.GetHashCode(); \r\n", a.Name)));

            var fieldList = String.Join(",", args.Select(a => String.Format("{0}", a.Name)));            

            var fieldsCompare = String.Join("\r\n                         && ", args.Select(a => String.Format("(one.{0}.Equals(other.{0}))", a.Name)));

            var deconstructInits = String.Join("\r\n", args.Select(a => String.Format("{0} = this.{0};", a.Name)));

            var deconstructMetSignature = String.Join(", ", args.Select(a => String.Format("out {0} {1}", a.TypeName, a.Name)));

            var valueTupleCreateArgs = String.Join(", ", args.Select(a => String.Format("o.{0}", a.Name)));

            var valueTupleTypes = String.Join(", ", args.Select(a => String.Format("{0}", a.TypeName)));

            var tupleCompare = String.Join("\r\n                         && ", args.Select((a, i) => String.Format("({0}.Equals(tuple.Item{1}))", a.Name, i + 1)));

            var singleFieldBackConvertion = args.Count != 1
                ? String.Empty
                : $@"
        public static implicit operator {args[0].TypeName} ({className} obj)
        {{
            return obj.{args[0].Name};
        }}";

            //https://stackoverflow.com/questions/13949941/using-symbol-in-string
            //<see cref=""Match""/>
            var newClassText1 = $@"
    /// <summary>
    /// Record of not nullable fields.
    /// Record is readonly and by field value equatable class,
    /// which implements <see cref=""Create""/>, <see cref=""With""/>, <see cref=""Deconstruct""/> and  implicit conversion to <see cref=""ValueTuple""/> methods.    
    /// </summary>
    public partial class {className}: IEquatable<{className}>
    {{       
        {fields}

        public static {className} {factoryMetName}({factoryMetSignature})
        {{
            {ifNullExc}
            return new {className}({factoryMetArgNames});
        }} 
    }}";

            var newClassText2 = $@"

    /// <summary>
    ///  Record.    
    /// </summary>
    public partial class {className}: IEquatable<{className}>
    {{
        private {className}({factoryMetSignature})
        {{
            {fildInits}
        }}                

        public {className} With({withSignature})
        {{
            return {className}.{factoryMetName}({withParams});
        }}  

        public void Desconstruct({deconstructMetSignature})
        {{
            {deconstructInits}          
        }}

        {singleFieldBackConvertion}
        
        public static implicit operator ValueTuple<{valueTupleTypes}> ({className} o)
        {{
            return ValueTuple.Create({valueTupleCreateArgs});
        }}

        public static bool operator ==({className} one, {className} other)
        {{
            return Equals(one, other);
        }}

        public static bool operator !=({className} one, {className} other)
        {{
            return !Equals(one, other);
        }}

        public static bool operator ==({className} one, ValueTuple<{valueTupleTypes}> tuple)
        {{
            return Equals(one, tuple);
        }}

        public static bool operator !=({className} one, ValueTuple<{valueTupleTypes}> tuple)
        {{
            return !Equals(one, tuple);
        }}

        public override bool Equals(object obj)
        {{
            return Equals(({className})obj);
        }}

        public bool Equals({className} other)
        {{          
            return Equals(this, other);
        }}

        public static bool Equals({className} one, {className} other)
        {{
            if( object.ReferenceEquals(one, null)) 
                return object.ReferenceEquals(other, null);

            if( object.ReferenceEquals(other, null)) return false;

            bool equal = ({fieldsCompare});
            return equal;
        }}

        public bool Equals(ValueTuple<{valueTupleTypes}> tuple)
        {{            
            bool equal = ({tupleCompare});
            return equal;
        }}

        public override int GetHashCode()
        {{
            var hash = ({fieldList}).GetHashCode();
            return hash;
        }}
    }}";
            return new List<string>() { newClassText1, newClassText2 };
        }
    
    }
}
