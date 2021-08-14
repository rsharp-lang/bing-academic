Namespace Models

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
End Namespace