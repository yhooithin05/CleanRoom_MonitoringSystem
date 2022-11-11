<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPage.Master" CodeBehind="ParticleCounter.aspx.vb" Inherits="SensorWeb.ParticleCounter" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .btn-group button {
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
        .btn.fa::before {
            font-family: 'FontAwesome';
        }

        .btn:hover {
            background-color: #DC143C;
            color: white;
        }

        .gvcontainer {
            margin-left: auto;
            margin-right: auto;
            width: 1800px;
            height: 190px;
            border: 1px solid #ccc;
            overflow-x: scroll;
            background: #ffffff;
        }

        .GridItem {
            background-color: #ffffff;
        }


        .GridPosition {
            /* position:absolute;*/ /*this will make scrollbar not working*/
            left: 150px;
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

        /* Panel */
        .panel {
            position: absolute;
            top: 12%;
            left: 40%;
            display: inline-block;
            background: #ffffff;
            min-height: max-content;
            height: max-content;
            border: 1px solid #aaa;
            border-style: dotted;
            border-radius: 5px;
            box-shadow: 0 0 5px 0 #c1c1c1;
            margin: 10px;
            padding: 5px 15px;
        }

        /* Top value controls */
        .panelTopValue {
            position: absolute;
            top: 15%;
            right: 9%;
            display: inline-block;
            /* background:#6BB5BA;*/
            min-height: max-content;
            height: max-content;
            margin: 5px;
            padding: 5px 15px;
        }

        .ddlTopValue {
            font-size: 20px;
            padding: 9px 10px;
            border-radius: 5px;
        }



        .min-add-button {
            /* width: 300px;*/
            margin: 5px auto;
        }

            .min-add-button a {
                background: #291B44;
                border-radius: 5px;
                color: #fff;
                border: 0px;
                padding: 8px;
                -webkit-transition: all 0.3s ease;
                -moz-transition: all 0.3s ease;
                -ms-transition: all 0.3s ease;
                -o-transition: all 0.3s ease;
                transition: all 0.3s ease;
            }

                .min-add-button a:hover {
                    background: #DC143C;
                }

            .min-add-button input {
                /* height: 55px;*/
                text-align: center;
                width: 35%;
                font-weight: 700;
                padding: 6px;
            }

        .MinusDisabled {
            pointer-events: none;
            cursor: default;
            opacity: 0.6;
        }

        .PlusDisabled {
            pointer-events: none;
            cursor: default;
            opacity: 0.6;
        }
        /* Makes link non-clickable: */
        .disabled:active {
            pointer-events: none;
        }

        .btn.disabled {
            opacity: 0.65;
            cursor: not-allowed;
            background-color: white;
            color: blue;
        }

        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript" src="https://cdn.fusioncharts.com/fusioncharts/latest/fusioncharts.js"></script>
    <script type="text/javascript" src="https://cdn.fusioncharts.com/fusioncharts/latest/fusioncharts.timeseries.js"></script>
    <script type="text/JavaScript" src=" https://MomentJS.com/downloads/moment.js"></script>

    <!-- Add icon library -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css" />

   <%-- <asp:UpdatePanel runat="server" ID="Panel">

        <ContentTemplate>--%>
            <div style="margin: -5px;">
                <table width="100%">
                    <tr>
                        <td bgcolor="#A4A1A9" style="padding: 5px 10px;">
            </div>
            <div class="btn-group">
                <button runat="server" id="btndaily" class="btn fa fa-calendar"></button>
                <button runat="server" id="btnweekly" class="btn fa fa-calendar">Date</button>
                <button runat="server" id="btnrefresh" class="btn fa fa-refresh" />
                <div class="btn-group" style="float: right; clear: both;">
                </div>
                <button id="btnexport" runat="server" causesvalidation="true" class="btn fa fa-download" validationgroup="PC">
                    Export as Excel
                </button>
                <button id="btnexportdisabled" runat="server" class="btn disabled fa fa-download" visible="false">
                    Export as Excel
                </button>
            </div>

           
            <div id="dvddldate" runat="server" class="panel" style="display: none">
                FROM
            <asp:TextBox ID="txtDateFrom" runat="server" autocomplete="off" onkeydown="return false;" Style="width: 150px"> </asp:TextBox>
                <%--autocomplete="off"  is to clear date history --%>
                <asp:RequiredFieldValidator ID="rfvPCFrom" runat="server" ControlToValidate="txtDateFrom" ErrorMessage="*" ForeColor="#FF3300" ValidationGroup="PC"></asp:RequiredFieldValidator>
                TO
            <asp:TextBox ID="txtDateTo" runat="server" autocomplete="off" onkeydown="return false;" Style="width: 150px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPCTo" runat="server" ControlToValidate="txtDateTo" ErrorMessage="*" ForeColor="#FF3300" ValidationGroup="PC"></asp:RequiredFieldValidator>
                <button id="btnsearch" class="btn fa fa-search" onserverclick="btnSearch_Click" runat="server" />

                <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.6/jquery.min.js" type="text/javascript"></script>
                <script src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8/jquery-ui.min.js" type="text/javascript"></script>
                <link href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8/themes/base/jquery-ui.css" rel="Stylesheet" type="text/css" />
                <script type="text/javascript">


    //$(function () {
    //    $("[id*=txtDateFrom]").datepicker({
    //        showOn: 'button',
    //        buttonImageOnly: true,
    //        buttonImage: 'images/calendar.png'
    //    });
    //});

    //on page Load
       $(function pageLoad() {                
                
                $("#" + '<%=txtDateFrom.ClientID%>').unbind();
                $("#" + '<%=txtDateFrom.ClientID%>').datepicker({
                    minDate: -65,
                    maxDate: '0',
                    dateFormat: "yy-mm-dd",
                    onClose: function (selectedDate) {
                        $("#" + '<%=txtDateTo.ClientID%>').datepicker("option", "minDate", selectedDate);
                    }
                    
                });
                $("#" + '<%=txtDateTo.ClientID%>').unbind();
                $("#" + '<%=txtDateTo.ClientID%>').datepicker({
                    dateFormat: "yy-mm-dd", maxDate: '0' 
                });
            });

    //On UpdatePanel Refresh (In case javascript msg pop up,still maintain the datepicker)
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    if (prm != null) {
        prm.add_endRequest(function (sender, e) {
            if (sender._postBackSettings.panelsToUpdate != null) {
                $("#" + '<%=txtDateFrom.ClientID%>').unbind();
                $("#" + '<%=txtDateFrom.ClientID%>').datepicker({
                    minDate: -55,
                    dateFormat: "yy-mm-dd",
                    onClose: function (selectedDate) {
                        $("#" + '<%=txtDateTo.ClientID%>').datepicker("option", "minDate", selectedDate);
                    }
                });
                $("#" + '<%=txtDateTo.ClientID%>').unbind();
                $("#" + '<%=txtDateTo.ClientID%>').datepicker({
                    dateFormat: "yy-mm-dd", maxDate: '0'
                });
            }
        });
    };
                </script>

            </div>

        
           <asp:HiddenField ID="hdfPeriod" runat="server" />
            <%--    </ContentTemplate>
</asp:UpdatePanel>--%>
            <asp:Literal ID="Pt1Chart" runat="server"></asp:Literal>

            <asp:HiddenField ID="hfDateTime" runat="server" />
            <asp:HiddenField ID="hfSensor" runat="server" />
            <asp:HiddenField ID="hfValue" runat="server" />

            <br />
            <asp:Label ID="lbl1KNoData" runat="server"></asp:Label><br />
            <asp:Label ID="lbl10KNoData" runat="server"></asp:Label>
            <div id="div1KChart" align="center" runat="server">
                <asp:Literal ID="lt1kChart" runat="server"></asp:Literal>

            </div>
            <div class="gvcontainer" id="gv1KAlarm" runat="server"  align="center">
                <asp:DataGrid ID="dgPCounter_1KAlarm" runat="server" OnItemDataBound="dg1K_ItemBound" AutoGenerateColumns="true" class="GridPosition">
                    <ItemStyle Wrap="False" CssClass="GridItem"></ItemStyle>
                </asp:DataGrid>


                <asp:Label ID="lbl1KEmptyData" Style="line-height: 200px; text-align: center;" runat="server" Text="No Alarm data to display" Font-Size="Medium"></asp:Label>
            </div>

            <br>
            <br>


            <div id="div10KChart" runat="server" align="center" style="display: block">
                <asp:Literal ID="lt10kChart" runat="server"></asp:Literal>
            </div>
            <div id="gv10KAlarm" runat="server" align="center" class="gvcontainer">
                <asp:DataGrid ID="dgPCounter_10KAlarm" runat="server" class="GridPosition"
                    AutoGenerateColumns="true" OnItemDataBound="dg10K_ItemBound">
                    <ItemStyle CssClass="GridItem" Wrap="False" />
                </asp:DataGrid>
                <%--<asp:GridView ID="gvPCounter_10KAlarm" runat="server" AutoGenerateColumns="true"/>   --%>
                <asp:Label ID="lbl10KEmptyData" runat="server" Font-Size="Medium"
                    Style="line-height: 200px; text-align: center;" Text="No Alarm data to display"></asp:Label>
            </div>
            <div id="divProgress" runat="server" class="progress">
                <asp:UpdateProgress ID="Progresscontainer" runat="server">
                    <ProgressTemplate>
                        <img src="images/spinloader.gif" />
                        <br>
                        Loading...
                      
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </div>
            <%-- <asp:Panel ID="Panel1" runat="server" style="overflow:auto"  >--%>
            <asp:GridView ID="gvPCExport" runat="server" AutoGenerateColumns="true" />

            <%--</asp:Panel>--%>
       <%-- </ContentTemplate>
    </asp:UpdatePanel>--%>
    <script type="text/javascript">

    function Get1K_SelectedRow(lnk, dateTime) {
       
        //use moment to deduct 2 hour for plotting start limit
        setStartLimit = moment(dateTime, "YYYY-MM-DD hh:mm:ss A")
            .subtract(2, 'hours')
            .format('YYYY-MM-DD hh:mm:ss A');
        //use moment to add 2 hour for plotting end limit
        setEndLimit = moment(dateTime, "YYYY-MM-DD hh:mm:ss A")
            .add(2, 'hours')
            .format('YYYY-MM-DD hh:mm:ss A');
        setSelection1K_setTime(setStartLimit, setEndLimit); //pass parameter
       // alert(setStartLimit + setEndLimit);       
    }

    function Get10K_SelectedRow(lnk, dateTime) {
        //use moment to deduct 2 hour for plotting start limit
        setStartLimit = moment(dateTime, "YYYY-MM-DD hh:mm:ss A")
            .subtract(2, 'hours')
            .format('YYYY-MM-DD hh:mm:ss A');
        //use moment to add 2 hour for plotting end limit
        setEndLimit = moment(dateTime, "YYYY-MM-DD hh:mm:ss A")
            .add(2, 'hours')
            .format('YYYY-MM-DD hh:mm:ss A');
        setSelection10K_setTime(setStartLimit, setEndLimit); //pass parameter

    }


    //setSelection() sets the start and end time
    function setSelection1K_setTime(startTime, endTime) {

        var s = new Date(startTime);
        var e = new Date(endTime);

        FusionCharts.items["particlecounter_1k_chart"].setTimeSelection({
            end: e.getTime(), //get miliseconds
            start: s.getTime()
        });
    }

    function setSelection10K_setTime(startTime, endTime) {

        var s = new Date(startTime);
        var e = new Date(endTime);

        FusionCharts.items["particlecounter_10k_chart"].setTimeSelection({
            end: e.getTime(),
            start: s.getTime()
        });
    }


    //Maintain scroll position when ajax trigger poback
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    prm.add_beginRequest(beginRequest);

    function beginRequest() {
        prm._scrollPosition = null;
    }
         

    </script>
</asp:Content>