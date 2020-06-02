
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<Package("search")>
Public Module academic

    Const searchApiTemplate$ = "https://cn.bing.com/academic/search?q=%s&FORM=HDRSC4"

    <ExportAPI("search")>
    Public Function search(term As String)

    End Function

End Module
