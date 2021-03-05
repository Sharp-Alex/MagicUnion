
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Security.Policy;
using static MagicUnion.Test.CreateRecordTest;

namespace MagicUnion.Test
{
    /// <summary>
    /// Manual test of c# refactoring extension.
    /// </summary>
    [TestClass]
    public partial class MagicUnionCreateTest
    {
        [TestMethod]
        public void MagicUnionCreateTestMethod()
        {
            //0. Run MagicUnion.vsix and install extension for VS2019.

            //0.a. This is manual test, one has to uncomment code lines put cursor on "Create" method
            //     and click on refactoring popup menu "Create Record in new file" or "Create Union in new file".

            //1. Create Record test.
            string Anna = "Anna";
            DateTime birthday = new DateTime(2017, 11, 04);

            //Uncomment code below and generate record type with help of refactoring extension:
            //var person = Person.Create(Name: Anna, Birthday: birthday);



            //2. Create Union test.            
            var exception = new Exception();
            var none = Empty.None;

            //Step 1. This is Union of 3 types: string, Exception and Empty.
            //         Uncomment the code below to generate disciminated union type with help of refactoring extension- "Create Union in new file".            
            //var nameResult = PersonUnion.Create(Value: Anna, Error: exception, None: none);

            //Step 2. Comment line above again - we need it only for code generation.

            //Step 3. Uncomment block below to test 3 different states of the union:
            //var uniontWithValue = PersonUnion.Create(Anna);
            //var uniontWithError = PersonUnion.Create(new Exception("Anna is on vacation."));
            //var unionWithNone = PersonUnion.Create(Empty.None);

            Func<string,string> onValue = (s) => string.Format("{0} will come to your birthday party!", s);
            Func<Exception, string> onError = (e) => string.Format("Sorry, {0}", e.Message);
            Func<Empty, string> onNone = (_) => string.Format("Sorry, who?");

            var handle = (onValue, onError, onNone); // <-- you can generate record instead of this tuple.

            //var msg1 = uniontWithValue.Match(handle);
            //var msg2 = uniontWithError.Match(handle);
            //var msg3 = unionWithNone.Match(handle);

            //3. Run or debug the test with newly generated union and record.
        }
    }
}


