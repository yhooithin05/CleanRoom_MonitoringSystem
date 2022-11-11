<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="SensorWeb._Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

    <style type="text/css">
    html  { 
       overflow: hidden;/* Hide scrollbars */       
    }
    
    body 
    {
        background-color: #524f56;
    }
    
    table{
        height: 10vh;
       /*border-collapse: collapse;  */         
    }    
      
    .table_temp_hum{
        table-layout:fixed;
        width:75%;   
    }
    
    .equal-width td{width: 50%;}
    
      /* SubHeader */
   
    .tdID
    {   color:White; }
    
     .tdBlackFont
    {   color:Black; }    
   
    
    .tdLabelID
    {
        font-weight:bold;
        text-decoration-line: underline;
        text-underline-offset: 7px;     
        line-height: 5%;
     }     
   
    .divLabelID
    {                
        width: 10%;
        position: absolute; /* DIVs to float around and not affect each other 130 130*/
        margin-top: 50px;
        margin-left: 15px;
        font-size: large;
    }
    
     .divPTContainer
    { 
        direction: rtl; 
        margin:auto;
        padding:20px 0;
    }
    
    .divPT1KValue
    { 
        width: 60%;
        margin-right: 22px;              
        direction: rtl;    
        font-size: 520%;
        font-weight:bold;   
        display: inline-block;
        height:150px;
        font-family: "Arial Narrow", Arial, sans-serif; 
        color:White;
    }
    
     .divPT10KValue
    { 
        width: 30%;
        margin-right: 22px;
        font-size: 520%;
        font-weight:bold;        
        display: inline-block;
        height:150px;
        font-family: "Arial Narrow", Arial, sans-serif; 
    }
    
    .divTempHumContainer
    {              
        line-height: -20%;
        padding:20px 0;
        margin-right: -10px; 
        direction: rtl;      
    }    
     
    .tdTempHumValue
    {
        font-size:120px;
        float: right;   
        font-weight:bold;   
        height:170px;          
    }
    
    .tdOffline
    {
        margin-top: 30px;
        display: inline-block;
        font-size: 60%;   
    }
    
    .divLimit
    {
        margin-top: 100px;
        margin-left: 100px; 
        position: absolute;        
        font-size: small;   
    }
    
    .divPt10KLimit
    {
        margin-top: 100px;
        margin-left: 220px; 
        position: absolute;        
        font-size: small;   
    }
    
     .divTempHumLimit
    {
        margin-top: 100px;
        margin-left: 130px; 
        position: absolute;        
        font-size: small;   
    }
    
    .divPtSensor
    {
        margin-top: -40px;
        margin-left: 0px; 
        position: absolute;
    }
    
    .divTempSensor
    {
        margin-top: -63px;
        margin-left: -1px; 
        position: absolute;
    }
    
    .divLastUpdated
    {
        margin-top: 10px;
        margin-left: 100px; 
        position: absolute;        
        font-size: small;          
    }
    
    
     .divPT10KLastUpdated
    {
        margin-top: 10px;
        margin-left: 220px; 
        position: absolute;        
        font-size: small;          
    }
    
    .divTempHumLastUpdated
    {  
        margin-top: 10px;
        margin-left: 130px; 
        position: absolute;        
        font-size: small;           
    }
    
    .divEventlogID
    {                
        width: 10%;
        position: absolute;
        margin-top: 30px;
        margin-left: 10px;       
        font-size: large;
    }
       
     .divEventLog1KValue
    { 
        width: 50%;
        margin-right: 22px;
        margin-top: -30px;
        font-size: 400%;
        font-weight:bold;        
        display: inline-block;
        padding:10px 0;
        height:90px;
        font-family: "Arial Narrow", Arial, sans-serif; 
    }  
    
     .divEventLog10KValue
    { 
        width: 50%;
        margin-right: 15px;
        margin-top: -30px;
        font-size: 400%;
        font-weight:bold;        
        display: inline-block;
        padding:10px 0;
        height:90px;
        font-family: "Arial Narrow", Arial, sans-serif; 
    }  


    .tdlog{                
        color:Black;
        text-align:center;      
        height:50px;
    }        

        
    .eventlog_highlight{  
        font-size:large;  
        font-weight:bold;                    
        text-align:center;
     }  
           
    .eventlog_normal{  
        font-size: large;                            
        text-align:center;
     }    
        
   .pt_sensorImage {
   width:100px; /*width of your image*/
   height:100px; /*height of your image*/
   background-image:url('door-sensor-alarmed.png');
}
        
