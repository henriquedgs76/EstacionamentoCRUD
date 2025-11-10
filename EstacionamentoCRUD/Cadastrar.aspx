<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Cadastrar.aspx.cs" Inherits="Estacionamento.Cadastrar" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Cadastrar Veículo</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="bg-light">
    <form id="form1" runat="server">
        <div class="container mt-5 col-md-6">
            <div class="card shadow">
                <div class="card-header bg-success text-white text-center">
                    <h4>Adicionar Veículo</h4>
                </div>
                <div class="card-body">
                    <div class="mb-3">
                        <label>Placa</label>
                        <asp:TextBox ID="txtPlaca" runat="server" CssClass="form-control" />
                    </div>
                    <div class="mb-3">
                        <label>Modelo</label>
                        <asp:TextBox ID="txtModelo" runat="server" CssClass="form-control" />
                    </div>
                    <div class="mb-3">
                        <label>Cor</label>
                        <asp:TextBox ID="txtCor" runat="server" CssClass="form-control" />
                    </div>
                    <asp:Label ID="lblMensagem" runat="server" />
                    <div class="text-center">
                        <asp:Button ID="btnSalvar" runat="server" Text="Salvar" CssClass="btn btn-success" OnClick="btnSalvar_Click" />
                        <a href="Home.aspx" class="btn btn-secondary">Voltar</a>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
