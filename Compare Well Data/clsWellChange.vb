Imports CNPPID
Imports ObservationWells
Imports System.Data.SqlClient

''' <remarks>
''' Dave Campbell
''' August 2005
''' 
''' clsWellChange
''' 
''' We want to calculate how a well has changed from one time to another.  This will calculate
''' from one quarter to another, one month to another or one year to another.
''' </remarks>
''' 
Public Class clsWellChange

    #Region " Declarations "

Private wtWell As clsObservationWell
Private boolMatchRecords As Boolean
Private boolAverageRecords As Boolean
Private ieReadInterval As IntervalEnum
Private intReadValue As Integer
Private dtStartDate As DateTime
Private dblStartReading As Double
Private dblEndReading As Double
Private dtEndDate As DateTime

    #End Region

    #Region " Properties "

Public Enum IntervalEnum
    Monthly = 0
    Quarterly = 1
    Annually = 2
    WithinAYear = 3
End Enum

''' <summary>
''' This is raised when an error occurs.
''' </summary>
Public Event ErrorOccurred(ByVal strMsg As String)

''' <summary>
''' This is raised when the class is done gathering the readings.
''' </summary>
Public Event ReadingsGathered()

''' <summary>
''' This is True if we're going to average multiple records for the start or end reading, if
''' we have multiples.  This will always be the opposite of MatchRecords.  Setting this
''' value also sets MatchRecords.
''' </summary>
Public Property AverageRecords() As Boolean
    Get
        Return boolAverageRecords
    End Get
    Set(ByVal Value As Boolean)
        boolAverageRecords = Value
        boolMatchRecords = Not Value
    End Set
End Property

''' <summary>
''' The end date of the well readings.
''' </summary>
Public Property EndDate() As DateTime
    Get
        Return dtEndDate
    End Get
    Set(ByVal Value As DateTime)
        dtEndDate = Value
    End Set
End Property

''' <summary>
''' The last reading we calculated.
''' </summary>
Public ReadOnly Property EndReading() As Double
    Get
        Return dblEndReading
    End Get
End Property

''' <summary>
''' The latitude of the well to which this data belongs, expressed as a decimal.
''' </summary>
Public ReadOnly Property LatDecimal() As Decimal
    Get
        Return wtWell.LatLong.LatDecimal
    End Get
End Property

''' <summary>
''' The longitude of the well to which this data belongs, expressed as a decimal.
''' </summary>
Public ReadOnly Property LongDecimal() As Decimal
    Get
        Return wtWell.LatLong.LongDecimal
    End Get
End Property

''' <summary>
''' The well key converted to English-like name with township and range identified
''' and the subsection and sequence number justified in spaces.
''' e.g., T05 N R38 W 01 CAD 23
''' </summary>
Public ReadOnly Property LegalDescription() As String
    Get
        Return "T" & Microsoft.VisualBasic.Right("00" & wtWell.WellKey.Township.ToString, 2) & " N " & _
                "R" & Microsoft.VisualBasic.Right("00" & wtWell.WellKey.Range.ToString, 2) & " W " & _
                Microsoft.VisualBasic.Right("00" & wtWell.WellKey.Section.ToString, 2) & " " & _
                wtWell.WellKey.Subsection & " " & wtWell.WellKey.SeqNo.ToString

    End Get
End Property

''' <summary>
''' This is True if we're going to match a single record for the start or end reading, if
''' we have multiples.  This means we will take the first reading in case of multiples.
''' This will always be the opposite of AverageRecords.
''' </summary>
Public ReadOnly Property MatchRecords() As Boolean
    Get
        Return boolMatchRecords
    End Get
End Property

''' <summary>
''' The difference between the EndReading and StartReading.
''' </summary>
Public ReadOnly Property ReadingChange() As Double
    Get
        Return dblEndReading - dblStartReading
    End Get
End Property

''' <summary>
''' This is the interval for the readings (monthly, quarterly, annually, within a year).
''' </summary>
Public Property ReadInterval() As IntervalEnum
    Get
        Return ieReadInterval
    End Get
    Set(ByVal Value As IntervalEnum)
        ieReadInterval = Value
    End Set
End Property

''' <summary>
''' This is interpreted in light of ReadInterval.
''' If ReadInterval is Monthly, ReadValue should be between 1 and 12.
''' If ReadInterval is Quarterly, ReadValue should be between 1 and 4.
''' If ReadInterval is Annually or WithinAyear, ReadValue doesn't matter.
''' </summary>
Public Property ReadValue() As Integer
    Get
        Return intReadValue
    End Get
    Set(ByVal Value As Integer)
        intReadValue = Value
    End Set
End Property

''' <summary>
''' The start date of the well readings to measure.
''' </summary>
Public Property StartDate() As DateTime
    Get
        Return dtStartDate
    End Get
    Set(ByVal Value As DateTime)
        dtStartDate = Value
    End Set
End Property

''' <summary>
''' The first reading we have.
''' </summary>
Public ReadOnly Property StartReading() As Double
    Get
        Return dblStartReading
    End Get
End Property

''' <summary>
''' The Township, Range, Section, Subsection and SeqNo of the well to which this
''' data belongs.
''' </summary>
Public ReadOnly Property WellKey() As CNPPID.PLSS
    Get
        Return wtWell.WellKey
    End Get
End Property

''' <summary>
''' The name of the well to which this data belongs.
''' </summary>
Public ReadOnly Property WellName() As String
    Get
        Return wtWell.WellName.Trim
    End Get
End Property

    Public ReadOnly Property WellId() As Integer
        Get
            Return wtWell.WellID
        End Get
    End Property

    #End Region

Public Sub New(ByVal intWellID As Integer)

    MyBase.New()
    wtWell = New clsObservationWell(intWellID)

