<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Estacionamento.Home" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h3>Dashboard</h3>
        <a href="Cadastrar.aspx" class="btn btn-success">+ Novo Veículo</a>
    </div>

    <asp:GridView ID="gvVeiculos" runat="server" CssClass="table table-striped table-hover text-center" AutoGenerateColumns="False" GridLines="None">
        <Columns>
            <asp:BoundField DataField="Placa" HeaderText="Placa" />
            <asp:BoundField DataField="Modelo" HeaderText="Modelo" />
            <asp:BoundField DataField="Status" HeaderText="Status" />
            <asp:BoundField DataField="DataEntrada" HeaderText="Entrada" DataFormatString="{0:dd/MM/yyyy}" />
            <asp:BoundField DataField="HoraEntrada" HeaderText="Hora" />
            <asp:TemplateField HeaderText="Ações">
                <ItemTemplate>
                    <asp:HyperLink runat="server" NavigateUrl='<%# Eval("Status").ToString() == "Finalizado" ? "" : "Editar.aspx?id=" + Eval("Id") %>'
                        CssClass='<%# Eval("Status").ToString() == "Finalizado" ? "btn btn-warning btn-sm me-1 disabled" : "btn btn-warning btn-sm me-1" %>' Text="✏️ Editar" />
                    <asp:HyperLink runat="server" NavigateUrl='<%# "Excluir.aspx?id=" + Eval("Id") %>' 
                        CssClass="btn btn-danger btn-sm me-1" Text="🗑️ Excluir" />
                    <asp:HyperLink runat="server" NavigateUrl='<%# Eval("Status").ToString() == "Finalizado" ? "" : "BaixaSaida.aspx?id=" + Eval("Id") %>'
                        CssClass='<%# Eval("Status").ToString() == "Finalizado" ? "btn btn-secondary btn-sm disabled" : "btn btn-secondary btn-sm" %>' Text="➡️ Saída" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
