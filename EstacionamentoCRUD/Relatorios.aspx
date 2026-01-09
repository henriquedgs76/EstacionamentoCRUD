<%@ Page Title="Relatórios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Relatorios.aspx.cs" Inherits="EstacionamentoCRUD.Relatorios" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootswatch@5.3.3/dist/darkly/bootstrap.min.css" rel="stylesheet" />

    <h2>Relatórios de Faturamento</h2>
    <hr />

    <%-- Seção para Relatório Diário --%>
    <div class="row align-items-end">

        <div class="col-md-3">

            <label for="<%=txtDataDiario.ClientID%>" class="form-label"><b>Relatório Diário</b></label>
            <asp:TextBox ID="txtDataDiario" runat="server" 
                CssClass="form-control" TextMode="Date"></asp:TextBox>
        </div>
        <div class="col-md-2">
            <asp:Button ID="btnGerarDiario" runat="server" 
                Text="Gerar Relatório Diário" CssClass="btn btn-primary" OnClick="btnGerarDiario_Click" />
        </div>
    </div>

    <hr />

    <%-- Seção para Relatório Mensal --%>
    <div class="row align-items-end">
        <div class="col-md-3">
            <label class="form-label"><b>Gerar Relatório Mensal</b></label>
            <asp:DropDownList ID="ddlMes" runat="server" CssClass="form-select">
                <asp:ListItem Value="1">Janeiro</asp:ListItem>
                <asp:ListItem Value="2">Fevereiro</asp:ListItem>
                <asp:ListItem Value="3">Março</asp:ListItem>
                <asp:ListItem Value="4">Abril</asp:ListItem>
                <asp:ListItem Value="5">Maio</asp:ListItem>
                <asp:ListItem Value="6">Junho</asp:ListItem>
                <asp:ListItem Value="7">Julho</asp:ListItem>
                <asp:ListItem Value="8">Agosto</asp:ListItem>
                <asp:ListItem Value="9">Setembro</asp:ListItem>
                <asp:ListItem Value="10">Outubro</asp:ListItem>
                <asp:ListItem Value="11">Novembro</asp:ListItem>
                <asp:ListItem Value="12">Dezembro</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="col-md-2">
            <asp:DropDownList ID="ddlAno" runat="server" 
                CssClass="form-select"></asp:DropDownList>
        </div>
        <div class="col-md-2">
            <asp:Button ID="btnGerarMensal" runat="server" 
                Text="Gerar Relatório Mensal" CssClass="btn btn-secondary" 
                OnClick="btnGerarMensal_Click" />
        </div>
    </div>

    <hr />

    <%-- Seção de Resultados --%>
    <div id="divResultados" runat="server" visible="false" class="mt-4">
        <div class="d-flex justify-content-between align-items-center">
            <h4>Resultados do Relatório</h4>
            <asp:Button ID="btnExportarPdf" runat="server" 
                Text="Transformar em PDF" CssClass="btn btn-outline-danger" 
                OnClick="btnExportarPdf_Click" Visible="false" />
        </div>
        <div class="alert alert-success">
            <h4>Faturamento Total: <asp:Label ID="lblTotalFaturamento" runat="server" 
                Text="R$ 0,00"></asp:Label></h4>
        </div>

        <asp:GridView ID="gvRelatorio" runat="server" CssClass="table table-striped table-hover" 
            GridLines="None" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="Placa" HeaderText="Placa" />
                <asp:BoundField DataField="Modelo" HeaderText="Modelo" />
                <asp:BoundField DataField="NomeServico" HeaderText="Serviço" />
                <asp:BoundField DataField="DataSaida" HeaderText="Data da Saída" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <asp:BoundField DataField="ValorPago" HeaderText="Valor Pago" DataFormatString="{0:c}" />
            </Columns>
        </asp:GridView>
         <asp:Label ID="lblMensagem" runat="server" CssClass="text-info"></asp:Label>
    </div>

</asp:Content>
