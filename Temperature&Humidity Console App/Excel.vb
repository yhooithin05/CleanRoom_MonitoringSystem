Imports Microsoft.Office.Interop.Excel
Imports System.Globalization
Imports System.Threading
Imports System.Web
Public Class Excel

    Dim path As String = ""
    Dim excel As _Application = New Microsoft.Office.Interop.Excel.Application()
    Dim wb As Workbook
    Dim ws As Worksheet
    Dim range As Range
    Dim conn As New ConnSensor

    Public Sub New(ByVal path As String, ByVal sheetN As Integer, ByVal excelLocked As Integer)

        Me.path = path

        If excelLocked = 2 Then
            wb = excel.Workbooks.Open(path, 0, [ReadOnly]:=False, Format:=6, Password:="", WriteResPassword:="", IgnoreReadOnlyRecommended:=True, Delimiter:=";", Editable:=True, Local:=True)
        Else
            wb = excel.Workbooks.Open(path, 0, [ReadOnly]:=True, Format:=6, Password:="", WriteResPassword:="", IgnoreReadOnlyRecommended:=True, Delimiter:=";", Editable:=True, Local:=True)
        End If

        Dim workboookName As String = wb.Name
        Dim rowCount As Integer = 0
        Dim colCount As Integer = 0
        ws = wb.Worksheets(sheetN)

        range = ws.UsedRange
        rowCount = range.Rows.Count

        colCount = range.Columns.Count
    End Sub

    Public Function GetRowCount() As Integer

        Dim rowCount As Integer = 0
        rowCount = range.Rows.Count
        Return rowCount

    End Function

    'Convert sample time from double to timespan format
    Public Shared Function ConvertFromDoubleToHHMMSSTT(ByVal days As Double) As String
        Dim cellValue = TimeSpan.FromDays(days).ToString("hh\:mm\:ss")
        Dim sampleTime = DateTime.ParseExact(cellValue, "HH:mm:ss", Nothing).ToString("h:mm:ss tt")
        Return sampleTime
    End Function

    Public Function ReadCellstr(ByVal i As Integer, ByVal j As Integer) As String

        Dim cellValue As String

        If range.Cells(i, j).Value IsNot Nothing Then

            cellValue = Convert.ToString(range.Cells(i, j).value)

            Return cellValue
        Else
            Return ""
        End If

    End Function

    Public Function ReadCelldouble(ByVal i As Integer, ByVal j As Integer) As Double

        If range.Cells(i, j).Value IsNot Nothing Then
            Return range.Cells(i, j).Value
        Else
            Return 0.0
        End If
    End Function

    Public Function CheckIfProcessAlreadyRunning() As Boolean
        If Process.GetProcessesByName("SensorReadOneDay").Length > 0 Then
            Return True
        End If

        Return False
    End Function


    Public Sub CloseWb()
        wb.Close(True)
        excel.Quit()
        Dim process As System.Diagnostics.Process() = System.Diagnostics.Process.GetProcessesByName("Excel")
        Dim processApp As System.Diagnostics.Process() = System.Diagnostics.Process.GetProcessesByName("SensorReadOneDay")

        For Each p As System.Diagnostics.Process In process

            If Not String.IsNullOrEmpty(p.ProcessName) Then

                Try
                    p.Kill()
                Catch
                End Try
            End If

        Next

        System.Runtime.InteropServices.Marshal.ReleaseComObject(Me.ws)
        System.Runtime.InteropServices.Marshal.ReleaseComObject(Me.wb)
        System.Runtime.InteropServices.Marshal.ReleaseComObject(Me.excel)


    End Sub

End Class
