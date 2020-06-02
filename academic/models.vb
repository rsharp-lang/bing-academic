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

    End Function

End Class