<%@ Page Title="Fidelização" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RelatorioClientes.aspx.cs" Inherits="EstacionamentoCRUD.RelatorioClientes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <link href="https://cdn.jsdelivr.net/npm/bootswatch@5.3.3/dist/darkly/bootstrap.min.css" rel="stylesheet" />

    <div class="container mt-4">
        <div class="card shadow bg-dark text-white">
            <div class="card-header bg-primary d-flex justify-content-between">
                <h4 class="mb-0">📢 Clientes que não voltam há (+) 15 dias</h4>
                <a href="Home.aspx" class="btn btn-sm btn-light">Voltar</a>
            </div>
            <div class="card-body">
                <asp:GridView ID="gvClientesAusentes" runat="server" CssClass="table table-dark table-hover" 
                    AutoGenerateColumns="false" GridLines="None" EmptyDataText="Todos os clientes visitaram recentemente!">
                    <Columns>
                        <asp:BoundField DataField="UltimaVisita" HeaderText="Última Visita" DataFormatString="{0:dd/MM/yyyy}" />
                        <asp:BoundField DataField="Placa" HeaderText="Placa" ItemStyle-CssClass="fw-bold text-info" />
                        <asp:BoundField DataField="Modelo" HeaderText="Veículo" />
                        <asp:TemplateField HeaderText="Ação">
                            <ItemTemplate>
                                <asp:HyperLink ID="lnkWhats" runat="server" Target="_blank" CssClass="btn btn-success btn-sm"
                                    NavigateUrl='<%# "https://wa.me/55" + Eval("Telefone").ToString().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "") + "?text=Olá! Sentimos sua falta aqui no Baronesa. Que tal uma limpeza no seu " + Eval("Modelo") + "?" %>'>
                                    <i class="fab fa-whatsapp"></i> Chamar no Whats
                                </asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>