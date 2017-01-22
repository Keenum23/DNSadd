Imports System.Net.NetworkInformation
Public Class Form1
    Private Data As New DataTable

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        With DataGridView1
            .ReadOnly = False
            .RowHeadersVisible = False
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToOrderColumns = False
            .DataSource = Data
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
        End With

        Data.Columns.Add("Server", GetType(String))
        Data.Columns.Add("IP", GetType(String))
        Data.Columns.Add("DNS Detected", GetType(String))

        LoadDataFrom("C:\test\servers.txt")
        DataGridView1.ClearSelection()

        For Each r As DataRow In Data.Rows
            Dim myPing As New Ping
            AddHandler myPing.PingCompleted, AddressOf PingRequestCompleted
            myPing.SendAsync(r.Item("Server").ToString, r)
        Next
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        For Each r As DataRow In Data.Rows
            Dim myPing As New Ping
            AddHandler myPing.PingCompleted, AddressOf PingRequestCompleted
            myPing.SendAsync(r.Item("Server").ToString, r)
        Next
    End Sub

    Private Sub LoadDataFrom(Filename As String)
        For Each line As String In IO.File.ReadLines(Filename)
            If line.Trim <> "" Then
                Dim NameAndIp() As String = line.Split(" "c)
                If NameAndIp.Length = 2 Then
                    Data.Rows.Add(New Object() {NameAndIp(0), NameAndIp(1), "-------"})
                End If
            End If
        Next
        DataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
    End Sub

    Public Sub PingRequestCompleted(sender As Object, e As PingCompletedEventArgs)
        Try
            CType(e.UserState, DataRow).Item("DNS Detected") = (e.Reply.Status = IPStatus.Success).ToString
            Dim p As Ping = CType(sender, Ping)
            RemoveHandler p.PingCompleted, AddressOf PingRequestCompleted
            p.Dispose()
        Catch ex As Exception
            CType(e.UserState, DataRow).Item("DNS Detected") = "Error"
            Debug.WriteLine(CType(e.UserState, DataRow).Item(0).ToString & "   " & ex.Message)
        End Try
    End Sub

    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridView1.CellFormatting
        If e.ColumnIndex = 2 Then
            Dim cclr As Color = Color.Black
            Select Case DataGridView1.Rows(e.RowIndex).Cells(2).Value.ToString
                Case "True"
                    cclr = Color.DarkGreen
                Case "False"
                    cclr = Color.Red
                Case "Error"
                    cclr = Color.Red
            End Select
            DataGridView1.Rows(e.RowIndex).DefaultCellStyle.ForeColor = cclr
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        Dim sb As New System.Text.StringBuilder 'create a StringBuilder to add all the lines for the CSV file to
        sb.AppendLine(Data.Columns(0).ColumnName & "," & Data.Columns(1).ColumnName)

        'iterate through all the DataRows of the DataTable that have the 3rd column set to "False" and add their data to the StringBuilder
        For Each dr As DataRow In (From r As DataRow In Data Where r.Item(2).ToString = "False" OrElse r.Item(2).ToString = "Error")
            'For Each dr As DataRow In (From r As DataRow In Data Where r.Item(2).ToString = "False")
            sb.AppendLine(dr.Item(0).ToString & "," & dr.Item(1).ToString) 'use whatever delimiter you want in place of the space " " in this line
        Next
        IO.File.WriteAllText("C:\test\failed.csv", sb.ToString) 'write the text in the StringBuilder to your file

        MessageBox.Show("Saved")
        'PowerShell ProcessStart will go here


    End Sub

End Class
