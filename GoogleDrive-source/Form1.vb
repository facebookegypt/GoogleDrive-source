Public Class Form1
    Private Sub TextBox1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles TextBox1.MouseDoubleClick
        Dim OFD As New OpenFileDialog
        With OFD
            .Filter = ("Microsoft Access Database 2007/2010 (*.accdb)|*.accdb")
            If .ShowDialog = DialogResult.OK Then
                TextBox1.Text = .FileName
            End If
        End With
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim N As String = Main(TextBox1)
        'TextBox1.Text = Environment.
        If String.IsNullOrEmpty(TextBox1.Text) Then
            '           MsgBox("Pick File.")
            '          Exit Sub
        End If
        'CreateService()
        'UploadFile(TextBox1.Text)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
