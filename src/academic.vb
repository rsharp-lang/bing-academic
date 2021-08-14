
Imports System.Text
Imports Microsoft.Bing.Academic.Models
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object

''' <summary>
''' Bing search for academic
''' </summary>
<Package("academic", Category:=APICategories.UtilityTools, Publisher:="xie.guigang@gcmodeller.org")>
Public Module academic

    Const searchApiTemplate$ = "https://cn.bing.com/academic/search?q=%s&first=%s&FORM=HDRSC4"
    Const literatureProfileApiTemplate$ = "https://cn.bing.com/academic/profile?id=%s&encoded=0&v=paper_preview&mkt=zh-cn"

    Sub New()
        Call Converts.makeDataframe.addHandler(GetType(literatureEntry()), AddressOf getListTable)

        Call Internal.ConsolePrinter.AttachConsoleFormatter(Of literature)(AddressOf printInfo)
    End Sub

    Private Function printInfo(literature As literature) As String
        Dim sb As New StringBuilder(literature.title & vbCrLf)

        If Not literature.DOI.StringEmpty Then
            Call sb.AppendLine("DOI: " & literature.DOI)
        End If
        If Not literature.journal.title.StringEmpty Then
            Call sb.AppendLine("cite as")
            Call sb.AppendLine($"{literature.authors.Select(Function(a) a.title).JoinBy(", ")}. {literature.title}. {literature.journal.title}. {literature.PubDate.ToString}")
        End If
        If Not literature.keywords.IsNullOrEmpty Then
            Call sb.AppendLine("keywords: " & literature.keywords.Select(Function(a) a.title).JoinBy("; "))
        End If

        Return sb.ToString
    End Function

    Private Function getListTable(x As literatureEntry(), args As list, env As Environment) As dataframe
        Dim data As New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {NameOf(literatureEntry.authors), x.Select(Function(a) a.authors.JoinBy(", ")).ToArray},
                {NameOf(literatureEntry.year), x.Select(Function(a) a.year).ToArray},
                {NameOf(literatureEntry.title), x.Select(Function(a) a.title).ToArray},
                {NameOf(literatureEntry.journal), x.Select(Function(a) a.journal).ToArray},
                {NameOf(literatureEntry.cites), x.Select(Function(a) a.cites).ToArray},
                {NameOf(literatureEntry.keywords), x.Select(Function(a) a.keywords.JoinBy("; ")).ToArray}
            },
            .rownames = x.Select(Function(a) a.guid).ToArray
        }

        Return data
    End Function

    <ExportAPI("summary")>
    Public Function createItem(title As String, guid As String, ref As String, cites As Integer, abstract As String) As literatureEntry
        Dim year As String = ref.Match("\d+")
        Dim jour As String = ref.Replace(year, "").Trim

        Return New literatureEntry With {
            .abstract = abstract,
            .cites = cites,
            .guid = guid,
            .title = title,
            .year = year,
            .journal = jour
        }
    End Function

End Module

