Imports CNPPID
Imports Miscellaneous
Imports System.Data.SqlClient

''' <remarks>
''' Dave Campbell
''' August 2005
'''  
''' Compare Wells
''' 
''' This allows the user to generate a report comparing the well data measurements for
''' a series of wells over a given period of time.  The user can compare data from within
''' the same year or the same quarter in different years, the average reading over two 
''' different years or the same month in different years.
''' 
''' The output can be put into a delimited file, a regular file or printed to a printer.
''' 
''' The program can be called from the command line.  See the sub SetUpProgram for a 
''' description of the proper format for the parameters.
''' </remarks>
Public Class frmCompareWells
    Inherits System.Windows.Forms.Form

'   This is how many lines we can fit on a page if it's 8 1/2 x 11 printed
'   in Landscape mode.
Const LinesPerPage As Integer = 62

Const LegalDescCol As Integer = 0
Const WellNameCol As Integer = 1
Const LatDecimalCol As Integer = 2
Const LongDecimalCol As Integer = 3
Const StartReadingCol As Integer = 4
Const EndReadingCol As Integer = 5
Const ChangeCol As Integer = 6

Private WithEvents Report As ReportDocument

Private Structure LayoutType
    Public Start As Integer
    Public Width As Integer
End Structure

Dim arrLayout(ChangeCol) As LayoutType

Private Quarters() = {"Jan-Mar", "Apr-Jun", "Jul-Sep", "Oct-Dec"}

Private Structure WellRangeType
    Public Township As Integer
    Public Range As Integer
    Public Section As Integer
