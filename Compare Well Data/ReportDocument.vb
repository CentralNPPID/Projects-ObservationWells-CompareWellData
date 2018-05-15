Imports System.ComponentModel
Imports System.Reflection
Imports System.Drawing
Imports System.Drawing.Printing

''' <summary>
''' <para>
''' ReportDocument extends the functionality of the standard standard <see cref="T:System.Drawing.Printing.PrintDocument" />
''' class by adding a number of new properties and events that make report generation easier.
''' Additionally, these events provide a <see cref="T:vbReport.ReportPageEventArgs" /> parameter
''' which provides extra properties and methods beyond the normal 
''' <see cref="T:System.Drawing.Printing.PrintPageEventArgs" />, again simplifying the
''' report generation process.
''' </para><para>
''' The ReportDocument class can be used just like a standard System.Drawing.Printing.PrintDocument
''' class. In other words, the standard Print method, print dialogs and print preview capabilities
''' of .NET work with ReportDocument just like they do with PrintDocument.
''' </para>
''' </summary>
Public Class ReportDocument
  Inherits PrintDocument

#Region " Event declarations "

  ''' <summary>
  ''' Raised once immediately before anything is printed to the report. The cursor is on the first line of the first page.
  ''' </summary>
  Public Event ReportBegin(ByVal sender As Object, _
    ByVal e As ReportPageEventArgs)
  ''' <summary>
  ''' Raised for each page immediately before anything is printed to that page. The cursor is on the first line of the page.
  ''' </summary>
  Public Event PrintPageBegin(ByVal sender As Object, _
    ByVal e As ReportPageEventArgs)
  ''' <summary>
  ''' Raised for each page immediately after the header for the page has been printed. The cursor is on the first line of the report body.
  ''' </summary>
  Public Event PrintPageBodyStart(ByVal sender As Object, _
    ByVal e As ReportPageEventArgs)
  ''' <summary>
  ''' Raised for each page immediately before the footer for the page is printed. The cursor is on the first line of the header.
  ''' </summary>
  Public Event PrintPageBodyEnd(ByVal sender As Object, _
    ByVal e As ReportPageEventArgs)
  ''' <summary>
  ''' Raised for each page after the footer has been printed. The cursor is past the end of the footer, typically into the bottom margin of the page.
  ''' </summary>
  Public Event PrintPageEnd(ByVal sender As Object, _
    ByVal e As ReportPageEventArgs)
  ''' <summary>
  ''' Raised once at the very end of the report after all other printing is complete. The cursor is past the end of the footer on the last page, typically into the bottom margin of the page.
  ''' </summary>
  Public Event ReportEnd(ByVal sender As Object, _
    ByVal e As ReportPageEventArgs)

#End Region

#Region " Report Properties and Settings "

  Private mPageNumber As Integer
  Private mSupressDefaultHeader As Boolean
  Private mTitle As String
  Private mSubTitleLeft As String
  Private mSubTitleRight As String
  Private mSupressDefaultFooter As Boolean
  Private mFooterLeft As String
  Private mFooterRight As String
  Private mFont As Font
  Private mBrush As Brush
  Private mFooterLines As Integer
  Private mColumns As New ReportColumnCollection()
  Private mDataSource As Object
  Private mDataMember As String
  Private mAutoDiscover As Boolean
  Private mShowHeaderLine As Boolean
  Private mHeaderJustification As ReportLineJustification
  Private WithEvents mBindingList As IBindingList

  Public Sub New()

    mFont = New Font("Courier New", 10)
    mBrush = Brushes.Black
    mShowHeaderLine = True
    FooterLines = 2

  End Sub

  Public Sub New(ByVal font As Font, ByVal brush As Brush)

    mFont = font
    mBrush = brush
    mShowHeaderLine = True
    FooterLines = 2

  End Sub

  ''' <summary>
  ''' Allows the developer to set or retrieve the Font object that is used
  ''' to render the text of the report. This defaults to a 10 point
  ''' Courier New font.
  ''' </summary>
  ''' <value>A Font object</value>
  Public Property Font() As Font
    Get
      Return mFont
    End Get
    Set(ByVal Value As Font)
      mFont = Value
    End Set
  End Property

  ''' <summary>
  ''' Allows the developer to set or retrieve the Brush object that is
  ''' used to render the text of the report. This defaults to a solid black
  ''' brush.
  ''' </summary>
  ''' <value>A Brush object</value>
  Public Property Brush() As Brush
    Get
      Return mBrush
    End Get
    Set(ByVal Value As Brush)
      mBrush = Value
    End Set
  End Property

