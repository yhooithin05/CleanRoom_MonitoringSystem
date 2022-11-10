Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Net.Mail

Public Class ConnSensor

    Public Shared Function GetConnString() As SqlConnection
        Dim ObjConn = ConfigurationManager.ConnectionStrings("SensorDB").ConnectionString
        Dim conn1 As SqlConnection = New SqlConnection(ObjConn)
        conn1.Open()
        If ObjConn Is Nothing Then Throw New ArgumentNullException("ObjConn")
        Return conn1
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

    Public Function exscalar(ByVal sql As String) As String

        Using conn As SqlConnection = GetConnString()
            Using update1 As SqlCommand = New SqlCommand(sql, conn)
                Return update1.ExecuteScalar
            End Using
        End Using

    End Function

    Public Sub nonquery(ByVal sql As String)

        Using conn As SqlConnection = GetConnString()
            Using cmd As SqlCommand = New SqlCommand(sql, conn)
                cmd.CommandTimeout = 0
                cmd.ExecuteNonQuery()
            End Using
        End Using

    End Sub

    Public Function myNow() As String
        Return Format(System.TimeZoneInfo.ConvertTime(Now, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")), "yyyy-MM-dd HH:mm:ss")
    End Function

    Public Function myDateNow() As String
        Return Format(System.TimeZoneInfo.ConvertTime(Now, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")), "yyyy-MM-dd")
    End Function


    Public Function GetSensorLimitation() As SensorLimitation
        Try
            Dim dt As DataTable = query("SELECT pc_min, pc_max,email_pc FROM tbsetting")
            GetSensorLimitation = New SensorLimitation
            GetSensorLimitation.pc_min = dt.Rows(0)("pc_min")
            GetSensorLimitation.pc_max = dt.Rows(0)("pc_max")
        Catch ex As Exception
            Console.WriteLine("Error:" + ex.ToString)
        End Try

    End Function


    Public Function GetSensorArea(ByVal sensor As String) As SensorLimitation
        Try
            Dim dt As DataTable = query("SELECT category FROM tbsensor WHERE SensorName = '" + sensor + "'")
            GetSensorArea = New SensorLimitation
            GetSensorArea.pc_category = dt.Rows(0)("category")
        Catch ex As Exception
            Console.WriteLine("Error:" + ex.ToString)
        End Try
    End Function


    Public Class SensorLimitation

        Public pc_min As Double
        Public pc_max As Double
        Public pc_category As String

    End Class



End Class
