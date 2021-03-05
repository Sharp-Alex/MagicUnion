//********************************************************************************************************************
//* Module name: Union.cs
//* Author     : Oleksandr Patalakha
//* Created    : 2021.03.05
//*
//* Description: Generates source code for the calss that play role of the discriminated union in C#.
//*
//********************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicUnion
{
    //https://stackoverflow.com/questions/34955015/creating-a-new-folder-in-a-roslyn-project

    internal static class Union
    {
        internal static IEnumerable<string> CreateUnionStr(string className, string factoryMetName, List<CArg> args)
        {
            var fieldFmt = "{0} {1} => ({0})_obj;";

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

            var ifNullReturnFmt = "if({0}==null) return null;";            

            var ifNullExcFmt = "if({0}==null) throw new ArgumentNullException(nameof({0}));\r\n";

            //Hash code with prime numbers  77773, 77723  from  https://primes.utm.edu/lists/small/10000.txt
            var hashCodes = String.Join("", args.Select(a => String.Format("hash = hash * 77773 + (((object){0})==null ? 0: {0}.GetHashCode()); \r\n", a.Name)));

            var fieldsCompare = String.Join("                         && ", args.Select(a => String.Format("({0}.Equals(other.{0})) \r\n", a.Name)));

            var deconstructInits = String.Join("\r\n", args.Select(a => String.Format("{0} = this.{0};", a.Name)));

            var deconstructMetSignature = String.Join(", ", args.Select(a => String.Format("out {0} {1}", a.TypeName, a.Name)));

            var valueTupleCreateArgs = String.Join(", ", args.Select(a => String.Format("o.{0}", a.Name)));

            var valueTupleTypes = String.Join(", ", args.Select(a => String.Format("{0}", a.TypeName)));

            Func<string, string> argNameToFunName = s => String.Format("from{0}", s);
            Func<string, string> argNameToActnName = s => String.Format("do{0}", s);

            var matchArgs = String.Join(", ", args.Select(a => String.Format("Func<{0},R> {1}", a.TypeName, argNameToFunName(a.Name))));
            var matchCases = String.Join("", args.Select((a, i) => String.Format("case {0}: return {1}({2});\r\n", i, argNameToFunName(a.Name), a.Name)));

            var matchActionArgs = String.Join(", ", args.Select(a => String.Format("Action<{0}> {1}", a.TypeName, argNameToActnName(a.Name))));
            var matchActionCases = String.Join("", args.Select((a, i) => String.Format("case {0}: {1}({2}); return;\r\n", i, argNameToActnName(a.Name), a.Name)));

            var matchTuple = "ValueTuple<" + String.Join(", ", args.Select(a => String.Format("Func<{0},R>", a.TypeName))) + "> map";
            var matchTupleCases = String.Join("", args.Select((a, i) => String.Format("case {0}: return map.Item{1}({2});\r\n", i, i + 1, a.Name)));

            var matchTupleAction = "ValueTuple<" + String.Join(", ", args.Select(a => String.Format("Action<{0}>", a.TypeName))) + "> act";
            var matchTupleActionCases = String.Join("", args.Select((a, i) => String.Format("case {0}: act.Item{1}({2}); return;\r\n", i, i + 1, a.Name)));

            var includedTypesDoc1 = String.Join("|", args.Select(a => String.Format("{0}", a.TypeName)));
            var includedTypesDoc2 = String.Join("|", args.Select(a => String.Format("<see cref=\"{0}\"/>", a.TypeName)));

            var constructMask = @"
        private {0}({1} {2})
        {{         
            _tag = {3};
            _obj = (object){2};            
        }}";

            var factoryeMask = @"
        public static {0} {1}({2} {3})
        {{
            {4}
            return new {0}({3});
        }}";

            var constructors = String.Join("\r\n", args.Select((a, i) =>
                String.Format(constructMask, className, a.TypeName, a.Name, i)));

            var factories = String.Join("\r\n", args.Select(a =>
               String.Format(factoryeMask, className, factoryMetName, a.TypeName, a.Name, IfNullCheck(a, ifNullExcFmt))));

            //Generate With methods
            var withMask = @"
         public {0} With({1})
         {{
             return {2}({3});
         }}";

            Func<CArg, string> getWithSignatur = 
                a =>
                {
                    var s = String.Format("{0} {1}", a.TypeName, a.Name);
                    return s;
                };

            Func<CArg, string> getWithParam =
                a =>
                {
                    var s = String.Format("{0}", a.Name);
                    return s;
                };

            var withMethods = String.Join("\r\n", args.Select(a =>
                {
                    var s = String.Format(withMask, className, getWithSignatur(a), factoryMetName, getWithParam(a));
                    return s;
                }));


            //https://stackoverflow.com/questions/13949941/using-symbol-in-string
            //<see cref=""Match""/>
            var newClassText1 = 
$@"/// <summary>
    /// Discraminated union of following types: ({includedTypesDoc1}).
    /// Union has only readonly members and any member of the union can be accessed via <see cref=""Match""/> method.
    /// Union is by field value equatable class, it implements <see cref=""Create""/> and <see cref=""With""/> methods.     
    /// </summary>
    public partial class {className}: IEquatable<{className}>
    {{                
         {fields}
    }}";

            var newClassText2 = $@"

    /// <summary>
    ///  Discraminated union of following types: ({includedTypesDoc2}).
    /// </summary>
    public partial class {className}: IEquatable<{className}>
    {{
        private readonly int _tag = -1;      

        private readonly object _obj ;
           
        {constructors}            

        {factories}  

        {withMethods}

        public R Match<R>({matchArgs})
        {{
            switch (_tag)
            {{
                {matchCases}                
                default: return default(R); //unreachable code
            }}
        }}  

        public R Match<R>({matchTuple})
        {{
            switch (_tag)
            {{
                {matchTupleCases}                
                default: return default(R); //unreachable code
            }}
        }} 
        
        public void Match({matchActionArgs})
        {{
            switch (_tag)
            {{
                {matchActionCases}                
                default: return; //unreachable code
            }}
        }}

        public void Match({matchTupleAction})
        {{
            switch (_tag)
            {{
                {matchTupleActionCases}                
                default: return; //unreachable code
            }}
        }} 

        public static bool operator ==({className} one, {className} other)
        {{
            if (ReferenceEquals(one, null))
                return ReferenceEquals(other, null);
            return one.Equals(other);
        }}

        public static bool operator !=({className} one, {className} other)
        {{
            return !(one==other);
        }}

        public override bool Equals(object obj)
        {{
            return Equals(({className})obj);
        }}

        public bool Equals({className} other)
        {{
            if( object.ReferenceEquals(other, null)) 
                return false;  
            if (_tag != other._tag)
                return false; 
            bool equal = _obj.Equals(other._obj);
            return equal;
        }}

        public override int GetHashCode()
        {{
            var hash = (_tag, _obj).GetHashCode();
            return hash;
        }}
    }}";

            return new List<string>() { newClassText1, newClassText2 };
        }
        private static string IfNullCheck(CArg arg, string ifNullReturnFmt)
        {
            return arg.IsValueType
                           ? ""
                           : String.Format(ifNullReturnFmt, arg.Name);
        }

        private static string IfNullExc(CArg arg, string ifNullExcFmt)
        {
            return arg.IsValueType
                           ? ""
                           : String.Format(ifNullExcFmt, arg.Name);
        }
    }
}
