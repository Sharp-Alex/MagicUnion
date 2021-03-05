//********************************************************************************************************************
//* Module name: CodeRefactoringProvider.cs
//* Author     : Oleksandr Patalakha
//* Created    : 2021.03.05
//*
//* Description: Registers refactoring callbacks(CodeActions) e.g. CreatUnion in the VisualStudio.
//*
//********************************************************************************************************************
using MagicUnion.Fun;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MagicUnion
{
    //How to upgrade extensions to support Visual Studio 2019
    //https://www.madskristensen.net/blog/how-to-upgrade-extensions-to-support-visual-studio-2019/

    //C# 7.0 Records
    //https://github.com/dotnet/csharplang/blob/master/proposals/records.md

    //https://superuser.com/questions/73675/how-do-i-install-a-vsix-file-in-visual-studio

    //Introduction to Roslyn and its use in program development
    //https://medium.com/@CPP_Coder/introduction-to-roslyn-and-its-use-in-program-development-bce2043fc45d

    // https://github.com/dotnet/roslyn/wiki/How-To-Write-a-C%23-Analyzer-and-Code-Fix

    //For Syntax see momre:
    //https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.basetypedeclarationsyntax?view=roslyn-dotnet

    //https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.editing.syntaxeditor.insertafter?view=roslyn-dotnet

    //https://docs.microsoft.com/de-de/dotnet/csharp/roslyn-sdk/get-started/syntax-transformation
    //how to Roslyn ok
    //https://riptutorial.com/Download/roslyn.pdf
    //??
    //https://carlos.mendible.com/2017/03/02/create-a-class-with-net-core-and-roslyn/

    // Parse c# format with Roslyn  
    //https://docs.microsoft.com/de-de/archive/msdn-magazine/2016/may/net-compiler-platform-maximize-your-model-view-viewmodel-experience-with-roslyn
    //https://roslynquoter.azurewebsites.net/


    //Todo
    //+ 1. Parse Clas, Method, Argument names and types
    //- 2. Offer Generate Record only if Create method is present
    //- 3. Generate new record class in new file or locally
    //  
    //  4. FunRecord 
    //  5. user IFunRecord
    //  6. Use TryResult<Record> as return vaöue for Create()
    //  5. Dict<FieldName,objVal>
    //  6. Compara Class1.allFields == Class2.allFields
    //  7. generate Lens


    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(CodeRefactoringProvider)), Shared]
    internal class CodeRefactoringProvider : Microsoft.CodeAnalysis.CodeRefactorings.CodeRefactoringProvider
    {
        public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {            
            if( true)
            {
                try
                {
                    AddActionCreateRecord(context);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine("CreateRecord Exception:" + ex.Message);
                }
            }

            if (true)
            {
                AddActionCreateRecordFun(context);
            }
            return;
        }

        /// <summary>
        /// Add action to create record in functional style
        /// </summary>
        /// <param name="context"></param>
        private async void AddActionCreateRecordFun(CodeRefactoringContext context)
        {            
        }

        /// <summary>
        /// Add refactoring action to generates simple record class
        /// https://github.com/dotnet/csharplang/blob/master/proposals/records.md
        /// </summary>
        /// <param name="context"></param>
        internal async void AddActionCreateRecord(CodeRefactoringContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // Find the node at the selection.
            var node = root.FindNode(context.Span);

            var identifierName = node as IdentifierNameSyntax;

            if (identifierName == null) return;

            var memberAccess = identifierName.Parent as MemberAccessExpressionSyntax;

            if (memberAccess == null) return;

            var invocation = memberAccess.Parent as InvocationExpressionSyntax;

            if (invocation == null) return;

            var semanticModel = await context.Document.GetSemanticModelAsync();


            List<CArg> args = new List<CArg>();

            //For all arguments determine thier name and type
            foreach (var arg in invocation.ArgumentList.Arguments)
            {
                if (arg.NameColon == null
                    || arg.NameColon.Name == null
                    || arg.NameColon.Name.Identifier == null
                    || String.IsNullOrWhiteSpace(arg.NameColon.Name.Identifier.Text)
                    || arg.Expression == null)
                    return;

                var argName = arg.NameColon.Name.Identifier.Text;

                //Set default argument type for case when type is unknown
                string argTypeName = argName + "Type";

                CArg cArg = new CArg(argName, argTypeName);

                try
                {
                    //Determine argument type
                    if (arg.Expression is InvocationExpressionSyntax //ArgumentName:obj.GetVal()
                        || arg.Expression is IdentifierNameSyntax    //ArgumentName: obj
                        || arg.Expression is LiteralExpressionSyntax)//ArgumentName:3.14      
                                                                     // Does not work for ArgumentName: methodName - GetTypeInfo returns null 
                    {
                        var info = semanticModel.GetTypeInfo(arg.Expression);
                        var symbol = semanticModel.GetSymbolInfo(arg.Expression);
                        var a1 = semanticModel.GetAliasInfo(arg.Expression);
                        var a2 = semanticModel.GetPreprocessingSymbolInfo(arg.Expression);
                        if (info.Type == null) goto _continue;
                        cArg = new CArg(argName, info);
                    }
                    else if (arg.Expression is MemberAccessExpressionSyntax) //ArgumentName:obj.Member1.Member2
                    {
                        //https://stackoverflow.com/questions/36031260/how-to-get-type-from-alias-using-roslyn-standalone-analysis
                        //https://stackoverflow.com/questions/49847358/getting-type-of-the-caller-of-a-property-with-roslyn   

                        var memExpr = arg.Expression as MemberAccessExpressionSyntax;
                        var childs = memExpr.ChildNodes();
                        var lastChild = childs.Last();
                        var childTypeInfo = semanticModel.GetTypeInfo(lastChild);

                        cArg = new CArg(argName, childTypeInfo);
                    }
                    else if (arg.Expression is ObjectCreationExpressionSyntax)
                    {
                        var info = semanticModel.GetTypeInfo(arg.Expression);
                        cArg = new CArg(argName, info);
                    }
                    else if (arg.Expression is ParenthesizedLambdaExpressionSyntax)
                    {
                        var memExpr = arg.Expression as ParenthesizedLambdaExpressionSyntax;
                        var info = semanticModel.GetTypeInfo(arg.Expression);
                        var childs = memExpr.ChildNodes();
                        info = semanticModel.GetTypeInfo(arg);
                    }
                }
                catch
                {
                }

            _continue:

                args.Add(cArg);
            }

            //Determine class name
            var className = memberAccess.Expression as IdentifierNameSyntax;
            var metName = memberAccess.Name;
            if (metName.Identifier == null
                || (metName.Identifier.Text != "Create" && metName.Identifier.Text != "New" && metName.Identifier.Text != "TryNew" && metName.Identifier.Text != "Try"))
                return;

            var nsNode = Tool.GetNameSpace(className);

            var usings = Tool. GetUsings(className);

            var uss = usings.First().ToString();            

            var sArgs = args.Select(a => a.Name + ":" + a.TypeName);

           
            /*
            //Generate union implementation in new file from given arguments    
            var msgUnionWithResultInNewFile = "Generate Union w/ Result in new file " + className.Identifier.Text;
            var unionWithResultInFileCreateAction = CodeAction.Create(
                msgUnionWithResultInNewFile,
                c => Tool.GenerateTypeInNewFileAsync(
                    className.Identifier.Text + ".cs",
                    usings,
                    nsNode,
                    context,
                    () => UnionWithResult.CreateStr(className.Identifier.Text, metName.Identifier.Text, args),
                context.CancellationToken));

            // Register this code action: unionWithResultInFileCreateAction
            //context.RegisterRefactoring(unionWithResultInFileCreateAction);
            
            /*
            //Generate record with result implementation in new file from given arguments    
            var msgRecordWithResultInNewFile = "Generate Record w/ Result in new file " + className.Identifier.Text;
            var recordWithResultInFileCreateAction = CodeAction.Create(
                msgRecordWithResultInNewFile,
                c => Tool.GenerateTypeInNewFileAsync(
                    className.Identifier.Text + ".cs",
                    usings,
                    nsNode,
                    context,
                    () => RecordWithResult.CreateStr(className.Identifier.Text, metName.Identifier.Text, args),
                context.CancellationToken));

            recordWithResultInFileCreateAction.Do(context.RegisterRefactoring);
            */

            //Generate record implementation from given arguments
            var msgRecord = "Generate Record " + className.Identifier.Text; // + "(" + string.Join(", ", sArgs) + ")";      
            var recordCreateAction = CodeAction.Create(msgRecord, c => Tool.GenerateTypeAsync(
                identifierName,
                usings,
                context.Document,
                () => Record.CreateRecordStr(
                        className.Identifier.Text,
                        metName.Identifier.Text,
                        args),
                context.CancellationToken));
            // Register this code action.
            //context.RegisterRefactoring(recordCreateAction);
           

            //Generate record implementation in new file from given arguments    
            var msgRecordInNewFile = "Create Record in new file " + className.Identifier.Text;
            var recordInFileCreateAction = CodeAction.Create(
                msgRecordInNewFile,
                c => Tool.GenerateTypeInNewFileAsync(
                    className.Identifier.Text + ".cs",
                    usings,
                    nsNode,
                    context,
                    () => Record.CreateRecordStr(className.Identifier.Text, metName.Identifier.Text, args),
                context.CancellationToken));
            // Register this code action.
            context.RegisterRefactoring(recordInFileCreateAction);


            //Generate union implementation from given arguments
            var msgUnion = "Create Union " + className.Identifier.Text; // + "(" + string.Join(", ", sArgs) + ")";
            var unionCreateAction = CodeAction.Create(msgUnion, c => Tool.GenerateTypeAsync(
                    identifierName,
                    usings,
                    context.Document,
                    () => Union.CreateUnionStr(
                        className.Identifier.Text,
                        metName.Identifier.Text,
                        args),
                    context.CancellationToken));
            // Register this code action.
            //context.RegisterRefactoring(unionCreateAction);


            //Generate record implementation in new file from given arguments    
            var msgUnionInNewFile = "Create Union in new file " + className.Identifier.Text;
            var unionInFileCreateAction = CodeAction.Create(
                msgUnionInNewFile,
                c => Tool.GenerateTypeInNewFileAsync(
                    className.Identifier.Text + ".cs",
                    usings,
                    nsNode,
                    context,
                    () => Union.CreateUnionStr(className.Identifier.Text, metName.Identifier.Text, args),
                context.CancellationToken));

            // Register this code action.           
            context.RegisterRefactoring(unionInFileCreateAction);
        }        
    }
}
