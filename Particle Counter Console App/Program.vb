Imports System
Imports System.IO
Imports System.Data
Imports System.Configuration
Imports System.Text.RegularExpressions
Imports System.Web
Module Program

    Dim limit_pc_min As Double
    Dim limit_pc_max As Double
    Dim pc_category As String

    Dim conn As New ConnSensor

    'Class consist of particlecounter details
    Public Class ParticleCounter

        Public Property Sample_Date As String
        Public Property Sample_Time As String
        Public Property Sensor_Name As String
        Public Property Sample_Value As Integer

        Public Const Sample_Date_index As Integer = 1
        Public Const Sample_Time_index As Integer = 2
        Public Const Sensor_Name_index As Integer = 3
        Public Const Sample_Value_index As Integer = 4
        Public Const LastUpdated_index As Integer = 5
    End Class


    Sub Main()

        Get_Limit_Set()

        ' PARTICLE COUNTER
        ReadWriteParticleCounter()

    End Sub


    'Assign alarm based on the sensor values
    Public Function set_alarm_pc(value As Double, sensorName As String)

        Dim sensorCategory As String = conn.exscalar("SELECT Category FROM tbsensor WHERE sensorName = '" + sensorName + "'")

        If sensorCategory = "1K" Then
            If value > limit_pc_min Then
                Return 1 'Flag red alarm
            Else
                Return 0
            End If

        Else
            If value > limit_pc_max Then
                Return 1 'Flag red alarm
            Else
                Return 0
            End If
        End If

    End Function

    Sub Get_Limit_Set()

        Dim limit As ConnSensor = New ConnSensor()
        limit_pc_min = limit.GetSensorLimitation.pc_min
        limit_pc_max = limit.GetSensorLimitation.pc_max

    End Sub


    Public Function ReadWriteParticleCounter()

        Dim openPath As String = ConfigurationManager.AppSettings("Sensor_RealTime_Filepath").ToString()
        Dim sensorFile As Excel = New Excel(openPath, 1, 1)
        Dim i As Integer
        Dim lastUpdatedDate As String = ""
        Dim lastUpdateTime As String = ""
        Dim lastSensorName As String = ""
        Dim lastCellIndex As Integer = sensorFile.GetRowCount() ' get total row number
        Dim TargetCellIndex As Integer
        Dim csvSampleDate As String
        Dim csvSampleTime As String
        Dim csvSensorName As String

        Try
            'Get last row of data 
            Dim getLastUpdatedDate As String = "Select Top 1 SampleDate,SampleTime,sensorName FROM tbsensordata ORDER BY id DESC"
            Dim dt As New DataTable
            dt = conn.query(getLastUpdatedDate)
            If dt.Rows.Count > 0 Then
                lastUpdatedDate = dt.Rows(0)("SampleDate")
                lastUpdateTime = dt.Rows(0)("SampleTime")
                lastSensorName = dt.Rows(0)("SensorName")
            End If

            'Searching and compare csv last row data and database last row data is match
            For i = lastCellIndex To 2 Step -1
                csvSampleDate = sensorFile.ReadCellstr(i, ParticleCounter.Sample_Date_index).ToString
                csvSampleTime = sensorFile.ReadCellstr(i, ParticleCounter.Sample_Time_index).ToString
                csvSensorName = sensorFile.ReadCellstr(i, ParticleCounter.Sensor_Name_index).ToString
                If csvSampleDate = lastUpdatedDate Then
                    If csvSampleTime = lastUpdateTime Then
                        If csvSensorName = lastSensorName Then
                            TargetCellIndex = i + 1  'start from next row to be insert
                            Console.WriteLine("Found last timestamp!")
                            Exit For
                        End If
                    End If
                End If
            Next i

            If Not TargetCellIndex = 0 Then
                For i = TargetCellIndex To lastCellIndex Step +1
                    Dim sampleDate = sensorFile.ReadCellstr(i, ParticleCounter.Sample_Date_index).ToString
                    Dim sampleTime = sensorFile.ReadCellstr(i, ParticleCounter.Sample_Time_index).ToString
                    Dim sensorName = sensorFile.ReadCellstr(i, ParticleCounter.Sensor_Name_index)
                    Dim lastUpdated = sensorFile.ReadCellstr(i, ParticleCounter.LastUpdated_index)

                    Dim tmp As String = sensorFile.ReadCellstr(i, ParticleCounter.Sensor_Name_index)
                    Console.WriteLine("Uploading " + sampleDate + "," + sampleTime + "," + tmp + " details...")

                    Dim sensorArea As ConnSensor = New ConnSensor()

                    pc_category = sensorArea.GetSensorArea(sensorName).pc_category

                    Dim pcValue As Double = sensorFile.ReadCelldouble(i, ParticleCounter.Sample_Value_index)
                    'need to be multiplied by 10, due to the result file not considering in the flow rate of the particle sensors.
                    Dim actualpcValue As Double = pcValue * 10

                    Dim flag As Integer = set_alarm_pc(actualpcValue, tmp) 'Get Flag alarm result

                    If flag = 1 Then
                        WriteToSensor(sampleDate, sampleTime, sensorName, actualpcValue, "1", pc_category, lastUpdated)
                    Else
                        WriteToSensor(sampleDate, sampleTime, sensorName, actualpcValue, "0", pc_category, lastUpdated)
                    End If
                    'End If
                Next i
                WriteToLogFile(conn.myNow + "- Insert_Particle_Counter Is success loaded")
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

    Sub WriteToSensor(SampleDate As String, SampleTime As String, SensorName As String, SampleValue As String, Alarm As String, Area As String, lastupdatedtime As String)

        Dim insert1 As String = String.Format(
            " INSERT INTO tbsensordata (SampleDate, SampleTime, SensorName, SampleValue, UpdatedOn,Alarm, Area) " &
            " VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", SampleDate, SampleTime, SensorName, SampleValue, lastupdatedtime, Alarm, Area)
        conn.nonquery(insert1)
        WriteToLogFile("Inserted : " + SampleDate + ", " + SampleTime + "," + SensorName + "," + SampleValue + "," + conn.myNow + "," + Area + "")
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


        log.WriteLine(text)
        log.Close()

    End Sub



End Module

