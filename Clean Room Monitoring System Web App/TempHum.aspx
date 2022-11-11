<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPage.Master" CodeBehind="TempHum.aspx.vb" Inherits="SensorWeb.TempHum" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style type="text/css">
 
.btn-group button 
{    
    background-color: white; /* Red */
    border: none;
    border-radius: 5px;
    box-shadow: 0 4px 10px 0 rgba(0,0,0,0.2);
    color: blue;
    padding: 10px 28px;
    text-align: center;
    text-decoration: none;
    display: inline-block;
    font-family: Keysight Sans, Arial, sans-serif;
    font-size: 20px;
    margin: 4px 2px;
    cursor: pointer; 
    /*float: left; /* Float the buttons side by side */
}

/* Maintain the fontawesome icon while change the keysight font*/
.btn.fa::before{
    font-family: 'FontAwesome';
}
  
 .btn:hover 
{
     background-color: #DC143C;
     color: white;
}

.gvcontainer 
{
 margin-left:auto;
 margin-right:auto;
 width: 1800px; 
 height: 190px;
 border: 1px solid #ccc;
 overflow-x: scroll;
 background: #ffffff;
}

.GridItem
{
    background-color: #ffffff;
}

.GridPosition
{
    position:relative;
    left:100px;    	
}

.pinned
{
    position: fixed; /* i.e. not scrolled */
    background-color: White; /* prevent the scrolled columns showing through */
    z-index: 100; /* keep the pinned on top of the scrollables */
}
 
 /* Panel */
.panel 
{
    position: absolute;
    top: 12%;
    left: 40%;
    display: inline-block;
    background: #ffffff;
    min-height: max-content;
    height: max-content;
    border:1px solid #aaa;
    border-style: dotted;
    border-radius: 5px;
    box-shadow: 0 0 5px 0 #c1c1c1;
    margin: 10px;
    padding: 5px 15px;
}

.progress {
  width: 500px;
  height: 50px;  
  /* Center vertically and horizontally */
  position: absolute;
  top: 50%;
  left: 50%;
  margin: -25px 0 0 -25px; /* apply negative top and left margins to truly center the element */
}

.btn.disabled {
  opacity: 0.65; 
  cursor: not-allowed;
    background-color: white;
      color: blue;
}

/* Makes link non-clickable: */
.disabled:active {
    pointer-events: none;
}


}
    
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <%-- <img src="images/graph_temperature_humidity.JPG" alt="" />--%>
     <script type="text/javascript" src="https://cdn.fusioncharts.com/fusioncharts/latest/fusioncharts.js"></script>       
    <script type="text/javascript" src="https://cdn.fusioncharts.com/fusioncharts/latest/fusioncharts.timeseries.js"></script>
    <script type = "text/JavaScript" src = " https://MomentJS.com/downloads/moment.js"></script>

      <!-- Add icon library -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css" />
    
    <%-- <asp:ScriptManager ID="ScriptManager2" runat="server" />--%>
   <%-- <asp:UpdatePanel runat="server" id="Panel" UpdateMode="Always">
         <ContentTemplate>--%>
         <div style = "margin: -5px;">
        <table width="100%" >
        <tr>
        <td bgcolor="#A4A1A9" style="padding: 5px 10px;">
          <div class="btn-group" >
            <button runat="server" id="btndaily" class="btn fa fa-calendar" ></button>
            <button runat="server" id="btnweekly" class="btn fa fa-calendar" > Date</button>
            <button runat="server" id="btnrefresh" class="btn fa fa-refresh" />
            
             <div class="btn-group" style="float:right;clear:both;">
           </div>    
              <button id="btnexport" runat="server" causesvalidation="true" class="btn fa fa-download" validationgroup="TempHum">
                  Export as Excel
              </button>
              <button id="btnexportdisabled" runat="server" class="btn disabled fa fa-download" visible="false">
                  Export as Excel
              </button>
         </div>

                </td>
</tr>
</table>  
</div>      
                
             <div id="dvddldate" runat="server" class="panel" style="display: none">
                 FROM
                 <asp:TextBox ID="txtDateFrom" runat="server" autocomplete="off" onkeydown="return false;" style="width: 150px"> </asp:TextBox>
                 <asp:RequiredFieldValidator ID="rfvTempFrom" runat="server" ControlToValidate="txtDateFrom" ErrorMessage="*" ForeColor="#FF3300" ValidationGroup="TempHum"></asp:RequiredFieldValidator>
                 TO
                 <asp:TextBox ID="txtDateTo" runat="server" autocomplete="off" style="width: 150px"></asp:TextBox>
                 <asp:RequiredFieldValidator ID="rfvTempTo" runat="server" ControlToValidate="txtDateTo" ErrorMessage="*" ForeColor="#FF3300" ValidationGroup="TempHum"></asp:RequiredFieldValidator>
                 <button  id="btnsearch" class="btn fa fa-search" OnServerClick="btnSearch_Click" runat="server"/>
                 <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.6/jquery.min.js" type="text/javascript"></script>
                <script src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js" type="text/javascript"></script>
                <link href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css" rel="Stylesheet" type="text/css" />
                <script type="text/javascript">
                

            $(function () {

                $("#" + '<%=txtDateFrom.ClientID%>').unbind();
                $("#" + '<%=txtDateFrom.ClientID%>').datepicker({
                    minDate: -115,
                    maxDate: '0',
                    dateFormat: "yy-mm-dd",
                    onClose: function (selectedDate) {
                        $("#" + '<%=txtDateTo.ClientID%>').datepicker("option", "minDate", selectedDate);
                    }
                });
                $("#" + '<%=txtDateTo.ClientID%>').unbind();
                $("#" + '<%=txtDateTo.ClientID%>').datepicker({
                    maxDate: '0',
                    dateFormat: "yy-mm-dd"
                });
            });

        </script>
                 
             </div>
           
             <asp:HiddenField ID="hdfPeriod" runat="server"  />       
   <%-- </ContentTemplate>
