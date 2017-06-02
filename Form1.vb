'
' @file Form1.vb
'
' An VB.net example using the FieldTalk Slave library in a Windows Forms project.
'
' @if NOTICE
'
' The following source file constitutes example program code and is
' intended merely to illustrate useful programming techniques.  The user
' is responsible for applying the code correctly.
'
' THIS SOFTWARE IS PROVIDED BY PROCONX AND CONTRIBUTORS ``AS IS'' AND ANY
' EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
' IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
' PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL PROCONX OR CONTRIBUTORS BE
' LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
' CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
' SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR
' BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
' WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
' OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
' ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
'
' @endif
'

Imports FieldTalk.Modbus.Slave


Public Class Form1
    ' Create an instance of the Modbus/TCP protocol
    Private mbusServer As MbusTcpSlaveProtocol = New MbusTcpSlaveProtocol()
    Private dataTable As New MyDatatable()

    Private Sub startButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles startButton.Click
        stopButton.Enabled = True
        startButton.Enabled = False
        BackgroundWorker1.RunWorkerAsync()
    End Sub

    Private Sub stopButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles stopButton.Click
        stopButton.Enabled = False
        startButton.Enabled = True
        BackgroundWorker1.CancelAsync()
    End Sub

    Private Sub backgroundWorker1_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim result As Integer

        '
        ' Starts up server
        '
        Me.BackgroundWorker1.ReportProgress(0, "Starting up Modbus Server...")
        result = mbusServer.addDataTable(1, dataTable) ' Unit ID is 1
        If result = BusProtocolErrors.FTALK_SUCCESS Then
            result = mbusServer.startupServer()
        End If
        If result <> BusProtocolErrors.FTALK_SUCCESS Then
            Me.BackgroundWorker1.ReportProgress(0, BusProtocolErrors.getBusProtocolErrorText(result))
            e.Result = BusProtocolErrors.getBusProtocolErrorText(result)
            Return
        End If
        Me.BackgroundWorker1.ReportProgress(0, "Modbus Server started on TCP interface.")

        '
        ' Run server
        '
        Do
            result = mbusServer.serverLoop()
            If result <> BusProtocolErrors.FTALK_SUCCESS Then
                Me.BackgroundWorker1.ReportProgress(0, BusProtocolErrors.getBusProtocolErrorText(result))
            Else
                Me.BackgroundWorker1.ReportProgress(100, "Running...")
            End If
        Loop While (Not Me.BackgroundWorker1.CancellationPending) AndAlso (result = BusProtocolErrors.FTALK_SUCCESS)

        '
        ' Shutdown server
        '
        Me.BackgroundWorker1.ReportProgress(0, "Shutting down Modbus Server...")
        If Me.BackgroundWorker1.CancellationPending Then
            e.Cancel = True
        End If
        mbusServer.shutdownServer()

        e.Result = BusProtocolErrors.getBusProtocolErrorText(result)
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        ToolStripStatusLabel1.Text = e.UserState.ToString()
        If e.ProgressPercentage < 100 Then
            ToolStripProgressBar1.Value = e.ProgressPercentage
            ToolStripProgressBar1.Style = ProgressBarStyle.Blocks
        Else
            ToolStripProgressBar1.Value = 100
            ToolStripProgressBar1.Style = ProgressBarStyle.Marquee
        End If
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        If e.Cancelled Then ' User pressed button
            ToolStripStatusLabel1.Text = "Modbus Server stopped by user."
        ElseIf e.Error IsNot Nothing Then ' An Exception occured
            ToolStripStatusLabel1.Text = e.Error.Message
        Else ' FieldTalk return code
            ToolStripStatusLabel1.Text = e.Result.ToString()
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        DataGridView1.DataSource = dataTable.getTable()
    End Sub
End Class
