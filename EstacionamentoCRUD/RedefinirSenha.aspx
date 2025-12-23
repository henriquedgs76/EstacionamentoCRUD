<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RedefinirSenha.aspx.cs" Inherits="EstacionamentoCRUD.RedefinirSenha" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Redefinir Senha - Estacionamento</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
<link href="https://cdn.jsdelivr.net/npm/bootswatch@5.3.3/dist/darkly/bootstrap.min.css" rel="stylesheet" /></head>
<body class="bg-light d-flex justify-content-center align-items-center vh-100">

    <form id="formRedefinir" runat="server" class="card shadow-lg p-4" style="width: 400px;">
        <h2 class="text-center text-primary mb-4">Redefinir sua Senha</h2>

        <div class="mb-3">
            <label for="txtToken" class="form-label">Token de Recuperação:</label>
            <asp:TextBox ID="txtToken" runat="server" CssClass="form-control" placeholder="Cole o token que você recebeu"></asp:TextBox>
        </div>

        <div class="mb-3">
            <label for="txtNovaSenha" class="form-label">Nova Senha:</label>
            <asp:TextBox ID="txtNovaSenha" runat="server" TextMode="Password" CssClass="form-control" placeholder="Crie uma nova senha"></asp:TextBox>
        </div>
        
        <div class="mb-3">
            <label for="txtConfirmarNovaSenha" class="form-label">Confirmar Nova Senha:</label>
            <asp:TextBox ID="txtConfirmarNovaSenha" runat="server" TextMode="Password" CssClass="form-control" placeholder="Digite a nova senha novamente"></asp:TextBox>
        </div>

        <div class="d-grid mb-3">
            <asp:Button ID="btnSalvar" runat="server" CssClass="btn btn-primary btn-lg" Text="Salvar Nova Senha" OnClick="btnSalvar_Click" />
        </div>

        <asp:Label ID="lblMensagem" runat="server" CssClass="text-center fw-bold d-block mt-2"></asp:Label>

        <div class="text-center mt-3">
            <a href="Login.aspx">Voltar para o Login</a>
        </div>
    </form>

</body>
</html>
