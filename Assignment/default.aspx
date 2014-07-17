<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="Assignment._default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Assignment application</title>
    <script language="javascript" type="text/javascript">
    </script>
    <link href="Styles/Style.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .style1
        {
            font-family: "Times New Roman" , Times, serif;
        }
    </style>
</head>
<body class="mystyle">
    <form id="Form1" method="post" enctype="multipart/form-data" runat="server">
    <div id="container">
        <div id="header">
            <h1 class="style1">
                Assignment application
            </h1>
        </div>
        <div id="upload">
            <h2 style="text-align: center">
                Upload datoteke:
            </h2>
            <asp:FileUpload ID="fileSubmit" runat="server" />
            <asp:Button ID="parseBtn" runat="server" Text="Submit" Width="92px" />
            <br />
            <br />
            <asp:Label ID="labelStatus" runat="server" Text="Label"></asp:Label>
        </div>
        <div id="Results">
            <asp:GridView ID="dataGridView1" runat="server" CellPadding="4" ForeColor="#333333"
                GridLines="None" Style="text-align: center" 
                onrowdatabound="dataGridView1_RowDataBound">
                <AlternatingRowStyle BackColor="White" />
                <EditRowStyle BackColor="#2461BF" />
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#EFF3FB" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                <SortedDescendingHeaderStyle BackColor="#4870BE" />
                

            </asp:GridView>
        </div>
        <div id="statistic">
            <asp:Panel ID="panelStatistic" runat="server" Height="369px" Width="254px" Style="text-align: left">
                <br />
                <asp:Label ID="Label1" runat="server" Text="Statistika"></asp:Label>
                <br />
                <br />
                <asp:Label ID="Label3" runat="server" Text="5 most productive cows"></asp:Label>
                <br />
                <asp:GridView ID="prodCow" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None"
                    Style="text-align: center">
                    <AlternatingRowStyle BackColor="White" />
                    <EditRowStyle BackColor="#2461BF" />
                    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                    <RowStyle BackColor="#EFF3FB" />
                    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                    <SortedAscendingCellStyle BackColor="#F5F7FB" />
                    <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                    <SortedDescendingCellStyle BackColor="#E9EBEF" />
                    <SortedDescendingHeaderStyle BackColor="#4870BE" />
                </asp:GridView>
                <br />
                <asp:Label ID="noAnimals" runat="server" Text="Label"></asp:Label>
                <br />
                <br />
                <asp:Label ID="avgMilkLabel" runat="server" Text="Label"></asp:Label>
                <br />
                <br />
                <asp:Label ID="totalMilkLabel" runat="server" Text="Label"></asp:Label>
                <br />
                <br />
            </asp:Panel>
        </div>
        <br class="clearfloat" />
    </div>
    </form>
</body>
</html>
