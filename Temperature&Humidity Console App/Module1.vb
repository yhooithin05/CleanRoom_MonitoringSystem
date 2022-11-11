Imports System
Imports System.IO
Imports System.Data
Imports System.Configuration
Imports System.Text.RegularExpressions
Imports System.Web


Module Module1

    Dim limit_temp_min As Double
    Dim limit_temp_max As Double
    Dim limit_temp_min_10k As Double
    Dim limit_temp_max_10k As Double
    Dim limit_hum_min As Double
    Dim limit_hum_max As Double

    Dim conn As New ConnSensor

    'Class consist of particlecounter details
    Public Class TempHum

        Public Const TempName_index As Integer = 1
        Public Const TempValue_index As Integer = 2
        Public Const HumName_index As Integer = 3
        Public Const HumValue_index As Integer = 4
        Public Const UpdatedTime_index As Integer = 5
    End Class


    Sub Main()

        Get_Limit_Set()

        'Temperature and Humidity
        ReadWriteTempHum()

    End Sub



    Public Function set_alarm_temp(tempvalue As Double, id As String)
        If tempvalue <> 0 Or tempvalue <> Nothing Then
            If id = "10K" Then
                If tempvalue < limit_temp_min_10k Or tempvalue > limit_temp_max_10k Then
                    Return 1 'Flag red alarm
                Else
                    Return 0
                End If
            Else
                If tempvalue < limit_temp_min Or tempvalue > limit_temp_max Then
                    Return 1 'Flag red alarm
                Else
                    Return 0
                End If
            End If
        Else
            Return 0
        End If
    End Function

    Public Function set_alarm_hum(humvalue As Double)
        If humvalue <> 0 Or humvalue <> Nothing Then

            If humvalue < limit_hum_min Then
                Return 1 'Flag red alarm
            ElseIf humvalue > limit_hum_max Then
                Return 1 'Flag red alarm
            Else
                Return 0
            End If
        Else
            Return 0
        End If
    End Function

    Sub Get_Limit_Set()

        Dim limit As ConnSensor = New ConnSensor()
        limit_temp_min = ConfigurationManager.AppSettings("temp_min").ToString()
        limit_temp_max = ConfigurationManager.AppSettings("temp_max").ToString()
        limit_temp_min_10k = ConfigurationManager.AppSettings("temp_min_10k").ToString()
        limit_temp_max_10k = ConfigurationManager.AppSettings("temp_max_10k").ToString()
        limit_hum_min = ConfigurationManager.AppSettings("hum_min").ToString()
        limit_hum_max = ConfigurationManager.AppSettings("hum_max").ToString()

    End Sub


    Public Function ReadWriteTempHum()

        Dim openPath As String = ConfigurationManager.AppSettings("Sensor_RealTime_Filepath").ToString()
        Dim sensorFile As Excel = New Excel(openPath, 1, 1)
        Dim i As Integer
        Dim lastTempId As String = ""
        Dim lastTempVal As String = ""
        Dim lastHumId As String = ""
        Dim lastHumVal As String = ""
        Dim lastUpdatedTime As String = ""
        Dim flagtemp As Integer
        Dim flaghum As Integer
        Dim lastCellIndex As Integer = sensorFile.GetRowCount() ' get total row number
        Dim TargetCellIndex As Integer
        Dim csvTempId As String
        Dim csvTempVal As String
        Dim csvHumId As String
        Dim csvHumVal As String
        Dim csvUpdatedTime As String

        Try
            'Get last row of data 
            Dim getLastUpdatedDate As String = "Select Top 1 tempid,tempval,humid,humval,CreatedOn FROM tbtemphum ORDER BY id DESC"
            Dim dt As New DataTable
            dt = conn.query(getLastUpdatedDate)
            If dt.Rows.Count > 0 Then
                lastTempId = dt.Rows(0)("tempid")
                lastTempVal = dt.Rows(0)("tempval")
                lastHumId = dt.Rows(0)("humid")
                lastHumVal = dt.Rows(0)("humval")
                lastUpdatedTime = dt.Rows(0)("CreatedOn")
            End If

            'Searching and compare csv last row data and database last row data is match
            For i = lastCellIndex To 2 Step -1
                csvTempId = sensorFile.ReadCellstr(i, TempHum.TempName_index).ToString
                csvTempVal = sensorFile.ReadCellstr(i, TempHum.TempValue_index).ToString
                csvHumId = sensorFile.ReadCellstr(i, TempHum.HumName_index).ToString
                csvHumVal = sensorFile.ReadCellstr(i, TempHum.HumValue_index).ToString
                csvUpdatedTime = sensorFile.ReadCellstr(i, TempHum.UpdatedTime_index).ToString
                If csvTempId = lastTempId Then
                    If csvTempVal = lastTempVal Then
                        If csvHumId = lastHumId Then
                            If csvHumVal = lastHumVal Then
                                If csvUpdatedTime = lastUpdatedTime Then
                                    TargetCellIndex = i + 1  'start from next row to be insert
                                    Console.WriteLine("Found last timestamp!")
                                    Exit For
                                End If
                            End If
                        End If
                    End If
                End If
            Next i

            If Not TargetCellIndex = 0 Then
                For i = TargetCellIndex To lastCellIndex Step +1
                    Dim tempId = sensorFile.ReadCellstr(i, TempHum.TempName_index).ToString
                    Dim tempVal = sensorFile.ReadCellstr(i, TempHum.TempValue_index).ToString
                    Dim humId = sensorFile.ReadCellstr(i, TempHum.HumName_index)
                    Dim humVal = sensorFile.ReadCellstr(i, TempHum.HumValue_index)
                    Dim createdTime = sensorFile.ReadCellstr(i, TempHum.UpdatedTime_index)

                    Console.WriteLine("Uploading " + tempId + "," + humId + "," + " details...")

                    If tempVal <> "" Or tempVal <> Nothing Then
                        flagtemp = set_alarm_temp(tempVal, tempId) 'Get Flag alarm result
                    Else
                        flagtemp = 0
                    End If
                    If humVal <> "" Or humVal <> Nothing Then
                        flaghum = set_alarm_hum(humVal)
                    Else
                        flaghum = 0
                    End If


                    'Validation checking for any empty value
                    'Insert temperature and humidity value into database
                    If Not ((tempVal = "" Or tempVal = Nothing) And (humVal = "" Or humVal = Nothing)) Then
                        WriteTempHum(tempId.ToString, humId.ToString, tempVal, humVal, flagtemp, flaghum, createdTime)
                        WriteToLogFile("Inserted : Temp" + tempId.ToString + "," + tempVal + ", AlarmTemp:" + flagtemp.ToString + ",Hum" + humId.ToString + "," + humVal + ", AlarmHum:" + flaghum.ToString + "," + conn.myNow + "")

                    ElseIf (tempVal <> "" Or tempVal <> Nothing) And (humVal = "" Or humVal = Nothing) Then
                        WriteTempHum(tempId.ToString, humId.ToString, tempVal, "", flagtemp, flaghum, "" + createdTime + "")
                        WriteToLogFile("Inserted : Temp" + tempId.ToString + "," + tempVal + ",Hum" + humId.ToString + "," + "" + "," + conn.myNow + "")
                    ElseIf (tempVal = "" Or tempVal = Nothing) And (humVal <> "" Or humVal <> Nothing) Then
                        WriteTempHum(tempId.ToString, humId.ToString, "", humVal, flagtemp, flaghum, "" + createdTime + "")
                        WriteToLogFile("Inserted : Temp" + tempId.ToString + ", Empty Value" + ",Hum" + humId.ToString + "," + humVal + "," + conn.myNow + "")
                    Else
                        WriteToLogFile("Temperature and Humidity data is empty on " + conn.myNow + " ")
                    End If

                Next i
                WriteToLogFile(conn.myNow + "- Insert TemperatureHumidity Is success loaded")
            Else
                'Console.WriteLine("Last Timestamp not found!")
                WriteToLogFile(conn.myNow + "- Last Timestamp not found!")
            End If

            sensorFile.CloseWb()
        Catch ex As Exception
            Console.WriteLine("Insert_Particle_Counter - row " + i.ToString + " fail to load (Error:   " + ex.ToString + " )")
            WriteToLogFile("Insert_Particle_Counter-row " + i.ToString + " fail to load (Error:  " + ex.ToString + " )")
            sensorFile.CloseWb()
        End Try

    End Function

    Sub WriteTempHum(TempID As String, HumID As String, strHtmTemp As String, strHtmlHum As String, alarmTemp As String, alarmHum As String, createdOn As String)
        Try
            Dim TempVal As String = strHtmTemp ' Need to get the correct Temperature value from webstring
            Dim HumVal As String = strHtmlHum ' Need to get the correct Humidity value from webstring

            If TempID = "Temp10K" Then
                Dim insert1 As String = String.Format(
               " INSERT INTO tbtemphum (tempid, tempval, temp_min, temp_max, alarmtemp, humid, humval,hum_min, hum_max,alarmhum, CreatedOn ) " &
               " VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                TempID, TempVal, limit_temp_min_10k, limit_temp_max_10k, alarmTemp, HumID, HumVal, limit_hum_min, limit_hum_max, alarmHum, createdOn)
                conn.nonquery(insert1)
            Else
                Dim insert1 As String = String.Format(
                " INSERT INTO tbtemphum (tempid, tempval, temp_min, temp_max, alarmtemp, humid, humval,hum_min, hum_max,alarmhum, CreatedOn ) " &
                " VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')",
                TempID, TempVal, limit_temp_min, limit_temp_max, alarmTemp, HumID, HumVal, limit_hum_min, limit_hum_max, alarmHum, createdOn)
                conn.nonquery(insert1)
            End If
        Catch ex As Exception
            WriteToLogFile(ex.ToString)
        End Try

    End Sub


    Public Sub WriteToLogFile(ByVal text As String)
        Dim log As StreamWriter
        Dim now As DateTime = DateTime.Now
        Dim format As String = "yyyyMMdd"
        Dim today As String = now.ToString(format)

        Dim logFilePath As String = ConfigurationManager.AppSettings("Sensor_LogFile_Filepath").ToString()
        Dim str As String = "" & logFilePath & "log_" & today & ".txt"

        'Auto generate textfile if not exist
        If Not File.Exists("" & logFilePath & "log_" & today & ".txt") Then
            Using stream As StreamWriter = New StreamWriter(str, False)
                stream.Write("LogFile/log_" & today & ".txt")
                stream.Close()
            End Using
        Else
            log = File.AppendText("" & logFilePath & "log_" & today & ".txt")
            log.WriteLine(text)
            log.Close()
        End If


    End Sub
End Module


