
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Language.C

''' <summary>
''' Bing search for academic
''' </summary>
<Package("search", Category:=APICategories.UtilityTools, Publisher:="xie.guigang@gcmodeller.org")>
Public Module academic

    Const searchApiTemplate$ = "https://cn.bing.com/academic/search?q=%s&FORM=HDRSC4"
    Const literatureProfileApiTemplate$ = "https://cn.bing.com/academic/profile?id=%s&encoded=0&v=paper_preview&mkt=zh-cn"

    ''' <summary>
    ''' Do bing academic term search
    ''' </summary>
    ''' <param name="term">the term string for search in bing.</param>
    ''' <returns></returns>
    <ExportAPI("search")>
    Public Function search(term As String) As Object
        Dim url As String = sprintf(searchApiTemplate, term.UrlEncode)
        Dim html As String = url.GET

        Return html
    End Function

    ''' <summary>
    ''' Get profile of the given literature entry
    ''' </summary>
    ''' <param name="literature">
    ''' the literature term entry from the <see cref="search(String)"/> result.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("profile.literature")>
    Public Function profile(literature As literatureEntry)
        Dim url As String = sprintf(literatureProfileApiTemplate, literature.guid.UrlEncode)
        Dim html As String = url.GET

        Return html
    End Function
End Module

