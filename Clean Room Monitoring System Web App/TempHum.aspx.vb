Imports System.IO
Imports System.Net
Imports FusionCharts.Charts

Public Class TempHum
    Inherits System.Web.UI.Page
    Dim conn As New Conn
    Const dateNow = "2022-11-10"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("PageTitle") = "Temperature / Humidity Analysis"
        If Not Page.IsPostBack Then
            btndaily.InnerText = " Today " + conn.todayDate()
            btndaily.Attributes.Add("style", "background:#291B44;color:white")

            bind_tempchart(" CONVERT(VARCHAR(25), CreatedOn, 126) LIKE '" + dateNow + "%'", "Daily")
            bind_humchart(" CONVERT(VARCHAR(25), CreatedOn, 126) LIKE '" + dateNow + "%'", "Daily")
            bind_temp_gridview(" CONVERT(VARCHAR(25), CreatedOn, 126) LIKE '" + dateNow + "%'")
            bind_hum_gridview(" CONVERT(VARCHAR(25), CreatedOn, 126) LIKE '" + dateNow + "%'")
            ConvertTemphorizontal()
            ConvertHumhorizontal()
            hdfPeriod.Value = "Daily"
        End If
    End Sub
    Protected Sub Page_Unload(sender As Object, e As System.EventArgs) Handles Me.Unload
        conn.closeSql()
    End Sub

    Protected Sub btnRefresh_Click(sender As Object, e As System.EventArgs) Handles btnrefresh.ServerClick
        'bind_chart("sampledate = '" + conn.myDateNow() + "'")
        btnweekly.Attributes.Remove("style")
        btndaily.Attributes.Add("style", "background:#291B44;color:white")
        bind_tempchart("CONVERT(VARCHAR(25), CreatedOn, 126)  LIKE '" + dateNow + "%'", "Daily")
        bind_humchart("CONVERT(VARCHAR(25), CreatedOn, 126)  LIKE '" + dateNow + "%'", "Daily")
        bind_temp_gridview(" CONVERT(VARCHAR(25), CreatedOn, 126) LIKE '" + dateNow + "%'")
        bind_hum_gridview(" CONVERT(VARCHAR(25), CreatedOn, 126) LIKE '" + dateNow + "%'")
        ConvertTemphorizontal()
        ConvertHumhorizontal()
        dvddldate.Style.Add("display", "none")
        hdfPeriod.Value = "Daily"
    End Sub

    Protected Sub btnDaily_Click(sender As Object, e As System.EventArgs) Handles btndaily.ServerClick

        If btnweekly.Attributes("style") = "background:#291B44;color:white" Then
            btnweekly.Attributes.Remove("style")
            btndaily.Attributes.Add("style", "background:#291B44;color:white")
        End If

        bind_tempchart(" CONVERT(VARCHAR(25), CreatedOn, 126) LIKE '" + dateNow + "%'", "Daily")
        bind_humchart(" CONVERT(VARCHAR(25), CreatedOn, 126) LIKE '" + dateNow + "%'", "Daily")
        bind_temp_gridview(" CONVERT(VARCHAR(25), CreatedOn, 126) LIKE '" + dateNow + "%'")
        bind_hum_gridview(" CONVERT(VARCHAR(25), CreatedOn, 126) LIKE '" + dateNow + "%'")
        ConvertTemphorizontal()
        ConvertHumhorizontal()
        dvddldate.Style.Add("display", "none")
        hdfPeriod.Value = "Daily"
    End Sub

    Protected Sub btnWeekly_Click(sender As Object, e As System.EventArgs) Handles btnweekly.ServerClick

        If btndaily.Attributes("style") = "background:#291B44;color:white" Then
            btndaily.Attributes.Remove("style")
            btnweekly.Attributes.Add("style", "background:#291B44;color:white")
        End If
        divTempChart.Style.Add("display", "none") 'hide existing temperature chart
        divHumChart.Style.Add("display", "none") 'hide existing humidity chart
        gvTempAlarm.Style.Add("display", "none") 'hide temperature alarm table
        gvHumAlarm.Style.Add("display", "none") 'hide humidity alarm table
        dvddldate.Style.Add("display", "block") 'visible calendar     
        ConvertTemphorizontal()
        ConvertHumhorizontal()

    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As System.EventArgs) Handles btnsearch.ServerClick
        btndaily.Attributes.Remove("style")
        'divChart.Style.Add("display", "none")
        If txtDateFrom.Text = "" Or txtDateTo.Text = "" Then
            Response.Write("<script>alert('Please fill up both date')</script>")
        Else
            Dim date_period As String = "createdon >= '" + txtDateFrom.Text + " 00:00:00' AND createdon <= '" + txtDateTo.Text + " 23:59:59'"
            Dim period_basis As String = "Date"
            bind_tempchart(date_period, period_basis)
            bind_humchart(date_period, period_basis)
            bind_temp_gridview(date_period)
            bind_hum_gridview(date_period)
            ConvertTemphorizontal()
            ConvertHumhorizontal()
        End If
        hdfPeriod.Value = "Date"
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
                date_period = " CONVERT(VARCHAR(25), CreatedOn, 126) LIKE '" + dateNow + "%'"
                Response.AddHeader("content-disposition", "attachment; filename= " + dateNow + "_TempHumDailyData.xls")
            ElseIf hdfPeriod.Value = "Date" Then
                date_period = "createdon >= '" + txtDateFrom.Text + " 00:00:00' AND createdon <= '" + txtDateTo.Text + " 23:59:59' "
                Response.AddHeader("content-disposition", "attachment; filename=" + txtDateFrom.Text + "_TO_" + txtDateTo.Text + "_TempHumData.xls")
            End If

            Me.bind_temphum_gridview(date_period)

            gvTempHumExport.RenderControl(hw)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.[End]()

        End Using
    End Sub

    Public Overrides Sub VerifyRenderingInServerForm(control As Control)
        ' Verifies that the control is rendered
    End Sub

    Sub bind_tempchart(ByVal createdDate As String, ByVal period As String)
        Dim schema As String
        Dim data As New StringBuilder
        Dim dataMarker As New StringBuilder
        Dim selectTemperature As String
        Dim fusionTable As FusionTable
        Dim timeSeries As TimeSeries

        Using client As WebClient = New WebClient()

            schema = ("[{name:'tempid',type:'string'}, {name: 'Time',type:'date', format:'%Y-%m-%d %H:%M:%S %p'}, {name:'Temperature Value',type:'number'}] ")
            'selectTemperature = String.Format(" SELECT * FROM tbtemphum where {0}", createdDate) 
            'use union select because to combine those data restored
            selectTemperature = String.Format("SELECT tempid,tempval,humid, humval,createdon FROM tbtemphum WHERE {0} " &
           " ORDER BY createdon", createdDate)
            Dim dt As New DataTable
            dt = conn.query(selectTemperature)
            If dt.Rows.Count > 0 Then
                data.Append("[")
                For i As Integer = 0 To dt.Rows.Count - 1
                    If i <> dt.Rows.Count - 1 Then
                        data.Append("['" + dt.Rows(i)("tempid").ToString + "'," &
                        "'" + DateTime.ParseExact(dt.Rows(i)("createdon").ToString, "d/M/yyyy h:mm:ss tt", Nothing).ToString("yyyy-MM-dd h:mm:ss tt") + "', " &
                        "'" + dt.Rows(i)("tempval").ToString + "'],")
                    Else
                        data.Append("['" + dt.Rows(i)("tempid").ToString + "', " &
                        "'" + DateTime.ParseExact(dt.Rows(i)("createdon").ToString, "d/M/yyyy h:mm:ss tt", Nothing).ToString("yyyy-MM-dd h:mm:ss tt") + "','" + dt.Rows(i)("tempval").ToString + "']]")
                    End If
                Next

                fusionTable = New FusionTable(schema, data.ToString)
                timeSeries = New TimeSeries(fusionTable)
                Dim tempMin As String = conn.exscalar("SELECT temp_min FROM tbsetting")
                Dim tempMin10K As String = conn.exscalar("SELECT temp_min_10k FROM tbsetting")
                Dim tempMax As String = conn.exscalar("SELECT temp_max FROM tbsetting")
                Dim tempMax10K As String = conn.exscalar("SELECT temp_max_10k FROM tbsetting")

                timeSeries.AddAttribute("caption", "{ text:'Temperature' }")
                timeSeries.AddAttribute("subcaption", "{text:'" + period + "'}")
                timeSeries.AddAttribute("navigator", "{enabled:'1'}") 'close time navigator

                'timeSeries.AddAttribute("extensions", "{customRangeSelector:{enabled:'0'}}") 'close calendar
                'timeSeries.AddAttribute("chart", "{exportEnabled:'1'}") 'enable export feature
                timeSeries.AddAttribute("yAxis", "[{plot: {value:  'Temperature Value',type:'line'},title:  'Temperature Value',showgridband:'0', " &
                                        "series:  'tempid',referenceline:[{label:'Temp MIN',value:'" + tempMin + "'},{label:'Temp MIN 10K',value:'" + tempMin10K + "',style:{'marker':{fill:'#7405eb','stroke-dasharray':'4 3'}}}, " &
                                        "{label:'Temp MAX',value:'" + tempMax + "'},{label:'Temp MAX 10K',value:'" + tempMax10K + "',style:{'marker':{fill:'#7405eb','stroke-dasharray':'4 3'}}}],style:{'grid-band':{fill:'#f5f5ef'}} }]")

                Dim selectAlarm As String = String.Format(" SELECT tempid,tempval,createdon FROM tbtemphum where {0} AND alarmtemp =1 " &
                                                          " order by createdon", createdDate)
                Dim dt2 As New DataTable
                dt2 = conn.query(selectAlarm)
                If dt2.Rows.Count > 0 Then
                    dataMarker.Append("[")
                    For i As Integer = 0 To dt2.Rows.Count - 1
                        If i <> dt2.Rows.Count - 1 Then
                            dataMarker.Append("{time:'" + DateTime.ParseExact(dt2.Rows(i)("createdon").ToString, "d/M/yyyy h:mm:ss tt", Nothing).ToString("yyyy-MM-dd h:mm:ss tt") + "', " &
                                              "type:'pin',timeformat:'%Y-%m-%d %H:%M:%S %p',series:{tempid: '" + dt2.Rows(i)("tempid").ToString + "'}}, ")
                        Else
                            dataMarker.Append("{time:'" + DateTime.ParseExact(dt2.Rows(i)("createdon").ToString, "d/M/yyyy h:mm:ss tt", Nothing).ToString("yyyy-MM-dd h:mm:ss tt") + "', " &
                                             "type:'pin',timeformat:'%Y-%m-%d %H:%M:%S %p',series:{tempid: '" + dt2.Rows(i)("tempid").ToString + "'}}] ")
                        End If
                    Next
                    timeSeries.AddAttribute("datamarker", dataMarker.ToString)
                End If

                Dim chart As Chart = New Chart("timeseries", "temp_chart", "1800", "500", "json", timeSeries)
                divTempChart.Style.Add("display", "block")
                gvTempAlarm.Style.Add("display", "block")
                lblTempNoData.Text = ""
                ltTempChart.Text = chart.Render()
            Else
                data.Append("[[]]")
                fusionTable = New FusionTable(schema, data.ToString)
                timeSeries = New TimeSeries(fusionTable)
                Dim chart As Chart = New Chart("timeseries", "temp_chart", "1800", "500", "json", timeSeries)
                ltTempChart.Text = chart.Render()
                lblTempNoData.Text = "No record found in Temperature Chart"
                ' Response.Write("<script>alert('No data within this period. Please reselect')</script>")
                btndaily.Attributes.Remove("style")
                divTempChart.Style.Add("display", "none")
                gvTempAlarm.Style.Add("display", "none")
            End If
        End Using
    End Sub

    Sub bind_humchart(ByVal createdDate As String, ByVal period As String)

        Dim schema As String
        Dim data As New StringBuilder
        Dim dataMarker As New StringBuilder
        Dim selectHumidity As String
        Dim fusionTable As FusionTable
        Dim timeSeries As TimeSeries

        Using client As WebClient = New WebClient()

            schema = ("[{name:'humid',type:'string'}, {name: 'Time',type:'date', format:'%Y-%m-%d %H:%M:%S %p'}, {name:'Humidity Value',type:'number'}] ")
            'selectHumidity = String.Format(" SELECT * FROM tbtemphum where {0}", createdDate)
            selectHumidity = String.Format("SELECT tempid,tempval,humid, humval,createdon FROM tbtemphum WHERE {0} " &
          " ORDER BY createdon", createdDate)
            Dim dt As New DataTable
            dt = conn.query(selectHumidity)
            If dt.Rows.Count > 0 Then
                data.Append("[")
                For i As Integer = 0 To dt.Rows.Count - 1
                    If i <> dt.Rows.Count - 1 Then
                        data.Append("['" + dt.Rows(i)("humid").ToString + "'," &
                        "'" + DateTime.ParseExact(dt.Rows(i)("createdon").ToString, "d/M/yyyy h:mm:ss tt", Nothing).ToString("yyyy-MM-dd h:mm:ss tt") + "', " &
                        "'" + dt.Rows(i)("humval").ToString + "'],")
                    Else
                        data.Append("['" + dt.Rows(i)("humid").ToString + "', " &
                        "'" + DateTime.ParseExact(dt.Rows(i)("createdon").ToString, "d/M/yyyy h:mm:ss tt", Nothing).ToString("yyyy-MM-dd h:mm:ss tt") + "'," &
                        "'" + dt.Rows(i)("humval").ToString + "']]")
                    End If
                Next

                fusionTable = New FusionTable(schema, data.ToString)
                timeSeries = New TimeSeries(fusionTable)
                Dim humMin As String = conn.exscalar("SELECT hum_min FROM tbsetting")
                Dim humMax As String = conn.exscalar("SELECT hum_max FROM tbsetting")

                timeSeries.AddAttribute("caption", "{ text:'Humidity' }")
                timeSeries.AddAttribute("subcaption", "{text:'" + period + "'}")
                timeSeries.AddAttribute("navigator", "{enabled:'1'}") 'close time navigator
                'timeSeries.AddAttribute("extensions", "{customRangeSelector:{enabled:'0'}}") 'close calendar
                'timeSeries.AddAttribute("chart", "{exportEnabled:'1'}") 'enable export feature
                timeSeries.AddAttribute("yAxis", "[{plot: {value:  'Humidity Value',type:'line'},title:  'Humidity Value', showgridband:'0', " &
                                        "series:  'humid',referenceline:[{label:'Hum MIN',value:'" + humMin + "'},{label:'Hum MAX',value:'" + humMax + "'}],style:{'grid-band':{fill:'#f5f5ef'}} }]")
                Dim selectAlarm As String = String.Format(" SELECT humid, humval,createdon FROM tbtemphum where {0} AND alarmhum = 1 " &
                                                          " order by createdon", createdDate)
                Dim dt2 As New DataTable
                dt2 = conn.query(selectAlarm)
                If dt2.Rows.Count > 0 Then
                    dataMarker.Append("[")
                    For i As Integer = 0 To dt2.Rows.Count - 1
                        If i <> dt2.Rows.Count - 1 Then
                            dataMarker.Append("{time:'" + DateTime.ParseExact(dt2.Rows(i)("createdon").ToString, "d/M/yyyy h:mm:ss tt", Nothing).ToString("yyyy-MM-dd h:mm:ss tt") + "', " &
                                              "type:'pin',timeformat:'%Y-%m-%d %H:%M:%S %p',series:{humid: '" + dt2.Rows(i)("humid").ToString + "'}}, ")
                        Else
                            dataMarker.Append("{time:'" + DateTime.ParseExact(dt2.Rows(i)("createdon").ToString, "d/M/yyyy h:mm:ss tt", Nothing).ToString("yyyy-MM-dd h:mm:ss tt") + "', " &
                                             "type:'pin',timeformat:'%Y-%m-%d %H:%M:%S %p',series:{humid: '" + dt2.Rows(i)("humid").ToString + "'}}] ")
                        End If
                    Next
                    timeSeries.AddAttribute("datamarker", dataMarker.ToString)
                End If

                Dim chart As Chart = New Chart("timeseries", "hum_chart", "1800", "500", "json", timeSeries)
                divHumChart.Style.Add("display", "block")
                gvHumAlarm.Style.Add("display", "block")
                lblHumNoData.Text = ""
                ltHumChart.Text = chart.Render()
            Else
                data.Append("[[]]")
                fusionTable = New FusionTable(schema, data.ToString)
                timeSeries = New TimeSeries(fusionTable)
                Dim chart As Chart = New Chart("timeseries", "hum_chart", "1800", "500", "json", timeSeries)
                ltHumChart.Text = chart.Render()
                lblHumNoData.Text = "No record found in Humidity Chart"

                btndaily.Attributes.Remove("style")
                divHumChart.Style.Add("display", "none")
                gvHumAlarm.Style.Add("display", "none")
            End If
        End Using

    End Sub

    Protected Sub dgTemp_ItemBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs)
        If (e.Item.ItemType = ListItemType.Item) Or (e.Item.ItemType = ListItemType.AlternatingItem) Or (e.Item.ItemType = ListItemType.SelectedItem) Then
            e.Item.Cells(0).BackColor = Drawing.Color.Gray
            e.Item.Cells(0).ForeColor = Drawing.Color.White
            'e.Item.Cells(0).Attributes.Add("css", "pinned")

            For i As Integer = 0 To e.Item.Cells.Count - 1
                If e.Item.Cells(0).Text = "SyncGraph" Then
                    e.Item.Cells(0).ForeColor = Drawing.Color.Gray 'hide the "SyncGraph" text
                    'e.Item.Cells(i).Attributes.Add("onclick", "alert('" & e.Item.Cells(i).Text & "');")                  
                    e.Item.Cells(i).Attributes.Add("onclick", "GetTemp_SelectedRow(this,'" & dgTemp.Items(0).Cells(i).Text & " " & dgTemp.Items(1).Cells(i).Text & "');")
                    If e.Item.Cells(i).Text = "Select" Then
                        e.Item.Cells(i).Font.Bold = True
                        e.Item.Cells(i).ForeColor = System.Drawing.ColorTranslator.FromHtml("#225792")
                        e.Item.Cells(i).Attributes.Add("onmouseover", "this.style.cursor = 'pointer';this.style.backgroundColor='#ccffff'") ' pointer and hover with color
                        e.Item.Cells(i).Attributes.Add("onmouseout", "this.style.backgroundColor='#ffffff'")
                    End If
                Else
                    If e.Item.Cells(0).Text = "Date" Or e.Item.Cells(0).Text = "Time" Or e.Item.Cells(0).Text = "Temperature" Or e.Item.Cells(0).Text = "TempValue" Then
                        e.Item.Cells(0).ForeColor = Drawing.Color.White
                    Else
                        e.Item.Cells(i).ForeColor = Drawing.Color.Black
                    End If
                End If
            Next
        End If
    End Sub

    Protected Sub dgHum_ItemBound(ByVal sender As Object, ByVal e As DataGridItemEventArgs)
        If (e.Item.ItemType = ListItemType.Item) Or (e.Item.ItemType = ListItemType.AlternatingItem) Or (e.Item.ItemType = ListItemType.SelectedItem) Then
            e.Item.Cells(0).BackColor = Drawing.Color.Gray
            e.Item.Cells(0).ForeColor = Drawing.Color.White

            For i As Integer = 0 To e.Item.Cells.Count - 1
                If e.Item.Cells(0).Text = "SyncGraph" Then
                    e.Item.Cells(0).ForeColor = Drawing.Color.Gray 'hide the "SyncGraph" text
                    'e.Item.Cells(i).Attributes.Add("onclick", "alert('" & e.Item.Cells(i).Text & "');")                  
                    e.Item.Cells(i).Attributes.Add("onclick", "GetHum_SelectedRow(this,'" & dgHum.Items(0).Cells(i).Text & " " & dgHum.Items(1).Cells(i).Text & "');")
                    If e.Item.Cells(i).Text = "Select" Then
                        e.Item.Cells(i).Font.Bold = True
                        e.Item.Cells(i).ForeColor = System.Drawing.ColorTranslator.FromHtml("#225792")
                        e.Item.Cells(i).Attributes.Add("onmouseover", "this.style.cursor = 'pointer';this.style.backgroundColor='#ccffff'") ' pointer and hover with color
                        e.Item.Cells(i).Attributes.Add("onmouseout", "this.style.backgroundColor='#ffffff'")
                    End If
                Else
                    If e.Item.Cells(0).Text = "Date" Or e.Item.Cells(0).Text = "Time" Or e.Item.Cells(0).Text = "Humidity" Or e.Item.Cells(0).Text = "HumValue" Then
                        e.Item.Cells(0).ForeColor = Drawing.Color.White
                    Else
                        e.Item.Cells(i).ForeColor = Drawing.Color.Black
                    End If
                End If
            Next
        End If
    End Sub

    Protected Sub bind_temp_gridview(ByVal createdDate As String)

        Dim select1 As String = String.Format(
     " SELECT CONVERT(VARCHAR(25), CreatedOn, 23) as Date, Convert(time(0),CreatedOn) AS Time, tempid As Temperature,Round(tempval,1) As TempValue" &
     " FROM tbtemphum  " &
     " WHERE {0} AND alarmtemp= 1", createdDate)

        Dim dt As New DataTable
        dt = conn.query(select1)
        dt.Columns.Add("SyncGraph")

        If dt.Rows.Count > 0 Then
            ViewState("dtTemp") = dt
            BindTempGrid(dt, False)
            lblTempEmptyData.Style.Add("display", "none")
            dgTemp.Visible = True
            dgTemp.DataSource = dt
            dgTemp.DataBind()
        Else
            lblTempEmptyData.Style.Add("display", "block")
            dgTemp.Visible = False
        End If

    End Sub

    Protected Sub bind_hum_gridview(ByVal createdDate As String)

        Dim select1 As String = String.Format(
    " SELECT CONVERT(VARCHAR(25), CreatedOn, 23) as Date, Convert(time(0),CreatedOn) AS Time, humid As Humidity,Round(humval,1) As HumValue" &
    " FROM tbtemphum  " &
    " WHERE {0} AND alarmhum = 1 order by Date", createdDate)

        Dim dt As New DataTable
        dt = conn.query(select1)
        dt.Columns.Add("SyncGraph")

        If dt.Rows.Count > 0 Then
            ViewState("dtHum") = dt
            BindHumGrid(dt, False)
            lblHumEmptyData.Style.Add("display", "none")
            dgHum.Visible = True
            dgHum.DataSource = dt
            dgHum.DataBind()
        Else
            lblHumEmptyData.Style.Add("display", "block")
            dgHum.Visible = False
        End If

    End Sub

    'export all to excel
    Protected Sub bind_temphum_gridview(ByVal createdDate As String)

        Dim select1 As String = String.Format(
       " SELECT Convert(date,CreatedOn) AS Date, Convert(time(0),CreatedOn) AS Time,  tempid AS Temperature," &
       "tempval as TempValue,humid as Humidity, humval as  HumValue FROM tbtemphum  " &
       " where {0} order by Date", createdDate)
        Dim dt As New DataTable
        dt = conn.query(select1)
        If dt.Rows.Count > 0 Then
            gvTempHumExport.DataSource = dt
            gvTempHumExport.DataBind()
            btnexport.Visible = True
            btnexportdisabled.Visible = False
        Else
            btnexport.Visible = False
            btnexportdisabled.Visible = True
        End If
    End Sub

    Private Sub BindTempGrid(dt As DataTable, rotate As Boolean)
        dgTemp.ShowHeader = Not rotate
        dgTemp.DataSource = dt
        dgTemp.DataBind()
        If rotate Then
            For Each row As DataGridItem In dgTemp.Items
                row.Cells(0).CssClass = "header"
            Next
        End If
    End Sub

    Private Sub BindHumGrid(dt As DataTable, rotate As Boolean)
        dgHum.ShowHeader = Not rotate
        dgHum.DataSource = dt
        dgHum.DataBind()
        If rotate Then
            For Each row As DataGridItem In dgHum.Items
                row.Cells(0).CssClass = "header"
            Next
        End If
    End Sub

    'Rotate temperature gridview to horizontal layout
    Public Sub ConvertTemphorizontal()
        Dim dt As DataTable = DirectCast(ViewState("dtTemp"), DataTable)

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
            BindTempGrid(dt2, True)
        End If
    End Sub

    Public Sub ConvertHumhorizontal()
        Dim dt As DataTable = DirectCast(ViewState("dtHum"), DataTable)

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
            BindHumGrid(dt2, True)
        End If
    End Sub
End Class