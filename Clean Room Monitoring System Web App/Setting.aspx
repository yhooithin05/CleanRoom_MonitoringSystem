<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPage.Master" CodeBehind="Setting.aspx.vb" Inherits="SensorWeb.Setting" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <style type="text/css">
    .EditTextBox{
        width: 35px;
    }
    
    input {    
        font-size:large;
    }
    
    input[type=submit]
{
    background-color: #291B44;
    border: none;
    border-radius: 5px;
    box-shadow: 0 4px 10px 0 rgba(0,0,0,0.2);
    color: white;
    padding: 10px 28px;
    text-align: center;
    text-decoration: none;
    display: inline-block;
    font-size: 14px;
    margin: 4px 2px;
    cursor: pointer;   
}

input[type=submit]:hover 
{
    background-color: #DC143C;
}

/* Panel */
.panel {
    display: inline-block;
    background: #ffffff;
    min-height: max-content;
    height: max-content;
    border:1px solid #aaa;
    border-style: dotted;
    box-shadow: 0 0 5px 0 #c1c1c1;
    margin: 10px;
    padding: 10px;
}

    .modalBackground{
        background-color: Black;
        filter: alpha(opacity=90);
        opacity: 0.4;
    }
    .modalPopup{
        background-color: #FFFFFF;
        border-width: 3px;
        border-style: solid;
        border-color: black;
        padding-top: 10px;
        padding-left: 10px;
        width: 400px;
        height: 180px;
    }
        
   .btnlogin{
         position: absolute;
          margin: 0;
          bottom: 18px;
          left: 50%;
          transform: translateX(-50%);
          background-color: #06a7e2;  
          color: white;  
          border: none;   
    }
    
   .wrapper {
        position: absolute;
        left: 1%;       
    }
    
/* Create two equal columns that floats next to each other */
.column {
  float: left;
  width: 50%;
  padding: 10px;
  height: 300px; /* Should be removed. Only for demonstration */
}

/* Clear floats after the columns */
.row:after {
  content: "";
  display: table;
  clear: both;
}
   
</style>   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="wrapper">
<h3>FOR ADMINISTRATOR LOGIN ONLY</h3> 
<asp:LinkButton ID="btnShow" runat="server" Text="Administrator Login"/>
</br>
<ajaxtoolkit:modalpopupextender ID="ModalPopupExt" runat="server" PopupControlID="ModalPanel" TargetControlID="btnShow"
   BackgroundCssClass="modalBackground">
</ajaxtoolkit:modalpopupextender>
<asp:Panel ID="ModalPanel" runat="server" CssClass="modalPopup" align="center"  BehaviorID="popup1">
    <div style="height: 70px" >
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>              
                <br>              
             </br> Please fill in Admin Password 
                <br> </br> 
              <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"  ></asp:TextBox>
               <asp:RequiredFieldValidator ID="rfvLogin" runat="server" Display="Dynamic"
                        ControlToValidate="txtPassword" ErrorMessage="*"
                        ForeColor="#FF3300" ></asp:RequiredFieldValidator>              
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:Button ID="btnlogin" runat="server" CssClass="btnlogin" height="18%" Width="30%" Text="Log In" OnClick="btnlogin_Click" />
   
</asp:Panel>
   <div id="divlimit" class="panel ">
<asp:Panel ID="pnlSetLimit" runat="server" Visible="true">
<h3>&nbsp;Sensor Limit</h3>
<table>
<tr>
    <td style="border:0"><asp:RequiredFieldValidator ID="rfvParticleCountMin" 
            runat="server" ControlToValidate="txtParticleCountMin" ErrorMessage="*" 
            ForeColor="Red" ValidationGroup="limit" Display="Dynamic" />&nbsp;Particle Count (1K)</td>
    <td style="border:0"><asp:TextBox ID="txtParticleCountMin" runat="server" Width="60" MaxLength="5" /></td>   
