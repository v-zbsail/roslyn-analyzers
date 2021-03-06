// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Globalization;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Test.Utilities.CSharpSecurityCodeFixVerifier<
    Microsoft.NetFramework.Analyzers.DoNotUseInsecureDtdProcessingAnalyzer,
    Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;
using VerifyVB = Test.Utilities.VisualBasicSecurityCodeFixVerifier<
    Microsoft.NetFramework.Analyzers.DoNotUseInsecureDtdProcessingAnalyzer,
    Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;

namespace Microsoft.NetFramework.Analyzers.UnitTests
{
    public partial class DoNotUseInsecureDtdProcessingAnalyzerTests
    {
        private static DiagnosticResult GetCA3075XPathDocumentCSharpResultAt(int line, int column)
            => VerifyCS.Diagnostic(DoNotUseInsecureDtdProcessingAnalyzer.RuleDoNotUseDtdProcessingOverloads).WithLocation(line, column).WithArguments(".ctor");

        private static DiagnosticResult GetCA3075XPathDocumentBasicResultAt(int line, int column)
            => VerifyVB.Diagnostic(DoNotUseInsecureDtdProcessingAnalyzer.RuleDoNotUseDtdProcessingOverloads).WithLocation(line, column).WithArguments(".ctor");

        [Fact]
        public async Task UseXPathDocumentWithoutReaderShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Xml;
using System.Xml.XPath;

namespace TestNamespace
{
    public class UseXmlReaderForXPathDocument
    {
        public void TestMethod(string path)
        {
            XPathDocument doc = new XPathDocument(path);
        }
    }
}
",
                GetCA3075XPathDocumentCSharpResultAt(11, 33)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Xml
Imports System.Xml.XPath

Namespace TestNamespace
    Public Class UseXmlReaderForXPathDocument
        Public Sub TestMethod(path As String)
            Dim doc As New XPathDocument(path)
        End Sub
    End Class
End Namespace",
                GetCA3075XPathDocumentBasicResultAt(8, 24)
            );
        }

        [Fact]
        public async Task UseXPathDocumentWithoutReaderInGetShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Xml.XPath;

class TestClass
{
    public XPathDocument Test
    {
        get
        {
            var xml = """";
            XPathDocument doc = new XPathDocument(xml);
            return doc;
        }
    }
}",
                GetCA3075XPathDocumentCSharpResultAt(11, 33)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Xml.XPath

Class TestClass
    Public ReadOnly Property Test() As XPathDocument
        Get
            Dim xml = """"
            Dim doc As New XPathDocument(xml)
            Return doc
        End Get
    End Property
End Class",
                GetCA3075XPathDocumentBasicResultAt(8, 24)
            );
        }

