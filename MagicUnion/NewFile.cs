//********************************************************************************************************************
//* Module name: NewFile.cs
//* Author     : Oleksandr Patalakha
//* Created    : 2021.03.05
//*
//* Description: Creates new file with a given source code in the MS VS solution.
//*
//********************************************************************************************************************
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicUnion
{
    /// <summary>
    /// Creates new file with a given source code in the MS VS solution.
    /// //https://stackoverflow.com/questions/34955015/creating-a-new-folder-in-a-roslyn-project
    /// </summary>
    public static class NewFile
    {
        public static string CreateText(IEnumerable<UsingDirectiveSyntax> usings, NamespaceDeclarationSyntax nameSpace, IEnumerable<string> classes)
        {
            string nameSpaceStr = nameSpace.Name.ToString();
            string usingsStr = String.Join("\r\n", usings.Select( n=> n.ToString()))+"\r\n";
            string classesStr = String.Join("", classes);

            var text = $@"{usingsStr}

namespace {nameSpaceStr}
{{
    {classesStr}
}}
";
            return text;
        }

        /// <summary>
        /// Creates new file with a given source code in the MS VS solution.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Document Create(string fileName, string text, CodeRefactoringContext context)
        {
            //className.Identifier.Text + ".cs"            
            var filesSyntax = Tool.StringToFileUnit(text);
            Document newDoc = context.Document.Project.AddDocument(fileName, filesSyntax, context.Document.Folders);
            var workspace = newDoc.Project.Solution.Workspace;

            return newDoc;
        }
    }
}
