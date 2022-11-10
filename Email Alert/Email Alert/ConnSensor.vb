Imports System.Configuration
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


    Public Sub closeSql()
        Using conn As SqlConnection = GetConnString()
            conn.Close()
        End Using
    End Sub

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
                Dim result = update1.ExecuteScalar
                If IsDBNull(result) Then
                    Return "0"
                Else
                    Return result
                End If
            End Using
        End Using

    End Function

    Public Sub nonquery(ByVal sql As String)
        Using conn As SqlConnection = GetConnString()
            Using cmd As SqlCommand = New SqlCommand(sql, conn)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Public Function GetSensorLimitation() As SensorLimitation

        Dim dt As DataTable = query("Select pc_min, pc_max, temp_min, temp_max, temp_min_10k, temp_max_10k, hum_min, hum_max,email_pc, email_temp, email_hum FROM tbsetting")
        GetSensorLimitation = New SensorLimitation
        GetSensorLimitation.pc_1k = dt.Rows(0)("pc_min")
        GetSensorLimitation.pc_10k = dt.Rows(0)("pc_max")
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

    Dim EMAIL_NO_REPLY As String = "yhooithin8905@outlook.com"
    Dim EMAIL_PASSWORD As String = "testing89"
    Dim EMAIL_ADMIN As String = "yhooithin05@yahoo.com"
    Public Sub WriteEmail(ByVal EmailTo As String, ByVal SubjectName As String, ByVal BodyContent As String)

        Dim client As SmtpClient = New SmtpClient("smtp-mail.outlook.com")
        client.Port = 587
        client.DeliveryMethod = SmtpDeliveryMethod.Network
        client.UseDefaultCredentials = False
        Dim credentials As System.Net.NetworkCredential = New System.Net.NetworkCredential(EMAIL_NO_REPLY, EMAIL_PASSWORD)
        client.EnableSsl = True
        client.Credentials = credentials

        Try
            Dim mail As MailMessage = New MailMessage(EMAIL_NO_REPLY, EmailTo, SubjectName, "")

            mail.CC.Add(EMAIL_ADMIN)

            mail.IsBodyHtml = True

            mail.Body = BodyContent

            client.Send(mail)

            mail = Nothing
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    Sub MailAlert(ByVal strSubject As String, ByVal strBodyContent As String)
        Dim select1 As String =
            " Select Name, Email FROM tbContact WHERE blnActive='1' ORDER BY Email "
        Dim dt As New DataTable
        dt = query(select1)

        '~~ Email To List
        Dim strEmailTo As String = String.Empty

        For i As Integer = 0 To dt.Rows.Count - 1
            ' Store the email to strEmailTo
            strEmailTo = strEmailTo & dt.Rows(i)("Email") & ","
        Next

        If strEmailTo.Length <> 0 Then
            ' Remove the last character of the string if only if the string is not empty
            strEmailTo = strEmailTo.Substring(0, strEmailTo.Length - 1)
        End If

        Try
            WriteEmail(strEmailTo, strSubject, strBodyContent)
        Catch ex As Exception
            WriteEmail(EMAIL_ADMIN, "Sensor Error (Sensor Email)", ex.Message)
        End Try

    End Sub

    Public Function myNow() As String
        Return Format(System.TimeZoneInfo.ConvertTime(Now, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")), "yyyy-MM-dd HH:mm:ss")
    End Function

    Public Function myDateNow() As String
        Return Format(System.TimeZoneInfo.ConvertTime(Now, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")), "yyyy-MM-dd")
    End Function

    Public Function myNowLastUpdate() As String
        Return Format(System.TimeZoneInfo.ConvertTime(Now, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")), "dd-MMM-yyyy (ddd) hh:mm tt")
    End Function

End Class

Public Class SensorLimitation

    Public pc_1k As Double
    Public pc_10k As Double
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


