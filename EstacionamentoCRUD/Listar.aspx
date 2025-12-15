<%@ Page Title="Lista de Veículos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Listar.aspx.cs" Inherits="EstacionamentoCRUD.Pages.Listar" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Veículos Estacionados</h2>
    <div class="table-responsive">
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
            CssClass="table table-striped table-hover"
            GridLines="None">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="ID" />
                <asp:BoundField DataField="Placa" HeaderText="Placa" />
                <asp:BoundField DataField="Modelo" HeaderText="Modelo" />
                <asp:BoundField DataField="Cor" HeaderText="Cor" />
                <asp:BoundField DataField="DataEntrada" HeaderText="Entrada" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:HyperLinkField Text="✏️ Editar" DataNavigateUrlFields="Id" DataNavigateUrlFormatString="Editar.aspx?Id={0}"
                    ControlStyle-CssClass="btn btn-sm btn-outline-primary" />
                <asp:HyperLinkField Text="🗑️ Excluir" DataNavigateUrlFields="Id" DataNavigateUrlFormatString="Excluir.aspx?Id={0}"
                    ControlStyle-CssClass="btn btn-sm btn-outline-danger" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
