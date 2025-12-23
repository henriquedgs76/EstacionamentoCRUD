<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SolicitarRecuperacao.aspx.cs" Inherits="EstacionamentoCRUD.SolicitarRecuperacao" ResponseEncoding="utf-8" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Recuperação de Senha - Estacionamento</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
<link href="https://cdn.jsdelivr.net/npm/bootswatch@5.3.3/dist/darkly/bootstrap.min.css" rel="stylesheet" /></head>
<body class="bg-light d-flex justify-content-center align-items-center vh-100">

    <form id="formSolicitar" runat="server" class="card shadow-lg p-4" style="width: 400px;">
        <h2 class="text-center text-primary mb-4">Recuperar Senha</h2>

        <div class="mb-3">
            <label for="txtUsuario" class="form-label">Digite seu nome de usuário:</label>
            <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" placeholder="Seu usuário cadastrado"></asp:TextBox>
        </div>

        <div class="d-grid mb-3">
            <asp:Button ID="btnSolicitar" runat="server" CssClass="btn btn-warning btn-lg" Text="Solicitar Token" OnClick="btnSolicitar_Click" />
        </div>

        <asp:Label ID="lblMensagem" runat="server" CssClass="text-center fw-bold d-block mt-2"></asp:Label>

        <div class="text-center mt-3">
            <a href="Login.aspx">Voltar para o Login</a>
        </div>
    </form>

</body>
</html>