End Structure

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents sbInfo As System.Windows.Forms.StatusBar
    Friend WithEvents lblInstructions As System.Windows.Forms.Label
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents gbPeriod As System.Windows.Forms.GroupBox
    Friend WithEvents radMonth As System.Windows.Forms.RadioButton
    Friend WithEvents radQuarter As System.Windows.Forms.RadioButton
    Friend WithEvents radAnnual As System.Windows.Forms.RadioButton
    Friend WithEvents radWithinAYear As System.Windows.Forms.RadioButton
    Friend WithEvents cboMonth As System.Windows.Forms.ComboBox
    Friend WithEvents cboQuarter As System.Windows.Forms.ComboBox
    Friend WithEvents lblEndYear As System.Windows.Forms.Label
    Friend WithEvents lblStartYear As System.Windows.Forms.Label
    Friend WithEvents cboEndYear As System.Windows.Forms.ComboBox
    Friend WithEvents cboStartYear As System.Windows.Forms.ComboBox
    Friend WithEvents gbMatchingRecords As System.Windows.Forms.GroupBox
    Friend WithEvents radMatchSingle As System.Windows.Forms.RadioButton
    Friend WithEvents radAverageRecords As System.Windows.Forms.RadioButton
    Friend WithEvents lstAvailableWells As System.Windows.Forms.ListBox
    Friend WithEvents lstSelectedWells As System.Windows.Forms.ListBox
    Friend WithEvents btnDefineRange As System.Windows.Forms.Button
    Friend WithEvents btnRemoveWells As System.Windows.Forms.Button
    Friend WithEvents btnAddWells As System.Windows.Forms.Button
    Friend WithEvents lblAvailableCount As System.Windows.Forms.Label
    Friend WithEvents lblSelectedCount As System.Windows.Forms.Label
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents gbOutput As System.Windows.Forms.GroupBox
    Friend WithEvents radScreen As System.Windows.Forms.RadioButton
    Friend WithEvents radPrinter As System.Windows.Forms.RadioButton
    Friend WithEvents radFile As System.Windows.Forms.RadioButton
    Friend WithEvents lblTo As System.Windows.Forms.Label
    Friend WithEvents lblFrom As System.Windows.Forms.Label
    Friend WithEvents txtTo As System.Windows.Forms.TextBox
    Friend WithEvents txtFrom As System.Windows.Forms.TextBox
    Friend WithEvents MainMenu1 As System.Windows.Forms.MainMenu
    Friend WithEvents mnuFile As System.Windows.Forms.MenuItem
    Friend WithEvents mnuHelp As System.Windows.Forms.MenuItem
    Friend WithEvents mnuAbout As System.Windows.Forms.MenuItem
    Friend WithEvents mnuExit As System.Windows.Forms.MenuItem
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmCompareWells))
Me.sbInfo = New System.Windows.Forms.StatusBar
Me.lblInstructions = New System.Windows.Forms.Label
Me.Panel1 = New System.Windows.Forms.Panel
Me.gbMatchingRecords = New System.Windows.Forms.GroupBox
Me.radAverageRecords = New System.Windows.Forms.RadioButton
Me.radMatchSingle = New System.Windows.Forms.RadioButton
Me.cboStartYear = New System.Windows.Forms.ComboBox
Me.cboEndYear = New System.Windows.Forms.ComboBox
Me.lblStartYear = New System.Windows.Forms.Label
Me.lblEndYear = New System.Windows.Forms.Label
Me.gbPeriod = New System.Windows.Forms.GroupBox
Me.lblTo = New System.Windows.Forms.Label
Me.lblFrom = New System.Windows.Forms.Label
Me.txtTo = New System.Windows.Forms.TextBox
Me.txtFrom = New System.Windows.Forms.TextBox
Me.cboQuarter = New System.Windows.Forms.ComboBox
Me.cboMonth = New System.Windows.Forms.ComboBox
Me.radWithinAYear = New System.Windows.Forms.RadioButton
Me.radAnnual = New System.Windows.Forms.RadioButton
Me.radQuarter = New System.Windows.Forms.RadioButton
Me.radMonth = New System.Windows.Forms.RadioButton
Me.lstAvailableWells = New System.Windows.Forms.ListBox
Me.lstSelectedWells = New System.Windows.Forms.ListBox
Me.btnDefineRange = New System.Windows.Forms.Button
Me.btnRemoveWells = New System.Windows.Forms.Button
Me.btnAddWells = New System.Windows.Forms.Button
Me.lblAvailableCount = New System.Windows.Forms.Label
Me.lblSelectedCount = New System.Windows.Forms.Label
Me.btnOK = New System.Windows.Forms.Button
Me.btnCancel = New System.Windows.Forms.Button
Me.gbOutput = New System.Windows.Forms.GroupBox
Me.radFile = New System.Windows.Forms.RadioButton
Me.radPrinter = New System.Windows.Forms.RadioButton
Me.radScreen = New System.Windows.Forms.RadioButton
Me.MainMenu1 = New System.Windows.Forms.MainMenu
Me.mnuFile = New System.Windows.Forms.MenuItem
Me.mnuHelp = New System.Windows.Forms.MenuItem
Me.mnuAbout = New System.Windows.Forms.MenuItem
Me.mnuExit = New System.Windows.Forms.MenuItem
Me.Panel1.SuspendLayout()
Me.gbMatchingRecords.SuspendLayout()
Me.gbPeriod.SuspendLayout()
Me.gbOutput.SuspendLayout()
Me.SuspendLayout()
'
'sbInfo
'
Me.sbInfo.Location = New System.Drawing.Point(0, 375)
Me.sbInfo.Name = "sbInfo"
Me.sbInfo.Size = New System.Drawing.Size(640, 22)
Me.sbInfo.TabIndex = 0
'
'lblInstructions
'
Me.lblInstructions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
Me.lblInstructions.Location = New System.Drawing.Point(0, 0)
Me.lblInstructions.Name = "lblInstructions"
Me.lblInstructions.Size = New System.Drawing.Size(640, 48)
Me.lblInstructions.TabIndex = 1
Me.lblInstructions.Text = "This allows you to compare the data from wells for a single month, quarter, betwe" & _
"en years or between months in the same year.  You can send the output to the scr" & _
"een, a file or the printer."
'
'Panel1
'
Me.Panel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
Me.Panel1.Controls.Add(Me.gbMatchingRecords)
Me.Panel1.Controls.Add(Me.cboStartYear)
Me.Panel1.Controls.Add(Me.cboEndYear)
Me.Panel1.Controls.Add(Me.lblStartYear)
Me.Panel1.Controls.Add(Me.lblEndYear)
Me.Panel1.Controls.Add(Me.gbPeriod)
Me.Panel1.Location = New System.Drawing.Point(0, 48)
Me.Panel1.Name = "Panel1"
Me.Panel1.Size = New System.Drawing.Size(640, 112)
Me.Panel1.TabIndex = 2
'
'gbMatchingRecords
'
Me.gbMatchingRecords.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
Me.gbMatchingRecords.Controls.Add(Me.radAverageRecords)
Me.gbMatchingRecords.Controls.Add(Me.radMatchSingle)
Me.gbMatchingRecords.Location = New System.Drawing.Point(424, 16)
Me.gbMatchingRecords.Name = "gbMatchingRecords"
Me.gbMatchingRecords.Size = New System.Drawing.Size(192, 80)
Me.gbMatchingRecords.TabIndex = 7
Me.gbMatchingRecords.TabStop = False
Me.gbMatchingRecords.Text = "With Multiple Records"
'
'radAverageRecords
'
Me.radAverageRecords.Location = New System.Drawing.Point(24, 48)
Me.radAverageRecords.Name = "radAverageRecords"
Me.radAverageRecords.Size = New System.Drawing.Size(112, 24)
Me.radAverageRecords.TabIndex = 1
Me.radAverageRecords.Text = "Average Records"
'
'radMatchSingle
'
Me.radMatchSingle.Checked = True
Me.radMatchSingle.Location = New System.Drawing.Point(24, 16)
Me.radMatchSingle.Name = "radMatchSingle"
Me.radMatchSingle.Size = New System.Drawing.Size(141, 24)
Me.radMatchSingle.TabIndex = 0
Me.radMatchSingle.TabStop = True
Me.radMatchSingle.Text = "Match a Single Record"
'
'cboStartYear
'
Me.cboStartYear.Location = New System.Drawing.Point(248, 32)
Me.cboStartYear.Name = "cboStartYear"
Me.cboStartYear.Size = New System.Drawing.Size(72, 21)
Me.cboStartYear.TabIndex = 6
Me.cboStartYear.Text = "ComboBox2"
'
'cboEndYear
'
Me.cboEndYear.Location = New System.Drawing.Point(248, 80)
Me.cboEndYear.Name = "cboEndYear"
Me.cboEndYear.Size = New System.Drawing.Size(72, 21)
Me.cboEndYear.TabIndex = 5
Me.cboEndYear.Text = "ComboBox1"
'
'lblStartYear
'
Me.lblStartYear.AutoSize = True
Me.lblStartYear.Location = New System.Drawing.Point(248, 16)
Me.lblStartYear.Name = "lblStartYear"
Me.lblStartYear.Size = New System.Drawing.Size(55, 16)
Me.lblStartYear.TabIndex = 4
Me.lblStartYear.Text = "Start Year"
'
'lblEndYear
'
Me.lblEndYear.AutoSize = True
Me.lblEndYear.Location = New System.Drawing.Point(256, 64)
Me.lblEndYear.Name = "lblEndYear"
Me.lblEndYear.Size = New System.Drawing.Size(51, 16)
Me.lblEndYear.TabIndex = 3
Me.lblEndYear.Text = "End Year"
'
'gbPeriod
'
Me.gbPeriod.Controls.Add(Me.lblTo)
Me.gbPeriod.Controls.Add(Me.lblFrom)
Me.gbPeriod.Controls.Add(Me.txtTo)
Me.gbPeriod.Controls.Add(Me.txtFrom)
Me.gbPeriod.Controls.Add(Me.cboQuarter)
Me.gbPeriod.Controls.Add(Me.cboMonth)
Me.gbPeriod.Controls.Add(Me.radWithinAYear)
Me.gbPeriod.Controls.Add(Me.radAnnual)
Me.gbPeriod.Controls.Add(Me.radQuarter)
Me.gbPeriod.Controls.Add(Me.radMonth)
Me.gbPeriod.Location = New System.Drawing.Point(0, 0)
Me.gbPeriod.Name = "gbPeriod"
Me.gbPeriod.Size = New System.Drawing.Size(200, 112)
Me.gbPeriod.TabIndex = 0
Me.gbPeriod.TabStop = False
Me.gbPeriod.Text = "Period"
'
'lblTo
'
Me.lblTo.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
Me.lblTo.AutoSize = True
Me.lblTo.Location = New System.Drawing.Point(160, 72)
Me.lblTo.Name = "lblTo"
Me.lblTo.Size = New System.Drawing.Size(17, 16)
Me.lblTo.TabIndex = 9
Me.lblTo.Text = "To"
Me.lblTo.Visible = False
'
'lblFrom
'
Me.lblFrom.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
Me.lblFrom.AutoSize = True
Me.lblFrom.Location = New System.Drawing.Point(96, 72)
Me.lblFrom.Name = "lblFrom"
Me.lblFrom.Size = New System.Drawing.Size(31, 16)
Me.lblFrom.TabIndex = 8
Me.lblFrom.Text = "From"
Me.lblFrom.Visible = False
'
'txtTo
'
Me.txtTo.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
Me.txtTo.Location = New System.Drawing.Point(152, 88)
Me.txtTo.Name = "txtTo"
Me.txtTo.Size = New System.Drawing.Size(32, 20)
Me.txtTo.TabIndex = 7
Me.txtTo.Text = ""
Me.txtTo.Visible = False
'
'txtFrom
'
Me.txtFrom.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
Me.txtFrom.Location = New System.Drawing.Point(96, 88)
Me.txtFrom.Name = "txtFrom"
Me.txtFrom.Size = New System.Drawing.Size(32, 20)
Me.txtFrom.TabIndex = 6
Me.txtFrom.Text = ""
Me.txtFrom.Visible = False
'
'cboQuarter
'
Me.cboQuarter.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
Me.cboQuarter.Location = New System.Drawing.Point(88, 48)
Me.cboQuarter.Name = "cboQuarter"
Me.cboQuarter.Size = New System.Drawing.Size(96, 21)
Me.cboQuarter.TabIndex = 5
Me.cboQuarter.Text = "ComboBox2"
Me.cboQuarter.Visible = False
'
'cboMonth
'
Me.cboMonth.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
Me.cboMonth.Location = New System.Drawing.Point(88, 16)
Me.cboMonth.Name = "cboMonth"
Me.cboMonth.Size = New System.Drawing.Size(96, 21)
Me.cboMonth.TabIndex = 4
Me.cboMonth.Text = "ComboBox1"
'
'radWithinAYear
'
Me.radWithinAYear.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
Me.radWithinAYear.Location = New System.Drawing.Point(8, 88)
Me.radWithinAYear.Name = "radWithinAYear"
Me.radWithinAYear.Size = New System.Drawing.Size(88, 16)
Me.radWithinAYear.TabIndex = 3
Me.radWithinAYear.Text = "Within Year"
'
'radAnnual
'
Me.radAnnual.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
Me.radAnnual.Location = New System.Drawing.Point(8, 72)
Me.radAnnual.Name = "radAnnual"
Me.radAnnual.Size = New System.Drawing.Size(64, 16)
Me.radAnnual.TabIndex = 2
Me.radAnnual.Text = "Annual"
'
'radQuarter
'
Me.radQuarter.Location = New System.Drawing.Point(8, 48)
Me.radQuarter.Name = "radQuarter"
Me.radQuarter.Size = New System.Drawing.Size(64, 16)
Me.radQuarter.TabIndex = 1
Me.radQuarter.Text = "Quarter"
'
'radMonth
'
Me.radMonth.Checked = True
Me.radMonth.Location = New System.Drawing.Point(8, 16)
Me.radMonth.Name = "radMonth"
Me.radMonth.Size = New System.Drawing.Size(61, 16)
Me.radMonth.TabIndex = 0
Me.radMonth.TabStop = True
Me.radMonth.Text = "Month"
'
'lstAvailableWells
'
Me.lstAvailableWells.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
Me.lstAvailableWells.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
Me.lstAvailableWells.ItemHeight = 14
Me.lstAvailableWells.Location = New System.Drawing.Point(0, 168)
Me.lstAvailableWells.Name = "lstAvailableWells"
Me.lstAvailableWells.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
Me.lstAvailableWells.Size = New System.Drawing.Size(184, 172)
Me.lstAvailableWells.Sorted = True
Me.lstAvailableWells.TabIndex = 3
'
'lstSelectedWells
'
Me.lstSelectedWells.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
Me.lstSelectedWells.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
Me.lstSelectedWells.ItemHeight = 14
Me.lstSelectedWells.Location = New System.Drawing.Point(288, 168)
Me.lstSelectedWells.Name = "lstSelectedWells"
Me.lstSelectedWells.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
Me.lstSelectedWells.Size = New System.Drawing.Size(192, 172)
Me.lstSelectedWells.Sorted = True
Me.lstSelectedWells.TabIndex = 4
'
'btnDefineRange
'
Me.btnDefineRange.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
Me.btnDefineRange.Location = New System.Drawing.Point(192, 272)
Me.btnDefineRange.Name = "btnDefineRange"
Me.btnDefineRange.Size = New System.Drawing.Size(88, 23)
Me.btnDefineRange.TabIndex = 5
Me.btnDefineRange.Text = "Define Range"
'
'btnRemoveWells
'
Me.btnRemoveWells.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
Me.btnRemoveWells.Enabled = False
Me.btnRemoveWells.Location = New System.Drawing.Point(192, 240)
Me.btnRemoveWells.Name = "btnRemoveWells"
Me.btnRemoveWells.Size = New System.Drawing.Size(88, 23)
Me.btnRemoveWells.TabIndex = 6
Me.btnRemoveWells.Text = "<<"
'
'btnAddWells
'
Me.btnAddWells.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
Me.btnAddWells.Location = New System.Drawing.Point(192, 208)
Me.btnAddWells.Name = "btnAddWells"
Me.btnAddWells.Size = New System.Drawing.Size(88, 23)
Me.btnAddWells.TabIndex = 7
Me.btnAddWells.Text = ">>"
'
'lblAvailableCount
'
Me.lblAvailableCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
Me.lblAvailableCount.Location = New System.Drawing.Point(40, 344)
Me.lblAvailableCount.Name = "lblAvailableCount"
Me.lblAvailableCount.Size = New System.Drawing.Size(100, 16)
Me.lblAvailableCount.TabIndex = 8
'
'lblSelectedCount
'
Me.lblSelectedCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
Me.lblSelectedCount.Location = New System.Drawing.Point(328, 344)
Me.lblSelectedCount.Name = "lblSelectedCount"
Me.lblSelectedCount.Size = New System.Drawing.Size(100, 16)
Me.lblSelectedCount.TabIndex = 9
'
'btnOK
'
Me.btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
Me.btnOK.Location = New System.Drawing.Point(480, 336)
Me.btnOK.Name = "btnOK"
Me.btnOK.TabIndex = 10
Me.btnOK.Text = "OK"
'
'btnCancel
'
Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
Me.btnCancel.Enabled = False
Me.btnCancel.Location = New System.Drawing.Point(560, 336)
Me.btnCancel.Name = "btnCancel"
Me.btnCancel.TabIndex = 11
Me.btnCancel.Text = "Cancel"
'
'gbOutput
'
Me.gbOutput.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
Me.gbOutput.Controls.Add(Me.radFile)
Me.gbOutput.Controls.Add(Me.radPrinter)
Me.gbOutput.Controls.Add(Me.radScreen)
Me.gbOutput.Location = New System.Drawing.Point(488, 176)
Me.gbOutput.Name = "gbOutput"
Me.gbOutput.Size = New System.Drawing.Size(136, 100)
Me.gbOutput.TabIndex = 12
Me.gbOutput.TabStop = False
Me.gbOutput.Text = "Output Options"
'
'radFile
'
Me.radFile.Location = New System.Drawing.Point(8, 40)
Me.radFile.Name = "radFile"
Me.radFile.Size = New System.Drawing.Size(48, 16)
Me.radFile.TabIndex = 2
Me.radFile.Text = "File"
'
'radPrinter
'
Me.radPrinter.Location = New System.Drawing.Point(8, 64)
Me.radPrinter.Name = "radPrinter"
Me.radPrinter.Size = New System.Drawing.Size(56, 16)
Me.radPrinter.TabIndex = 1
Me.radPrinter.Text = "Printer"
'
'radScreen
'
Me.radScreen.Checked = True
Me.radScreen.Location = New System.Drawing.Point(8, 16)
Me.radScreen.Name = "radScreen"
Me.radScreen.Size = New System.Drawing.Size(64, 16)
Me.radScreen.TabIndex = 0
Me.radScreen.TabStop = True
Me.radScreen.Text = "Screen"
'
'MainMenu1
'
Me.MainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuFile, Me.mnuHelp})
'
'mnuFile
'
Me.mnuFile.Index = 0
Me.mnuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuExit})
Me.mnuFile.Text = "File"
'
'mnuHelp
'
Me.mnuHelp.Index = 1
Me.mnuHelp.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuAbout})
Me.mnuHelp.Text = "Help"
'
'mnuAbout
'
Me.mnuAbout.Index = 0
Me.mnuAbout.Shortcut = System.Windows.Forms.Shortcut.CtrlB
Me.mnuAbout.Text = "About"
'
'mnuExit
'
Me.mnuExit.Index = 0
Me.mnuExit.Shortcut = System.Windows.Forms.Shortcut.CtrlQ
Me.mnuExit.Text = "Exit"
'
'frmCompareWells
'
Me.AcceptButton = Me.btnOK
Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
Me.CancelButton = Me.btnCancel
Me.ClientSize = New System.Drawing.Size(640, 397)
Me.Controls.Add(Me.gbOutput)
Me.Controls.Add(Me.btnCancel)
Me.Controls.Add(Me.btnOK)
Me.Controls.Add(Me.lblSelectedCount)
Me.Controls.Add(Me.lblAvailableCount)
Me.Controls.Add(Me.btnAddWells)
Me.Controls.Add(Me.btnRemoveWells)
Me.Controls.Add(Me.btnDefineRange)
Me.Controls.Add(Me.lstSelectedWells)
Me.Controls.Add(Me.lstAvailableWells)
Me.Controls.Add(Me.Panel1)
Me.Controls.Add(Me.lblInstructions)
Me.Controls.Add(Me.sbInfo)
Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
Me.Menu = Me.MainMenu1
Me.Name = "frmCompareWells"
Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
Me.Text = "Compare Well Data"
Me.Panel1.ResumeLayout(False)
Me.gbMatchingRecords.ResumeLayout(False)
Me.gbPeriod.ResumeLayout(False)
Me.gbOutput.ResumeLayout(False)
Me.ResumeLayout(False)

    End Sub

