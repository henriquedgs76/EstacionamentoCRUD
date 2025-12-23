<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CadastrarUsuario.aspx.cs" Inherits="EstacionamentoCRUD.CadastrarUsuario" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Cadastro de Usuário - Estacionamento</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
<link href="https://cdn.jsdelivr.net/npm/bootswatch@5.3.3/dist/darkly/bootstrap.min.css" rel="stylesheet" /></head>
<body class="bg-light d-flex justify-content-center align-items-center vh-100">

    <form id="formCadastrar" runat="server" class="card shadow-lg p-4" style="width: 400px;">
        <h2 class="text-center text-primary mb-4">Cadastro de Novo Usuário</h2>

        <div class="mb-3">
            <label for="txtUsuario" class="form-label">Usuário:</label>
            <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" placeholder="Escolha um nome de usuário"></asp:TextBox>
        </div>

        <div class="mb-3">
            <label for="txtSenha" class="form-label">Senha:</label>
            <asp:TextBox ID="txtSenha" runat="server" TextMode="Password" CssClass="form-control" placeholder="Crie uma senha forte"></asp:TextBox>
        </div>
        
        <div class="mb-3">
            <label for="txtConfirmarSenha" class="form-label">Confirmar Senha:</label>
            <asp:TextBox ID="txtConfirmarSenha" runat="server" TextMode="Password" CssClass="form-control" placeholder="Digite a senha novamente"></asp:TextBox>
        </div>

        <div class="d-grid mb-3">
            <asp:Button ID="btnCadastrar" runat="server" CssClass="btn btn-success btn-lg" Text="Cadastrar" OnClick="btnCadastrar_Click" />
        </div>

        <asp:Label ID="lblMensagem" runat="server" CssClass="text-center fw-bold d-block mt-2"></asp:Label>

        <div class="text-center mt-3">
            <a href="Login.aspx">Já tem uma conta? Faça o login</a>
        </div>
    </form>

</body>
</html>