Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Public Class literature

    Public Property guid As String


End Class

Public Class literatureEntry

    Public Property guid As String
    Public Property title As String
    Public Property authors As author()
    Public Property year As String
    Public Property journal As String
    Public Property cites As Integer
    Public Property abstract As String
    Public Property keywords As keyword()

    Public Overrides Function ToString() As String
        Return title
    End Function

    Friend Shared Function literatureEntry(html As String) As literatureEntry
        Dim title = html.Match("<h\d+.+?</h\d+>", RegexICSng)
        Dim div = html.Matches("<div.+?</div>", RegexICSng).ToArray

        If div.Length = 0 Then
            Return Nothing
        End If

        Dim authors = div(Scan0) _
            .Matches("<a.+?</a>", RegexICSng) _
            .Select(Function(a)
                        Return New author With {
                            .guid = a.href.DoCall(AddressOf getGuid),
                            .name = a.GetValue
                        }
                    End Function) _
            .ToArray
        Dim keywords = div(3) _
            .Matches("<a.+?</a>", RegexICSng) _
            .Select(Function(a)
                        Return New keyword With {
                            .guid = a.href.DoCall(AddressOf getGuid),
                            .word = a.StripHTMLTags
                        }
                    End Function) _
            .ToArray
        Dim summary As New literatureEntry With {
            .title = title.StripHTMLTags.Trim(" "c, ASCII.TAB, ASCII.CR, ASCII.LF),
            .guid = title.href.DoCall(AddressOf getGuid),
            .authors = authors,
            .keywords = keywords,
            .abstract = div(2).StripHTMLTags
        }
        Dim info = div(1) _
            .Matches("<a.+?</a>", RegexICSng) _
            .Select(AddressOf GetValue) _
            .Select(AddressOf StripHTMLTags) _
            .ToArray

        summary.year = div(1).Match("\d{4}")

        If info.Length = 1 Then
            summary.cites = info(Scan0)
        Else
            summary.journal = info(Scan0)
            summary.cites = info(1)
        End If

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

Public Class keyword
    Public Property word As String
    Public Property guid As String

    Public Overrides Function ToString() As String
        Return word
    End Function
End Class