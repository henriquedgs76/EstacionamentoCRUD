<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BaixaSaida.aspx.cs" Inherits="EstacionamentoCRUD.BaixaSaida" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Baixa de Saída - Estacionamento</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="bg-light">
    <form id="form1" runat="server" class="container mt-5">
        <div class="card shadow-lg p-4">
            <h2 class="text-center mb-4 text-primary">🚗 Dar Baixa de Saída</h2>

            <div class="mb-3">
                <label for="txtPlaca" class="form-label">Placa do Veículo:</label>
                <asp:TextBox ID="txtPlaca" runat="server" CssClass="form-control" placeholder="Digite a placa do veículo"></asp:TextBox>
            </div>

            <div class="mb-3">
                <label for="txtValorPago" class="form-label">Valor a Pagar (R$):</label>
                <asp:TextBox ID="txtValorPago" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
            </div>

            <div class="mb-3">
                <asp:Button ID="btnCalcular" runat="server" CssClass="btn btn-info w-100 mb-3" Text="💰 Calcular Valor" OnClick="btnCalcular_Click" />
                <asp:Button ID="btnDarBaixa" runat="server" CssClass="btn btn-success w-100" Text="✅ Confirmar Saída" OnClick="btnDarBaixa_Click" />
            </div>

            <asp:Label ID="lblMensagem" runat="server" CssClass="text-center fw-bold mt-3 d-block"></asp:Label>

            <div class="text-center mt-3">
                <a href="Home.aspx" class="btn btn-secondary">⬅️ Voltar para Home</a>
            </div>
        </div>
    </form>
</body>
</html>
