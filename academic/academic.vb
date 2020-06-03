
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime

''' <summary>
''' Bing search for academic
''' </summary>
<Package("search", Category:=APICategories.UtilityTools, Publisher:="xie.guigang@gcmodeller.org")>
Public Module academic

    Const searchApiTemplate$ = "https://cn.bing.com/academic/search?q=%s&first=%s&FORM=HDRSC4"
    Const literatureProfileApiTemplate$ = "https://cn.bing.com/academic/profile?id=%s&encoded=0&v=paper_preview&mkt=zh-cn"

    Sub New()
        Call Converts.makeDataframe.addHandler(GetType(literatureEntry()), AddressOf getListTable)
    End Sub

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
            .rownames = Nothing
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
    Public Function search(term As String, Optional offset% = 1) As Object
        Dim url As String = sprintf(searchApiTemplate, term.UrlEncode, offset)
        Dim html As String = url.GET(echo:=False)

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
    ''' Get profile of the given literature entry
    ''' </summary>
    ''' <param name="literature">
    ''' the literature term entry from the <see cref="search"/> result.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("profile.literature")>
    Public Function profile(literature As literatureEntry)
        Dim url As String = sprintf(literatureProfileApiTemplate, literature.guid.UrlEncode)
        Dim html As String = url.GET

        Return html
    End Function
End Module