#End Region

'   alReport is the report that's printed to the screen, printer or file.
'   intNumLines is how many lines are in the array.
'   intCurrentLine is where we are in the printing.
'   intNumHeaderLines is how many lines from 0..intNumHeaderLines - 1 are taken up 
'   with the header for the report.
Private alReport As New ArrayList(500)
Private intNumLines As Integer
Private intCurrentLine As Integer
Private intNumHeaderLines As Integer

'   This holds any errors that happen while the program is running.  
'   If the program is called from the command line the contents of this
'   will be written to the file specified in strErrorFileName.
'   The array list is cleared in PrepareReport and written to each time an
'   error occurs.
Private alErrors As New ArrayList

'   strFilename is the name of the file we're going to save the data to, if any.
'   strDelimiter will be a tab, a comma or empty.
'   These are set in GetFilename.
Private strFilename As String
Private strDelimiter As String

'   This is true if we called the program from the command line.
Private CalledFromCommandLine As Boolean

'   These are the two file names that will be used if the program is called
'   from the command line.  If the program is called from the command line
'   it will delete a file called Flag.txt in the same directory as the
'   output file when the program is done.  The error flag will be
'   put out if the program exits with an error.
Private strFlagFileName As String
Private strErrorFileName As String
Private ErrorsOccurred As Boolean

'   This is True when the user clicks the Cancel button.
Private CancelPressed As Boolean

