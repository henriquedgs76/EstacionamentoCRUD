<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Excluir.aspx.cs" Inherits="EstacionamentoCRUD.Excluir" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Excluir Veículo</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="bg-light">
    <form id="form1" runat="server" class="container mt-5">
        <div class="card shadow-lg p-4">
            <h2 class="text-center text-danger mb-4">❌ Excluir Veículo</h2>

            <div class="mb-3">
                <label for="txtPlaca" class="form-label">Placa:</label>
                <asp:TextBox ID="txtPlaca" runat="server" CssClass="form-control" placeholder="Digite a placa"></asp:TextBox>
            </div>

            <div class="d-flex justify-content-between">
                <asp:Button ID="btnBuscar" runat="server" CssClass="btn btn-info" Text="🔍 Buscar" OnClick="btnBuscar_Click" />
                <asp:Button ID="btnExcluir" runat="server" CssClass="btn btn-danger" Text="🗑️ Excluir" OnClick="btnExcluir_Click" />
            </div>

            <asp:Label ID="lblMensagem" runat="server" CssClass="text-center fw-bold mt-3 d-block"></asp:Label>

            <div class="text-center mt-3">
                <a href="Home.aspx" class="btn btn-secondary">⬅️ Voltar para Home</a>
            </div>
        </div>
    </form>
</body>
</html>
