Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
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
            .title = title.StripHTMLTags.Trim(" "c, ASCII.TAB, ASCII.CR, ASCII.LF),
            .guid = title.href.DoCall(AddressOf getGuid)
        }
        Dim div = html.Matches("<div.+?</div>", RegexICSng).ToArray
        Dim authors = div(Scan0) _
            .Matches("<a.+?</a>", RegexICSng) _
            .Select(Function(a)
                        Return New author With {
                            .guid = a.href.DoCall(AddressOf getGuid),
                            .name = a.GetValue
                        }
                    End Function) _
            .ToArray

        Return summary
    End Function

    Friend Shared Function getGuid(href As String) As String
        Return href _
            .Match("id[=].+", RegexICSng) _
            .GetTagValue("=").Value _
           ?.Split("&"c) _
            .FirstOrDefault
    End Function

End Class

Public Class author

    Public Property name As String
    Public Property guid As String

    Public Overrides Function ToString() As String
        Return name
    End Function

End Class