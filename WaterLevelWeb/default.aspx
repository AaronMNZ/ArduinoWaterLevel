<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="WaterLevelWeb._default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Water Level Meter</title>
    <script src="Scripts/jquery-2.1.3.min.js"></script>
    <script src="Scripts/Highcharts-4.0.1/js/highcharts.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:RadioButtonList ID="groupBy" runat="server" AutoPostBack="True" OnSelectedIndexChanged="groupBy_SelectedIndexChanged" RepeatDirection="Horizontal" >
                <asp:ListItem Text="Minute"></asp:ListItem>
                <asp:ListItem Text="Hour"></asp:ListItem>
                <asp:ListItem Text="Day"></asp:ListItem>
                <asp:ListItem Text="Week"></asp:ListItem>
                <asp:ListItem Text="Month"></asp:ListItem>
            </asp:RadioButtonList>
        </div>

        <asp:Literal ID="ltrChart" runat="server"></asp:Literal>
    </form>

</body>
</html>
