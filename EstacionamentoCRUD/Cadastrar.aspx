<%@ Page Title="Cadastrar Veículo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cadastrar.aspx.cs" Inherits="Estacionamento.Cadastrar" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootswatch@5.3.3/dist/darkly/bootstrap.min.css" rel="stylesheet" />
    <div class="col-md-6 mx-auto">
        <div class="card shadow">
            <div class="card-header bg-success text-white text-center">
                <h4>Adicionar Veículo</h4>
            </div>
            <div class="card-body">
                <div class="mb-3">
                    <label class="form-label">Placa</label>
                    <asp:TextBox ID="txtPlaca" runat="server" CssClass="form-control" />
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
                    <label class="form-label">Vaga Disponível</label>
                    <asp:DropDownList ID="ddlVagas" runat="server" CssClass="form-select" />
                </div>
                <asp:Label ID="lblMensagem" runat="server" />
                <div class="text-center mt-3">
                    <asp:Button ID="btnSalvar" runat="server" Text="Salvar" CssClass="btn btn-success" OnClick="btnSalvar_Click" />
                    <a href="Home.aspx" class="btn btn-secondary">Voltar</a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