</tr>
<tr>
    <td style="border:0"><asp:RequiredFieldValidator ID="rfvParticleCountMax" 
            runat="server" ControlToValidate="txtParticleCountMax" ErrorMessage="*" 
            ForeColor="Red" ValidationGroup="limit" Display="Dynamic" />&nbsp;Particle Count (10K)</td>
    <td style="border:0"><asp:TextBox ID="txtParticleCountMax" runat="server" Width="60" MaxLength="5" /></td>   
</tr>
<tr>
    <td style="border:0"><asp:RequiredFieldValidator ID="rfvTemperatureMin" 
            runat="server" ControlToValidate="txtTemperatureMin" ErrorMessage="*" 
            ForeColor="Red" ValidationGroup="limit" Display="Dynamic" />&nbsp;Temperature 1K (Min)</td>
    <td style="border:0"><asp:TextBox ID="txtTemperatureMin" runat="server" Width="50" MaxLength="3" /></td>    
</tr>
<tr>
    <td style="border:0"><asp:RequiredFieldValidator ID="rfvTemperatureMax" 
            runat="server" ControlToValidate="txtTemperatureMax" ErrorMessage="*" 
            ForeColor="Red" ValidationGroup="limit" Display="Dynamic" />&nbsp;Temperature 1K (Max)</td>
    <td style="border:0"><asp:TextBox ID="txtTemperatureMax" runat="server" Width="50" MaxLength="3" /></td>    
</tr>
<tr>
    <td style="border:0"><asp:RequiredFieldValidator ID="rfvTemperatureMin10K" 
            runat="server" ControlToValidate="txtTemperatureMin10K" ErrorMessage="*" 
            ForeColor="Red" ValidationGroup="limit" Display="Dynamic" />&nbsp;Temperature 10K (Min)</td>
    <td style="border:0"><asp:TextBox ID="txtTemperatureMin10K" runat="server" Width="50" MaxLength="3" /></td>    
</tr>
<tr>
    <td style="border:0"><asp:RequiredFieldValidator ID="rfvTemperatureMax10K" 
            runat="server" ControlToValidate="txtTemperatureMax10K" ErrorMessage="*" 
            ForeColor="Red" ValidationGroup="limit" Display="Dynamic" />&nbsp;Temperature 10K (Max)</td>
    <td style="border:0"><asp:TextBox ID="txtTemperatureMax10K" runat="server" Width="50" MaxLength="3" /></td>    
</tr>
<tr>
    <td style="border:0"><asp:RequiredFieldValidator ID="rfvHumidityMin" 
            runat="server" ControlToValidate="txtHumidityMin" ErrorMessage="*" 
            ForeColor="Red" ValidationGroup="limit" Display="Dynamic" />&nbsp;Humidity (Min)</td>
    <td style="border:0"><asp:TextBox ID="txtHumidityMin" runat="server" Width="50" MaxLength="3" /></td>    
</tr>
<tr>
    <td style="border:0"><asp:RequiredFieldValidator ID="rfvHumidityMax" 
            runat="server" ControlToValidate="txtHumidityMax" ErrorMessage="*" 
            ForeColor="Red" ValidationGroup="limit" Display="Dynamic" />&nbsp;Humidity (Max)</td>
    <td style="border:0"><asp:TextBox ID="txtHumidityMax" runat="server" Width="50" MaxLength="3" /></td>    
</tr>
<tr>
    <td style="border:0">&nbsp;</td>
</tr>
<tr>
    <td colspan="2" style="border:0"><asp:Button ID="btnSubmit" runat="server" Text="Submit" Width="100%" ValidationGroup="limit" /></td>
</tr>
</table>
    <asp:Label ID="lblMsgLimit" runat="server" ForeColor="Blue" />
</asp:Panel>
</div>
</br>