''' <summary>
''' The current page number.
''' </summary>
Public Property PageNumber() As Integer
    Get
        Return mPageNumber
    End Get
    Set(ByVal Value As Integer)
        mPageNumber = Value
    End Set
End Property

  ''' <summary>
  ''' If this is set to True the default header will not be rendered at
  ''' the top of each page.
  ''' </summary>
  ''' <value>A Boolean indicating whether the default header should be supressed.</value>
  Public Property SupressDefaultHeader() As Boolean
    Get
      Return mSupressDefaultHeader
    End Get
    Set(ByVal Value As Boolean)
      mSupressDefaultHeader = Value
    End Set
  End Property

  ''' <summary>
  ''' If this is set to True the default footer will not be rendered at
  ''' the bottom of each page.
  ''' </summary>
  ''' <value>A Boolean indicating whether the default footer should be supressed.</value>
  Public Property SupressDefaultFooter() As Boolean
    Get
      Return mSupressDefaultFooter
    End Get
    Set(ByVal Value As Boolean)
      mSupressDefaultFooter = Value
    End Set
  End Property

  ''' <summary>
  ''' Sets or returns the number of lines reserved at the bottom of each page
  ''' for the footer. This defaults to 2 lines for the default footer. If you
  ''' want to add extra lines to the footer you should increase this value accordingly.
  ''' </summary>
  ''' <value>The number of lines reserved for the page footer.</value>
  Public Property FooterLines() As Integer
    Get
      Return mFooterLines
    End Get
    Set(ByVal Value As Integer)
      mFooterLines = Value
    End Set
  End Property

  ''' <summary>
  ''' Returns a collection of <see cref="T:vbReport.ReportColumn" /> objects that
  ''' represent the columns to be rendered in a table if the report is bound to
  ''' a data source via the <see cref="P:vbReport.ReportDocument.DataSource" />
  ''' property.
  ''' </summary>
  ''' <value>A collection of columns to be rendered in the report.</value>
  Public ReadOnly Property Columns() As ReportColumnCollection
    Get
      Return mColumns
    End Get
  End Property

  ''' <summary>
  ''' The report title displayed at the top of each page.
  ''' </summary>
  ''' <value>Text to be displayed.</value>
  Public Property Title() As String
    Get
      Return mTitle
    End Get
    Set(ByVal Value As String)
      mTitle = Value
    End Set
  End Property

  ''' <summary>
  ''' Text to be displayed on the left side of the line below the title on each page.
  ''' </summary>
  ''' <value>Text to be displayed.</value>
  Public Property SubTitleLeft() As String
    Get
      Return mSubTitleLeft
    End Get
    Set(ByVal Value As String)
      mSubTitleLeft = Value
    End Set
  End Property

  ''' <summary>
  ''' Text to be displayed on the right side of the line below the title on each page.
  ''' </summary>
  ''' <value>Text to be displayed.</value>
  Public Property SubTitleRight() As String
    Get
      Return mSubTitleRight
    End Get
    Set(ByVal Value As String)
      mSubTitleRight = Value
    End Set
  End Property

  ''' <summary>
  ''' Text to be displayed on the left side of the footer below the separator line
  ''' at the bottom of each page.
  ''' </summary>
  ''' <value>Text to be displayed.</value>
  Public Property FooterLeft() As String
    Get
      Return mFooterLeft
    End Get
    Set(ByVal Value As String)
      mFooterLeft = Value
    End Set
  End Property

  ''' <summary>
  ''' Text to be displayed on the right side of the footer below the separator line
  ''' at the bottom of each page.
  ''' </summary>
  ''' <value>Text to be displayed.</value>
  Public Property FooterRight() As String
    Get
      Return mFooterRight
    End Get
    Set(ByVal Value As String)
      mFooterRight = Value
    End Set
  End Property

  ''' <summary>
  ''' Whether or not to print a line after the header to separate the header
  ''' and the body.  Default is True.
  ''' </summary>
  ''' <value>Print a line to separate the header and body. </value>
  Public Property ShowHeaderLine() As Boolean
    Get
        Return mShowHeaderLine
    End Get
    Set(ByVal Value As Boolean)
            mShowHeaderLine = Value
    End Set
  End Property

  ''' <summary>
  ''' How to print the header -- left, center or right.
  ''' The default is Center.
  ''' </summary>
  ''' <value>How to print the header -- left, center or right. </value>
  Public Property HeaderJustification() As ReportLineJustification
    Get
        Return mHeaderJustification
    End Get
    Set(ByVal Value As ReportLineJustification)
            mHeaderJustification = Value
    End Set
  End Property

  ''' <summary>
  ''' The left margin of the page in hundredths of an inch.
  '''</summary>
  ''' <value> The left margin of the page in hundredths of an inch. </value>
  Public Property LeftMargin() As Integer
    Get
        Return MyBase.DefaultPageSettings.Margins.Left
    End Get
    Set(ByVal Value As Integer)
            MyBase.DefaultPageSettings.Margins.Left = Value
    End Set
  End Property

  ''' <summary>
  ''' The right margin of the page in hundredths of an inch.
  '''</summary>
  ''' <value> The right margin of the page in hundredths of an inch. </value>
  Public Property RightMargin() As Integer
    Get
        Return MyBase.DefaultPageSettings.Margins.Right
    End Get
    Set(ByVal Value As Integer)
            MyBase.DefaultPageSettings.Margins.Right = Value
    End Set
  End Property

  ''' <summary>
  ''' The top margin of the page in hundredths of an inch.
  '''</summary>
  ''' <value> The top margin of the page in hundredths of an inch. </value>
  Public Property TopMargin() As Integer
    Get
        Return MyBase.DefaultPageSettings.Margins.Top
    End Get
    Set(ByVal Value As Integer)
            MyBase.DefaultPageSettings.Margins.Top = Value
    End Set
  End Property

  ''' <summary>
  ''' The bottom margin of the page in hundredths of an inch.
  '''</summary>
  ''' <value> The bottom margin of the page in hundredths of an inch. </value>
  Public Property BottomMargin() As Integer
    Get
        Return MyBase.DefaultPageSettings.Margins.Bottom
    End Get
    Set(ByVal Value As Integer)
            MyBase.DefaultPageSettings.Margins.Bottom = Value
    End Set
  End Property