</asp:UpdatePanel>--%>
      <asp:Label ID="lblTempNoData" runat="server"></asp:Label><br />  
      <asp:Label ID="lblHumNoData" runat="server"></asp:Label>  
  <div id="divTempChart" align="center"  runat="server" >  
      <asp:Literal ID="ltTempChart" runat="server"></asp:Literal>       
  </div>
  <div class="gvcontainer" id="gvTempAlarm" runat="server" >
        <%--<asp:GridView ID="gvTemp" runat="server" AutoGenerateColumns="true"/>   --%> 
        <asp:DataGrid id="dgTemp" runat="server" OnItemDataBound="dgTemp_ItemBound" AutoGenerateColumns="true" class="GridPosition">   
              <ItemStyle Wrap ="False" CssClass="GridItem"></ItemStyle>
        </asp:DataGrid>  
        <asp:Label ID="lblTempEmptyData" style="line-height:200px; text-align: center;" runat="server" Text="No Alarm data to display" Font-Size="Medium"></asp:Label>
  </div>

    <div id="divProgress" class="progress"  runat="server" >
    <asp:UpdateProgress runat="server" id="Progresscontainer">
            <ProgressTemplate>
                <img src="images/spinloader.gif" />&nbsp;
            </ProgressTemplate>
        </asp:UpdateProgress>           
    </div>
    <br />
    <div id="divHumChart" align="center"  runat="server" >  
        <asp:Literal ID="ltHumChart" runat="server"></asp:Literal>
    </div>
     <div class="gvcontainer" id="gvHumAlarm" runat="server" >
      <%--  <asp:GridView ID="gvHum" runat="server" AutoGenerateColumns="true"/> --%>  
       <asp:DataGrid id="dgHum" runat="server" OnItemDataBound="dgHum_ItemBound" AutoGenerateColumns="true" class="GridPosition">             
              <ItemStyle Wrap ="False" CssClass="GridItem"></ItemStyle>
       </asp:DataGrid>   
        <asp:Label ID="lblHumEmptyData" style="line-height:200px; text-align: center;" runat="server" Text="No Alarm data to display" Font-Size="Medium"></asp:Label>
  </div>
   <asp:GridView ID="gvTempHumExport" runat="server" AutoGenerateColumns="true"/>  

  <script type="text/javascript">

      function GetTemp_SelectedRow(lnk, dateTime) {
          //use moment to deduct 20 mins for plotting start limit
          setStartLimit = moment(dateTime, "YYYY-MM-DD hh:mm:ss A")
                            .subtract(20, 'minutes')
                            .format('YYYY-MM-DD hh:mm:ss A');
          //use moment to add 20 mins for plotting end limit
          setEndLimit = moment(dateTime, "YYYY-MM-DD hh:mm:ss A")
                            .add(20, 'minutes')
                            .format('YYYY-MM-DD hh:mm:ss A');
          setSelectionTemp_setTime(setStartLimit, setEndLimit); //pass parameter             
      }

      function GetHum_SelectedRow(lnk, dateTime) {
          //use moment to deduct 20 mins for plotting start limit
          setStartLimit = moment(dateTime, "YYYY-MM-DD hh:mm:ss A")
                        .subtract(20, 'minutes')
                        .format('YYYY-MM-DD hh:mm:ss A');
          //use moment to add 20 mins for plotting end limit
          setEndLimit = moment(dateTime, "YYYY-MM-DD hh:mm:ss A")
                        .add(20, 'minutes')
                        .format('YYYY-MM-DD hh:mm:ss A');
          setSelectionHum_setTime(setStartLimit, setEndLimit); //pass parameter
      }

      function setSelectionTemp_setTime(startTime, endTime) {
         
          var s = new Date(startTime);
          var e = new Date(endTime);
          
          FusionCharts.items["temp_chart"].setTimeSelection({
              end: e.getTime(), //get miliseconds
              start: s.getTime()
          });
      }

      function setSelectionHum_setTime(startTime, endTime) {

          var s = new Date(startTime);
          var e = new Date(endTime);

          FusionCharts.items["hum_chart"].setTimeSelection({
              end: e.getTime(),
              start: s.getTime()
          });
      }
           
    
    //Maintain scroll position when ajax trigger postback
      var prm = Sys.WebForms.PageRequestManager.getInstance();
      prm.add_beginRequest(beginRequest);

      function beginRequest() {
          prm._scrollPosition = null;
      }
    
  </script>
</asp:Content>