/* basic positioning */
.legend { list-style: none; }
.legend li:last-child {float: left; margin-left:-45px;}
.legend li { float: right; margin-right: 40px; }
.legend span { border: 1px solid #ccc; float: left; width: 12px; height: 12px; margin: 2px; }
.legend .legend_red { background-color: #FF0000; border: #ff0000 1px solid; }
.legend .legend_yellow { background-color: #FFA623;border: #FFA623 1px solid;  }
.legend .legend_white { background-color: #FFFFFF; }
.legend .legend_green { background-color: #68B449; border: #68B449 1px solid; }
.legend .legend_silver { background-color: #A4A1A9; border: #A4A1A9 1px solid; }
                     
</style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:HiddenField ID="hfAlert" runat="server" Value="0" />
 
<div>
 <%--  change from 1 second to 5 second--%>
        <asp:Timer ID="Timer1" OnTick="Timer1_Tick" runat="server" Interval="5000">
        </asp:Timer>
</div>

 <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" />
            </Triggers>
<ContentTemplate>
<table style="table-layout: fixed; width:100%; " >
<%--<tr>
  <td align="left" style="border:0">
          
  </td>
     <td align="right" colspan="2" style="border:0">       
        Last update: <asp:Label ID="lblLastUpdate" runat="server" /></td>
</tr>--%>

<tr style="background-color:#A4A1A9">
   <%-- <td colspan="2">AREA <b>1K</b></td>--%>
    <td colspan="2"><ul class="legend">        
     <li class="tbSubHeader"><span class="legend_red"></span> Over Limit</li> 
     <li class="tbSubHeader"><span class="legend_green"></span> Within Limit</li>      
     <li class="tbSubHeader"><span class="legend_silver"></span>AREA <b>1K</b></li>  
</ul>
</td>
</tr>
<tr>
    <td style="border:0;padding-right: 0px;">    
    <table width="125%" style="table-layout:fixed; height:20vh"><tr>
        <td runat="server" id="td1k_p1" class="tdID">       
         <div class="divLastUpdated"><asp:Label ID="lblLastUpdatedPt1" runat="server" /></div>
             <div class="divLabelID">
                <asp:Label ID="Label1" runat="server" Text = "PT" class="tdLabelID"/>
                    <div class="divPtSensor"><img src="images/sensor-alarmed.PNG" alt="" /></div></p>          
                <asp:Label ID="Label2" runat="server" Text = "1"  />                
             </div>
             
           <div id="divPT1KContainer"  runat="server" class="divPTContainer">        
                  <div class="divPT1KValue" style="float:right;clear:both;"><asp:Label ID="lbl1kp1" runat="server" /></div> 
            </div>
        <div class="divLimit"><asp:Label ID="lblLimit1" runat="server" Text = "Limit : 1000"  /></div>
       </td>

        <td runat="server" id="td1k_p2" class="tdID">
            <div class="divLastUpdated"><asp:Label ID="lblLastUpdatedPt2" runat="server" /></div>
            <div class="divLabelID">
                <asp:Label ID="Label3"  runat="server" Text = "PT" class="tdLabelID"/><br />    
                 <div class="divPtSensor"><img src="images/sensor-alarmed.PNG" alt="" /></div></p>      
                <asp:Label ID="Label4"  runat="server" Text = "2"  />
             </div>
             <div class="divPTContainer"> 
                  <div class="divPT1KValue" style="float:right;clear:both;"><asp:Label ID="lbl1kp2" runat="server"/></div> 
             </div>
              <div class="divLimit"><asp:Label ID="Label22" runat="server" Text = "Limit : 1000"  /></div>
        </td>
        <td runat="server" id="td1k_p3" class="tdID">
         <div class="divLastUpdated"><asp:Label ID="lblLastUpdatedPt3" runat="server" /></div>
            <div class="divLabelID">
                <asp:Label ID="Label5"  runat="server" Text = "PT" class="tdLabelID"/><br />  
                 <div class="divPtSensor"><img src="images/sensor-alarmed.PNG" alt="" /></div></p>          
                <asp:Label ID="Label6"  runat="server" Text = "3"  />
             </div>
             <div class="divPTContainer">
                  <div class="divPT1KValue" style="float:right;clear:both;"><asp:Label ID="lbl1kp3" runat="server" /></div>
             </div>
                 <div class="divLimit"><asp:Label ID="Label23" runat="server" Text = "Limit : 1000"  /></div>
        </td>
        <td runat="server" id="td1k_p4" class="tdID">
           <div class="divLastUpdated"><asp:Label ID="lblLastUpdatedPt4" runat="server" /></div>
            <div class="divLabelID">
                <asp:Label ID="Label7" runat="server" Text = "PT" class="tdLabelID"/><br />
                 <div class="divPtSensor"><img src="images/sensor-alarmed.PNG" alt="" /></div></p>           
                <asp:Label ID="Label8" runat="server" Text = "4"  />
             </div>
             <div class="divPTContainer">
                  <div class="divPT1KValue" style="float:right;clear:both;"><asp:Label ID="lbl1kp4" runat="server" /></div>
             </div> 
               <div class="divLimit"><asp:Label ID="Label24" runat="server" Text = "Limit : 1000"  /></div>
        </td>
    </tr></table>
    </td>
    <td style="border:0;padding-left: 0px;">
    <table  class="table_temp_hum" style="table-layout:fixed;float: right; height:20vh"><tr>
    
        <td runat="server" id="td1k_temp" class="tdID" width="15%">
         <div class="divTempHumLastUpdated"><asp:Label ID="lblLastUpdatedTemp1K" runat="server" /></div>
             <div class="divLabelID">
                <asp:Label ID="Label9" runat="server" Text = "TEMP(°C)" class="tdLabelID"/><br /><br /> 
                 <div class="divTempSensor"><img src="images/temperature.PNG" alt="" /></div>      
    
            <asp:Label ID="Label10" runat="server" Text = "1K"  />
             </div> 
             <div class="divTempHumContainer">                  
                 <div class="divPT1KValue" style="float:right;clear:both;"><asp:Label ID="lbl1ktemp" runat="server" class="tdTempHumValue"/></div>
             </div> 
              <div class="divTempHumLimit"><asp:Label ID="Label21" runat="server" Text = "Limit : 21 - 23"  /></div>
                 <%--<asp:Label ID="lbl1KCelcius" runat="server" Text="°C"/>--%>        
               
        </td>
        <td runat="server" id="td1k_hum"  class="tdID" width="15%">
         <div class="divTempHumLastUpdated"><asp:Label ID="lblLastUpdatedHum1K" runat="server" /></div>
             <div class="divLabelID">
                <asp:Label ID="Label11" runat="server" Text = "HMD(%)" class="tdLabelID"/><br /><br />
                 <div class="divTempSensor"><img src="images/humidity.PNG" alt="" /></div>            
               <asp:Label ID="Label12" runat="server" Text = "1K"  />
             </div>
             <div class="divTempHumContainer"> 
                    <div class="divPT1KValue" style="float:right;clear:both;"><asp:Label ID="lbl1khum" runat="server" class="tdTempHumValue" /></div>
             </div>
               <div class="divTempHumLimit"><asp:Label ID="Label25" runat="server" Text = "Limit : 45 - 55"  /></div></td>
        <%--<asp:Label ID="lbl1KPercent" runat="server" class="tdTempHumMeasure" Text="%"/>--%>
    </tr></table>
    </td>
</tr>

<tr style="background-color:#A4A1A9">
    <td colspan="2" class="tbSubHeader">&nbsp;AREA <b>10K</b></td>
</tr>
<tr>
    <td style="border:1px;padding-right: 0px;">
    <table width="125%" style="table-layout:fixed; height:20vh" ><tr>
        <td runat="server" id="td10k_p5" class="tdID">
        <div class="divPT10KLastUpdated"><asp:Label ID="lblLastUpdatedPt5" runat="server" /></div>
             <div class="divLabelID">
                <asp:Label ID="Label13"  runat="server" Text = "PT" class="tdLabelID"/><br />    
                  <div class="divPtSensor"><img src="images/sensor-alarmed.PNG" alt="" /></div></p>       
                <asp:Label ID="lblPt5"  runat="server" Text = "5"  />
             </div>
             <div class="divPTContainer">
                <div class="divPT10KValue" style="float:right;clear:both;"><asp:Label ID="lbl10kp5" runat="server" /></div>
             </div>
              <div class="divPt10KLimit"><asp:Label ID="Label14" runat="server" Text = "Limit : 10000"  /></div>
        </td>
        <td runat="server" id="td10k_p6" class="tdID">
        <div class="divPT10KLastUpdated"><asp:Label ID="lblLastUpdatedPt6" runat="server" /></div>
             <div class="divLabelID">
                <asp:Label ID="Label15" runat="server" Text = "PT" class="tdLabelID"/><br />
                  <div class="divPtSensor"><img src="images/sensor-alarmed.PNG" alt="" /></div></p>          
                <asp:Label ID="lblPt6" runat="server" Text = "6"  />
             </div>
             <div class="divPTContainer">
                 <div class="divPT10KValue" style="float:right;clear:both;"> <asp:Label ID="lbl10kp6" runat="server"/></div>
              </div>
               <div class="divPt10KLimit"><asp:Label ID="Label16" runat="server" Text = "Limit : 10000"  /></div>
                </td>       
    </tr></table>
    </td>
    <td style="border:0;padding-left: 0px;">
    <table class="table_temp_hum" style="table-layout:fixed; float: right; height:20vh"><tr>
        <td runat="server" id="td10k_temp"  class="tdID" width="15%">
         <div class="divTempHumLastUpdated"><asp:Label ID="lblLastUpdatedTemp10K" runat="server" /></div>
             <div class="divLabelID">
                <asp:Label ID="Label17" runat="server" Text = "TEMP(°C)" class="tdLabelID"/><br /><br /> 
                 <div class="divTempSensor"><img src="images/temperature.PNG" alt="" /></div>            
                <asp:Label ID="Label18" runat="server" Text = "10K"  />
             </div>
             <div class="divTempHumContainer"> 
                 <div class="divPT1KValue" style="float:right;clear:both;"><asp:Label ID="lbl10ktemp" runat="server"  class="tdTempHumValue" /></div>
             </div>
               <div class="divTempHumLimit"><asp:Label ID="Label26" runat="server" Text = "Limit : 20 - 22"  /></div>
        <%--<asp:Label ID="lbl10KCelcius" runat="server" class="tdTempHumMeasure" Text="°C"/>--%>
        </td>
        <td runat="server" id="td10k_hum"  class="tdID" width="15%">
         <div class="divTempHumLastUpdated"><asp:Label ID="lblLastUpdatedHum10K" runat="server" /></div>
             <div class="divLabelID">
                <asp:Label ID="Label19" runat="server" Text = "HMD(%)" class="tdLabelID"/><br /><br />  
                <div class="divTempSensor"><img src="images/humidity.PNG" alt="" /></div>          
              <asp:Label ID="Label20" runat="server" Text = "10K"  />
             </div>
             </div>
              <div class="divTempHumContainer"> 
                   <div class="divPT1KValue" style="float:right;clear:both;"><asp:Label ID="lbl10khum" runat="server" class="tdTempHumValue" /></div>
              </div>
               <div class="divTempHumLimit"><asp:Label ID="Label27" runat="server" Text = "Limit : 45 - 55"  /></div>
        <%--<asp:Label ID="lbl10KPercent" runat="server" class="tdTempHumMeasure" Text="%"/>--%>
        </td>
    </tr></table>
    </td>
</tr>
<%--<tr><td style="border:0"></td></tr>--%>
<%--<tr><td style="border:0"></td></tr>--%>
<tr style="background-color:#A4A1A9" >
   <%-- <td colspan="2">EVENT LOG (AREA <b>1K</b>)</td>--%>
     <td colspan="2"><ul class="legend">    
     <li class="tbSubHeader"><span class="legend_white"></span> Last 35 days</li>
     <li class="tbSubHeader"><span class="legend_yellow"></span>Last 7 days</li>
     <li class="tbSubHeader"><span class="legend_red"></span>Today</li>  
     <li class="tbSubHeader"><span class="legend_silver"></span> ALL EVENTS</li> 
    
</ul>
</td>
</tr>
    <table style="table-layout: fixed; width:100%; border-collapse:collapse;">
        <tr>
            <td style="border:0;padding-right: 0px;padding-bottom: 0px;padding-top: 0px;">
                <table style="table-layout:fixed" width="125%">
                    <tr>
                        <td id="tdlog1" runat="server" class="tdID">
                            <div class="divEventlogID">
                                <asp:Label ID="lbleventlogPoint1" runat="server" class="tdLabelID" Text="PT 1" />
                                <br />
                                <asp:Label ID="lbleventlogDate1" runat="server" Font-Size="small" Text="No event" />
                                <br />
                                <asp:Label ID="lbleventlogTime1" runat="server" Font-Size="small" />
                            </div>
                            <div class="divPTContainer">
                                <div class="divEventLog1KValue" style="float:right;clear:both;">
                                    <asp:Label ID="lbleventlogPC1" runat="server" />
                                </div>
                            </div>
                        </td>
                        <td id="tdlog2" runat="server" class="tdID">
                            <div class="divEventlogID">
                                <asp:Label ID="lbleventlogPoint2" runat="server" class="tdLabelID" Text="PT 2" />
                                <br />
                                <asp:Label ID="lbleventlogDate2" runat="server" Font-Size="small" Text="No event" />
                                <br />
                                <asp:Label ID="lbleventlogTime2" runat="server" Font-Size="small" />
                            </div>
                            <div class="divPTContainer">
                                <div class="divEventLog1KValue" style="float:right;clear:both;">
                                    <asp:Label ID="lbleventlogPC2" runat="server" />
                                </div>
                            </div>
                        </td>
                        <td id="tdlog3" runat="server" class="tdID">
                            <div class="divEventlogID">
                                <asp:Label ID="lbleventlogPoint3" runat="server" class="tdLabelID" Text="PT 3" />
                                <br />
                                <asp:Label ID="lbleventlogDate3" runat="server" Font-Size="small" Text="No event" />
                                <br />
                                <asp:Label ID="lbleventlogTime3" runat="server" Font-Size="small" />
                            </div>
                            <div class="divPTContainer">
                                <div class="divEventLog1KValue" style="float:right;clear:both;">
                                    <asp:Label ID="lbleventlogPC3" runat="server" />
                                </div>
                            </div>
                        </td>
                        <td id="tdlog4" runat="server" class="tdID">
                            <div class="divEventlogID">
                                <asp:Label ID="lbleventlogPoint4" runat="server" class="tdLabelID" Text="PT 4" />
                                <br />
                                <asp:Label ID="lbleventlogDate4" runat="server" Font-Size="small" Text="No event" />
                                <br />
                                <asp:Label ID="lbleventlogTime4" runat="server" Font-Size="small" />
                            </div>
                            <div class="divPTContainer">
                                <div class="divEventLog1KValue" style="float:right;clear:both;">
                                    <asp:Label ID="lbleventlogPC4" runat="server" />
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="border:0;padding-left: 0px;">
                <table class="table_temp_hum" style="table-layout:fixed;float: right; ">
                    <tr>
                        <%--<table class="table_temp_hum" style="table-layout:fixed;display: inline-block;float: right; "><tr>
              <%--td runat="server" id="tdlogTemp1K" class="tdID"  width="22%">--%>
                        <td id="tdlogTemp1K" runat="server" class="tdID">
                            <div class="divEventlogID">
                                <asp:Label ID="lbleventlog_temp1k_point" runat="server" class="tdLabelID" Text="TEMP 1K" />
                                <br />
                                <asp:Label ID="lbleventlog_temp1k_date" runat="server" Font-Size="small" Text="No event" />
                                <br />
                                <asp:Label ID="lbleventlog_temp1k_time" runat="server" Font-Size="small" />
                            </div>
                            <div class="divPTContainer">
                                <div class="divEventLog1KValue" style="float:right;clear:both;">
                                    <asp:Label ID="lbleventlog_temp1k_pc" runat="server" />
                                </div>
                            </div>
                        </td>
                        <td id="tdlogHum1K" runat="server" class="tdID">
                            <div class="divEventlogID">
                                <asp:Label ID="lbleventlog_Hum1k_point" runat="server" class="tdLabelID" Text="HUM 1K" />
                                <br />
                                <asp:Label ID="lbleventlog_Hum1k_date" runat="server" Font-Size="small" Text="No event" />
                                <br />
                                <asp:Label ID="lbleventlog_Hum1k_time" runat="server" Font-Size="small" />
                            </div>
                            <div class="divPTContainer">
                                <div class="divEventLog1KValue" style="float:right;clear:both;">
                                    <asp:Label ID="lbleventlog_Hum1k_pc" runat="server" />
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
<tr>
       
</tr>
   <%-- <tr style="background-color:Silver">
        <td colspan="2">EVENT LOG (AREA <b>10K</b>)</td>
    </tr>--%>
   <%-- <tr>--%>
   <%-- <td style="border:0" colspan="2">--%>
    <table style="table-layout: fixed; width:100%;" ><tr>
    <td style="border:0;padding-right: 0px; padding-top: 0px;">
    <table width="125%" style="table-layout:fixed" ><tr>
        <td runat="server" id="tdlog5" class="tdID" >
         <div class="divEventlogID">
            <asp:Label ID="lbleventlogPoint5" runat="server" class="tdLabelID"  Text = "PT 5"/><br />
            <asp:Label ID="lbleventlogDate5" runat="server"  Font-Size="small"  Text= "No event"/><br />
            <asp:Label ID="lbleventlogTime5" runat="server"  Font-Size="small"/>
         </div>
         <div class="divPTContainer">
                <div class="divEventLog10KValue" style="float:right;clear:both;">
                       <asp:Label ID="lbleventlogPC5" runat="server" />
                </div>
        </div>
        </td>
        <td runat="server" id="tdlog6" class="tdID" >
        <div class="divEventlogID">
            <asp:Label ID="lbleventlogPoint6" runat="server" class="tdLabelID"  Text = "PT 6"/><br />
            <asp:Label ID="lbleventlogDate6"  runat="server"  Font-Size="small" Text= "No event"/><br />
            <asp:Label ID="lbleventlogTime6"  runat="server"  Font-Size="small"  />
        </div>
         <div class="divPTContainer">
              <div class="divEventLog10KValue" style="float:right;clear:both;">
                 <asp:Label ID="lbleventlogPC6" runat="server"  />
              </div>
         </div>
        </td>   
        </tr>
     </table>
    </td>
        <td style="border: 0;padding-left: 0px;padding-top: 0px;">
            <table class="table_temp_hum" style="table-layout:fixed;float: right; "><tr>
                <td runat="server" id="tdlogTemp10K" class="tdID" >                 
                 <div class="divEventlogID">
                    <asp:Label ID="lbleventlog_temp10k_point" runat="server"  class="tdLabelID" Text = "TEMP 10K"/><br />
                    <asp:Label ID="lbleventlog_temp10k_date" runat="server"  Font-Size="small" Text= "No event"/><br />
                    <asp:Label ID="lbleventlog_temp10k_time" runat="server"  Font-Size="small" />
                 </div>
                  <div class="divPTContainer">
                      <div class="divEventLog1KValue" style="float:right;clear:both;">
                        <asp:Label ID="lbleventlog_temp10k_pc" runat="server"  />
                      </div>
                 </div>
                </td>
               
                <td runat="server" id="tdlogHum10K" class="tdID" >
                 <div class="divEventlogID">
                    <asp:Label ID="lbleventlog_Hum10k_point" runat="server"  class="tdLabelID" Text = "HUM 10K"/><br />
                    <asp:Label ID="lbleventlog_Hum10k_date" runat="server"  Font-Size="small" Text= "No event"/><br />
                    <asp:Label ID="lbleventlog_Hum10k_time" runat="server"   Font-Size="small"  />
                 </div>
                 <div class="divPTContainer">
                      <div class="divEventLog1KValue" style="float:right;clear:both;">
                         <asp:Label ID="lbleventlog_Hum10k_pc" runat="server"    />
                      </div>
                 </div>
                </td>
                 
            </tr></table>
        </td>
    </tr></table>
<br /><br />
 </ContentTemplate>

        </asp:UpdatePanel>

<asp:Panel ID="pnl" runat="server" Visible="false">
Particle Count (Min): <asp:Label ID="lblParticleCountMin" runat="server" /><br />
Particle Count (Max): <asp:Label ID="lblParticleCountMax" runat="server" /><br /><br />
Temperature (Min): <asp:Label ID="lblTemperatureMin" runat="server" />°C<br />
Temperature (Max): <asp:Label ID="lblTemperatureMax" runat="server" />°C<br /><br />
Temperature (Min_10K): <asp:Label ID="lblTemperatureMin_10K" runat="server" />°C<br />
Temperature (Max_10K): <asp:Label ID="lblTemperatureMax_10K" runat="server" />°C<br /><br />
Humidity (Min): <asp:Label ID="lblHumidityMin" runat="server" />%<br />
Humidity (Max): <asp:Label ID="lblHumidityMax" runat="server" />%<br />
</asp:Panel>
<br /><br />


    <asp:Label ID="lblMessage" runat="server" ForeColor="Blue" Visible="false" />   

 
</asp:Content>
