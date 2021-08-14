
Imports System.Text
Imports Microsoft.Bing.Academic.Bing.Academic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' Bing search for academic
''' </summary>
<Package("search", Category:=APICategories.UtilityTools, Publisher:="xie.guigang@gcmodeller.org")>
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

    ''' <summary>
    ''' Do bing academic term search
    ''' </summary>
    ''' <param name="term">the term string for search in bing.</param>
    ''' <returns>
    ''' a vector of the literature summary entry items.
    ''' </returns>
    <ExportAPI("search")>
    <RApiReturn(GetType(literatureEntry))>
    Public Function search(term As String, Optional offset% = 1, Optional env As Environment = Nothing) As Object
        Dim url As String = sprintf(searchApiTemplate, term.UrlEncode, offset)
        Dim html As String = url.GET(echo:=env.globalEnvironment.Rscript.debug)

        ' clean up html codes
        html = html.RemovesJavaScript.RemovesCSSstyles

        Dim blocks = html.Matches("<ol.+?</ol>", RegexICSng).ToArray
        Dim list As literatureEntry() = blocks(Scan0) _
            .Matches("<li[^>]+?class[=].+?</li>", RegexICSng) _
            .Select(AddressOf literatureEntry.literatureEntry) _
            .Where(Function(a) Not a Is Nothing) _
            .ToArray

        Return list
    End Function

    ''' <summary>
    ''' Get profile of the details information about the given literature entry
    ''' 
    ''' the information of the details contains reference list, cites data, DOI, etc...
    ''' </summary>
    ''' <param name="literature">
    ''' the literature term entry from the <see cref="search"/> result or 
    ''' the bing guid of the literature.
    ''' </param>
    ''' <returns>
    ''' the details information of the given article
    ''' </returns>
    <ExportAPI("literature")>
    <RApiReturn(GetType(literature))>
    Public Function profile(literature As Object, Optional env As Environment = Nothing) As Object
        Dim guid As String

        If literature Is Nothing Then
            Return Internal.debug.stop("no data input for get literature data!", env)
        ElseIf TypeOf literature Is literatureEntry Then
            guid = DirectCast(literature, literatureEntry).guid
        Else
            guid = Scripting.ToString(literature)
        End If

        Dim url As String = sprintf(literatureProfileApiTemplate, guid.UrlEncode)
        Dim html As String = url.GET(echo:=env.globalEnvironment.Rscript.debug)
        Dim details As literature = ProfileResult.GetProfile(html)

        Return details
    End Function
End Module