<div id="EmailAlert" class="panel">
<asp:Panel ID="pnEmail" runat="server" Visible="true">
<h3>Email Alert</h3>
<asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" DataKeyNames="ID" ShowFooter="true">
<Columns>
    <asp:TemplateField HeaderText="No" >                           
        <ItemTemplate>                    
            <%# Container.DataItemIndex + 1 %>
        </ItemTemplate>         
        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />              
    </asp:TemplateField>

    <asp:TemplateField HeaderText="ID" Visible="false" >
        <ItemTemplate>
            <asp:Label ID="ID" runat="server" Text='<%# Eval("ID") %>' />
        </ItemTemplate>
    </asp:TemplateField>

    <asp:TemplateField HeaderText="Name" SortExpression="EmrNo" Visible="true" >
        <ItemTemplate>
            <asp:Label ID="Name" runat="server" Text='<%# Eval("Name") %>' />
        </ItemTemplate>
        <FooterTemplate>
            <asp:TextBox ID="txtName" runat="server" MaxLength="128" />
            <asp:RequiredFieldValidator ID="rfvName" runat="server" ErrorMessage="*" ForeColor="Red" ControlToValidate="txtName" ValidationGroup="AddNew" Display="Dynamic" />
        </FooterTemplate>
    </asp:TemplateField>

    <asp:TemplateField HeaderText="Email" SortExpression="EmrNo" Visible="true">
        <ItemTemplate>
            <asp:Label ID="Email" runat="server" Text='<%# Eval("Email") %>' />
        </ItemTemplate>
        <FooterTemplate>
            <asp:TextBox ID="txtEmail" runat="server" MaxLength="256" Width="388" />
            <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ErrorMessage="*" ForeColor="Red" ControlToValidate="txtEmail" ValidationGroup="AddNew" Display="Dynamic" />
            <asp:RegularExpressionValidator ID="revEmail" runat="server" ErrorMessage="Invalid Email" ForeColor="Red" ControlToValidate="txtEmail" ValidationGroup="AddNew" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic" />
        </FooterTemplate>
    </asp:TemplateField>

    <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Center" Visible="true">
        <ItemTemplate>               
            <asp:LinkButton ID="lbtnDelete" runat="server" CommandName="Delete" CausesValidation="false" Text="Delete"
                OnClientClick="return confirm('Are you sure you want to remove this email permanently?');" />
        </ItemTemplate>
        <FooterTemplate>
            <asp:Button ID="btnAdd" runat="server" Text="Add New" ValidationGroup="AddNew" OnClick="btnAdd_Click" />
        </FooterTemplate>
    </asp:TemplateField>

    
</Columns>
</asp:GridView>

<br />

<table>
<tr>
    <td style="border:0"><asp:RequiredFieldValidator ID="rfvEmailTimePc" 
            runat="server" ControlToValidate="txtEmailTimePc" ErrorMessage="*" 
            ForeColor="Red" ValidationGroup="emailtime" Display="Dynamic" />&nbsp;Particle Count</td>
    <td style="border:0"><asp:TextBox ID="txtEmailTimePc" runat="server" Width="50" MaxLength="3" /> min</td>   
</tr>
<tr>
    <td style="border:0"><asp:RequiredFieldValidator ID="rfvEmailTimeTemp" 
            runat="server" ControlToValidate="txtEmailTimeTemp" ErrorMessage="*" 
            ForeColor="Red" ValidationGroup="emailtime" Display="Dynamic" />&nbsp;Temperature</td>
    <td style="border:0"><asp:TextBox ID="txtEmailTimeTemp" runat="server" Width="50" MaxLength="3" /> min</td>    
</tr>
<tr>
    <td style="border:0"><asp:RequiredFieldValidator ID="rfvEmailTimeHum" 
            runat="server" ControlToValidate="txtEmailTimeHum" ErrorMessage="*" 
            ForeColor="Red" ValidationGroup="emailtime" Display="Dynamic" />&nbsp;Humidity</td>
    <td style="border:0"><asp:TextBox ID="txtEmailTimeHum" runat="server" Width="50" MaxLength="3" /> min</td>    
</tr>
<tr>
    <td style="border:0">&nbsp;</td>
</tr>
<tr>
    <td colspan="2" style="border:0"><asp:Button ID="btnEmailTimeSubmit" runat="server" Text="Update Email Alert Time"
     Width="100%" ValidationGroup="emailtime" /></td>
</tr>
</table>

<br />
    <asp:Label ID="lblMessage" runat="server" ForeColor="Blue" />
</asp:Panel>
</div>
</br>


</br>


</div>
<p>
</div>
</asp:Content>
