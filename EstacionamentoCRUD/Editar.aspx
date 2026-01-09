<%@ Page Title="Editar Veículo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Editar.aspx.cs" Inherits="EstacionamentoCRUD.Editar" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootswatch@5.3.3/dist/darkly/bootstrap.min.css" rel="stylesheet" />
    <div class="col-md-8 mx-auto">
        <div class="card shadow p-4">
            <h3 class="text-center mb-4 text-primary">Editar Veículo</h3>

            <div class="row mb-3">
                <div class="col-md-9">
                    <label class="form-label">Placa</label>
                    <asp:TextBox ID="txtPlaca" runat="server" CssClass="form-control" MaxLength="8" />
                </div>
                <div class="col-md-3 d-flex align-items-end">
                    <asp:Button ID="btnBuscar" runat="server" Text=" Buscar" CssClass="btn btn-secondary w-100" OnClick="btnBuscar_Click" />
                </div>
            </div>

            <div class="mb-3">
                <label class="form-label">Marca</label>
                <asp:DropDownList ID="ddlMarcas" runat="server" CssClass="form-select" />
            </div>

            <div class="mb-3">
                <label class="form-label">Modelo</label>
                <asp:TextBox ID="txtModelo" runat="server" CssClass="form-control" />
            </div>

            <div class="mb-3">
                <label class="form-label">Cor</label>
                <asp:TextBox ID="txtCor" runat="server" CssClass="form-control" />
            </div>

            <div class="row">
                <div class="col-md-6 mb-3">
                    <label class="form-label">Data de Entrada</label>
                    <asp:TextBox ID="txtDataEntrada" runat="server" CssClass="form-control" TextMode="Date" />
                </div>
                <div class="col-md-6 mb-3">
                    <label class="form-label">Hora de Entrada</label>
                    <asp:TextBox ID="txtHoraEntrada" runat="server" CssClass="form-control" TextMode="Time" />
                </div>
            </div>
            <asp:HiddenField ID="hfVeiculoId" runat="server" />
            <div class="text-center">
                <asp:Button ID="btnSalvar" runat="server" Text=" Salvar Alterações" CssClass="btn btn-primary" OnClick="btnSalvar_Click" />
                <a href="Home.aspx" class="btn btn-outline-secondary ms-2"> Voltar</a>
            </div>

            <div class="mt-4 text-center">
                <asp:Label ID="lblMensagem" runat="server" CssClass="fw-bold"></asp:Label>
            </div>
        </div>
    </div>
</asp:Content>
