Imports System.Data
Imports System.Drawing
Imports System.Globalization
Imports System.Net
Imports System.Net.Mail
Imports System.Web.Mail
Imports Newtonsoft.Json.Linq
Imports MailMessage = System.Net.Mail.MailMessage

Public Class _Default
    Inherits Page

    Dim limit_pc_min As Double
    Dim limit_pc_max As Double
    Dim limit_temp_min As Double
    Dim limit_temp_max As Double
    Dim limit_temp_min_10k As Double
    Dim limit_temp_max_10k As Double
    Dim limit_hum_min As Double
    Dim limit_hum_max As Double
    Dim eventlogList As New List(Of String)
    Dim conn As New Conn

    Dim Pt1 As String = "Pt 1"
    Dim Pt2 As String = "Pt 2"
    Dim Pt3 As String = "Pt 3"
    Dim Pt4 As String = "Pt 4"
    Dim Pt5 As String = "Pt 5"
    Dim Pt6 As String = "Pt 6"
    Dim temp10K As String = "Temp10K"
    Dim temp1K As String = "Temp1K"
    Dim hum10K As String = "Hum10K"
    Dim hum1K As String = "Hum1K"

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Session("PageTitle") = "Dashboard"
    End Sub

    Protected Sub Page_LoadComplete(sender As Object, e As System.EventArgs) Handles Me.LoadComplete

        Get_Limit_Set()
        setAllValue()

    End Sub

    Sub setAllValue()
        'set particle counter value
        set_pc()

        'set particle counter color
        set_td_pc(lbl1kp1.Text, td1k_p1, Pt1)
        set_td_pc(lbl1kp2.Text, td1k_p2, Pt2)
        set_td_pc(lbl1kp3.Text, td1k_p3, Pt3)
        set_td_pc(lbl1kp4.Text, td1k_p4, Pt4)
        set_td_pc(lbl10kp5.Text, td10k_p5, Pt5)
        set_td_pc(lbl10kp6.Text, td10k_p6, Pt6)

        'set temperature and humidity value
        set_temphum()

        'set temperature and humidity color
        If Not lbl1ktemp.Text = "" Then
            set_td_temp(lbl1ktemp.Text, "1K", td1k_temp)
        End If
        If Not lbl10ktemp.Text = "" Then
            set_td_temp(lbl10ktemp.Text, "10K", td10k_temp)
        End If
        If Not lbl1khum.Text = "" Then
            set_td_hum(lbl1khum.Text, td1k_hum)
        End If
        If Not lbl10khum.Text = "" Then
            set_td_hum(lbl10khum.Text, td10k_hum)
        End If

        set_eventlog()

    End Sub

    'Call this to auto refresh page every 5 seconds
    Protected Sub Timer1_Tick(ByVal sender As Object, ByVal e As EventArgs)

        Get_Limit_Set()
        setAllValue()
        set_eventlog()

    End Sub


    Sub set_pc()
        'Dim select1 As String = "SELECT * FROM (SELECT * FROM tbsensordata ORDER BY UpdatedOn DESC)a group by sensorname"
        Dim select1 As String = " SELECT  * FROM " &
                                " (SELECT SensorName, Max(UpdatedOn) As UpdatedOn  FROM tbsensordata group by SensorName) AS data " &
                                " JOIN tbsensordata ON data.SensorName = tbsensordata.SensorName AND data.UpdatedOn = tbsensordata.UpdatedOn"
        Dim dt As New DataTable
        dt = conn.query(select1)
        For i As Integer = 0 To dt.Rows.Count - 1
            If dt.Rows(i)("SampleValue").ToString <> "" Then

                Select Case dt.Rows(i)("SensorName").ToString
                    Case Pt1
                        lbl1kp1.Text = dt.Rows(i)("SampleValue").ToString
                        lbl1kp1.CssClass = "divPTContainer" 'To prevent after offline status remain at smaller font
                    Case Pt2
                        lbl1kp2.Text = dt.Rows(i)("SampleValue").ToString
                        lbl1kp2.CssClass = "divPTContainer"
                    Case Pt3
                        lbl1kp3.Text = dt.Rows(i)("SampleValue").ToString
                        lbl1kp3.CssClass = "divPTContainer"
                    Case Pt4
                        lbl1kp4.Text = dt.Rows(i)("SampleValue").ToString
                        lbl1kp4.CssClass = "divPTContainer"
                    Case Pt5
                        lbl10kp5.Text = dt.Rows(i)("SampleValue").ToString
                        lbl10kp5.CssClass = "divPTContainer"
                    Case Pt6
                        lbl10kp6.Text = dt.Rows(i)("SampleValue").ToString
                        lbl10kp6.CssClass = "divPTContainer"
                End Select
            End If
        Next
    End Sub



    Sub set_temphum()

        'Dim select1 As String = "SELECT * FROM (SELECT * FROM tbtemphum ORDER BY CreatedOn DESC )a group by tempid,humid"
        Dim select1 As String = "SELECT TOP 2 tempid, tempval,humid, humval,CreatedOn FROM tbtemphum ORDER BY CreatedOn DESC"
        Dim dt As New DataTable
        dt = conn.query(select1)
        For i As Integer = 0 To dt.Rows.Count - 1

            If dt.Rows(i)("tempval").ToString <> "" Then
                Select Case dt.Rows(i)("tempid").ToString
                    Case temp10K
                        lbl10ktemp.Text = Math.Round(Convert.ToDouble(dt.Rows(i)("tempval").ToString), 1)
                        lbl10ktemp.CssClass = "divPTContainer"
                    Case temp1K
                        lbl1ktemp.Text = Math.Round(Convert.ToDouble(dt.Rows(i)("tempval").ToString), 1)
                        lbl1ktemp.CssClass = "divPTContainer"
                End Select
            End If
            If dt.Rows(i)("humval").ToString <> "" Then
                Select Case dt.Rows(i)("humid").ToString
                    Case hum10K
                        lbl10khum.Text = Convert.ToDouble(dt.Rows(i)("humval").ToString)
                        lbl10khum.CssClass = "divPTContainer"
                    Case hum1K
                        lbl1khum.Text = Convert.ToDouble(dt.Rows(i)("humval").ToString)
                        lbl1khum.CssClass = "divPTContainer"
                End Select
            End If
        Next

    End Sub


    Sub set_td_pc(ByVal lbl As String, ByVal td As Web.UI.HtmlControls.HtmlTableCell, ByVal sensorName As String)

        Dim sensorCategory As String = conn.exscalar("SELECT Category FROM tbsensor WHERE sensorName = '" + sensorName + "'")
        If sensorCategory = "1K" Then
            If Not lbl.ToString = "Offline" Then
                If Convert.ToDouble(lbl) > limit_pc_min Then
                    td.BgColor = "Red"
                    hfAlert.Value = 1
                Else
                    td.BgColor = "#68B449"
                End If
            Else
                td.BgColor = "Red"
                hfAlert.Value = 1
            End If
        Else
            If Not lbl.ToString = "Offline" Then
                If Convert.ToDouble(lbl) > limit_pc_max Then
                    td.BgColor = "Red"
                    hfAlert.Value = 1
                Else
                    td.BgColor = "#68B449"
                End If
            Else
                td.BgColor = "Red"
                hfAlert.Value = 1
            End If

        End If
    End Sub

    Sub set_td_temp(lbl As String, id As String, td As Web.UI.HtmlControls.HtmlTableCell)
        If Not lbl.ToString = "Offline" Then
            If id = "10K" Then
                If Convert.ToDouble(lbl) > limit_temp_max_10k OrElse Convert.ToDouble(lbl) < limit_temp_min_10k Then
                    td.BgColor = "Red"
                    hfAlert.Value = 1
                Else
                    td.BgColor = "#68B449"
                End If
            Else
                If Convert.ToDouble(lbl) > limit_temp_max OrElse Convert.ToDouble(lbl) < limit_temp_min Then
                    td.BgColor = "Red"
                    hfAlert.Value = 1
                Else
                    td.BgColor = "#68B449"
                End If
            End If
        Else
            td.BgColor = "Red"
            hfAlert.Value = 1
        End If
    End Sub

    Sub set_td_hum(lbl As String, td As Web.UI.HtmlControls.HtmlTableCell)
        If Not lbl.ToString = "Offline" Then
            If Convert.ToDouble(lbl) > limit_hum_max OrElse Convert.ToDouble(lbl) < limit_hum_min Then
                td.BgColor = "Red"
                hfAlert.Value = 1
            Else
                td.BgColor = "#68B449"
            End If
        Else
            td.BgColor = "Red"
            hfAlert.Value = 1
        End If

    End Sub


    Sub set_eventlog()

        Dim countPC1today As Integer = 0
        Dim countPC2today As Integer = 0
        Dim countPC3today As Integer = 0
        Dim countPC4today As Integer = 0
        Dim countPC5today As Integer = 0
        Dim countPC6today As Integer = 0
        Dim countPC1_7days As Integer = 0
        Dim countPC2_7days As Integer = 0
        Dim countPC3_7days As Integer = 0
        Dim countPC4_7days As Integer = 0
        Dim countPC5_7days As Integer = 0
        Dim countPC6_7days As Integer = 0
        Dim countTemp1Ktoday As Integer = 0
        Dim countTemp10Ktoday As Integer = 0
        Dim countHum1Ktoday As Integer = 0
        Dim countHum10Ktoday As Integer = 0
        Dim countTemp1K_7days As Integer = 0
        Dim countTemp10K_7days As Integer = 0
        Dim countHum1K_7days As Integer = 0
        Dim countHum10K_7days As Integer = 0

        Dim dateNow As String = "2022-11-10"

        'Get 24hours of today latest data that over limit for each sensor
        For Each sensorName As String In conn.GetSensorName()
            Dim selectPCtoday As String = "SELECT Top 1 SampleTime, SampleValue,SampleDate FROM tbsensordata  Where alarm = 1 AND SampleDate = '" + dateNow + "' " &
                                          "AND SensorName = '" + sensorName + "' ORDER BY UpdatedOn DESC"
            Dim dt As New DataTable
            dt = conn.query(selectPCtoday)
            If dt.Rows.Count > 0 Then
                If sensorName = lbleventlogPoint1.Text Then
                    lbleventlogDate1.Text = dt.Rows(0)("SampleDate").ToString
                    lbleventlogTime1.Text = dt.Rows(0)("SampleTime").ToString
                    lbleventlogPC1.Text = dt.Rows(0)("SampleValue").ToString
                    tdlog1.BgColor = "Red"
                    countPC1today += 1
                ElseIf sensorName = lbleventlogPoint2.Text Then
                    lbleventlogDate2.Text = dt.Rows(0)("SampleDate").ToString
                    lbleventlogTime2.Text = dt.Rows(0)("SampleTime").ToString
                    lbleventlogPC2.Text = dt.Rows(0)("SampleValue").ToString
                    tdlog2.BgColor = "Red"
                    countPC2today += 1
                ElseIf sensorName = lbleventlogPoint3.Text Then
                    lbleventlogDate3.Text = dt.Rows(0)("SampleDate").ToString
                    lbleventlogTime3.Text = dt.Rows(0)("SampleTime").ToString
                    lbleventlogPC3.Text = dt.Rows(0)("SampleValue").ToString
                    tdlog3.BgColor = "Red"
                    countPC3today += 1
                ElseIf sensorName = lbleventlogPoint4.Text Then
                    lbleventlogDate4.Text = dt.Rows(0)("SampleDate").ToString
                    lbleventlogTime4.Text = dt.Rows(0)("SampleTime").ToString
                    lbleventlogPC4.Text = dt.Rows(0)("SampleValue").ToString
                    tdlog4.BgColor = "Red"
                    countPC4today += 1
                ElseIf sensorName = lbleventlogPoint5.Text Then
                    lbleventlogDate5.Text = dt.Rows(0)("SampleDate").ToString
                    lbleventlogTime5.Text = dt.Rows(0)("SampleTime").ToString
                    lbleventlogPC5.Text = dt.Rows(0)("SampleValue").ToString
                    tdlog5.BgColor = "Red"
                    countPC5today += 1
                ElseIf sensorName = lbleventlogPoint6.Text Then
                    lbleventlogDate6.Text = dt.Rows(0)("SampleDate").ToString
                    lbleventlogTime6.Text = dt.Rows(0)("SampleTime").ToString
                    lbleventlogPC6.Text = dt.Rows(0)("SampleValue").ToString
                    tdlog6.BgColor = "Red"
                    countPC6today += 1
                End If

            End If
        Next

        'Get within 1 week latest data that over limit for each sensor after 24 hour
        For Each sensorName As String In conn.GetSensorName()
            Dim selectPC7days As String = "SELECT Top 1 SampleTime, SampleValue,SampleDate FROM tbsensordata  Where alarm = 1 " &
                                          " AND SensorName = '" + sensorName + "' AND sampledate > DATEADD(day, -7,'" + dateNow + "') AND " &
                                          " sampledate < '" + dateNow + "' ORDER BY UpdatedOn DESC"
            Dim dt As New DataTable
            dt = conn.query(selectPC7days)
            If dt.Rows.Count > 0 Then

                If sensorName = lbleventlogPoint1.Text Then
                    If countPC1today = 0 Then 'If within 24 hours no over limit but 1 week before have over limit
                        lbleventlogDate1.Text = dt.Rows(0)("SampleDate").ToString
                        lbleventlogTime1.Text = dt.Rows(0)("SampleTime").ToString
                        lbleventlogPC1.Text = dt.Rows(0)("SampleValue").ToString
                        tdlog1.BgColor = "#FFA623"
                        countPC1_7days += 1
                    End If
                ElseIf sensorName = lbleventlogPoint2.Text Then
                    If countPC2today = 0 Then
                        lbleventlogDate2.Text = dt.Rows(0)("SampleDate").ToString
                        lbleventlogTime2.Text = dt.Rows(0)("SampleTime").ToString
                        lbleventlogPC2.Text = dt.Rows(0)("SampleValue").ToString
                        tdlog2.BgColor = "#FFA623"
                        countPC2_7days += 1
                    End If
                ElseIf sensorName = lbleventlogPoint3.Text Then
                    If countPC3today = 0 Then
                        lbleventlogDate3.Text = dt.Rows(0)("SampleDate").ToString
                        lbleventlogTime3.Text = dt.Rows(0)("SampleTime").ToString
                        lbleventlogPC3.Text = dt.Rows(0)("SampleValue").ToString
                        tdlog3.BgColor = "#FFA623"
                        countPC3_7days += 1
                    End If
                ElseIf sensorName = lbleventlogPoint4.Text Then
                    If countPC4today = 0 Then
                        lbleventlogDate4.Text = dt.Rows(0)("SampleDate").ToString
                        lbleventlogTime4.Text = dt.Rows(0)("SampleTime").ToString
                        lbleventlogPC4.Text = dt.Rows(0)("SampleValue").ToString
                        tdlog4.BgColor = "#FFA623"
                        countPC4_7days += 1
                    End If
                ElseIf sensorName = lbleventlogPoint5.Text Then
                    If countPC5today = 0 Then
                        lbleventlogDate5.Text = dt.Rows(0)("SampleDate").ToString
                        lbleventlogTime5.Text = dt.Rows(0)("SampleTime").ToString
                        lbleventlogPC5.Text = dt.Rows(0)("SampleValue").ToString
                        tdlog5.BgColor = "#FFA623"
                        countPC5_7days += 1
                    End If
                ElseIf sensorName = lbleventlogPoint6.Text Then
                    If countPC6today = 0 Then
                        lbleventlogDate6.Text = dt.Rows(0)("SampleDate").ToString
                        lbleventlogTime6.Text = dt.Rows(0)("SampleTime").ToString
                        lbleventlogPC6.Text = dt.Rows(0)("SampleValue").ToString
                        tdlog6.BgColor = "#FFA623"
                        countPC6_7days += 1
                    End If
                End If
            End If
        Next

        'Get latest data that over limit for each sensor after 7 days (monthly data)
        For Each sensorName As String In conn.GetSensorName()
            Dim selectPC30days As String = "SELECT Top 1 SampleTime, SampleValue,SampleDate FROM tbsensordata  Where alarm = 1 " &
                                          " AND SensorName = '" + sensorName + "' AND sampledate <= DATEADD(day, -7,'" + dateNow + "')" &
                                          " AND sampledate >= DATEADD(day, -35,'" + dateNow + "')" &
                                          " ORDER BY UpdatedOn DESC"
            Dim dt As New DataTable
            dt = conn.query(selectPC30days)
            If dt.Rows.Count > 0 Then
                If sensorName = lbleventlogPoint1.Text Then
                    If countPC1today = 0 And countPC1_7days = 0 Then 'If within 1 weeks's date no over limit but 1 month ago have over limit
                        lbleventlogDate1.Text = dt.Rows(0)("SampleDate").ToString
                        lbleventlogTime1.Text = dt.Rows(0)("SampleTime").ToString
                        lbleventlogPC1.Text = dt.Rows(0)("SampleValue").ToString
                        tdlog1.Attributes.Add("class", "tdBlackFont")
                        tdlog1.BgColor = "White"
                    End If
                ElseIf sensorName = lbleventlogPoint2.Text Then
                    If countPC2today = 0 And countPC2_7days = 0 Then
                        lbleventlogDate2.Text = dt.Rows(0)("SampleDate").ToString
                        lbleventlogTime2.Text = dt.Rows(0)("SampleTime").ToString
                        lbleventlogPC2.Text = dt.Rows(0)("SampleValue").ToString
                        tdlog2.Attributes.Add("class", "tdBlackFont")
                        tdlog2.BgColor = "White"
                    End If
                ElseIf sensorName = lbleventlogPoint3.Text Then
                    If countPC3today = 0 And countPC3_7days = 0 Then
                        lbleventlogDate3.Text = dt.Rows(0)("SampleDate").ToString
                        lbleventlogTime3.Text = dt.Rows(0)("SampleTime").ToString
                        lbleventlogPC3.Text = dt.Rows(0)("SampleValue").ToString
                        tdlog3.Attributes.Add("class", "tdBlackFont")
                        tdlog3.BgColor = "White"
                    End If
                ElseIf sensorName = lbleventlogPoint4.Text Then
                    If countPC4today = 0 And countPC4_7days = 0 Then
                        lbleventlogDate4.Text = dt.Rows(0)("SampleDate").ToString
                        lbleventlogTime4.Text = dt.Rows(0)("SampleTime").ToString
                        lbleventlogPC4.Text = dt.Rows(0)("SampleValue").ToString
                        tdlog4.Attributes.Add("class", "tdBlackFont")
                        tdlog4.BgColor = "White"
                    End If
                ElseIf sensorName = lbleventlogPoint5.Text Then
                    If countPC5today = 0 And countPC5_7days = 0 Then
                        lbleventlogDate5.Text = dt.Rows(0)("SampleDate").ToString
                        lbleventlogTime5.Text = dt.Rows(0)("SampleTime").ToString
                        lbleventlogPC5.Text = dt.Rows(0)("SampleValue").ToString
                        tdlog5.Attributes.Add("class", "tdBlackFont")
                        tdlog5.BgColor = "White"
                    End If
                ElseIf sensorName = lbleventlogPoint6.Text Then
                    If countPC6today = 0 And countPC6_7days = 0 Then
                        lbleventlogDate6.Text = dt.Rows(0)("SampleDate").ToString
                        lbleventlogTime6.Text = dt.Rows(0)("SampleTime").ToString
                        lbleventlogPC6.Text = dt.Rows(0)("SampleValue").ToString
                        tdlog6.Attributes.Add("class", "tdBlackFont")
                        tdlog6.BgColor = "White"
                    End If
                End If

            End If
        Next

        '******Temperature and Humidity********
        'Get 24hours of today latest data that over limit for each temp/hum
        For Each temp As String In conn.GetTempName()

            Dim selectTemptoday As String = " SELECT Top 1 tempid," &
                                            " CASE WHEN alarmtemp=1 Then tempval" &
                                            " WHEN alarmtemp = 0 then 0 " &
                                            " End as tempvalue, Convert(date,CreatedOn) AS CreatedDate,  Convert(time(0),CreatedOn) AS CreatedTime" &
                    " FROM tbtemphum WHERE alarmtemp = 1 AND tempid ='" + temp.Replace(" ", "") + "' AND CONVERT(VARCHAR(25), CreatedOn, 126) LIKE '" + dateNow + "%' " &
                    " ORDER BY CreatedOn DESC "
            Dim dt As New DataTable
            dt = conn.query(selectTemptoday)

            If dt.Rows.Count > 0 Then
                Dim eventDate = DateTime.ParseExact(dt.Rows(0)("CreatedDate"), "dd/mm/yyyy", DateTimeFormatInfo.InvariantInfo).ToString("yyyy-mm-dd")
                Dim sampleTime = DateTime.ParseExact(dt.Rows(0)("CreatedTime").ToString, "HH:mm:ss", Nothing).ToString("h:mm:ss tt")

                If temp = lbleventlog_temp1k_point.Text.Replace(" ", "") Then
                    lbleventlog_temp1k_date.Text = eventDate
                    lbleventlog_temp1k_time.Text = sampleTime
                    lbleventlog_temp1k_pc.Text = Math.Round(Convert.ToDouble(dt.Rows(0)("tempvalue").ToString), 1)
                    tdlogTemp1K.BgColor = "Red"
                    lbleventlog_temp1k_point.Text = "TEMP(°C) 1K"
                    countTemp1Ktoday += 1
                ElseIf temp = lbleventlog_temp10k_point.Text.Replace(" ", "") Then
                    lbleventlog_temp10k_date.Text = eventDate
                    lbleventlog_temp10k_time.Text = sampleTime
                    lbleventlog_temp10k_pc.Text = Math.Round(Convert.ToDouble(dt.Rows(0)("tempvalue").ToString), 1)
                    'lbleventlog_temp10k_pc.Text = String.Format("{0}°C", Math.Round(Convert.ToDouble(dt.Rows(0)("tempvalue").ToString), 1))
                    tdlogTemp10K.BgColor = "Red"
                    lbleventlog_temp10k_point.Text = "TEMP(°C) 10K"
                    countTemp10Ktoday += 1
                End If
            End If
        Next

        For Each hum As String In conn.GetHumName()

            Dim selectHumtoday As String = " SELECT Top 1 humid," &
                                           " CASE WHEN alarmhum=1 Then humval" &
                                           " WHEN alarmhum = 0 then 0 " &
                                           " End as humvalue, Convert(date,CreatedOn) AS CreatedDate,  Convert(time(0),CreatedOn) AS CreatedTime" &
                    " FROM tbtemphum WHERE alarmhum = 1 AND humid ='" + hum.Replace(" ", "") + "' AND CONVERT(VARCHAR(25), CreatedOn, 126) LIKE '" + dateNow + "%' " &
                    " ORDER BY CreatedOn DESC "
            Dim dt As New DataTable
            dt = conn.query(selectHumtoday)

            If dt.Rows.Count > 0 Then
                Dim eventDate = DateTime.ParseExact(dt.Rows(0)("CreatedDate"), "dd/mm/yyyy", DateTimeFormatInfo.InvariantInfo).ToString("yyyy-mm-dd")
                Dim sampleTime = DateTime.ParseExact(dt.Rows(0)("CreatedTime").ToString, "HH:mm:ss", Nothing).ToString("h:mm:ss tt")
                If hum = lbleventlog_Hum1k_point.Text.Replace(" ", "") Then
                    lbleventlog_Hum1k_date.Text = eventDate
                    lbleventlog_Hum1k_time.Text = sampleTime
                    lbleventlog_Hum1k_pc.Text = dt.Rows(0)("humvalue").ToString
                    tdlogHum1K.BgColor = "Red"
                    countHum1Ktoday += 1
                    lbleventlog_Hum1k_point.Text = "HMD(%) 1K"
                ElseIf hum = lbleventlog_Hum10k_point.Text.Replace(" ", "") Then
                    lbleventlog_Hum10k_date.Text = eventDate
                    lbleventlog_Hum10k_time.Text = sampleTime
                    lbleventlog_Hum10k_pc.Text = dt.Rows(0)("humvalue").ToString
                    tdlogHum10K.BgColor = "Red"
                    countHum10Ktoday += 1
                    lbleventlog_Hum10k_point.Text = "HMD(%) 10K"
                End If
            End If
        Next

        'Get past 1 week latest data that over limit for each temp/hum
        For Each temp As String In conn.GetTempName()

            Dim selectTemp7days As String = " SELECT Top 1 tempid," &
                                            " CASE WHEN alarmtemp=1 Then tempval" &
                                            " WHEN alarmtemp = 0 then 0 " &
                                            " End as tempvalue, Convert(date,CreatedOn) AS CreatedDate,  Convert(time(0),CreatedOn) AS CreatedTime" &
                    " FROM tbtemphum WHERE alarmtemp = 1 AND tempid ='" + temp.Replace(" ", "") + "' AND CONVERT(VARCHAR(25), CreatedOn, 126) > DATEADD(day, -7,'" + dateNow + "')" &
                    " AND CONVERT(VARCHAR(25), CreatedOn, 126) < '" + dateNow + "'" &
                    " ORDER BY CreatedOn DESC "
            Dim dt As New DataTable
            dt = conn.query(selectTemp7days)

            If dt.Rows.Count > 0 Then
                Dim test = dt.Rows(0)("CreatedDate")
                Dim eventDate = DateTime.ParseExact(dt.Rows(0)("CreatedDate"), "dd/mm/yyyy", DateTimeFormatInfo.InvariantInfo).ToString("yyyy-mm-dd")
                Dim sampleTime = DateTime.ParseExact(dt.Rows(0)("CreatedTime").ToString, "HH:mm:ss", Nothing).ToString("h:mm:ss tt")
                If temp = lbleventlog_temp1k_point.Text.Replace(" ", "") Then
                    If countTemp1Ktoday = 0 Then
                        lbleventlog_temp1k_date.Text = eventDate
                        lbleventlog_temp1k_time.Text = sampleTime
                        lbleventlog_temp1k_pc.Text = Math.Round(Convert.ToDouble(dt.Rows(0)("tempvalue").ToString), 1)
                        'lbleventlog_temp1k_pc.Text = String.Format("{0}°C", Math.Round(Convert.ToDouble(dt.Rows(0)("tempvalue").ToString), 1))
                        tdlogTemp1K.BgColor = "#FFA623"
                        countTemp1K_7days += 1
                        lbleventlog_temp1k_point.Text = "TEMP(°C) 1K"
                    End If
                ElseIf temp = lbleventlog_temp10k_point.Text.Replace(" ", "") Then
                    If countTemp10Ktoday = 0 Then
                        lbleventlog_temp10k_date.Text = eventDate
                        lbleventlog_temp10k_time.Text = sampleTime
                        lbleventlog_temp10k_pc.Text = Math.Round(Convert.ToDouble(dt.Rows(0)("tempvalue").ToString), 1)
                        'lbleventlog_temp10k_pc.Text = String.Format("{0}°C", Math.Round(Convert.ToDouble(dt.Rows(0)("tempvalue").ToString), 1))
                        tdlogTemp10K.BgColor = "#FFA623"
                        countTemp10K_7days += 1
                        lbleventlog_temp10k_point.Text = "TEMP(°C) 10K"
                    End If
                End If
            End If
        Next

        For Each hum As String In conn.GetHumName()

            Dim selectHum7days As String = " SELECT Top 1 humid," &
                                           " CASE WHEN alarmhum=1 Then humval" &
                                           " WHEN alarmhum = 0 then 0 " &
                                           " End as humvalue, Convert(date,CreatedOn) AS CreatedDate,  Convert(time(0),CreatedOn) AS CreatedTime" &
                    " FROM tbtemphum WHERE alarmhum = 1 AND humid ='" + hum.Replace(" ", "") + "' AND CONVERT(VARCHAR(25), CreatedOn, 126) > DATEADD(day, -7,'" + dateNow + "')" &
                    " AND CONVERT(VARCHAR(25), CreatedOn, 126) < '" + dateNow + "'" &
                    " ORDER BY CreatedOn DESC "
            Dim dt As New DataTable
            dt = conn.query(selectHum7days)

            If dt.Rows.Count > 0 Then
                Dim eventDate = DateTime.ParseExact(dt.Rows(0)("CreatedDate"), "dd/mm/yyyy", DateTimeFormatInfo.InvariantInfo).ToString("yyyy-mm-dd")
                Dim sampleTime = DateTime.ParseExact(dt.Rows(0)("CreatedTime").ToString, "HH:mm:ss", Nothing).ToString("h:mm:ss tt")
                If hum = lbleventlog_Hum1k_point.Text.Replace(" ", "") Then
                    If countHum1Ktoday = 0 Then
                        lbleventlog_Hum1k_date.Text = eventDate
                        lbleventlog_Hum1k_time.Text = sampleTime
                        lbleventlog_Hum1k_pc.Text = dt.Rows(0)("humvalue").ToString
                        'lbleventlog_Hum1k_pc.Text = dt.Rows(0)("humvalue").ToString + "%"
                        lbleventlog_Hum1k_point.Text = "HMD(%) 1K"
                        tdlogHum1K.BgColor = "#FFA623"
                        countHum1K_7days += 1
                    End If
                ElseIf hum = lbleventlog_Hum10k_point.Text.Replace(" ", "") Then
                    If countHum10Ktoday = 0 Then
                        lbleventlog_Hum10k_date.Text = eventDate
                        lbleventlog_Hum10k_time.Text = sampleTime
                        lbleventlog_Hum10k_pc.Text = dt.Rows(0)("humvalue").ToString
                        'lbleventlog_Hum10k_pc.Text = dt.Rows(0)("humvalue").ToString + "%"
                        lbleventlog_Hum10k_point.Text = "HMD(%) 10K"
                        tdlogHum10K.BgColor = "#FFA623"
                        countHum10K_7days += 1
                    End If
                End If
            End If
        Next

        'Get latest data that over limit for each temp/hum after 7 days(monthly data)
        For Each temp As String In conn.GetTempName()

            Dim selectTemp30days As String = " SELECT Top 1 tempid," &
                                            " CASE WHEN alarmtemp=1 Then tempval" &
                                            " WHEN alarmtemp = 0 then 0 " &
                                            " End as tempvalue, Convert(date,CreatedOn) AS CreatedDate,  Convert(time(0),CreatedOn) AS CreatedTime" &
                    " FROM tbtemphum WHERE alarmtemp = 1 AND tempid ='" + temp.Replace(" ", "") + "' AND CONVERT(VARCHAR(25), CreatedOn, 126) <= DATEADD(day,-7, '" + dateNow + "')" &
                    " AND CONVERT(VARCHAR(25), CreatedOn, 126)  >= DATEADD(day, -35,'" + dateNow + "')" &
                    " ORDER BY CreatedOn DESC "
            Dim dt As New DataTable
            dt = conn.query(selectTemp30days)

            If dt.Rows.Count > 0 Then
                Dim eventDate = DateTime.ParseExact(dt.Rows(0)("CreatedDate"), "dd/mm/yyyy", DateTimeFormatInfo.InvariantInfo).ToString("yyyy-mm-dd")
                Dim sampleTime = DateTime.ParseExact(dt.Rows(0)("CreatedTime").ToString, "HH:mm:ss", Nothing).ToString("h:mm:ss tt")
                If temp = lbleventlog_temp1k_point.Text.Replace(" ", "") Then
                    If countTemp1K_7days = 0 And countTemp1Ktoday = 0 Then
                        lbleventlog_temp1k_date.Text = eventDate
                        lbleventlog_temp1k_time.Text = sampleTime
                        'lbleventlog_temp1k_pc.Text = String.Format("{0}°C", Math.Round(Convert.ToDouble(dt.Rows(0)("tempvalue").ToString), 1))
                        lbleventlog_temp1k_pc.Text = Math.Round(Convert.ToDouble(dt.Rows(0)("tempvalue").ToString), 1)
                        tdlogTemp1K.BgColor = "White"
                        tdlogTemp1K.Attributes.Add("class", "tdBlackFont")
                        lbleventlog_temp1k_point.Text = "TEMP(°C) 1K"
                    End If
                ElseIf temp = lbleventlog_temp10k_point.Text.Replace(" ", "") Then
                    If countTemp10K_7days = 0 And countTemp10Ktoday = 0 Then
                        lbleventlog_temp10k_date.Text = eventDate
                        lbleventlog_temp10k_time.Text = sampleTime
                        lbleventlog_temp10k_pc.Text = Math.Round(Convert.ToDouble(dt.Rows(0)("tempvalue").ToString), 1)
                        tdlogTemp10K.BgColor = "White"
                        tdlogTemp10K.Attributes.Add("class", "tdBlackFont")
                        lbleventlog_temp10k_point.Text = "TEMP(°C) 10K"
                    End If
                End If
            End If
        Next

        For Each hum As String In conn.GetHumName()

            Dim selectHum30days As String = " SELECT Top 1 humid," &
                                            " CASE WHEN alarmhum=1 Then humval" &
                                            " WHEN alarmhum = 0 then 0 " &
                                            " End as humvalue, Convert(date,CreatedOn) AS CreatedDate,  Convert(time(0),CreatedOn) AS CreatedTime" &
                    " FROM tbtemphum WHERE alarmhum = 1 AND humid ='" + hum.Replace(" ", "") + "' AND CONVERT(VARCHAR(25), CreatedOn, 126) <= DATEADD(day,-7, '" + dateNow + "')" &
                    " AND CONVERT(VARCHAR(25), CreatedOn, 126)  >= DATEADD(day, -35,'" + dateNow + "')" &
                    " ORDER BY CreatedOn DESC "
            Dim dt As New DataTable
            dt = conn.query(selectHum30days)

            If dt.Rows.Count > 0 Then
                Dim eventDate = DateTime.ParseExact(dt.Rows(0)("CreatedDate"), "dd/mm/yyyy", DateTimeFormatInfo.InvariantInfo).ToString("yyyy-mm-dd")
                Dim sampleTime = DateTime.ParseExact(dt.Rows(0)("CreatedTime").ToString, "HH:mm:ss", Nothing).ToString("h:mm:ss tt")
                If hum = lbleventlog_Hum1k_point.Text.Replace(" ", "") Then
                    If countHum1K_7days = 0 And countHum1Ktoday = 0 Then
                        lbleventlog_Hum1k_date.Text = eventDate
                        lbleventlog_Hum1k_time.Text = sampleTime
                        lbleventlog_Hum1k_pc.Text = dt.Rows(0)("humvalue").ToString
                        tdlogHum1K.BgColor = "White"
                        tdlogHum1K.Attributes.Add("class", "tdBlackFont") 'change black font
                        lbleventlog_Hum1k_point.Text = "HMD(%) 1K"
                    End If
                ElseIf hum = lbleventlog_Hum10k_point.Text.Replace(" ", "") Then
                    If countHum10K_7days = 0 And countHum10Ktoday = 0 Then
                        lbleventlog_Hum10k_date.Text = eventDate
                        lbleventlog_Hum10k_time.Text = sampleTime
                        lbleventlog_Hum10k_pc.Text = dt.Rows(0)("humvalue").ToString
                        tdlogHum10K.BgColor = "White"
                        tdlogHum10K.Attributes.Add("class", "tdBlackFont")
                        lbleventlog_Hum10k_point.Text = "HMD(%) 10K"
                    End If
                End If
            End If
        Next

        'No event log (Particle Counter)
        If lbleventlogDate1.Text = "No event" Then
            tdlog1.BgColor = "#524f56"
            tdlog1.Attributes.Add("class", "tdID") 'change white font
        End If
        If lbleventlogDate2.Text = "No event" Then
            tdlog2.BgColor = "#524f56"
            tdlog2.Attributes.Add("class", "tdID")
        End If
        If lbleventlogDate3.Text = "No event" Then
            tdlog3.BgColor = "#524f56"
            tdlog3.Attributes.Add("class", "tdID")
        End If
        If lbleventlogDate4.Text = "No event" Then
            tdlog4.BgColor = "#524f56"
            tdlog4.Attributes.Add("class", "tdID")
        End If
        If lbleventlogDate5.Text = "No event" Then
            tdlog5.BgColor = "#524f56"
            tdlog5.Attributes.Add("class", "tdID")
        End If
        If lbleventlogDate6.Text = "No event" Then
            tdlog6.BgColor = "#524f56"
            tdlog6.Attributes.Add("class", "tdID")
        End If

        'No event log (Temperature/Humidity)
        If lbleventlog_temp1k_date.Text = "No event" Then
            tdlogTemp1K.BgColor = "#524f56"
            tdlogTemp1K.Attributes.Add("class", "tdID") 'change white font
            lbleventlog_temp1k_point.Text = "TEMP(°C) 1K"
        End If
        If lbleventlog_Hum1k_date.Text = "No event" Then
            tdlogHum1K.BgColor = "#524f56"
            tdlogHum1K.Attributes.Add("class", "tdID")
            lbleventlog_Hum1k_point.Text = "HMD(%) 1K"
        End If
        If lbleventlog_temp10k_date.Text = "No event" Then
            tdlogTemp10K.BgColor = "#524f56"
            tdlogTemp10K.Attributes.Add("class", "tdID")
            lbleventlog_temp10k_point.Text = "TEMP(°C) 10K"
        End If
        If lbleventlog_Hum10k_date.Text = "No event" Then
            tdlogHum10K.BgColor = "#524f56"
            tdlogHum10K.Attributes.Add("class", "tdID")
            lbleventlog_Hum10k_point.Text = "HMD(%) 10K"
        End If
    End Sub

    Sub Get_Limit_Set()
        Dim limit As SensorLimitation = conn.GetSensorLimitation
        limit_pc_min = limit.pc_min
        limit_pc_max = limit.pc_max
        limit_temp_min = limit.temp_min
        limit_temp_max = limit.temp_max
        limit_temp_min_10k = limit.temp_min_10k
        limit_temp_max_10k = limit.temp_max_10k
        limit_hum_min = limit.hum_min
        limit_hum_max = limit.hum_max

        lblParticleCountMin.Text = limit_pc_min
        lblParticleCountMax.Text = limit_pc_max
        lblTemperatureMin.Text = limit_temp_min
        lblTemperatureMax.Text = limit_temp_max
        lblTemperatureMin_10K.Text = limit_temp_min_10k
        lblTemperatureMax_10K.Text = limit_temp_max_10k
        lblHumidityMin.Text = limit_hum_min
        lblHumidityMax.Text = limit_hum_max
    End Sub


    'Public Shared Sub sendShortEmail()
    '    Dim client As SmtpClient = New SmtpClient("smtp-mail.outlook.com")
    '    client.Port = 587
    '    client.DeliveryMethod = SmtpDeliveryMethod.Network
    '    client.UseDefaultCredentials = False
    '    Dim credentials As System.Net.NetworkCredential = New System.Net.NetworkCredential("yhooithin8905@outlook.com", "testing89")
    '    client.EnableSsl = True
    '    client.Credentials = credentials

    '    Try
    '        'Dim mail As MailMessage = New MailMessage()
    '        Dim mail As MailMessage = New MailMessage("yhooithin8905@outlook.com", "yhooithin05@yahoo.com")
    '        mail.Subject = "Testing Outlook sending"
    '        mail.Body = "Hello ht"
    '        mail.IsBodyHtml = True
    '        client.Send(mail)
    '    Catch ex As Exception
    '        Throw ex
    '    End Try
    'End Sub



End Class