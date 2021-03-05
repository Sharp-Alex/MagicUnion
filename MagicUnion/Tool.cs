//********************************************************************************************************************
//* Module name: Tool.cs
//* Author     : Oleksandr Patalakha
//* Created    : 2021.03.05
//*
//* Description: Helper routines for refactoring in C#.
//*
//********************************************************************************************************************
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MagicUnion
{
    internal delegate IEnumerable<ClassDeclarationSyntax> DeclCreatoer(string clsName, string metName, List<CArg> args);
    

    internal static class Tool
    {
        public static async Task<Solution> GenerateTypeInNewFileAsync(
            string fileName, 
            IEnumerable<UsingDirectiveSyntax> usings, 
            NamespaceDeclarationSyntax nameSpace, 
            CodeRefactoringContext context, 
            Func<IEnumerable<string>> textCreate, 
            CancellationToken cancellationToken)
        {
            //see https://docs.microsoft.com/de-de/archive/msdn-magazine/2016/may/net-compiler-platform-maximize-your-model-view-viewmodel-experience-with-roslyn            

            var classesStr = textCreate();

            var fileText = NewFile.CreateText(usings, nameSpace, classesStr);

            var newDoc = NewFile.Create(fileName, fileText, context);
            
            var newSolution = newDoc.Project.Solution;

            // Return the new solution
            return newSolution;
        }

        public static async Task<Solution> GenerateTypeAsync(SyntaxNode identifierNode, IEnumerable<UsingDirectiveSyntax> usings, Document document, Func<IEnumerable<string>> classCreate, CancellationToken cancellationToken)
        {
            //see https://docs.microsoft.com/de-de/archive/msdn-magazine/2016/may/net-compiler-platform-maximize-your-model-view-viewmodel-experience-with-roslyn
            var nsNode = Tool.GetNameSpace(identifierNode);

            var newClsStrings = classCreate();// CreateRecordDecl(clsName, metName, args);

            var newClsDecls = newClsStrings.Select(s => Tool.StringToClassNode(s));            

            var newNsNode = nsNode.AddMembers(newClsDecls.ToArray());//.NormalizeWhitespace();

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var newRoot = root.ReplaceNode(nsNode, newNsNode);

            // Generate a new solution based on the new SyntaxNode
            var newSolution = document.Project.Solution.WithDocumentSyntaxRoot(document.Id, newRoot);

            // Return the new solution
            return newSolution;
        }
      
        public static void DebugLine(string msg)
        {
            Debug.WriteLine("");
            Debug.WriteLine(msg);
            Debug.WriteLine("");
        }

        public static ClassDeclarationSyntax StringToClassNode(string newClass1)
        {
            return SyntaxFactory.ParseSyntaxTree(newClass1)
                                       .GetRoot().DescendantNodes()
                                       .OfType<ClassDeclarationSyntax>()
                                       .FirstOrDefault()
                                       .WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation);
        }

        public static CompilationUnitSyntax StringToFileUnit(string newClass1)
        {
            var tree = SyntaxFactory.ParseSyntaxTree(newClass1);
            var root = tree.GetRoot() as CompilationUnitSyntax;
            return root.WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation);
        }

        public static  NamespaceDeclarationSyntax GetNameSpace(SyntaxNode syntaxNode)
        {
            //NamespaceDeclarationSyntax
            SyntaxNode node = syntaxNode;

            while (node != null)
            {
                var nsNode = node as NamespaceDeclarationSyntax;

                if (nsNode != null)
                    return nsNode;

                node = node.Parent;
            }
            return node as NamespaceDeclarationSyntax;
        }

        public static IEnumerable<UsingDirectiveSyntax> GetUsings(SyntaxNode syntaxNode)
        {
            var ns = GetNameSpace(syntaxNode);
            if (ns == null && ns.Parent == null)
                return null;

            SyntaxNode node = ns.Parent;
            var childs = ns.Parent.ChildNodes();

            var r = childs
                .Select(n =>
                {
                    var us = n is UsingDirectiveSyntax ? (UsingDirectiveSyntax)n : null;
                    return us;
                })
                .Where(n => n != null);

            return r;
        }

    }
}
