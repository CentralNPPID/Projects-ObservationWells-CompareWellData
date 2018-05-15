Public Class frmAbout
    Inherits AboutBase.AboutBox

#Region " Windows Form Designer generated code "

Public Sub New(ByVal strName As String)
    MyBase.New(strName)
    InitializeComponent()
End Sub

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
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
Me.SuspendLayout()
'
'lblCustomInfo
'
Me.lblCustomInfo.Name = "lblCustomInfo"
'
'frmAbout
'
Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
Me.ClientSize = New System.Drawing.Size(482, 239)
Me.Name = "frmAbout"
Me.ShowApplicationInfo = True
Me.ShowCopyrightInfo = True
Me.ShowCustomInfo = True
Me.ShowOK = True
Me.ShowStatusInfo = True
Me.ResumeLayout(False)

    End Sub

#End Region

End Class
