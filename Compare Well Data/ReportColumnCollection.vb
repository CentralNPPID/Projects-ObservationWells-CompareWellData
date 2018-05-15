''' <summary>
''' Defines a strongly-typed collection that contains
''' <see cref="T:vbReport.ReportColumn" /> objects.
''' </summary>
Public Class ReportColumnCollection
  Inherits CollectionBase

  ''' <summary>
  ''' Returns a specific <see cref="T:vbReport.ReportColumn" /> object
  ''' from the collection.
  ''' </summary>
  ''' <param name="index"></param>
  ''' <value>A specific column object.</value>
  Default Public ReadOnly Property Item(ByVal index As Integer) As ReportColumn
    Get
      Return CType(list(index), ReportColumn)
    End Get
  End Property

  ''' <summary>
  ''' Adds a <see cref="T:vbReport.ReportColumn" /> object
  ''' to the collection.
  ''' </summary>
  ''' <param name="column">A column object.</param>
  Public Sub Add(ByVal column As ReportColumn)
    list.Add(column)
  End Sub

  ''' <summary>
  ''' Adds a <see cref="T:vbReport.ReportColumn" /> object
  ''' to the collection based on a field name. The Name and Field
  ''' of the column are set to the provided field name. The 
  ''' Left and Width values are 0 and must be set separately.
  ''' </summary>
  ''' <param name="Field">The name of the data field.</param>
  Public Sub Add(ByVal Field As String)
    Dim col As New ReportColumn()
    col.Name = Field
    col.Field = Field
    Add(col)
  End Sub

  ''' <summary>
  ''' Adds a <see cref="T:vbReport.ReportColumn" /> object
  ''' to the collection based on a field name. The Name and Field
  ''' of the column are set to the provided field name. The 
  ''' Left value is set to the provided value. The Width value
  ''' is 0 and must be set separately.
  ''' </summary>
  ''' <param name="Field">The name of the data field.</param>
  ''' <param name="Left">The X position of the column.</param>
  Public Sub Add(ByVal Field As String, ByVal Left As Integer)
    Dim col As New ReportColumn()
    col.Name = Field
    col.Field = Field
    col.Left = Left
    Add(col)
  End Sub

  ''' <summary>
  ''' Adds a <see cref="T:vbReport.ReportColumn" /> object
  ''' to the collection based on a field name. The Name and Field
  ''' of the column are set to the provided values. The 
  ''' Left value is set to the provided value. The Width value
  ''' is 0 and must be set separately.
  ''' </summary>
  ''' <param name="Name">The human-readable column name.</param>
  ''' <param name="Field">The name of the data field.</param>
  ''' <param name="Left">The X position of the column.</param>
  Public Sub Add(ByVal Name As String, ByVal Field As String, ByVal Left As Integer)
    Dim col As New ReportColumn()
    col.Name = Name
    col.Field = Field
    col.Left = Left
    Add(col)
  End Sub

  ''' <summary>
  ''' Removes the specified column object from the collection.
  ''' </summary>
  ''' <param name="column">A column object.</param>
  Public Sub Remove(ByVal column As ReportColumn)
    list.Remove(column)
  End Sub

  ''' <summary>
  ''' Called by the data binding mechanism to automatically run
  ''' through all the columns defined by this collection and to
  ''' set their widths to evenly consume all the horizontal space
  ''' on a line.
  ''' </summary>
  ''' <param name="Width">The total width of a printed line.</param>
  Friend Sub SetEvenSpacing(ByVal Width As Integer)

    Dim space As Integer = CInt(Width / list.Count)
    Dim index As Integer

    For index = 0 To list.Count - 1
      With CType(list(index), ReportColumn)
        .Left = space * index
        .Width = space
      End With
    Next

  End Sub

End Class
