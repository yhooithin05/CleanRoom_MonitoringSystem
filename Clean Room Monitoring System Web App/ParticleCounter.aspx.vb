Imports System.IO
Imports FusionCharts.Charts
Imports System.Text
Imports System.Collections.Generic
Imports System.Net
Imports System
Imports System.Data

Public Class ParticleCounter
    Inherits System.Web.UI.Page

    Dim conn As New Conn
    Const dateNow = "2022-11-01"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("PageTitle") = "Particle Counter Sensor Analysis"

        If Not Page.IsPostBack Then

            btndaily.InnerText = " Today " + conn.todayDate()
            btndaily.Attributes.Add("style", "background:#291B44;color:white")
            bind_1k_chart("sampledate = '" + dateNow + "'", "Daily")
            bind_10k_chart("sampledate = '" + dateNow + "'", "Daily")
            bind_1k_gridview("sampledate = '" + dateNow + "'")
            bind_10k_gridview("sampledate = '" + dateNow + "'")

            Convert_1K_ToVertical()
            Convert_10K_ToVertical()
            hdfPeriod.Value = "Daily"

        End If

    End Sub

    Protected Sub Page_Unload(sender As Object, e As System.EventArgs) Handles Me.Unload
        conn.closeSql()
    End Sub

    'Refresh daily chart for latest plotting
    Protected Sub btnRefresh_Click(sender As Object, e As System.EventArgs) Handles btnrefresh.ServerClick
        btnweekly.Attributes.Remove("style")
        btndaily.Attributes.Add("style", "background:#291B44;color:white")
        bind_1k_chart("sampledate = '" + dateNow + "'", "Daily")
        bind_10k_chart("sampledate = '" + dateNow + "'", "Daily")
        bind_1k_gridview("sampledate = '" + dateNow + "'")
        bind_10k_gridview("sampledate = '" + dateNow + "'")

        Convert_1K_ToVertical()
        Convert_10K_ToVertical()
        dvddldate.Style.Add("display", "none")
        hdfPeriod.Value = "Daily"
    End Sub

    Protected Sub btnDaily_Click(sender As Object, e As System.EventArgs) Handles btndaily.ServerClick
        GenerateDailyData()
    End Sub

    Sub GenerateDailyData()
        If btnweekly.Attributes("style") = "background:#291B44;color:white" Then
            btnweekly.Attributes.Remove("style")
            btndaily.Attributes.Add("style", "background:#291B44;color:white")
        End If

        bind_1k_chart("sampledate = '" + dateNow + "'", "Daily")
        bind_10k_chart("sampledate = '" + dateNow + "'", "Daily")
        bind_1k_gridview("sampledate = '" + dateNow + "'")
        bind_10k_gridview("sampledate = '" + dateNow + "'")

        Convert_1K_ToVertical()
        Convert_10K_ToVertical()
        dvddldate.Style.Add("display", "none")
        hdfPeriod.Value = "Daily"

    End Sub

    Protected Sub btnWeekly_Click(sender As Object, e As System.EventArgs) Handles btnweekly.ServerClick

        If btndaily.Attributes("style") = "background:#291B44;color:white" Then
            btndaily.Attributes.Remove("style")
            btnweekly.Attributes.Add("style", "background:#291B44;color:white")
        End If
        div1KChart.Style.Add("display", "none") 'hide existing 1k chart
        div10KChart.Style.Add("display", "none") 'hide existing 10k chart
        gv1KAlarm.Style.Add("display", "none") 'hide alarm table 1k        
        gv10KAlarm.Style.Add("display", "none") 'hide alarm table 10k
        dvddldate.Style.Add("display", "block") 'visible calendar     
        hdfPeriod.Value = "Date"

    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As System.EventArgs) Handles btnsearch.ServerClick
        btndaily.Attributes.Remove("style")

        If txtDateFrom.Text = "" Or txtDateTo.Text = "" Then
            Response.Write("<script>alert('Please fill up both date')</script>")
        Else
            Dim date_period As String = "sampledate >= '" + txtDateFrom.Text + "' AND sampledate <= '" + txtDateTo.Text + "'"
            Dim period_basis As String = "Date"

            bind_1k_chart(date_period, period_basis)
            bind_10k_chart(date_period, period_basis)
            bind_1k_gridview(date_period)
            bind_10k_gridview(date_period)
            Convert_1K_ToVertical()
            Convert_10K_ToVertical()
        End If

    End Sub

    Protected Sub btnExport_Click(sender As Object, e As System.EventArgs) Handles btnexport.ServerClick
        ExportToExcel()
    End Sub

    Protected Sub ExportToExcel()
        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"


        Dim date_period As String = Nothing
        Using sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)

            If hdfPeriod.Value = "Daily" Then
                date_period = "sampledate = '" + dateNow + "'"
                Response.AddHeader("content-disposition", "attachment; filename= " + dateNow + "_ParticleCounterDailyData.xls")
            ElseIf hdfPeriod.Value = "Date" Then
                date_period = "sampledate >= '" + txtDateFrom.Text + "' AND sampledate <= '" + txtDateTo.Text + "'"
                Response.AddHeader("content-disposition", "attachment; filename= " + txtDateFrom.Text + "_TO_" + txtDateTo.Text + "_ParticleCounterData.xls")
            End If

            Me.bind_export_gridview(date_period)

            gvPCExport.RenderControl(hw)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.[End]()

        End Using
    End Sub

    Public Overrides Sub VerifyRenderingInServerForm(control As Control)
        ' Verifies that the control is rendered
    End Sub


    'show chart with Pt 1 0.5 to Pt 4 0.5
    Sub bind_1k_chart(ByVal sampleDate As String, ByVal period As String)
        Dim schema As String
        Dim data As New StringBuilder
        Dim dataMarker As New StringBuilder
        Dim fusionTable As FusionTable
        Dim timeSeries As TimeSeries
        Dim chart As Chart

        Using client As WebClient = New WebClient()
            'data = client.DownloadString("https://s3.eu-central-1.amazonaws.com/fusion.store/ft/data/line-chart-with-time-axis-data.json")
            'schema = client.DownloadString("https://s3.eu-central-1.amazonaws.com/fusion.store/ft/schema/line-chart-with-time-axis-schema.json")
            'test data
            'data = ("[['Pt 1 0.5','2021-01-10 6:01:01',886],['Pt 1 0.5','2021-01-10 6:02:39',217]," +
            '         "['Pt 1 0.5','2021-01-10 6:03:30',208],['Pt 2 0.5','2021-01-11 6:04:29',153]," +
            '         "['Pt 2 0.5','2021-01-11 6:05:22',498]," +
            '         "['Pt 2 0.5','2021-01-11 6:06:24',466]," +
            '         "['Pt 2 0.5','2021-01-12 6:07:21',104]," +
            '         "['Pt 3 0.5','2021-01-12 6:08:34',81]," +
            '         "['Pt 3 0.5','2021-01-13 6:09:35',736]," +
            '         "['Pt 3 0.5','2021-01-13 6:30:20',899]]")
            schema = ("[{name:'SensorName',type:'string'}, {name: 'Time',type:'date', format:'%Y-%m-%d %I:%M:%S %p'}, {name:'Particle Counter Value',type:'number'}] ")

            Dim selectPCounter As String = String.Format(" SELECT * FROM tbsensordata where {0} AND Area= '1K'", sampleDate)
            'Dim selectPCounter As String = String.Format(" SELECT * FROM tbsensordata where {0} AND SensorName <> 'Pt 5' AND SensorName <> 'Pt 6'", "sampledate = '2021-02-15'")
            Dim dt As New DataTable
            dt = conn.query(selectPCounter)
            If dt.Rows.Count > 0 Then
                data.Append("[")
                For i As Integer = 0 To dt.Rows.Count - 1
                    If i <> dt.Rows.Count - 1 Then
                        data.Append("['" + dt.Rows(i)("SensorName").ToString + "','" + dt.Rows(i)("SampleDate").ToString + " " + dt.Rows(i)("SampleTime").ToString + "'," +
                            "'" + dt.Rows(i)("SampleValue").ToString + "'],")
                    Else
                        data.Append("['" + dt.Rows(i)("SensorName").ToString + "','" + dt.Rows(i)("SampleDate").ToString + " " + dt.Rows(i)("SampleTime").ToString + "'," +
                            "'" + dt.Rows(i)("SampleValue").ToString + "']]")
                    End If
                Next
                fusionTable = New FusionTable(schema, data.ToString)
                timeSeries = New TimeSeries(fusionTable)
                Dim pcMin As String = conn.exscalar("SELECT pc_min FROM tbsetting")
                timeSeries.AddAttribute("caption", "{ text:'Particle Counter (1K) - Refreshed every 1 min' }")
                timeSeries.AddAttribute("subcaption", "{text:'" + period + "'}")
                timeSeries.AddAttribute("navigator", "{enabled:'1'}") 'close time navigator    
                timeSeries.AddAttribute("yAxis", "[{plot: {value:  'Particle Counter Value',type:'line'},title:  'Particle Counter Value',showgridband:'1', " &
                                        "series:  'SensorName',referenceline:[{label:'Area 1K',value:'" + pcMin + "'}],style:{'grid-band':{fill:'#f5f5ef'}} }]")

                Dim selectAlarm As String = String.Format(" SELECT * FROM tbsensordata where {0} AND Alarm=1", sampleDate)
                Dim dt2 As New DataTable
                dt2 = conn.query(selectAlarm)
                If dt2.Rows.Count > 0 Then
                    dataMarker.Append("[")
                    For i As Integer = 0 To dt2.Rows.Count - 1
                        If i <> dt2.Rows.Count - 1 Then
                            dataMarker.Append("{time:'" + dt2.Rows(i)("SampleDate").ToString + " " + dt2.Rows(i)("SampleTime").ToString + "', " &
                                              "type:'pin',timeformat:'%Y-%m-%d %I:%M:%S %p',series:{SensorName: '" + dt2.Rows(i)("SensorName").ToString + "'}}, ")
                        Else
                            dataMarker.Append("{time:'" + dt2.Rows(i)("SampleDate").ToString + " " + dt2.Rows(i)("SampleTime").ToString + "', " &
                                             "type:'pin',timeformat:'%Y-%m-%d %I:%M:%S %p',series:{SensorName: '" + dt2.Rows(i)("SensorName").ToString + "'}}] ")
                        End If
                    Next
                    timeSeries.AddAttribute("datamarker", dataMarker.ToString)
                End If

                chart = New Chart("timeseries", "particlecounter_1k_chart", "1800", "500", "json", timeSeries)

                div1KChart.Style.Add("display", "block")
                gv1KAlarm.Style.Add("display", "block")
                lbl1KNoData.Text = ""
                lt1kChart.Text = chart.Render()
                btnexport.Visible = True
                btnexportdisabled.Visible = False
                'lblEmptyData.Visible = True ' pending wait marker
            Else

                data.Append("[[]]")
                fusionTable = New FusionTable(schema, data.ToString)
                timeSeries = New TimeSeries(fusionTable)
                chart = New Chart("timeseries", "particlecounter_1k_chart", "1800", "500", "json", timeSeries)
                div1KChart.Style.Add("display", "block")
                lt1kChart.Text = chart.Render()
                lbl1KNoData.Text = "No record found in Particle Counter 1K Chart"
                gv1KAlarm.Style.Add("display", "none")
                btnexport.Visible = False
                btnexportdisabled.Visible = True
                ' btndaily.Attributes.Remove("style")
            End If
        End Using

    End Sub


    'show chart with Pt 5 and Pt 6
    Sub bind_10k_chart(ByVal sampleDate As String, ByVal period As String)
        Dim schema As String
        Dim data As New StringBuilder
        Dim dataMarker As New StringBuilder
        Dim fusionTable As FusionTable
        Dim timeSeries As TimeSeries

        Using client As WebClient = New WebClient()
            schema = ("[{name:'SensorName',type:'string'}, {name: 'Time',type:'date', format:'%Y-%m-%d %I:%M:%S %p'}, {name:'Particle Counter Value',type:'number'}] ")
            Dim selectPCounter As String = String.Format(" SELECT * FROM tbsensordata where {0} AND (Area= '10K')", sampleDate)
            'Dim selectPCounter As String = String.Format(" SELECT * FROM tbsensordata where {0} AND (SensorName = 'Pt 5' OR SensorName = 'Pt 6')", "sampledate = '2021-02-15'")
            Dim dt As New DataTable
            dt = conn.query(selectPCounter)
            If dt.Rows.Count > 0 Then
                data.Append("[")
                For i As Integer = 0 To dt.Rows.Count - 1
                    If i <> dt.Rows.Count - 1 Then
                        data.Append("['" + dt.Rows(i)("SensorName").ToString + "','" + dt.Rows(i)("SampleDate").ToString + " " + dt.Rows(i)("SampleTime").ToString + "'," +
                            "'" + dt.Rows(i)("SampleValue").ToString + "'],")
                    Else
                        data.Append("['" + dt.Rows(i)("SensorName").ToString + "','" + dt.Rows(i)("SampleDate").ToString + " " + dt.Rows(i)("SampleTime").ToString + "'," +
                            "'" + dt.Rows(i)("SampleValue").ToString + "']]")
                    End If
                Next
                fusionTable = New FusionTable(schema, data.ToString)
                timeSeries = New TimeSeries(fusionTable)
                Dim pcMax As String = conn.exscalar("SELECT pc_max FROM tbsetting")

                timeSeries.AddAttribute("caption", "{ text:'Particle Counter (10K) - Refreshed every 1 minutes' }")
                timeSeries.AddAttribute("subcaption", "{text:'" + period + "'}")
                timeSeries.AddAttribute("navigator", "{enabled:'1'}") 'close time navigator    
                'timeSeries.AddAttribute("update-chart-using-API-methods", "{theme:'candy'}")
                'timeSeries.AddAttribute("legend", "{enabled:'1'}")
                'timeSeries.AddAttribute("extensions", "{customRangeSelector:{enabled:'0'}}") 'close calendar
                'timeSeries.AddAttribute("chart", "{exportEnabled:'1',exportfilename:'ParticleCounterReport',exportMode:'server'}") 'enable export feature
                'timeSeries.AddAttribute("xAxis", "{initialinterval:{from: '2021-01-21 12:00:00 AM', to:'2021-01-27 12:00:00 AM'}}")
                timeSeries.AddAttribute("yAxis", "[{plot: {value:  'Particle Counter Value',type:'line'},title:  'Particle Counter Value',showgridband:'1', " &
                                        "series:  'SensorName',referenceline:[{label:'Area 10K',value:'" + pcMax + "'}],style:{'grid-band':{fill:'#f5f5ef'}} }]")
                'timeSeries.AddAttribute("datamarker", @"[{value:'Particle Counter Value',time:'2021-01-11 6:06:24 AM',timeformat:'%Y-%m-%d %H:%M:%S %p'},{value:'Particle Counter Value',time:'2021-01-12 6:08:34 AM',timeformat:'%Y-%m-%d %H:%M:%S %p'}]");
                Dim selectAlarm As String = String.Format(" SELECT * FROM tbsensordata where {0} AND Alarm=1", sampleDate)
                Dim dt2 As New DataTable
                dt2 = conn.query(selectAlarm)
                If dt2.Rows.Count > 0 Then
                    dataMarker.Append("[")
                    For i As Integer = 0 To dt2.Rows.Count - 1
                        If i <> dt2.Rows.Count - 1 Then
                            dataMarker.Append("{time:'" + dt2.Rows(i)("SampleDate").ToString + " " + dt2.Rows(i)("SampleTime").ToString + "', " &
                                              "type:'pin',timeformat:'%Y-%m-%d %I:%M:%S %p',series:{SensorName: '" + dt2.Rows(i)("SensorName").ToString + "'}}, ")
                        Else
                            dataMarker.Append("{time:'" + dt2.Rows(i)("SampleDate").ToString + " " + dt2.Rows(i)("SampleTime").ToString + "', " &
                                             "type:'pin',timeformat:'%Y-%m-%d %I:%M:%S %p',series:{SensorName: '" + dt2.Rows(i)("SensorName").ToString + "'}}] ")
                        End If
                    Next
                    timeSeries.AddAttribute("datamarker", dataMarker.ToString)
                End If


                Dim chart As Chart = New Chart("timeseries", "particlecounter_10k_chart", "1800", "500", "json", timeSeries)
                lt10kChart.Text = chart.Render()
                div10KChart.Style.Add("display", "block")
                gv10KAlarm.Style.Add("display", "block")
                lbl10KNoData.Text = ""
                btnexport.Visible = True
                btnexportdisabled.Visible = False

                'lblEmptyData.Visible = True ' pending wait marker
            Else
                data.Append("[[]]")
                fusionTable = New FusionTable(schema, data.ToString)
                timeSeries = New TimeSeries(fusionTable)
                Dim chart As Chart = New Chart("timeseries", "particlecounter_10k_chart", "1800", "500", "json", timeSeries)
                div10KChart.Style.Add("display", "block")
                lt10kChart.Text = chart.Render()
                lbl10KNoData.Text = "No record found in Particle Counter 10K Chart"
                gv10KAlarm.Style.Add("display", "none")
                btnexport.Visible = False
                btnexportdisabled.Visible = True
                'btndaily.Attributes.Remove("style")
            End If

        End Using

    End Sub

    Protected Sub dg1K_ItemBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs)
        If (e.Item.ItemType = ListItemType.Item) Or (e.Item.ItemType = ListItemType.AlternatingItem) Or (e.Item.ItemType = ListItemType.SelectedItem) Then
            e.Item.Cells(0).BackColor = Drawing.Color.Gray
            e.Item.Cells(0).ForeColor = Drawing.Color.White

            For i As Integer = 0 To e.Item.Cells.Count - 1
                If e.Item.Cells(0).Text = "SyncGraph" Then
                    e.Item.Cells(0).ForeColor = Drawing.Color.Gray 'hide the "SyncGraph" text

                    e.Item.Cells(i).Attributes.Add("onclick", "Get1K_SelectedRow(this,'" & dgPCounter_1KAlarm.Items(0).Cells(i).Text & " " & dgPCounter_1KAlarm.Items(1).Cells(i).Text & "');")
                    If e.Item.Cells(i).Text = "Select" Then
                        e.Item.Cells(i).Font.Bold = True
                        e.Item.Cells(i).ForeColor = System.Drawing.ColorTranslator.FromHtml("#225792")
                        e.Item.Cells(i).Attributes.Add("onmouseover", "this.style.cursor = 'pointer';this.style.backgroundColor='#ccffff'") ' pointer and hover with color
                        e.Item.Cells(i).Attributes.Add("onmouseout", "this.style.backgroundColor='#ffffff'")
                    End If
                Else
                    If e.Item.Cells(0).Text = "Date" Or e.Item.Cells(0).Text = "Time" Or e.Item.Cells(0).Text = "Sensor" Or e.Item.Cells(0).Text = "Value" Then
                        e.Item.Cells(0).ForeColor = Drawing.Color.White
                    Else
                        e.Item.Cells(i).ForeColor = Drawing.Color.Black
                    End If

                End If
            Next
        End If
    End Sub

    Protected Sub dg10K_ItemBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs)
        If (e.Item.ItemType = ListItemType.Item) Or (e.Item.ItemType = ListItemType.AlternatingItem) Or (e.Item.ItemType = ListItemType.SelectedItem) Then
            e.Item.Cells(0).BackColor = Drawing.Color.Gray
            e.Item.Cells(0).ForeColor = Drawing.Color.White

            For i As Integer = 0 To e.Item.Cells.Count - 1
                If e.Item.Cells(0).Text = "SyncGraph" Then
                    e.Item.Cells(0).ForeColor = Drawing.Color.Gray 'hide the "SyncGraph" text
                    e.Item.Cells(i).Attributes.Add("onclick", "Get10K_SelectedRow(this,'" & dgPCounter_10KAlarm.Items(0).Cells(i).Text & " " & dgPCounter_10KAlarm.Items(1).Cells(i).Text & "');")
                    If e.Item.Cells(i).Text = "Select" Then
                        e.Item.Cells(i).Font.Bold = True
                        e.Item.Cells(i).ForeColor = System.Drawing.ColorTranslator.FromHtml("#225792")
                        e.Item.Cells(i).Attributes.Add("onmouseover", "this.style.cursor = 'pointer';this.style.backgroundColor='#ccffff'") ' pointer and hover with color
                        e.Item.Cells(i).Attributes.Add("onmouseout", "this.style.backgroundColor='#ffffff'")
                    End If
                Else
                    If e.Item.Cells(0).Text = "Date" Or e.Item.Cells(0).Text = "Time" Or e.Item.Cells(0).Text = "Sensor" Or e.Item.Cells(0).Text = "Value" Then
                        e.Item.Cells(0).ForeColor = Drawing.Color.White
                    Else
                        e.Item.Cells(i).ForeColor = Drawing.Color.Black
                    End If
                End If
            Next
        End If
    End Sub


    Protected Sub bind_1k_gridview(ByVal sampleDate As String)

        Dim select1 As String = String.Format(
       " SELECT SampleDate As Date,SampleTime AS Time, s.SensorName AS Sensor,SampleValue AS Value " &
       " FROM tbsensordata s join tbsensor c on c.sensorname = s.sensorname " &
       " where {0} AND c.category = '1K' AND Alarm ='1'", sampleDate)
        Dim dt As New DataTable
        dt = conn.query(select1)
        dt.Columns.Add("SyncGraph")

        If dt.Rows.Count > 0 Then
            ViewState("dt1K") = dt
            BindGrid(dt, False)
            lbl1KEmptyData.Style.Add("display", "none")
            dgPCounter_1KAlarm.Visible = True
            dgPCounter_1KAlarm.DataSource = dt
            dgPCounter_1KAlarm.DataBind()
        Else
            lbl1KEmptyData.Style.Add("display", "block")
            dgPCounter_1KAlarm.Visible = False
        End If

    End Sub
    Protected Sub bind_10k_gridview(ByVal sampleDate As String)

        Dim select1 As String = String.Format(
        " SELECT SampleDate As Date,SampleTime AS Time, s.SensorName AS Sensor,SampleValue AS Value " &
        " FROM tbsensordata s join tbsensor c on c.sensorname = s.sensorname " &
        " where {0} AND c.category = '10K' AND Alarm ='1'", sampleDate)
        Dim dt As New DataTable
        dt = conn.query(select1)
        dt.Columns.Add("SyncGraph")

        If dt.Rows.Count > 0 Then
            ViewState("dt10K") = dt
            Bind10KGrid(dt, False)
            lbl10KEmptyData.Style.Add("display", "none")
            dgPCounter_10KAlarm.Visible = True
            dgPCounter_10KAlarm.DataSource = dt
            dgPCounter_10KAlarm.DataBind()
        Else
            lbl10KEmptyData.Style.Add("display", "block")
            dgPCounter_10KAlarm.Visible = False
        End If
    End Sub

    Protected Sub bind_export_gridview(ByVal sampleDate As String)

        Dim select1 As String = String.Format(
     " SELECT SampleDate As Date,SampleTime As Time, SensorName,SampleValue As Value FROM tbsensordata  " &
     " where {0} ", sampleDate)
        Dim dt As New DataTable
        dt = conn.query(select1)
        If dt.Rows.Count > 0 Then
            gvPCExport.DataSource = dt
            gvPCExport.DataBind()
            btnexport.Visible = True
            btnexportdisabled.Visible = False
        Else
            btnexport.Visible = False
            btnexportdisabled.Visible = True
        End If

    End Sub

    Private Sub BindGrid(dt As DataTable, rotate As Boolean)

        dgPCounter_1KAlarm.ShowHeader = Not rotate
        dgPCounter_1KAlarm.DataSource = dt
        dgPCounter_1KAlarm.DataBind()
        If rotate Then
            For Each row As DataGridItem In dgPCounter_1KAlarm.Items
                row.Cells(0).CssClass = "header"
            Next
        End If
    End Sub

    Private Sub Bind10KGrid(dt As DataTable, rotate As Boolean)
        dgPCounter_10KAlarm.ShowHeader = Not rotate
        dgPCounter_10KAlarm.DataSource = dt
        dgPCounter_10KAlarm.DataBind()
        If rotate Then
            For Each row As DataGridItem In dgPCounter_10KAlarm.Items
                row.Cells(0).CssClass = "header"
            Next
        End If
    End Sub

    'Rotate gridview to vertical layout
    Public Sub Convert_1K_ToVertical()
        Dim dt As DataTable = DirectCast(ViewState("dt1K"), DataTable)

        If Not dt Is Nothing Then
            Dim dt2 As New DataTable()

            For i As Integer = 0 To dt.Rows.Count
                dt2.Columns.Add()
            Next
            For i As Integer = 0 To dt.Columns.Count - 1
                dt2.Rows.Add()
                dt2.Rows(i)(0) = dt.Columns(i).ColumnName
            Next
            For i As Integer = 0 To dt.Columns.Count - 1
                For j As Integer = 0 To dt.Rows.Count - 1
                    dt2.Rows(i)(j + 1) = dt.Rows(j)(i)
                    If i = 4 Then
                        dt2.Rows(4)(j + 1) = "Select"
                    End If
                Next
            Next
            BindGrid(dt2, True)
        End If
    End Sub

    Public Sub Convert_10K_ToVertical()
        Dim dt As DataTable = DirectCast(ViewState("dt10K"), DataTable)

        If Not dt Is Nothing Then
            Dim dt2 As New DataTable()

            For i As Integer = 0 To dt.Rows.Count
                dt2.Columns.Add()
            Next
            For i As Integer = 0 To dt.Columns.Count - 1
                dt2.Rows.Add()
                dt2.Rows(i)(0) = dt.Columns(i).ColumnName
            Next
            For i As Integer = 0 To dt.Columns.Count - 1
                For j As Integer = 0 To dt.Rows.Count - 1
                    dt2.Rows(i)(j + 1) = dt.Rows(j)(i)
                    If i = 4 Then
                        dt2.Rows(4)(j + 1) = "Select"
                    End If
                Next
            Next
            Bind10KGrid(dt2, True)
        End If
    End Sub



End Class