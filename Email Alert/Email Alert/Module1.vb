Imports System.Globalization
Imports System.Net.Mail

Module Module1

    Dim conn As New ConnSensor

    Dim limit_pc_1k As Double
    Dim limit_pc_10k As Double
    Dim limit_temp_min As Double
    Dim limit_temp_max As Double
    Dim limit_temp_min_10k As Double
    Dim limit_temp_max_10k As Double
    Dim limit_hum_min As Double
    Dim limit_hum_max As Double
    Dim limit_email_pc As Double
    Dim limit_email_temp As Double
    Dim limit_email_hum As Double
    Sub Main()

        Get_Limit_Setting()
        SendMailAlertPc()
        SendMailAlertTemp()
        SendMailAlertHum()

    End Sub

    Sub Get_Limit_Setting()
        Dim limit As SensorLimitation = conn.GetSensorLimitation
        limit_pc_1k = limit.pc_1k
        limit_pc_10k = limit.pc_10k
        limit_temp_min = limit.temp_min
        limit_temp_max = limit.temp_max
        limit_temp_min_10k = limit.temp_min_10k
        limit_temp_max_10k = limit.temp_max_10k
        limit_hum_min = limit.hum_min
        limit_hum_max = limit.hum_max
        limit_email_pc = limit.email_pc
        limit_email_temp = limit.email_temp
        limit_email_hum = limit.email_hum

    End Sub

    Sub SendMailAlertPc()
        Dim NowDateTime As DateTime = conn.myNow
        Dim strCondition As String = String.Format(" WHERE alarm='1' AND EmailSent='0' AND CONCAT(SampleDate,' ',SampleTime) < '{0}' ", Format(NowDateTime, "yyyy-MM-dd HH:mm:ss"))
        strCondition = String.Format(" WHERE alarm='1' AND EmailSent='0' ")

        Dim strSampleDateTime As String = String.Format(
            " SELECT CONCAT(SampleDate,' ',SampleTime) AS SampleDateTime " &
            " FROM tbsensordata {0} ", strCondition)

        If Not String.IsNullOrEmpty(conn.exscalar("SELECT * FROM tbsensordata WHERE EXISTS(" & strSampleDateTime & ")")) Then
            Dim SampleDateTime As DateTime = conn.exscalar(strSampleDateTime)

            'NowDateTime = #03/23/2021 1:27:38 AM#
            Dim duration As TimeSpan = NowDateTime - SampleDateTime

            If duration.TotalMinutes >= limit_email_pc Then

                '~~ Email Content 
                Dim strBodyContent As String = String.Empty
                Dim strBody As New Text.StringBuilder
                strBody.Append("<b>Alert for Particle Count</b><br/><br />")

                Dim select1 As String = String.Format(
                    " SELECT ID, SampleDate, SampleTime, SensorName, SampleValue, pc_min, pc_max, " &
                    "        Alarm, CONCAT(SampleDate,' ',SampleTime) AS SampleDateTime, EmailSent FROM tbsensordata {0}", strCondition)

                Dim dt As New DataTable
                dt = conn.query(select1)

                If dt.Rows.Count > 0 Then
                    strBody.Append("<table border=""1"" cellspacing=""0"" cellpadding=""3"">")
                    strBody.Append("<tr style=""background:#add8e6;"">")
                    strBody.Append("<th>No</th>")
                    strBody.Append("<th>Name</th>")
                    strBody.Append("<th>Date</th>")
                    strBody.Append("<th>Time</th>")
                    strBody.Append("<th>Value</th>")
                    strBody.Append("</tr>")
                End If

                For i As Integer = 0 To dt.Rows.Count - 1
                    Dim SensorName As String = dt.Rows(i)("SensorName")
                    Dim SampleDate As String = Convert.ToDateTime(dt.Rows(i)("SampleDate")).ToString("dd-MMM-yyyy")
                    Dim SampleTime As String = dt.Rows(i)("SampleTime")
                    Dim SampleValue As String = dt.Rows(i)("SampleValue")

                    strBody.Append("<tr>")
                    strBody.Append("<td>" & i + 1 & "</td>")
                    strBody.Append("<td>" & SensorName & "</td>")
                    strBody.Append("<td align=""center"">" & SampleDate & "</td>")
                    strBody.Append("<td align=""center"">" & SampleTime & "</td>")
                    strBody.Append("<td align=""center"">" & SampleValue & "</td>")
                    strBody.Append("</tr>")
                Next

                strBody.Append("</table><br /><br />")

                strBody.Append(String.Format("<u>Limits</u><br />"))
                strBody.Append(String.Format("Particle Count 1K (Max): {0}<br />", limit_pc_1k))
                strBody.Append(String.Format("Particle Count 10K (Max): {0}<br /><br />", limit_pc_10k))

                ' Content: Ending 
                strBody.Append("<p>This is an auto-generated email. Please do not reply. </p>")

                strBodyContent = strBody.ToString

                '~~ Email Subject 
                ' Dim strSubject As String = String.Format("Particle Counter Alert! Clean Room Monitoring System - {0}", conn.myNowLastUpdate)
                Dim strSubject As String = String.Format("Particle Counter Alert! Clean Room Monitoring System : {0}", Format(NowDateTime, "dd-MMM-yyyy (ddd) hh:mm tt"))

                conn.MailAlert(strSubject, strBodyContent)

                Dim update1 As String = String.Format("UPDATE tbsensordata SET EmailSent='1' {0} ", strCondition)
                conn.nonquery(update1)

            End If

        End If

    End Sub

    Sub SendMailAlertTemp()
        Dim strSampleDateTime As String = "SELECT CreatedOn FROM tbtemphum WHERE AlarmTemp='1' AND EmailSentTemp='0'"

        If Not String.IsNullOrEmpty(conn.exscalar("SELECT * FROM tbtemphum WHERE EXISTS(" & strSampleDateTime & ")")) Then

            Dim SampleDateTime As DateTime = conn.exscalar(strSampleDateTime)
            Dim NowDateTime As DateTime = conn.myNow
            'NowDateTime = #03/23/2021 1:27:38 AM#
            Dim duration As TimeSpan = NowDateTime - SampleDateTime

            If duration.TotalMinutes >= limit_email_temp Then

                '~~ Email Content 
                Dim strBodyContent As String = String.Empty
                Dim strBody As New Text.StringBuilder
                strBody.Append("<b>Alert for Temperature</b><br/><br />")

                Dim strCondition As String = String.Format("WHERE alarmtemp='1' AND EmailSentTemp='0' AND CreatedOn < '{0}'", Format(NowDateTime, "yyyy-MM-dd HH:mm:ss"))

                Dim select1 As String = String.Format(
                    " SELECT tempid, tempval, temp_min, temp_max, CreatedOn,Convert(time(0),CreatedOn) AS CreatedTime, alarmtemp " &
                    " FROM tbtemphum {0} ORDER BY CreatedOn ", strCondition)
                Dim dt As New DataTable
                dt = conn.query(select1)

                If dt.Rows.Count > 0 Then
                    strBody.Append("<table border=""1"" cellspacing=""0"" cellpadding=""3"">")
                    strBody.Append("<tr style=""background:#add8e6;"">")
                    strBody.Append("<th>No</th>")
                    strBody.Append("<th>Name</th>")
                    strBody.Append("<th>Date</th>")
                    strBody.Append("<th>Time</th>")
                    strBody.Append("<th>Value</th>")
                    strBody.Append("</tr>")
                End If

                For i As Integer = 0 To dt.Rows.Count - 1
                    Dim SensorName As String = dt.Rows(i)("tempid")
                    Dim SampleDate As String = Convert.ToDateTime(dt.Rows(i)("CreatedOn")).ToString("dd-MMM-yyyy")
                    Dim createdTime As TimeSpan = DirectCast(dt.Rows(i)("CreatedTime"), TimeSpan)
                    Dim SampleTime As String = createdTime.ToString()
                    Dim SampleValue As String = GetDouble(dt.Rows(i)("tempval").ToString).ToString(("F1"))

                    strBody.Append("<tr>")
                    strBody.Append("<td>" & i + 1 & "</td>")
                    strBody.Append("<td>" & SensorName & "</td>")
                    strBody.Append("<td align=""center"">" & SampleDate & "</td>")
                    strBody.Append("<td align=""center"">" & SampleTime & "</td>")
                    strBody.Append("<td align=""center"">" & SampleValue & "&#176;C</td>")
                    strBody.Append("</tr>")
                Next

                strBody.Append("</table><br /><br />")

                strBody.Append(String.Format("<u>Limits</u><br />"))
                strBody.Append(String.Format("Temperature 1K (Min): {0}&#176;C<br />", limit_temp_min))
                strBody.Append(String.Format("Temperature 1K (Max): {0}&#176;C<br /><br />", limit_temp_max))
                strBody.Append(String.Format("Temperature 10K (Min): {0}&#176;C<br />", limit_temp_min_10k))
                strBody.Append(String.Format("Temperature 10K (Max): {0}&#176;C<br /><br />", limit_temp_max_10k))
                ' Content: Ending 
                strBody.Append("<p>This is an auto-generated email. Please do not reply. </p>")

                strBodyContent = strBody.ToString

                '~~ Email Subject 
                Dim strSubject As String = String.Format("Temperature Alert! Clean Room Monitoring System : {0}", Format(NowDateTime, "dd-MMM-yyyy (ddd) hh:mm tt"))

                conn.MailAlert(strSubject, strBodyContent)

                Dim update1 As String = String.Format("UPDATE tbtemphum SET EmailSentTemp='1' {0} ", strCondition)
                conn.nonquery(update1)
            End If

        End If
    End Sub

    Sub SendMailAlertHum()
        Dim strSampleDateTime As String = "SELECT CreatedOn FROM tbtemphum WHERE AlarmHum='1' AND EmailSentHum='0'"

        If Not String.IsNullOrEmpty(conn.exscalar("SELECT * FROM tbtemphum WHERE EXISTS(" & strSampleDateTime & ")")) Then

            Dim SampleDateTime As DateTime = conn.exscalar(strSampleDateTime)
            Dim NowDateTime As DateTime = conn.myNow
            'NowDateTime = #03/23/2021 1:27:38 AM#
            Dim duration As TimeSpan = NowDateTime - SampleDateTime
            If duration.TotalMinutes >= limit_email_hum Then

                '~~ Email Content 
                Dim strBodyContent As String = String.Empty
                Dim strBody As New Text.StringBuilder
                strBody.Append("<b>Alert for Humidity</b><br/><br />")

                Dim strCondition As String = String.Format("WHERE alarmhum=1 AND EmailSentHum=0 AND CreatedOn < '{0}'", Format(NowDateTime, "yyyy-MM-dd HH:mm:ss"))

                Dim select1 As String = String.Format(
                " SELECT humid, humval, hum_min, hum_max, CreatedOn, Convert(time(0),CreatedOn) AS CreatedTime " &
                " FROM tbtemphum {0} ORDER BY CreatedOn ", strCondition)

                Dim dt As New DataTable
                dt = conn.query(select1)

                If dt.Rows.Count > 0 Then
                    strBody.Append("<table border=""1"" cellspacing=""0"" cellpadding=""3"">")
                    strBody.Append("<tr style=""background:#add8e6;"">")
                    strBody.Append("<th>No</th>")
                    strBody.Append("<th>Name</th>")
                    strBody.Append("<th>Date</th>")
                    strBody.Append("<th>Time</th>")
                    strBody.Append("<th>Value</th>")
                    strBody.Append("</tr>")
                End If

                For i As Integer = 0 To dt.Rows.Count - 1
                    Dim SensorName As String = dt.Rows(i)("humid")
                    Dim SampleDate As String = Convert.ToDateTime(dt.Rows(i)("CreatedOn")).ToString("dd-MMM-yyyy")
                    Dim createdTime As TimeSpan = DirectCast(dt.Rows(i)("CreatedTime"), TimeSpan)
                    Dim SampleTime As String = createdTime.ToString()
                    Dim SampleValue As String = GetDouble(dt.Rows(i)("humval").ToString).ToString(("F1"))

                    strBody.Append("<tr>")
                    strBody.Append("<td>" & i + 1 & "</td>")
                    strBody.Append("<td>" & SensorName & "</td>")
                    strBody.Append("<td align=""center"">" & SampleDate & "</td>")
                    strBody.Append("<td align=""center"">" & SampleTime & "</td>")
                    strBody.Append("<td align=""center"">" & SampleValue & "%</td>")
                    strBody.Append("</tr>")
                Next

                strBody.Append("</table><br /><br />")

                strBody.Append(String.Format("<u>Limits</u><br />"))
                strBody.Append(String.Format("Humidity (Min): {0}%<br />", limit_hum_min))
                strBody.Append(String.Format("Humidity (Max): {0}%<br /><br />", limit_hum_max))

                ' Content: Ending 
                strBody.Append("<p>This is an auto-generated email. Please do not reply. </p>")

                strBodyContent = strBody.ToString

                '~~ Email Subject 
                Dim strSubject As String = String.Format("Humidity Alert! Clean Room Monitoring System : {0}", Format(NowDateTime, "dd-MMM-yyyy (ddd) hh:mm tt"))

                conn.MailAlert(strSubject, strBodyContent)

                Dim update1 As String = String.Format("UPDATE tbtemphum SET EmailSentHum='1' {0} ", strCondition)
                conn.nonquery(update1)
            End If


        End If
    End Sub

    Public Function GetDouble(ByVal doublestring As String) As Double
        Dim retval As Double
        Dim sep As String = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator

        Double.TryParse(Replace(Replace(doublestring, ".", sep), ",", sep), retval)
        Return retval
    End Function


End Module
