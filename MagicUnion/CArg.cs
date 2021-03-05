//********************************************************************************************************************
//* Module name: CArg.cs
//* Author     : Oleksandr Patalakha
//* Created    : 2021.03.05
//*
//* Description: CArg describes the property in the record or the type in discriminated union.
//*
//********************************************************************************************************************
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicUnion
{
    
    internal class CArg
    {        
        private readonly TypeInfo _typeInfo;
        

        public CArg(string Name, Type Type)
        {
            this.Name = Name;
            this.Type = Type;            
            TypeName = Type.Name;
            IsValueType = Type.IsValueType;
            HasType = true;
        }

        public CArg(string Name, TypeInfo info)
        {
            this.Name = Name;            
            _typeInfo = info;           
            
            var iNamedType = _typeInfo.ConvertedType as INamedTypeSymbol;
                    
            TypeName = _typeInfo.ConvertedType.ToDisplayString();
    
            IsValueType = iNamedType.IsValueType;

            HasType = false;
        }

        public CArg(string Name, string TypeName, bool IsValueType = false)
        {
            this.Name = Name;            
            this.TypeName = TypeName;
            this.IsValueType = IsValueType;
            HasType = false;
        }

        /// <summary>
        /// Argument name
        /// </summary>
        public readonly string Name;

        public TypeInfo TypeInfo => _typeInfo;

        public readonly bool HasType;
        public readonly Type Type;
        
        /// <summary>
        /// Argument type name
        /// </summary>
        public readonly string TypeName;

        public readonly bool IsValueType;

    }
}