<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Listar.aspx.cs" Inherits="EstacionamentoCRUD.Pages.Listar" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Lista de Veículos</title>
</head>
<body>
    <h2>Veículos Estacionados</h2>
    <form id="form1" runat="server">
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="ID" />
                <asp:BoundField DataField="Placa" HeaderText="Placa" />
                <asp:BoundField DataField="Modelo" HeaderText="Modelo" />
                <asp:BoundField DataField="Cor" HeaderText="Cor" />
                <asp:BoundField DataField="DataEntrada" HeaderText="Entrada" />
                <asp:HyperLinkField Text="Editar" DataNavigateUrlFields="Id" DataNavigateUrlFormatString="Editar.aspx?Id={0}" />
                <asp:HyperLinkField Text="Excluir" DataNavigateUrlFields="Id" DataNavigateUrlFormatString="Excluir.aspx?Id={0}" />
            </Columns>
        </asp:GridView>
    </form>
</body>
</html>
