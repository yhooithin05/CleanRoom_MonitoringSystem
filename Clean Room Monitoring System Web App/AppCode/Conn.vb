Imports System.Data
Imports System.Data.SqlClient
Imports System.Net.Mail

Public Class Conn

    Public Shared Function GetConnString() As SqlConnection
        Dim ObjConn = ConfigurationManager.ConnectionStrings("MyConnSensor").ConnectionString
        Dim conn1 As SqlConnection = New SqlConnection(ObjConn)
        conn1.Open()
        If ObjConn Is Nothing Then Throw New ArgumentNullException("ObjConn")
        Return conn1
    End Function

    'Public Sub openSql()
    '    If conn1.State = ConnectionState.Closed Then
    '        conn1.Open()
    '    End If
    'End Sub

    Public Sub closeSql()

        Using conn As SqlConnection = GetConnString()
            conn.Close()
        End Using
        'If conn1.State = ConnectionState.Open Then
        '    conn1.Close()
        'End If
    End Sub

    Public Sub nonquery(ByVal sql As String)

        Using conn As SqlConnection = GetConnString()
            Using cmd As SqlCommand = New SqlCommand(sql, conn)
                cmd.ExecuteNonQuery()
            End Using
        End Using

    End Sub

    Public Function exscalar(ByVal sql As String) As String

        Using conn As SqlConnection = GetConnString()
            Using update1 As SqlCommand = New SqlCommand(sql, conn)
                Dim result = update1.ExecuteScalar
                If IsDBNull(result) Then
                    Return 0
                Else
                    Return result
                End If
            End Using
        End Using

    End Function

    Public Function query(ByVal sql As String) As DataTable

        Using conn As SqlConnection = GetConnString()
            Using comm As SqlCommand = New SqlCommand(sql, conn)
                Using oDataAdapter As SqlDataAdapter = New SqlDataAdapter(comm)
                    Dim odt As DataTable = New DataTable
                    oDataAdapter.SelectCommand.CommandTimeout = 0
                    oDataAdapter.Fill(odt)
                    Return odt
                End Using
            End Using
        End Using

    End Function



    Public Function GetSensorLimitation() As SensorLimitation

        Dim dt As DataTable = query(
            " SELECT pc_min, pc_max, temp_min, temp_max, temp_min_10k, temp_max_10k, hum_min, hum_max, email_pc, email_temp, email_hum " &
            " FROM tbsetting")
        GetSensorLimitation = New SensorLimitation
        GetSensorLimitation.pc_min = dt.Rows(0)("pc_min")
        GetSensorLimitation.pc_max = dt.Rows(0)("pc_max")
        GetSensorLimitation.temp_min = dt.Rows(0)("temp_min")
        GetSensorLimitation.temp_max = dt.Rows(0)("temp_max")
        GetSensorLimitation.temp_min_10k = dt.Rows(0)("temp_min_10k")
        GetSensorLimitation.temp_max_10k = dt.Rows(0)("temp_max_10k")
        GetSensorLimitation.hum_min = dt.Rows(0)("hum_min")
        GetSensorLimitation.hum_max = dt.Rows(0)("hum_max")
        GetSensorLimitation.email_pc = dt.Rows(0)("email_pc")
        GetSensorLimitation.email_temp = dt.Rows(0)("email_temp")
        GetSensorLimitation.email_hum = dt.Rows(0)("email_hum")
    End Function

    Public Function fileExportDate() As String
        Return Format(System.TimeZoneInfo.ConvertTime(Now, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")), "Ymd")
    End Function

    Public Function todayDate() As String
        Return Format(System.TimeZoneInfo.ConvertTime(Now, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")), "dd MMMM yy")
    End Function

    Public Function myDateNow() As String
        Return Format(System.TimeZoneInfo.ConvertTime(Now, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")), "yyyy-MM-dd")
    End Function

    Public Function myNow() As String
        Return Format(System.TimeZoneInfo.ConvertTime(Now, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")), "yyyy-MM-dd HH:mm:ss")
    End Function

    Public Function myNowLastUpdate() As String
        Return Format(System.TimeZoneInfo.ConvertTime(Now, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")), "dd-MMM-yyyy (ddd) hh:mm tt")
    End Function

    Public Function myDateTimeNow() As String
        Return Format(System.TimeZoneInfo.ConvertTime(Now, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")), "dddd, dd-MMM-yyyy hh:mm tt")
    End Function

    Public Function ToMySqlText(ByVal str As String) As String

        str = str.Replace("\", "\\")
        str = str.Replace("`", "\`")
        str = str.Replace("%", "\%")
        str = str.Replace(Environment.NewLine, "<br>")
        Return str

    End Function



#Region "eventlog"
    Public Function GetSensorName() As List(Of String)

        Dim dt As DataTable = query("SELECT sensorName FROM tbsensor WHERE SensorName LIKE 'PT%'")
        Dim SensorList As New List(Of String)
        For i As Integer = 0 To dt.Rows.Count - 1
            SensorList.Add(dt.Rows(i)("sensorName").ToString())
        Next

        Return SensorList
    End Function

    Public Function GetTempName() As List(Of String)

        Dim dt As DataTable = query("SELECT sensorName FROM tbsensor WHERE SensorName LIKE 'Temp%'")
        Dim TempList As New List(Of String)
        For i As Integer = 0 To dt.Rows.Count - 1
            TempList.Add(dt.Rows(i)("sensorName").ToString())
        Next

        Return TempList
    End Function

    Public Function GetHumName() As List(Of String)

        Dim dt As DataTable = query("SELECT sensorName FROM tbsensor WHERE SensorName LIKE 'Hum%'")
        Dim HumList As New List(Of String)
        For i As Integer = 0 To dt.Rows.Count - 1
            HumList.Add(dt.Rows(i)("sensorName").ToString())
        Next

        Return HumList
    End Function

#End Region



End Class


Public Class SensorLimitation

    Public pc_min As Double
    Public pc_max As Double
    Public temp_min As Double
    Public temp_max As Double
    Public temp_min_10k As Double
    Public temp_max_10k As Double
    Public hum_min As Double
    Public hum_max As Double
    Public email_pc As Integer
    Public email_temp As Integer
    Public email_hum As Integer

End Class