End Sub     '   New

Public Sub New(ByVal plssKey As CNPPID.PLSS)

    MyBase.New()
    wtWell = New clsObservationWell(plssKey)

End Sub     '   New

''' <summary>
''' Generate a query to execute the proper stored procedure, based on the
''' options the user configured before calling GatherReadings.
''' </summary>
Private Function BuildQuery(ByVal intYear As Integer, ByVal intMonth As Integer) As String
Dim strQuery As New System.Text.StringBuilder

    If ieReadInterval = IntervalEnum.Annually Then

        strQuery.Append("EXEC usp_GetWellDataForYear " & wtWell.WellID.ToString & ", " & _
                                                        intYear.ToString)

    ElseIf ieReadInterval = IntervalEnum.Monthly Or ieReadInterval = IntervalEnum.WithinAYear Then

        strQuery.Append("EXEC usp_GetWellDataForMonth " & wtWell.WellID.ToString & ", " & _
                                                    intMonth.ToString & ", " & intYear.ToString)

    ElseIf ieReadInterval = IntervalEnum.Quarterly Then

        strQuery.Append("EXEC usp_GetWellDataForMonths " & wtWell.WellID.ToString & ", " & _
                                                intYear.ToString & ", ")
        If ReadValue = 1 Then
            strQuery.Append("1, 3")
        ElseIf ReadValue = 2 Then
            strQuery.Append("4,6")
        ElseIf ReadValue = 3 Then
            strQuery.Append("7,9")
        Else
            strQuery.Append("10,12")
        End If

    End If

    Return strQuery.ToString

End Function        '   BuildQuery

''' <summary>
''' Call this to gather the readings.  Value should be set before calling this, like:
''' ReadInterval, ReadValue (if applicable), AverageRecords, StartDate and EndDate.
''' </summary>
Public Sub GatherReadings()
Dim intStartYear As Integer
Dim intEndYear As Integer
Dim intStartMonth As Integer
Dim intEndMonth As Integer

    If ieReadInterval = IntervalEnum.Annually Then

        intStartYear = Year(dtStartDate)
        intStartMonth = 1
        intEndYear = Year(dtEndDate)
        intEndMonth = 12

    ElseIf ieReadInterval = IntervalEnum.Monthly Or ieReadInterval = IntervalEnum.WithinAYear Then

        intStartYear = Year(dtStartDate)
        intEndYear = Year(dtEndDate)
        intStartMonth = Month(dtStartDate)
        intEndMonth = Month(dtEndDate)

    Else

        intStartYear = Year(dtStartDate)
        intEndYear = Year(dtEndDate)
        intStartMonth = intReadValue
        intEndMonth = intReadValue

    End If

    dblStartReading = GetReading(intStartYear, intStartMonth)
    dblEndReading = GetReading(intEndYear, intEndMonth)

    RaiseEvent ReadingsGathered()

End Sub     '   GatherReadings

''' <summary>
''' Retrieve a reading for the well, month and year.  Return the first reading if
''' we're matching a single record or the average of the readings if we're averaging them.
''' Raise ErrorOccurred if an error occurs or if we can't get the measuring point that
''' corresponds to the well measurement.
''' </summary>
Private Function GetReading(ByVal intYear As Integer, ByVal intMonth As Integer) As Double
Dim Conn As SqlConnection
Dim dbCmd As SqlCommand
Dim strQuery As String
Dim Rdr As SqlDataReader
Dim dblReturn As Double
Dim dblTotal As Double
Dim intNumRecords As Integer

    Try

        Conn = New SqlConnection(Data.GetObsWells)
        strQuery = BuildQuery(intYear, intMonth)
        dbCmd = New SqlCommand(strQuery, Conn)

        Dim dtDate As DateTime
        Dim dblReading As Double
        Dim dblGround As Double
        Dim dblStickup As Double
        'Dim MP As ObservationWells.clsWellElevation

        dbCmd.Connection.Open()
        Rdr = dbCmd.ExecuteReader

        intNumRecords = 0

        If Rdr.HasRows Then

            Do While Rdr.Read

                If Not Rdr.IsDBNull(0) Then
                    dblReading = Rdr.GetDecimal(0)
                Else
                    dblReading = 0
                End If

                dtDate = Rdr.GetDateTime(1)

                If Not Rdr.IsDBNull(2) Then
                    dblGround = Rdr.GetDouble(2)
                End If
                If Not Rdr.IsDBNull(3) Then
                    dblStickup = Rdr.GetDouble(3)
                End If

                dblTotal += dblGround + dblStickup - dblReading

                intNumRecords += 1

                If boolMatchRecords Then
                    Exit Do
                End If
            Loop

        End If

        Rdr.Close()
        dbCmd.Connection.Close()

    Catch ex As Exception

        dblTotal = -1
        RaiseEvent ErrorOccurred("Error trying to retrieve data for the Well ID " & _
                                wtWell.WellID.ToString & " with legal description " & _
                                LegalDescription & " for the year " & intYear.ToString & _
                                " and the month " & intMonth.ToString & ":" & vbCrLf & _
                                ex.ToString)

    Finally

        If Not Rdr Is Nothing AndAlso Not Rdr.IsClosed Then
            Rdr.Close()
        End If
        If Not Conn Is Nothing AndAlso Conn.State <> ConnectionState.Closed Then
            Conn.Close()
        End If

    End Try

    '   If we matched a single record, this will just return the number we got.
    '   If we want to average the records, this will get us the right answer.
    If intNumRecords <> 0 Then
        dblReturn = dblTotal / intNumRecords
    End If

    Return dblReturn

End Function        '   GetReading

End Class       '   clsWellChange
