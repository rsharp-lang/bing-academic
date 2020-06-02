Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Public Class literature

    Public Property guid As String


End Class

Public Class literatureEntry

    Public Property guid As String
    Public Property title As String
    Public Property authors As String()
    Public Property year As String
    Public Property journal As String
    Public Property cites As Integer
    Public Property abstract As String
    Public Property keywords As String()

    Public Overrides Function ToString() As String
        Return title
    End Function

    Friend Shared Function literatureEntry(html As String) As literatureEntry
        Dim title = html.Match("<h\d+.+?</h\d+>", RegexICSng)
        Dim summary As New literatureEntry With {
            .title = title.StripHTMLTags,
            .guid = title.href.Match("id[=].+?[&]", RegexICSng).GetTagValue("=").Value.Trim("&"c)
        }
        Dim div = html.Matches("<div.+?</div>", RegexICSng).ToArray


        Return summary
    End Function

End Class