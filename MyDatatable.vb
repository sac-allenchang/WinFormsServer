'
' @file MyDatatable.vb
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

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Data
Imports FieldTalk.Modbus.Slave


'''
''' Data provider class
'''
Friend Class MyDatatable
    Inherits MbusDataTableInterface

    Public modbusData As New DataTable()

    Public Sub New()
        'modbusData.Columns.Add("Register", GetType(Integer))
        'modbusData.Columns.Add("Value", GetType(Int16))
        modbusData.Columns.Add("Address", GetType(Integer))     ' Test
        modbusData.Columns.Add("Register", GetType(Int16))      ' Test
        modbusData.Columns.Add("Coil", GetType(Boolean))        ' Test
        For i As Integer = 0 To 999
            modbusData.Rows.Add()
            modbusData.Rows(i)(0) = i
            modbusData.Rows(i)(1) = 0
            modbusData.Rows(i)(2) = False                       ' Test
        Next i
        modbusData.Columns(0).ReadOnly = True
    End Sub

    Public Function getTable() As DataTable
        Return modbusData
    End Function

    Protected Overrides Function readHoldingRegistersTable(ByVal startRef As Int32, ByVal regArr() As Int16) As Boolean
        ' Adjust Modbus reference counting from 1-based to 0-based
        startRef -= 1
        ' Validate range
        If startRef + regArr.Length > modbusData.Rows.Count Then
            Return False
        End If
        ' Copy registers from local data table to Modbus
        For i As Integer = 0 To regArr.Length - 1
            regArr(i) = modbusData.Rows(startRef + i)(1)
        Next i
        Return True
    End Function

    Protected Overrides Function writeHoldingRegistersTable(ByVal startRef As Int32, ByVal regArr() As Int16) As Boolean
        ' Adjust Modbus reference counting from 1-based to 0-based
        startRef -= 1
        ' Validate range
        If startRef + regArr.Length > modbusData.Rows.Count Then
            Return False
        End If
        ' Copy registers from Modbus to local data table
        For i As Integer = 0 To regArr.Length - 1
            modbusData.Rows(startRef + i)(1) = regArr(i)
        Next i
        Return True
    End Function

    Protected Overrides Function readCoilsTable(ByVal startRef As Int32, ByVal bitArr() As Boolean) As Boolean

        Debug.WriteLine("readCoilsTable from " + startRef.ToString + ", " + bitArr.Length.ToString + " references")

        ' Adjust Modbus reference counting from 1-based to 0-based
        startRef -= 1
        ' Validate range 
        If startRef + bitArr.Length > modbusData.Rows.Count Then
            Return False
        End If
        ' Copy registers from local data array to Modbus 
        For i As Integer = 0 To bitArr.Length - 1
            bitArr(i) = modbusData.Rows(startRef + i)(2)
        Next i
        Return True
    End Function

    'Protected Overrides Function writeCoilsTable(ByVal startRef As Int32, ByVal bitArr() As Boolean) As Boolean

    '    Debug.WriteLine("writeCoilsTable from " + startRef.ToString + ", " + bitArr.Length.ToString + " references")

    '    ' Adjust Modbus reference counting from 1-based to 0-based
    '    startRef -= 1
    '    ' Validate range 
    '    If startRef + bitArr.Length > modbusData.Rows.Count Then
    '        Return False
    '    End If
    '    ' Copy registers from local data array to Modbus 
    '    For i As Integer = 0 To bitArr.Length - 1
    '        modbusData.Rows(startRef + i)(2) = bitArr(i)
    '    Next i
    '    Return True
    'End Function

End Class