        [Fact]
        public async Task UseXPathDocumentWithoutReaderInSetShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Xml.XPath;

class TestClass
{
XPathDocument privateDoc;
public XPathDocument GetDoc
        {
            set
            {
                if (value == null)
                {
                    var xml = """";
                    XPathDocument doc = new XPathDocument(xml);
                    privateDoc = doc;
                }
                else
                    privateDoc = value;
            }
        }
}",
                GetCA3075XPathDocumentCSharpResultAt(14, 41)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Xml.XPath

Class TestClass
    Private privateDoc As XPathDocument
    Public WriteOnly Property GetDoc() As XPathDocument
        Set
            If value Is Nothing Then
                Dim xml = """"
                Dim doc As New XPathDocument(xml)
                privateDoc = doc
            Else
                privateDoc = value
            End If
        End Set
    End Property
End Class",
                GetCA3075XPathDocumentBasicResultAt(10, 28)
            );
        }

        [Fact]
        public async Task UseXPathDocumentWithoutReaderInTryBlockShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
  using System;
  using System.Xml.XPath;

    class TestClass
    {
        private void TestMethod()
        {
            try
            {
                var xml = """";
                XPathDocument doc = new XPathDocument(xml);
            }
            catch (Exception) { throw; }
            finally { }
        }
    }",
                GetCA3075XPathDocumentCSharpResultAt(12, 37)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System
Imports System.Xml.XPath

Class TestClass
    Private Sub TestMethod()
        Try
            Dim xml = """"
            Dim doc As New XPathDocument(xml)
        Catch generatedExceptionName As Exception
            Throw
        Finally
        End Try
    End Sub
End Class",
                GetCA3075XPathDocumentBasicResultAt(9, 24)
            );
        }

        [Fact]
        public async Task UseXPathDocumentWithoutReaderInCatchBlockShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
   using System;
   using System.Xml.XPath;

    class TestClass
    {
        private void TestMethod()
        {
            try { }
            catch (Exception)
            {
                var xml = """";
                XPathDocument doc = new XPathDocument(xml);
            }
            finally { }
        }
    }",
                GetCA3075XPathDocumentCSharpResultAt(13, 37)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System
Imports System.Xml.XPath

Class TestClass
    Private Sub TestMethod()
        Try
        Catch generatedExceptionName As Exception
            Dim xml = """"
            Dim doc As New XPathDocument(xml)
        Finally
        End Try
    End Sub
End Class",
                GetCA3075XPathDocumentBasicResultAt(10, 24)
            );
        }

        [Fact]
        public async Task UseXPathDocumentWithoutReaderInFinallyBlockShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System;
using System.Xml.XPath;

class TestClass
{
    private void TestMethod()
    {
        try { }
        catch (Exception) { throw; }
        finally
        {
            var xml = """";
            XPathDocument doc = new XPathDocument(xml);
        }
    }
}",
                GetCA3075XPathDocumentCSharpResultAt(14, 33)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System
Imports System.Xml.XPath

Class TestClass
    Private Sub TestMethod()
        Try
        Catch generatedExceptionName As Exception
            Throw
        Finally
            Dim xml = """"
            Dim doc As New XPathDocument(xml)
        End Try
    End Sub
End Class",
                GetCA3075XPathDocumentBasicResultAt(12, 24)
            );
        }

        [Fact]
        public async Task UseXPathDocumentWithoutReaderInAsyncAwaitShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Threading.Tasks;
using System.Xml.XPath;

class TestClass
{
    private async Task TestMethod()
    {
        await Task.Run(() => {
            var xml = """";
            XPathDocument doc = new XPathDocument(xml);
        });
    }

    private async void TestMethod2()
    {
        await TestMethod();
    }
}",
                GetCA3075XPathDocumentCSharpResultAt(11, 33)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Threading.Tasks
Imports System.Xml.XPath

Class TestClass
    Private Async Function TestMethod() As Task
        Await Task.Run(Function() 
        Dim xml = """"
        Dim doc As New XPathDocument(xml)

        End Function)
    End Function

    Private Async Sub TestMethod2()
        Await TestMethod()
    End Sub
End Class",
                GetCA3075XPathDocumentBasicResultAt(9, 20)
            );
        }

        [Fact]
        public async Task UseXPathDocumentWithoutReaderInDelegateShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Xml.XPath;

class TestClass
{
    delegate void Del();

    Del d = delegate () {
        var xml = """";
        XPathDocument doc = new XPathDocument(xml);
    };
}",
                GetCA3075XPathDocumentCSharpResultAt(10, 29)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Xml.XPath

Class TestClass
    Private Delegate Sub Del()

    Private d As Del = Sub() 
    Dim xml = """"
    Dim doc As New XPathDocument(xml)

End Sub
End Class",
                GetCA3075XPathDocumentBasicResultAt(9, 16)
            );
        }

        [Fact]
        public async Task UseXPathDocumentWithXmlReaderShouldNotGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Xml;
using System.Xml.XPath;

namespace TestNamespace
{
    public class UseXmlReaderForXPathDocument
    {
        public void TestMethod17(XmlReader reader)
        {
            XPathDocument doc = new XPathDocument(reader);
        }
    }
}
"
            );
        }
    }
}