#End Region

#Region " DataSource/DataMember "

  ''' <summary>
  ''' By setting this property we provide the report with a data source. The
  ''' data in the data source will be rendered into the report in tabular
  ''' format based on the columns defined in the <see cref="P:vbReport.ReportDocument.Columns" />
  ''' property.
  ''' </summary>
  ''' <value>A valid data source.</value>
  <Category("Data"), RefreshProperties(RefreshProperties.Repaint), TypeConverter("System.Windows.Forms.Design.DataSourceConverter," & "System.Design")> Public Property DataSource() As Object
    Get
      Return mDataSource
    End Get
    Set(ByVal Value As Object)
      mDataSource = Value
      SetSource()
      If mAutoDiscover Then
        DoAutoDiscover()
      End If
      mRow = 0
    End Set
  End Property

  ''' <summary>
  ''' The DataMember property allows us to easily set a single column
  ''' of data to be displayed when the report is bound to a data source.
  ''' If we want to display multiple columns of data in the report
  ''' we should use the <see cref="P:vbReport.ReportDocument.Columns" />
  ''' property to define the columns.
  ''' </summary>
  ''' <value>A valid data source.</value>
  <Category("Data"), Editor("System.Windows.Forms.Design.DataMemberListEditor," & "System.Design", GetType(System.Drawing.Design.UITypeEditor))> Public Property DataMember() As String
    Get
      Return mDataMember
    End Get
    Set(ByVal Value As String)
      mDataMember = Value
      SetSource()
      If mAutoDiscover Then
        DoAutoDiscover()
      End If
      mRow = 0
    End Set
  End Property

  Private Sub SetSource()

    Dim InnerSource As IList = InnerDataSource()

    If TypeOf InnerSource Is IBindingList Then
      mBindingList = CType(InnerSource, IBindingList)

    Else
      mBindingList = Nothing
    End If

  End Sub

  Private Function InnerDataSource() As IList

    If TypeOf mDataSource Is DataSet Then
      If Len(mDataMember) > 0 Then
        Return CType(CType(mDataSource, DataSet).Tables(mDataMember), IListSource).GetList

      Else
        Return CType(CType(mDataSource, DataSet).Tables(0), IListSource).GetList
      End If

    ElseIf TypeOf mDataSource Is IListSource Then
      Return CType(mDataSource, IListSource).GetList

    Else
      Return CType(mDataSource, IList)
    End If

  End Function

#End Region

#Region " AutoDiscover "

  <Category("Data")> _
  Public Property AutoDiscover() As Boolean
    Get
      Return mAutoDiscover
    End Get
    Set(ByVal Value As Boolean)
      If mAutoDiscover = False AndAlso Value = True Then
        mAutoDiscover = Value
        DoAutoDiscover()
      Else
        mAutoDiscover = Value
        If mAutoDiscover = False Then
          mColumns.Clear()
        End If
      End If
    End Set
  End Property

  Private Sub DoAutoDiscover()

    Dim InnerSource As IList = InnerDataSource()
    mColumns.Clear()

    If InnerSource Is Nothing Then Exit Sub

    If TypeOf InnerSource Is DataView Then
      DoAutoDiscover(CType(InnerSource, DataView))

    Else
      DoAutoDiscover(InnerSource)
    End If

  End Sub

  Private Sub DoAutoDiscover(ByVal ds As DataView)

    Dim field As Integer
    Dim col As ReportColumn

    For field = 0 To ds.Table.Columns.Count - 1
      col = New ReportColumn()
      col.Name = ds.Table.Columns(field).Caption
      col.Field = ds.Table.Columns(field).ColumnName
      mColumns.Add(col)
    Next

    mColumns.SetEvenSpacing(650)

  End Sub

  Private Sub DoAutoDiscover(ByVal ds As IList)

    If ds.Count > 0 Then
      ' retrieve the first item from the list
      Dim obj As Object = ds.Item(0)

      If TypeOf obj Is ValueType AndAlso obj.GetType.IsPrimitive Then
        ' the value is a primitive value type
        Dim col As ReportColumn
        col = New ReportColumn()
        col.name = "Value"
        mColumns.Add(col)

      ElseIf TypeOf obj Is String Then
        ' the value is a simple string
        Dim col As ReportColumn
        col = New ReportColumn()
        col.name = "Text"
        mColumns.Add(col)

      Else
        ' the value is an object or Structure
        Dim SourceType As Type = obj.GetType
        Dim column As Integer

        ' retrieve a list of all public properties
        Dim props As PropertyInfo() = SourceType.GetProperties()
        If UBound(props) >= 0 Then
          For column = 0 To UBound(props)
            mColumns.Add(props(column).Name)
          Next
        End If

        ' retrieve a list of all public fields
        Dim fields As FieldInfo() = SourceType.GetFields()
        If UBound(fields) >= 0 Then
          For column = 0 To UBound(fields)
            mColumns.Add(fields(column).Name)
          Next
        End If

        mColumns.SetEvenSpacing(650)

      End If
    End If
  End Sub

#End Region

#Region " GetField "

  Private Function GetField(ByVal obj As Object, ByVal FieldName As String) As String
    If TypeOf obj Is DataRowView Then
      ' this is a DataRowView from a DataView
      Return CType(obj, DataRowView).Item(FieldName).ToString

    ElseIf TypeOf obj Is ValueType AndAlso obj.GetType.IsPrimitive Then
      ' this is a primitive value type
      Return obj.ToString

    ElseIf TypeOf obj Is String Then
      ' this is a simple string
      Return CStr(obj)

    Else
      ' this is an object or Structure
      Try
        Dim sourcetype As Type = obj.GetType

        ' see if the field is a property
        Dim prop As PropertyInfo = sourcetype.GetProperty(FieldName)

        If prop Is Nothing OrElse Not prop.CanRead Then
          ' no readable property of that name exists - check for a field
          Dim field As FieldInfo = sourcetype.GetField(FieldName)

          If field Is Nothing Then
            ' no field exists either, return the field name
            ' as a debugging indicator
            Return "No such value " & FieldName

          Else
            ' got a field, return its value
            Return field.GetValue(obj).ToString
          End If

        Else
          ' found a property, return its value
          Return prop.GetValue(obj, Nothing).ToString
        End If

      Catch ex As Exception
        Return ex.Message
      End Try
    End If
  End Function

#End Region

#Region " Do printing "

  Dim mRow As Integer

  Private Sub ReportDocument_BeginPrint(ByVal sender As Object, _
      ByVal e As System.Drawing.Printing.PrintEventArgs) Handles MyBase.BeginPrint

    mPageNumber = 0
    mRow = 0

  End Sub

  Private Sub ReportDocument_PrintPage(ByVal sender As Object, _
      ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles MyBase.PrintPage

    mPageNumber += 1

    ' create our ReportPageEventArgs object for this page
    Dim page As New ReportPageEventArgs(e, mPageNumber, mFont, mBrush, mFooterLines)

    ' if we're generating page 1 raise the ReportBegin event
    If mPageNumber = 1 Then
      RaiseEvent ReportBegin(Me, page)
    End If

    ' set column widths if we have data bound columns
    If mColumns.Count > 0 Then
      Dim space As Integer = CInt(e.MarginBounds.Width / mColumns.Count)
      Dim index As Integer
      For index = 0 To mColumns.Count - 1
        mColumns(index).Width = space
      Next
    End If

    ' generate the page header/body/footer
    GeneratePage(page)

    ' if there are no more pages to generate then raise
    ' the ReportEnd event
    If Not page.HasMorePages Then
      RaiseEvent ReportEnd(Me, page)
    End If

    ' the client code may have overridden the Cancel or
    ' HasMorePages values somewhere during the process,
    ' so we restore them to the underlying PrintPageEventArgs
    ' object - thus allowing our base class to take care
    ' of these details for us
    e.Cancel = page.Cancel
    e.HasMorePages = page.HasMorePages

  End Sub

  Private Sub GeneratePage(ByVal e As ReportPageEventArgs)

    Dim InnerSource As IList = InnerDataSource()
    Dim Field As Integer

    ' we're about to print the page
    RaiseEvent PrintPageBegin(Me, e)

    ' generate the header unless it is supressed
    If Not SupressDefaultHeader Then
      PrintHeader(e)
    End If

    ' we're about to print the body of the page
    RaiseEvent PrintPageBodyStart(Me, e)

    ' if we're data bound automatically generate the output
    ' based on the data from the data source
    If Not mDataSource Is Nothing AndAlso mColumns.Count > 0 Then
      ' load the data into the control
      While Not e.EndOfPage AndAlso mRow < InnerSource.Count
        ' load all subfields
        For Field = 0 To mColumns.Count - 1
          e.WriteColumn(GetField(InnerSource.Item(mRow), mColumns(Field).Field).ToString, mColumns(Field))
        Next
        e.WriteLine()
        mRow += 1
      End While

      e.HasMorePages = mRow < InnerSource.Count
    End If

    ' we're done generating the body of this page
    RaiseEvent PrintPageBodyEnd(Me, e)

    ' generate the page footer unless it is supressed
    If Not SupressDefaultFooter Then
      PrintFooter(e)
    End If

    ' we're all done with the page
    RaiseEvent PrintPageEnd(Me, e)

  End Sub

  Private Sub PrintHeader(ByVal e As ReportPageEventArgs)

    Dim field As Integer
    Dim strLines() As String

    With e
      ' print the title
      strLines = Split(mTitle, vbCrLf)
      For Each strLine As String In strLines
        .WriteLine(strLine, mHeaderJustification)
      Next

      ' print the sub-title line
      .Write(mSubTitleLeft)
      .WriteLine(mSubTitleRight, ReportLineJustification.Right)

      ' if we are data bound display column titles for the
      ' data bound columns
      If mColumns.Count > 0 Then
        ' load the column headers
        For field = 0 To mColumns.Count - 1
          .WriteColumn(mColumns(field).Name, mColumns(field))
        Next
        .WriteLine()
      End If

      ' display a horizontal line to separate the header from
      ' the body
      If mShowHeaderLine Then
        .HorizontalRule()
      End If
    End With

  End Sub

  Private Sub PrintFooter(ByVal e As ReportPageEventArgs)

    With e
      ' set our vertical position to the top of the footer region
      .CurrentY = e.PageBottom

      ' display a horizontal line to separate the body from
      ' the footer
      .HorizontalRule()

      ' write the left-side footer text
      If Len(mFooterLeft) > 0 Then
        .Write(mFooterLeft)

      Else
        ' we default to displaying the current date
        .Write(Format(Now, "Short date"))
      End If

      ' write the right-side footer text
      If Len(mFooterRight) > 0 Then
        .WriteLine(mFooterRight)

      Else
        ' we default to displaying the current page number
        .WriteLine("Page " & e.PageNumber, ReportLineJustification.Right)
      End If
    End With

  End Sub

#End Region

End Class
