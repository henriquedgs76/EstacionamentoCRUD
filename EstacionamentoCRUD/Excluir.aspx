<%@ Page Title="Excluir Veículo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Excluir.aspx.cs" Inherits="EstacionamentoCRUD.Excluir" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootswatch@5.3.3/dist/darkly/bootstrap.min.css" rel="stylesheet" />

    <div class="col-md-6 mx-auto">
        <div class="card shadow-lg p-4">
            <h2 class="text-center text-danger mb-4">Excluir Veículo</h2>

            <div class="mb-3">
                <label class="form-label">Placa:</label>
                <asp:TextBox ID="txtPlaca" runat="server" CssClass="form-control" placeholder="Digite a placa"></asp:TextBox>
            </div>

            <div class="row">
                <div class="col-6">
                    <asp:Button ID="btnBuscar" runat="server" CssClass="btn btn-info w-100" Text=" Buscar" OnClick="btnBuscar_Click" />
                </div>
                <div class="col-6">
                    <asp:LinkButton ID="btnExcluir" runat="server" CssClass="btn btn-danger w-100" OnClick="btnExcluir_Click"><i class="bi bi-trash me-1"></i> Excluir
                    </asp:LinkButton>
                </div>
            </div>

            <asp:Label ID="lblMensagem" runat="server" CssClass="text-center fw-bold mt-3 d-block"></asp:Label>

            <div class="text-center mt-4">
                <a href="Home.aspx" class="btn btn-secondary">Voltar para Home</a>
            </div>
        </div>
    </div>
</asp:Content>