Private Sub btnAddWells_Click(ByVal sender As Object, _
ByVal e As System.EventArgs) Handles btnAddWells.Click
Dim NumWells As Integer
Dim LoopCount As Integer
Dim strDescription As String

    If lstAvailableWells.SelectedItems.Count = 0 Then
        '   Nothing selected
        Exit Sub
    End If

    NumWells = lstAvailableWells.SelectedItems.Count

    For LoopCount = NumWells - 1 To 0 Step -1
        strDescription = lstAvailableWells.SelectedItems(LoopCount)
        lstSelectedWells.Items.Add(strDescription)
        lstAvailableWells.Items.Remove(strDescription)
    Next

    If lstAvailableWells.Items.Count = 0 Then
        btnAddWells.Enabled = False
    Else
        btnAddWells.Enabled = True
    End If

    If lstSelectedWells.Items.Count = 0 Then
        btnRemoveWells.Enabled = False
    Else
        btnRemoveWells.Enabled = True
    End If

    Call UpdateCounts()

End Sub     '   btnAddWells_Click

Private Sub btnCancel_Click(ByVal sender As Object, _
ByVal e As System.EventArgs) Handles btnCancel.Click

    CancelPressed = True
    Me.Cursor = Cursors.Default
    EnableForm(True)

    If CalledFromCommandLine Then
        Me.Close()
    End If

End Sub     '   btnCancel_Click

Private Sub btnDefineRange_Click(ByVal sender As Object, _
ByVal e As System.EventArgs) Handles btnDefineRange.Click
Dim frmRange As New frmDefineRange

    AddHandler frmRange.RangeChosen, AddressOf RangeDefined

    frmRange.ShowDialog()


End Sub     '   btnDefineRange_Click

Private Sub btnOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOK.Click

    If radMonth.Checked And cboMonth.Text = vbNullString Then
        cboMonth.SelectedIndex = 0
    ElseIf radQuarter.Checked And cboQuarter.Text = vbNullString Then
        cboQuarter.SelectedIndex = 0
    ElseIf radWithinAYear.Checked Then
        If txtFrom.Text = vbNullString Then
            txtFrom.Text = "1"
        End If
        If txtTo.Text = vbNullString Then
            txtTo.Text = "12"
        End If
    End If

    EnableForm(False)

    PrepareReport()

End Sub     '   btnOK_Click

Private Sub btnRemoveWells_Click(ByVal sender As Object, _
ByVal e As System.EventArgs) Handles btnRemoveWells.Click
'   Go through the list of selected wells and for each well that has been selected,
'   add it to the list of available wells.  Then remove it from the list of selected wells
'   so it won't be added a second time.
Dim NumWells As Integer
Dim LoopCount As Integer
Dim strDescription As String

    If lstSelectedWells.SelectedItems.Count = 0 Then
        '   Nothing selected
        Exit Sub
    End If

    NumWells = lstSelectedWells.SelectedItems.Count

    For LoopCount = NumWells - 1 To 0 Step -1

        strDescription = lstSelectedWells.SelectedItems(LoopCount)
        lstAvailableWells.Items.Add(strDescription)
        lstSelectedWells.Items.Remove(strDescription)


    Next LoopCount

    If lstAvailableWells.Items.Count = 0 Then
        btnAddWells.Enabled = False
    Else
        btnAddWells.Enabled = True
    End If

    If lstSelectedWells.Items.Count = 0 Then
        btnRemoveWells.Enabled = False
    Else
        btnRemoveWells.Enabled = True
    End If

    Call UpdateCounts()

End Sub     '   btnRemoveWells_Click

