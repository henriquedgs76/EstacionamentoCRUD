<%@ Page Title="Baixa de Saída" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BaixaSaida.aspx.cs" Inherits="EstacionamentoCRUD.BaixaSaida" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootswatch@5.3.3/dist/darkly/bootstrap.min.css" rel="stylesheet" />
    <div class="col-md-6 mx-auto">
        <div class="card shadow-lg p-4">
            <h2 class="text-center mb-4 text-primary">Saída de Veículo</h2>

            <div class="row">
                <div class="col-md-9 mb-3">
                    <label class="form-label">Placa do Veículo:</label>
                    <asp:TextBox ID="txtPlaca" runat="server" CssClass="form-control" placeholder="Digite a placa do veículo"></asp:TextBox>
                </div>
                 <div class="col-md-3 d-flex align-items-end mb-3">
                    <asp:Button ID="btnCalcular" runat="server" CssClass="btn btn-info w-100" Text="Calcular" OnClick="btnCalcular_Click" />
                </div>
            </div>

            <div class="mb-3">
                <label class="form-label">Valor a Pagar (R$):</label>
                <asp:TextBox ID="txtValorPago" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
            </div>

            <div class="mb-3 d-grid">
                <asp:Button ID="btnDarBaixa" runat="server" CssClass="btn btn-success" Text="✅ Confirmar Saída" OnClick="btnDarBaixa_Click" />
            </div>

            <asp:Label ID="lblMensagem" runat="server" CssClass="text-center fw-bold mt-3 d-block"></asp:Label>

            <div class="text-center mt-3">
                <a href="Home.aspx" class="btn btn-secondary">⬅️ Voltar para Home</a>
            </div>
        </div>
    </div>
</asp:Content>
