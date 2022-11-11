Public Class Setting
    Inherits System.Web.UI.Page

    Dim conn As New Conn
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("PageTitle") = "Setting"
        If Not Page.IsPostBack Then
            ModalPopupExt.Show()
        End If
        txtPassword.Focus()
    End Sub

    Protected Sub Page_LoadComplete(sender As Object, e As System.EventArgs) Handles Me.LoadComplete
        Get_Limit_Set()
        BindGridView()

    End Sub

    Protected Sub Page_Unload(sender As Object, e As System.EventArgs) Handles Me.Unload
        conn.closeSql()
    End Sub

    Sub Get_Limit_Set()
        Dim limit As SensorLimitation = conn.GetSensorLimitation
        txtParticleCountMin.Text = limit.pc_min
        txtParticleCountMax.Text = limit.pc_max
        txtTemperatureMin.Text = limit.temp_min
        txtTemperatureMax.Text = limit.temp_max
        txtTemperatureMin10K.Text = limit.temp_min_10k
        txtTemperatureMax10K.Text = limit.temp_max_10k
        txtHumidityMin.Text = limit.hum_min
        txtHumidityMax.Text = limit.hum_max

        txtEmailTimePc.Text = limit.email_pc
        txtEmailTimeTemp.Text = limit.email_temp
        txtEmailTimeHum.Text = limit.email_hum
    End Sub

    Sub CheckNumeric(txt As TextBox)
        If Not IsNumeric(txt.Text) Then
            txt.Text = String.Empty
            txt.Focus()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As System.EventArgs) Handles btnSubmit.Click
        Try
            CheckNumericTextbox()
            If txtParticleCountMin.Text <> "" AndAlso txtParticleCountMax.Text <> "" _
                  AndAlso txtTemperatureMin.Text <> "" AndAlso txtTemperatureMax.Text <> "" _
                  AndAlso txtTemperatureMin10K.Text <> "" AndAlso txtTemperatureMax10K.Text <> "" _
                AndAlso txtHumidityMin.Text <> "" AndAlso txtHumidityMax.Text <> "" Then

                Dim update1 As String = String.Format(
                    " UPDATE tbsetting " &
                    " SET pc_min={0}, pc_max={1}, temp_min={2}, temp_max={3}, hum_min={4}, hum_max={5}, temp_min_10k={6}, temp_max_10k={7}",
                    txtParticleCountMin.Text.Trim, txtParticleCountMax.Text.Trim,
                    txtTemperatureMin.Text.Trim, txtTemperatureMax.Text.Trim, txtHumidityMin.Text.Trim, txtHumidityMax.Text.Trim,
                    txtTemperatureMin10K.Text.Trim, txtTemperatureMax10K.Text.Trim)
                conn.nonquery(update1)

                lblMsgLimit.Text = String.Format("Sensor limit has been updated succesfully.")
            End If
        Catch ex As Exception
            lblMsgLimit.Text = "Invalid sensor limit entered"
        End Try

    End Sub

    Protected Sub btnEmailTimeSubmit_Click(sender As Object, e As System.EventArgs) Handles btnEmailTimeSubmit.Click
        Try
            CheckNumericTextboxEmailTime()
            If txtEmailTimePc.Text <> "" AndAlso txtParticleCountMax.Text <> "" AndAlso txtEmailTimeHum.Text Then

                Dim update1 As String = String.Format(
                    " UPDATE tbsetting SET email_pc={0}, email_temp={1}, email_hum={2} ",
                    txtEmailTimePc.Text.Trim, txtEmailTimeTemp.Text.Trim, txtEmailTimeHum.Text.Trim)
                conn.nonquery(update1)

                lblMessage.Text = String.Format("Email Alert time has been updated succesfully.")
            End If
        Catch ex As Exception
            lblMessage.Text = "Invalid email alert time entered"
        End Try

    End Sub

    Sub CheckNumericTextbox()
        CheckNumeric(txtParticleCountMin)
        CheckNumeric(txtParticleCountMax)
        CheckNumeric(txtTemperatureMin)
        CheckNumeric(txtTemperatureMax)
        CheckNumeric(txtTemperatureMin10K)
        CheckNumeric(txtTemperatureMax10K)
        CheckNumeric(txtHumidityMin)
        CheckNumeric(txtHumidityMax)
    End Sub

    Sub CheckNumericTextboxEmailTime()
        CheckNumeric(txtEmailTimePc)
        CheckNumeric(txtEmailTimeTemp)
        CheckNumeric(txtEmailTimeHum)
    End Sub

    Sub BindGridView()
        Dim dt As DataTable = conn.query("SELECT ID, Name, Email FROM tbContact WHERE blnActive=1")
        gv.DataSource = dt
        gv.DataBind()
    End Sub

    Protected Sub gv_RowDeleting(sender As Object, e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles gv.RowDeleting
        Try
            Dim row = gv.Rows(Convert.ToInt32(e.RowIndex))
            Dim ID As String = CType(row.FindControl("ID"), Label).Text.ToUpper
            Dim Name As String = CType(row.FindControl("Name"), Label).Text.ToUpper
            Dim Email As String = CType(row.FindControl("Email"), Label).Text.ToLower

            conn.nonquery(String.Format("UPDATE tbContact SET blnActive=0 WHERE ID={0}", ID))

            lblMessage.Text = String.Format("{0} ({1}) has been removed succesfully.", Name, Email)

            BindGridView()
        Catch ex As Exception
            lblMessage.Text = "Error occured during delete action"
        End Try

    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            Dim strName As String = CType(gv.FooterRow.Cells(2).FindControl("txtName"), TextBox).Text.Trim.ToUpper
            Dim strEmail As String = CType(gv.FooterRow.Cells(3).FindControl("txtEmail"), TextBox).Text.Trim.ToLower

            Dim select1 As String = String.Format(" SELECT * FROM tbContact Where EXISTS (SELECT * FROM tbContact WHERE Email='{0}')", strEmail)

            If Not (strName = "" And strEmail = "") Then
                If conn.exscalar(select1) <> 1 Then
                    Dim insert1 As String = String.Format(
                   "INSERT INTO tbContact (Name, EMAIL) VALUES ('{0}','{1}')", strName, strEmail)
                    conn.nonquery(insert1)

                    lblMessage.Text = String.Format("{0} ({1}) has been added successfully.", strName, strEmail)
                Else
                    lblMessage.Text = String.Format("{0} is already in the email list.", strEmail)
                End If
            Else
                lblMessage.Text = String.Format("Please enter email details.", strEmail)
            End If

        Catch ex As Exception
            lblMessage.Text = "Error occurred during adding new email "
        End Try

    End Sub

    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            Dim select1 As String = "SELECT UserPassword,IsAdmin FROM tbuser"
            Dim dt As New DataTable
            dt = conn.query(select1)
            Dim password As String = dt.Rows(0)("UserPassword")
            Dim admin As String = dt.Rows(0)("IsAdmin")
            If password = txtPassword.Text And admin = "1" Then
                ModalPopupExt.Hide()
            Else
                ModalPopupExt.Show()
                Response.Write("<script>alert('Incorrect password!')</script>")
            End If

        Catch ex As Exception
            Console.WriteLine("Error:" + ex.ToString)
        End Try

    End Sub
End Class