Private Sub frmCompareWells_Closing(ByVal sender As Object, _
ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing

    '   If this is called from the command line, try deleting the flag file
    '   so the GIS system knows the program is done.  If we had errors, 
    '   write an entry to the error file.
    If CalledFromCommandLine Then

        If strErrorFileName <> vbNullString AndAlso alErrors.Count > 0 Then
            Try
                If System.IO.File.Exists(strErrorFileName) Then
                    System.IO.File.Delete(strErrorFileName)
                End If
                Dim FS As New System.IO.FileStream(strErrorFileName, IO.FileMode.Create)
                Dim Writer As New System.IO.StreamWriter(FS)
                For Each strLine As String In alErrors
                    Writer.WriteLine(strLine)
                Next
                Writer.Close()
                FS.Close()
            Catch ex As Exception
                MessageBox.Show(ex.ToString, "Error Writing Error File for GIS", _
                                    MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End If      '   strErrorFileName <> vbNullString

        If strFlagFileName <> vbNullString Then
            Try
                System.IO.File.Delete(strFlagFileName)
            Catch ex As Exception
                MessageBox.Show(ex.ToString, "Error Deleting the Flag File", _
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If      '   strFlagFileName <> vbNullString

    End If      '   CalledFromCommandLine


End Sub     '   frmCompareWells_Closing

Private Sub frmCompareWells_Load(ByVal sender As Object, _
ByVal e As System.EventArgs) Handles MyBase.Load

    PopulateLayout()
    PopulateQuarters()
    PopulateMonths()
    PopulateDates()

    Dim strCmd As String = Microsoft.VisualBasic.Command

    If strCmd <> vbNullString Then
        SetUpProgram(strCmd)
        btnOK_Click(sender, e)
    Else
        PopulateAvailableWells()
        UpdateCounts()
    End If


End Sub     '   frmCompareWells_Load

Private Sub mnuAbout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuAbout.Click
Dim frmA As New frmAbout("Compare Well Data")

    frmA.ShowCopyrightInfo = True
    frmA.ShowApplicationInfo = True
    frmA.ShowInTaskbar = False
    frmA.ShowDialog()

End Sub     '   mnuAbout_Click

Private Sub mnuExit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuExit.Click
    btnCancel_Click(sender, e)
    Me.Close()
End Sub     '   mnuExit_Click

Private Sub radAnnual_CheckedChanged(ByVal sender As Object, _
ByVal e As System.EventArgs) Handles radAnnual.CheckedChanged
    radAverageRecords.Checked = True
    radMatchSingle.Enabled = False
End Sub     '   radAnnual_CheckedChanged

Private Sub radMonth_CheckedChanged(ByVal sender As Object, _
ByVal e As System.EventArgs) Handles radMonth.CheckedChanged
    cboMonth.Visible = radMonth.Checked
    radMatchSingle.Enabled = True
End Sub     '   radMonth_CheckedChanged

Private Sub radQuarter_CheckedChanged(ByVal sender As Object, _
ByVal e As System.EventArgs) Handles radQuarter.CheckedChanged
    cboQuarter.Visible = radQuarter.Checked
    radMatchSingle.Enabled = True
End Sub     '   radQuarter_CheckedChanged

Private Sub radWithinAYear_CheckedChanged(ByVal sender As Object, _
ByVal e As System.EventArgs) Handles radWithinAYear.CheckedChanged
    lblFrom.Visible = radWithinAYear.Checked
    lblTo.Visible = radWithinAYear.Checked
    txtFrom.Visible = radWithinAYear.Checked
    txtTo.Visible = radWithinAYear.Checked
    cboEndYear.Visible = Not radWithinAYear.Checked
    lblEndYear.Visible = cboEndYear.Visible
    radMatchSingle.Enabled = True
End Sub     '   radWithinAYear_CheckedChanged

Private Sub Report_PrintPageBodyStart(ByVal sender As Object, _
ByVal e As ReportPageEventArgs) Handles Report.PrintPageBodyStart
Dim intLinesOnPage As Integer = intNumHeaderLines

    Do While intCurrentLine < intNumLines

        e.WriteLine(CType(alReport.Item(intCurrentLine), String))
        intCurrentLine += 1
        intLinesOnPage += 1

        If intLinesOnPage Mod LinesPerPage = 0 Then
            Exit Do
        End If

    Loop

    e.HasMorePages = (intCurrentLine < intNumLines)

End Sub     '   Report_PrintPageBodyStart

''' <summary>
''' Add the information from this well change object to the report.  Put it into the right
''' format as necessary, for delimited or regular formatting.
''' </summary>
Private Sub AddToReport(ByVal WC As clsWellChange)
Dim intLineWidth As Integer = arrLayout(ChangeCol).Start + arrLayout(ChangeCol).Width
Dim strLine As String

    If strDelimiter = vbNullString Then

        strLine = Space(intLineWidth)
        strLine = Misc.InsertIntoLine(strLine, WC.LegalDescription, arrLayout(LegalDescCol).Start)
        strLine = Misc.InsertIntoLine(strLine, WC.WellName, arrLayout(WellNameCol).Start)
        strLine = Misc.InsertRightJustified(strLine, WC.LatDecimal.ToString, _
                        arrLayout(LatDecimalCol).Start + arrLayout(LatDecimalCol).Width)
        strLine = Misc.InsertRightJustified(strLine, WC.LongDecimal.ToString, _
                        arrLayout(LongDecimalCol).Start + arrLayout(LongDecimalCol).Width)
        strLine = Misc.InsertRightJustified(strLine, FormatNumber(WC.StartReading, 2, TriState.True), _
                        arrLayout(StartReadingCol).Start + arrLayout(StartReadingCol).Width)
        strLine = Misc.InsertRightJustified(strLine, FormatNumber(WC.EndReading, 2, TriState.True), _
                        arrLayout(EndReadingCol).Start + arrLayout(EndReadingCol).Width)
        strLine = Misc.InsertRightJustified(strLine, FormatNumber(WC.ReadingChange, 2, TriState.True), _
                        arrLayout(ChangeCol).Start + arrLayout(ChangeCol).Width)

    Else

        With WC
            strLine = .WellId & strDelimiter & _
                        .LegalDescription & strDelimiter & _
                        .WellName & strDelimiter & _
                        .LatDecimal.ToString & strDelimiter & _
                        .LongDecimal.ToString & strDelimiter & _
                        .StartReading.ToString & strDelimiter & _
                        .EndReading.ToString & strDelimiter & _
                        .ReadingChange.ToString
        End With

    End If

    alReport.Add(strLine)
    intNumLines += 1

End Sub     '   AddToReport

''' <summary>
''' Create the header for the report based on strDelimiter.  If it's empty, create a regular
''' header suitable for printing.  If it's not, create the field names separated by strDelimiter.
''' Add the lines to alReport and set intNumHeaderLines and intNumLines.
''' </summary>
Private Sub CreateHeader()
Dim intLineWidth As Integer = arrLayout(ChangeCol).Start + arrLayout(ChangeCol).Width
Dim strLine As String

    alReport.Clear()
    intNumLines = 0

    If strDelimiter = vbNullString Then

        '   This is something that needs to be printed so make it suitable for printing.
        strLine = Space(intLineWidth)
        alReport.Add(Misc.InsertIntoLineCentered(strLine, "THE CENTRAL NEBRASKA PUBLIC POWER " & _
                                                "AND IRRIGATION DISTRICT"))
        alReport.Add(Misc.InsertIntoLineCentered(strLine, "WELL COMPARISON REPORT"))

        If radWithinAYear.Checked Then

            strLine = Microsoft.VisualBasic.Right("00" & txtFrom.Text, 2) & "/" & cboStartYear.Text & _
                        " through " & _
                    Microsoft.VisualBasic.Right("00" & txtTo.Text, 2) & "/" & cboStartYear.Text

        ElseIf radQuarter.Checked Then

            strLine = "Qtr " & CStr(cboQuarter.SelectedIndex + 1) & " " & cboStartYear.Text & _
                        " through " & _
                    "Qtr " & CStr(cboQuarter.SelectedIndex + 1) & " " & cboEndYear.Text

        ElseIf radAnnual.Checked Then

            strLine = cboStartYear.Text & " through " & cboEndYear.Text

        Else

            strLine = Microsoft.VisualBasic.Right("00" & cboMonth.Text, 2) & "/" & cboStartYear.Text & _
                        " through " & _
                    Microsoft.VisualBasic.Right("00" & cboMonth.Text, 2) & "/" & cboEndYear.Text

        End If

        alReport.Add(Misc.InsertIntoLineCentered(Space(intLineWidth), strLine))
        alReport.Add(Misc.InsertIntoLineCentered(Space(intLineWidth), Now.ToShortDateString))
        alReport.Add(vbNullString)

        strLine = Space(intLineWidth)
        strLine = Misc.InsertIntoLine(strLine, "Legal Description", arrLayout(LegalDescCol).Start)
        strLine = Misc.InsertIntoLine(strLine, "Well Name", arrLayout(WellNameCol).Start)
        strLine = Misc.InsertIntoLine(strLine, "Lat Decimal", _
                    arrLayout(LatDecimalCol).Start + arrLayout(LatDecimalCol).Width - Len("Lat Decimal"))
        strLine = Misc.InsertIntoLine(strLine, "Long Decimal", _
                    arrLayout(LongDecimalCol).Start + arrLayout(LongDecimalCol).Width - Len("Long Decimal"))

        If radWithinAYear.Checked Then

            strLine = Misc.InsertRightJustified(strLine, txtFrom.Text & "/" & cboStartYear.Text, _
                    arrLayout(StartReadingCol).Start + arrLayout(StartReadingCol).Width)
            strLine = Misc.InsertRightJustified(strLine, txtTo.Text & "/" & cboStartYear.Text, _
                    arrLayout(EndReadingCol).Start + arrLayout(EndReadingCol).Width)

        ElseIf radQuarter.Checked Then

            strLine = Misc.InsertRightJustified(strLine, "Qtr " & CStr(cboQuarter.SelectedIndex + 1) & _
                    " " & cboStartYear.Text, arrLayout(StartReadingCol).Start + arrLayout(StartReadingCol).Width)
            strLine = Misc.InsertRightJustified(strLine, "Qtr " & CStr(cboQuarter.SelectedIndex + 1) & _
                    " " & cboEndYear.Text, arrLayout(EndReadingCol).Start + arrLayout(EndReadingCol).Width)

        ElseIf radAnnual.Checked Then

            strLine = Misc.InsertRightJustified(strLine, cboStartYear.Text, _
                        arrLayout(StartReadingCol).Start + arrLayout(StartReadingCol).Width)
            strLine = Misc.InsertRightJustified(strLine, cboEndYear.Text, _
                        arrLayout(EndReadingCol).Start + arrLayout(EndReadingCol).Width)
        Else

            strLine = Misc.InsertRightJustified(strLine, cboMonth.Text & "/" & cboStartYear.Text, _
                        arrLayout(StartReadingCol).Start + arrLayout(StartReadingCol).Width)
            strLine = Misc.InsertRightJustified(strLine, cboMonth.Text & "/" & cboEndYear.Text, _
                        arrLayout(EndReadingCol).Start + arrLayout(EndReadingCol).Width)

        End If

        strLine = Misc.InsertRightJustified(strLine, "Change", _
                    arrLayout(ChangeCol).Start + arrLayout(ChangeCol).Width)

        alReport.Add(strLine)
        alReport.Add(Replace(Space(intLineWidth), " ", "="))
        intNumHeaderLines = 7

    Else

        strLine = "WellId" & strDelimiter & _
                "Legal" & strDelimiter & _
                "Name" & strDelimiter & _
                "LatDec" & strDelimiter & _
                "LongDec" & strDelimiter & _
                "Start" & strDelimiter & _
                "End" & strDelimiter & _
                "Change"
        alReport.Add(strLine)
        intNumHeaderLines = 1

    End If

    intNumLines = intNumHeaderLines

End Sub     '   CreateHeader

''' <summary>
''' Given all the available fields, create a string that contains the description with
''' descriptors ("T", "R", etc).  The names have to be taken apart later so making these
''' standard will make life easier later.
''' </summary>
Private Function CreateLegalDesc(ByVal Township As Integer, ByVal NS As String, _
    ByVal Range As Integer, ByVal EW As String, ByVal Section As Integer, _
    ByVal Subsection As String, ByVal SeqNo As Integer) As String
Dim ReturnStr As String

    ReturnStr = vbNullString

    '   If you change how this is done, be sure to change the PrepareReport sub,
    '   DescriptionIsInRange function, GetWellKey function
    '   and the MoveWells sub because they assume this format.
    ReturnStr = "T" & Microsoft.VisualBasic.Right("00" & Township.ToString, 2) & _
                NS & _
                " R" & Microsoft.VisualBasic.Right("00" & Range.ToString, 2) & _
                EW & _
                " " & Microsoft.VisualBasic.Right("00" & Section.ToString, 2) & _
                " " & Microsoft.VisualBasic.Right(Space(4) & Subsection, 4) & _
                " " & Microsoft.VisualBasic.Right(Space(5) & SeqNo.ToString, 5)

    CreateLegalDesc = ReturnStr

End Function        '   CreateLegalDesc

Private Function DescriptionIsInRange(ByVal CurrDesc As String, ByVal StartDesc As WellRangeType, _
    ByVal EndDesc As WellRangeType) As Boolean
'   The user wants to define a range of legal descriptions to be included in the report.
'   This is being used by the sub MoveWells to determine if the legal description
'   in CurrDesc is within the range of StartDesc and EndDesc.  The format of CurrDesc is
'   T00N R00W 00 AAAA 00000 and StartDesc and EndDesc only have information in the
'   Township, Range and Section fields.
Dim TS As String
Dim RA As String
Dim SE As String

    If CurrDesc = vbNullString Then
        Return False
    End If

    TS = Mid(CurrDesc, 2, 2)
    RA = Mid(CurrDesc, 7, 2)
    SE = Mid(CurrDesc, 11, 2)

    DescriptionIsInRange = False

    If CInt(TS) >= StartDesc.Township And CInt(TS) <= EndDesc.Township Then
        If CInt(RA) >= StartDesc.Range And CInt(RA) <= EndDesc.Range Then
            If CInt(SE) >= StartDesc.Section And CInt(SE) <= EndDesc.Section Then
                DescriptionIsInRange = True
            End If
        End If
    End If

End Function        '   DescriptionIsInRange

''' <summary>
''' A value has been passed into the program on the command line and we want to
''' find the selected index of the combo box for the value.
''' <param name = "cboBox"> cboBox is the box to search in. </param>
''' <param name = "strToFind"> strToFind is the string to find.  It is case sensitive. </param>
''' <returns> Returns the index if strToFind is found. </returns>
''' <returns> Returns -1 if nothing is found. </returns>
''' </summary>
Private Function FindIndex(ByVal cboBox As ComboBox, ByVal strToFind As String) As Integer

    For intCount As Integer = 0 To cboBox.Items.Count - 1
        If cboBox.Items(intCount) = strToFind Then
            Return intCount
        End If
    Next

    Return -1

End Function        '   FindIndex

''' <summary>
''' The OK or the Cancel button has been clicked so we want to enable/disable all
''' of the controls so the user can't change them while we're doing the report.
''' <param name = "Enable"> Enable is True to enable all of the controls or False to
'''                         disable them all. </param>
''' </summary>
Private Sub EnableForm(ByVal Enable As Boolean)

    gbPeriod.Enabled = Enable
    Panel1.Enabled = Enable
    lstAvailableWells.Enabled = Enable
    lstSelectedWells.Enabled = Enable

    btnDefineRange.Enabled = Enable
    gbOutput.Enabled = Enable

    If Enable Then
        btnOK.Enabled = True
        btnCancel.Enabled = False
        btnAddWells.Enabled = (lstAvailableWells.Items.Count > 0)
        btnRemoveWells.Enabled = (lstSelectedWells.Items.Count > 0)
    Else
        btnCancel.Enabled = True
        btnOK.Enabled = False
        btnAddWells.Enabled = False
        btnRemoveWells.Enabled = False
    End If

End Sub     '   EnableForm

''' <summary>
''' Ask the user for a file to save the data to and set strFilename and strDelimiter
''' to the proper values.  If the file exists, ask the user for permission to overwrite it.
''' </summary>
Private Sub GetFilename()
Dim SaveDialog As New SaveFileDialog

    strFilename = vbNullString
    strDelimiter = vbNullString

    With SaveDialog
        .CheckPathExists = True
        .Filter = "Text Files (*.txt)|*.txt|" & _
                    "Tab-Delimited Files (*.txt)|*.txt|" & _
                    "Comma-Separated Values (*.csv)|*.csv|" & _
                    "All Files (*.*)|*.*"
        .OverwritePrompt = True
        .Title = "Save Well Change Data As..."
        .ShowDialog()
    End With

    If SaveDialog.FileName <> vbNullString Then
        strFilename = SaveDialog.FileName
        Select Case SaveDialog.FilterIndex
            Case 2 : strDelimiter = vbTab
            Case 3 : strDelimiter = ","
            Case Else : strDelimiter = vbNullString
        End Select
    End If

End Sub     '   GetFilename

''' <summary>
''' Pull the well key data out of the Legal Desc so that it can be used to load
''' a well for a clsWellChange object.  This assumes that LegalDesc is in the form
''' that was created from CreateLegalDesc and looks to extract the relevant information
''' out of this string.
''' </summary>
Private Function GetWellKey(ByVal LegalDesc As String) As CNPPID.PLSS
Dim intTownship As Integer
Dim intRange As Integer
Dim intSection As Integer
Dim strSubsection As String
Dim intSeqNo As Integer
Dim objReturn As CNPPID.PLSS

    If LegalDesc = vbNullString Then
        Return Nothing
    End If

    Try

        intTownship = CInt(Mid(LegalDesc, 2, 2))
        intRange = CInt(Mid(LegalDesc, 7, 2))
        intSection = CInt(Mid(LegalDesc, 11, 2))
        strSubsection = Mid(LegalDesc, 14, 4).Trim
        intSeqNo = CInt(Microsoft.VisualBasic.Right(LegalDesc, Len(LegalDesc) - 18))

        objReturn = New CNPPID.PLSS(intTownship, intRange, intSection, strSubsection, intSeqNo)

    Catch ex As Exception

        objReturn = Nothing
        GiveError(ex.ToString, "Error Retrieving Legal Description in GetWellKey")

    End Try

    Return objReturn

End Function        '   GetWellKey

''' <summary>
''' Give an error to the user.  If the program was called from the command line
''' then the message will be added to alErrors.
''' <param namae = "strMsg"> strMsg is what we're going to tell the user. </param>
''' <param name = "strTitle"> strTitle will be the title of the message box. </param>
''' </summary>
Private Sub GiveError(ByVal strMsg As String, ByVal strTitle As String)

    If CalledFromCommandLine Then
        alErrors.Add(strMsg)
        alErrors.Add(vbNullString)
    End If

    MessageBox.Show(strMsg, strTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)

End Sub     '   GiveError

''' <summary>
''' Get a list of the legal descriptions of the wells from the WellHeader table
''' and fill the lstAvailableWells list box.  Each legal description will be formatted.
''' The Sorted property of the list box should be set to True.
''' </summary>
Private Sub PopulateAvailableWells()
Dim Conn As SqlConnection
Dim dbCmd As SqlCommand
Dim Rdr As SqlDataReader
Dim strDescription As String

    Try

        Conn = New SqlConnection(Data.GetObsWells)
        Dim strQuery As String = "EXEC usp_ListLegalDescriptions"
        dbCmd = New SqlCommand(strQuery, Conn)

        dbCmd.Connection.Open()
        Rdr = dbCmd.ExecuteReader

        lstAvailableWells.Items.Clear()
        lstAvailableWells.BeginUpdate()

        If Rdr.HasRows Then

            '   None of the fields should be NULL because they are not allowed to
            '   be in the database.  They originally were the key to the table.
            Do While Rdr.Read

                strDescription = CreateLegalDesc(Rdr.GetInt32(0), Rdr.GetString(1).Trim, _
                                                Rdr.GetInt32(2), Rdr.GetString(3).Trim, _
                                                Rdr.GetInt32(4), Rdr.GetString(5).Trim, _
                                                Rdr.GetInt32(6))

                If strDescription <> vbNullString Then
                    lstAvailableWells.Items.Add(strDescription)
                End If

            Loop

        End If

        Rdr.Close()
        dbCmd.Connection.Close()

    Catch ex As Exception

        GiveError(ex.ToString, "Error Getting the Available Wells")

    Finally

        If Not Rdr Is Nothing AndAlso Not Rdr.IsClosed Then
            Rdr.Close()
        End If
        If Not Conn Is Nothing AndAlso Conn.State = ConnectionState.Open Then
            Conn.Close()
        End If

        lstAvailableWells.EndUpdate()

    End Try


End Sub     '   PopulateAvailableWells

''' <summary>
''' Fill cboBox with the integers from intStart to intEnd, inclusive.
''' <param name = "cboBox"> cboBox is the combo box to fill. </param>
''' <param name = "intStart"> intStart is the lowest number. </param>
''' <param name = "intEnd"> intEnd is the highest number. </param>
''' The text of cboBox is cleared and the SelectedIndex is set to 0.
''' </summary>
Private Sub PopulateBox(ByVal cboBox As ComboBox, ByVal intStart As Integer, ByVal intEnd As Integer)

    With cboBox

        .Items.Clear()
        For intCount As Integer = intStart To intEnd
            cboBox.Items.Add(intCount.ToString)
        Next

        .Text = vbNullString
        If .Items.Count > 0 Then
            .SelectedIndex = 0
        End If

    End With
End Sub     '   PopulateBox

''' <summary>
''' Get the start and end dates from the ObsWells.WellData table for all of the
''' wells and populate the years boxes with every year from the start date
''' to the end date.
''' </summary>
Private Sub PopulateDates()
Dim intStart As Integer
Dim intEnd As Integer
Dim Conn As SqlConnection
Dim dbCmd As SqlCommand
Dim Rdr As SqlDataReader

    Try

        Conn = New SqlConnection(Data.GetObsWells)
        Dim strQuery As String = "SELECT Top 1 Year(MeasureDate) AS StartYear " & _
                                "FROM WellData " & _
                                "ORDER BY MeasureDate ASC"
        dbCmd = New SqlCommand(strQuery, Conn)

        dbCmd.Connection.Open()
        Rdr = dbCmd.ExecuteReader

        If Rdr.HasRows Then
            Rdr.Read()
            If Not Rdr.IsDBNull(0) Then
                intStart = Rdr.GetInt32(0)
            End If
        End If

        Rdr.Close()
        dbCmd.CommandText = "SELECT Top 1 Year(MeasureDate) AS EndYear " & _
                            "FROM WellData " & _
                            "ORDER BY MeasureDate DESC"

        Rdr = dbCmd.ExecuteReader
        If Rdr.HasRows Then
            Rdr.Read()
            If Not Rdr.IsDBNull(0) Then
                intEnd = Rdr.GetInt32(0)
            End If
        End If

        Rdr.Close()
        dbCmd.Connection.Close()

    Catch ex As Exception

        GiveError(ex.ToString, "Error in frmCompareWells.PopulateDates")

    Finally

        If Not Rdr Is Nothing AndAlso Not Rdr.IsClosed Then
            Rdr.Close()
        End If

        If Not Conn Is Nothing AndAlso Conn.State = ConnectionState.Open Then
            Conn.Close()
        End If

    End Try

    PopulateBox(cboStartYear, intStart, intEnd)
    PopulateBox(cboEndYear, intStart, intEnd)

End Sub     '   PopulateDates

Private Sub PopulateLayout()
'   Field Name      Start   Width   Justification
'   ============    =====   =====   =============
'   Legal Desc      3       25      left
'   Well Name       30      50      left
'   Lat Decimal     80      12      right
'   Long Decimal    103     12      right
'   Start Year      119     10      right
'   End Year        134     10      right
'   Change          148     10      right

    With arrLayout(LegalDescCol)
        .Start = 3
        .Width = 25
    End With

    With arrLayout(WellNameCol)
        .Start = 30
        .Width = 50
    End With

    With arrLayout(LatDecimalCol)
        .Start = 80
        .Width = 12
    End With

    With arrLayout(LongDecimalCol)
        .Start = 103
        .Width = 12
    End With

    With arrLayout(StartReadingCol)
        .Start = 119
        .Width = 10
    End With

    With arrLayout(EndReadingCol)
        .Start = 134
        .Width = 10
    End With

    With arrLayout(ChangeCol)
        .Start = 148
        .Width = 10
    End With

End Sub

Private Sub PopulateMonths()

    PopulateBox(cboMonth, 1, 12)

End Sub     '   PopulateMonths

Private Sub PopulateQuarters()

    With cboQuarter
        .Items.Clear()
        .Items.Add(Quarters(0))
        .Items.Add(Quarters(1))
        .Items.Add(Quarters(2))
        .Items.Add(Quarters(3))
        .Text = vbNullString
        .SelectedIndex = 0
    End With

End Sub     '   PopulateQuarters

''' <summary>
''' The user has clicked the OK button so we're going to get a file name if necessary and
''' create a header, then go through all of the wells in lstSelectedWells and gather the 
''' changes for them, then put them into the report.
''' </summary>
Private Sub PrepareReport()
Dim intWellsAdded As Integer

    alReport.Clear()
    intNumLines = 0
    CancelPressed = False
    alErrors.Clear()

    If radFile.Checked AndAlso Not CalledFromCommandLine Then

        GetFilename()
        If strFilename = vbNullString Then
            btnCancel_Click(Nothing, Nothing)
            Exit Sub
        End If

        Me.Refresh()

    End If

    Me.Cursor = Cursors.WaitCursor

    CreateHeader()

    '   Go through each well in lstSelectedWells and create a new clsWellChange object
    '   to make the calculations.
    Dim WellKey As CNPPID.PLSS
    Dim WellChange As clsWellChange
    Dim Interval As clsWellChange.IntervalEnum
    Dim ReadValue As Integer
    Dim dtStartDate As DateTime
    Dim dtEndDate As DateTime

    '   Let's get the parameters for the well change object set up so that
    '   we don't have to do this every time through the loop.
    If radMonth.Checked Then

        Interval = clsWellChange.IntervalEnum.Monthly
        dtStartDate = New DateTime(CInt(cboStartYear.Text), cboMonth.SelectedIndex + 1, 1)
        dtEndDate = New DateTime(CInt(cboEndYear.Text), cboMonth.SelectedIndex + 1, 1)
        ReadValue = cboMonth.SelectedIndex + 1

    ElseIf radQuarter.Checked Then

        Interval = clsWellChange.IntervalEnum.Quarterly
        dtStartDate = New DateTime(CInt(cboStartYear.Text), 1, 1)
        dtEndDate = New DateTime(CInt(cboEndYear.Text), 1, 1)
        ReadValue = cboQuarter.SelectedIndex + 1

    ElseIf radAnnual.Checked Then

        Interval = clsWellChange.IntervalEnum.Annually
        dtStartDate = New DateTime(CInt(cboStartYear.Text), 1, 1)
        dtEndDate = New DateTime(CInt(cboEndYear.Text), 1, 1)

    Else

        Interval = clsWellChange.IntervalEnum.WithinAYear
        dtStartDate = New DateTime(CInt(cboStartYear.Text), CInt(txtFrom.Text), 1)
        dtEndDate = New DateTime(CInt(cboStartYear.Text), CInt(txtTo.Text), 1)

    End If

    For intCount As Integer = 0 To lstSelectedWells.Items.Count - 1

        sbInfo.Text = "Calculating changes for the well " & lstSelectedWells.Items(intcount) & "..."

        WellKey = GetWellKey(lstSelectedWells.Items(intCount))
        If Not WellKey Is Nothing Then

            WellChange = New clsWellChange(WellKey)
            With WellChange
                .StartDate = dtStartDate
                .EndDate = dtEndDate
                .ReadInterval = Interval
                .ReadValue = ReadValue
            End With

            AddHandler WellChange.ErrorOccurred, AddressOf WellChange_ErrorOccurred

            WellChange.GatherReadings()

            If WellChange.StartReading > 0 And WellChange.EndReading > 0 Then
                intWellsAdded += 1
                AddToReport(WellChange)
            End If

            Application.DoEvents()
            If CancelPressed Then
                Exit For
            End If

        Else

            MessageBox.Show("I could not retrieve the key for the well " & _
                        lstSelectedWells.Items(intCount) & " so I am exiting the data " & _
                        "gathering.", "Oops", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit For

        End If
    Next

    sbInfo.Text = vbNullString

    If Not CancelPressed AndAlso intWellsAdded > 0 Then

        If radFile.Checked Then
            PrintToFile()
        Else
            '   Set up the preview
            Report = New ReportDocument
            Report.ShowHeaderLine = False
            Report.DefaultPageSettings.Landscape = True
            Report.TopMargin = 1
            Report.LeftMargin = 0
            Report.RightMargin = 0
            Report.BottomMargin = 1
            Report.Font = New Font("Courier New", 8)

            '   Create the title
            Dim strTitle As String = alReport.Item(0)
            For intCount As Integer = 1 To intNumHeaderLines - 1
                strTitle &= vbCrLf & alReport.Item(intcount)
            Next
            Report.Title = strTitle
            intCurrentLine = intNumHeaderLines

            Dim dlg As New PrintPreviewDialog
            dlg.Document = Report
            dlg.WindowState = FormWindowState.Maximized
            dlg.ShowDialog()

        End If      '   radFile.Checked

    End If      '   Not CancelPressed

    If Not CalledFromCommandLine Then

        Dim strMsg As String

        If intWellsAdded = 0 Then
            strMsg = "I could not prepare the report because I could not find any data."
        ElseIf intWellsAdded = 1 Then
            strMsg = "I added one well to the report."
        Else
            strMsg = "I added " & intWellsAdded.ToString & " wells to the report."
        End If

        MessageBox.Show(strMsg, "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information)

    End If      '   Not CalledFromCommandLine

    btnCancel_Click(Nothing, Nothing)

End Sub     '   PrepareReport

''' <summary>
''' The user wants to print the output to a file.
''' strFilename has the name of the file to print to.  The output is in alReport and
''' has already been formatted.
''' </summary>
Private Sub PrintToFile()

    Try

        Dim FS As New System.IO.FileStream(strFilename, IO.FileMode.Create)
        Dim Writer As New System.IO.StreamWriter(FS)

        For Each strLine As String In alReport
            Writer.WriteLine(strLine)
        Next

        Writer.Close()
        FS.Close()

    Catch ex As Exception

        GiveError(ex.ToString, "Error Writing Output To a File")

    End Try

End Sub     '   PrintToFile

''' <summary>
''' The range has been defined by the user so we're going to go through the list 
''' of available wells and for each one that's within that range, add it to 
''' the list of selected wells.
''' </summary>
Private Sub RangeDefined(ByVal intTStart As Integer, ByVal intRStart As Integer, ByVal intSStart As Integer, _
ByVal intTEnd As Integer, ByVal intREnd As Integer, ByVal intSEnd As Integer)
Dim RangeStart As WellRangeType
Dim RangeEnd As WellRangeType

    With RangeStart
        .Township = intTStart
        .Range = intRStart
        .Section = intSStart
    End With

    With RangeEnd
        .Township = intTEnd
        .Range = intREnd
        .Section = intSEnd
    End With

    For intCount As Integer = lstAvailableWells.Items.Count - 1 To 0 Step -1
        If DescriptionIsInRange(lstAvailableWells.Items(intCount), RangeStart, RangeEnd) Then
            lstSelectedWells.Items.Add(lstAvailableWells.Items(intCount))
            lstAvailableWells.Items.RemoveAt(intCount)
        End If
    Next

    UpdateCounts()

End Sub     '   RangeDefined

''' <summary>
''' This program can be called by parameters.  Call it like this:
''' 
''' Period, Start Year, End Year, Match/Average, Output File Name, List...
''' 
''' where Period = Month=MM or      MM = 1 - 12
'''                 Quarter=Q or    Q = 1-4
'''                 Annual,
'''                 WithinAYear
''' StartYear = MM/YYYYY if WithinAYear, Start Year = YYYY otherwise,
''' StartYear= MM/YYYYY if WithinAYear, End Year = YYYY otherwise,
''' Match/Average = "Match" or "Average"
''' OutputFileName = a valid file name
''' List will be a series of TT RR SS SB SeqNo
''' 
''' The output from the program will be put into a tab-delimited file by the name of
''' OutputFileName.  The program will not show itself and will exit when done processing.
''' </summary>
Private Sub SetUpProgram(ByVal strCmdLine As String)
Dim strCurrField As String
Dim intLoopCount As Integer
Dim strParts() As String
Dim intNumWells As Integer
Dim strWell() As String

    Try

        strParts = Split(strCmdLine, ",")
        strCurrField = strParts(0)

        If LCase(Microsoft.VisualBasic.Left(strCurrField, 5)) = "month" Then

            radMonth.Checked = True
            strCurrField = Replace(Microsoft.VisualBasic.Right(strCurrField, 2), "=", vbNullString)
            cboMonth.SelectedIndex = CInt(strCurrField) - 1

        ElseIf LCase(Microsoft.VisualBasic.Left(strCurrField, 7)) = "quarter" Then

            radQuarter.Checked = True
            strCurrField = Quarters(CInt(Microsoft.VisualBasic.Right(strCurrField, 1)) - 1)
            cboQuarter.SelectedIndex = FindIndex(cboQuarter, strCurrField)

        ElseIf LCase(Microsoft.VisualBasic.Left(strCurrField, 6)) = "annual" Then
            radAnnual.Checked = True
        Else
            radWithinAYear.Checked = True
        End If

        If radWithinAYear.Checked Then
            txtFrom.Text = Microsoft.VisualBasic.Left(strParts(1), 2)
            txtTo.Text = Microsoft.VisualBasic.Left(strParts(2), 2)
            cboStartYear.SelectedIndex = FindIndex(cboStartYear, Microsoft.VisualBasic.Right(strParts(1), 4))
            cboEndYear.SelectedIndex = FindIndex(cboEndYear, Microsoft.VisualBasic.Right(strParts(2), 4))
        Else
            cboStartYear.SelectedIndex = FindIndex(cboStartYear, strParts(1).Trim)
            cboEndYear.SelectedIndex = FindIndex(cboEndYear, strParts(2).Trim)
        End If

        If LCase(strParts(3).Trim) = "match" And Not radAnnual.Checked Then
            radMatchSingle.Checked = True
        Else
            radAverageRecords.Checked = True
        End If

        '   Get the file name.  Create the name for the flag file and put it
        '   out there and create the name for the error log file if there were
        '   any errors.
        strFilename = strParts(4)
        strDelimiter = vbTab
        strFlagFileName = New System.IO.FileInfo(strFilename).DirectoryName

        If Not strFlagFileName.EndsWith("\") Then
            strFlagFileName = strFlagFileName & "\"
        End If

        strErrorFileName = strFlagFileName & "Error.txt"
        strFlagFileName = strFlagFileName & "Flag.txt"

        '   Since this is being called from the command line, we're going
        '   to send the output to a file.
        radFile.Checked = True

        '   The list of the wells from this point on will be a series like
        '   TT RR SS SB 00, TT RR SS SB 01, etc.  Since we have used the first four spaces
        '   in the array, we need to start the loop and go to the upper bound of the array.
        intNumWells = UBound(strParts)

        lstSelectedWells.Items.Clear()

        For intLoopCount = 5 To intNumWells

            strWell = Split(Trim(strParts(intLoopCount)), " ")

            strCurrField = CreateLegalDesc(CInt(strWell(0)), "N", CInt(strWell(1)), "W", _
                            CInt(strWell(2)), strWell(3), CInt(strWell(4)))

            'strCurrField = "T" & Format(strWell(0), "00") & "N " & _
            '                "R" & Format(strWell(1), "00") & "W " & _
            '                    Format(strWell(2), "00") & " " & _
            '                    strWell(3) & Space(8) & _
            '                    strWell(4)

            lstSelectedWells.Items.Add(strCurrField)

        Next intLoopCount

        CalledFromCommandLine = True

    Catch ex As Exception

        ErrorsOccurred = True

        GiveError(ex.ToString, "Error Setting Up the Program")
        Me.Close()

    End Try

End Sub     '   SetUpProgram

Private Sub UpdateCounts()
'   Update the list counts for the two boxes.

    If lstAvailableWells.Items.Count = 1 Then
        lblAvailableCount.Text = "1 Well"
    Else
        lblAvailableCount.Text = CStr(lstAvailableWells.Items.Count) & " Wells"
    End If
    If lstSelectedWells.Items.Count = 1 Then
        lblSelectedCount.Text = "1 Well"
    Else
        lblSelectedCount.Text = CStr(lstSelectedWells.Items.Count) & " Wells"
    End If

End Sub     '   UpdateCounts

''' <summary>
''' An error has occurred in a well change object so we're going to tell the
''' user about it.
''' </summary>
Private Sub WellChange_ErrorOccurred(ByVal strMsg As String)

    GiveError(strMsg, "Error from the Well Change Object")

End Sub     '   WellChange_ErrorOccurred









End Class       '   frmCompareWells

