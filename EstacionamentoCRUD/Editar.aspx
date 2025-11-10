<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Editar.aspx.cs" Inherits="EstacionamentoCRUD.Editar" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Editar Veículo - Estacionamento</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="bg-light">
    <form id="form1" runat="server" class="container mt-5">
        <div class="card shadow p-4">
            <h3 class="text-center mb-4 text-primary">✏️ Editar Veículo</h3>

            <div class="mb-3">
                <label class="form-label">Placa</label>
                <asp:TextBox ID="txtPlaca" runat="server" CssClass="form-control" MaxLength="8" />
            </div>

            <div class="mb-3">
                <label class="form-label">Modelo</label>
                <asp:TextBox ID="txtModelo" runat="server" CssClass="form-control" />
            </div>

            <div class="mb-3">
                <label class="form-label">Cor</label>
                <asp:TextBox ID="txtCor" runat="server" CssClass="form-control" />
            </div>

            <div class="mb-3">
                <label class="form-label">Data de Entrada</label>
                <asp:TextBox ID="txtDataEntrada" runat="server" CssClass="form-control" TextMode="Date" />
            </div>

            <div class="mb-3">
                <label class="form-label">Hora de Entrada</label>
                <asp:TextBox ID="txtHoraEntrada" runat="server" CssClass="form-control" TextMode="Time" />
            </div>

            <div class="text-center">
                <asp:Button ID="btnBuscar" runat="server" Text="🔍 Buscar" CssClass="btn btn-secondary me-2" OnClick="btnBuscar_Click" />
                <asp:Button ID="btnSalvar" runat="server" Text="💾 Salvar Alterações" CssClass="btn btn-primary" OnClick="btnSalvar_Click" />
                <a href="Home.aspx" class="btn btn-outline-danger ms-2">⬅ Voltar</a>
            </div>

            <div class="mt-4 text-center">
                <asp:Label ID="lblMensagem" runat="server" CssClass="fw-bold"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
