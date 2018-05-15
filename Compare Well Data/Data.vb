'   Dave Campbell
'   June 2004

'   This module has the functions to return the names of the
'   DSNs for the databases.

Module Data
Private Function GetValue(keyName As String) As String
        Dim reader As New System.Configuration.AppSettingsReader
        Dim setting As String = reader.GetValue(keyName, GetType(String))
        Return setting
    End Function

    Public Function GetObsWells() As String
        Return GetValue("ObsWellsConnStr")
    End Function


    Public Function QuoteDB(ByVal strToQuote As String) As String
    '   Quote the value for a database query by enclosing it in single
    '   quotes and escaping any that already exist in the string.

        Return "'" & Replace(strToQuote, "'", "''") & "'"

    End Function

End Module
