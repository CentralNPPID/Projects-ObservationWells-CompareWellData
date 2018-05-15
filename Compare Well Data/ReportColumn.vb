''' <summary>
''' Defines a column into whch text can be rendered on a line
''' of a table when the <see cref="T:vbReport.ReportDocument" />
''' is bound to a datasource.
''' </summary>
Public Class ReportColumn
  ''' <summary>
  ''' Defines the human-readable name of the column. This value
  ''' can be useful for generating descriptive headers.
  ''' </summary>
  Public Name As String
  ''' <summary>
  ''' Contains the name of the field within the data source that
  ''' contains the data. This value is used to retrieve the data
  ''' value from the data source. It corresponds to the column
  ''' name in a DataTable, or a property name of an object.
  ''' </summary>
  Public Field As String
  ''' <summary>
  ''' Defines the horizontal start location (X coordinate) of the
  ''' column. When text is written to the column by the 
  ''' <see cref="M:vbReport.ReportPageEventArgs.WriteColumn(System.String,vbReport.ReportColumn)" /> method
  ''' it is rendered starting at this horizontal location.
  ''' </summary>
  Public Left As Integer
  ''' <summary>
  ''' Defines the width of the column. Before text is written to the 
  ''' column by the 
  ''' <see cref="M:vbReport.ReportPageEventArgs.WriteColumn(System.String,vbReport.ReportColumn)" /> method
  ''' the column is filled with a white rectangle defined by the width
  ''' of the column. This helps prevent text from overwriting other
  ''' text within our columns.
  ''' </summary>
  Friend Width As Integer
End Class
