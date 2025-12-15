<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="EstacionamentoCRUD.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login - Estacionamento</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="bg-light d-flex justify-content-center align-items-center vh-100">

    <form id="form1" runat="server" class="card shadow-lg p-4" style="width: 400px;">
        <h2 class="text-center text-primary mb-4"> Sistema de Estacionamento</h2>

        <div class="mb-3">
            <label for="txtUsuario" class="form-label">Usuário:</label>
            <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" placeholder="Digite seu usuário"></asp:TextBox>
        </div>

        <div class="mb-3">
            <label for="txtSenha" class="form-label">Senha:</label>
            <asp:TextBox ID="txtSenha" runat="server" TextMode="Password" CssClass="form-control" placeholder="Digite sua senha"></asp:TextBox>
        </div>

        <div class="d-grid mb-3">
            <asp:Button ID="btnLogin" runat="server" CssClass="btn btn-primary btn-lg" Text="Entrar" OnClick="btnLogin_Click" />
        </div>

        <asp:Label ID="lblMensagem" runat="server" CssClass="text-center fw-bold d-block mt-2"></asp:Label>

        <div class="text-center mt-3">
            <small class="text-muted">© 2025 - Sistema Estacionamento</small>
        </div>
    </form>

</body>
</html>
