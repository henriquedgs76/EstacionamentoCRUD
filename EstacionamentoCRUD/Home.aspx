<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Estacionamento.Home" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Gestão de Estacionamento</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="bg-light">
    <form id="form1" runat="server">
        <div class="container mt-5">
            <div class="card shadow-lg">
                <div class="card-header bg-primary text-white text-center">
                    <h3>🚗 Controle de Veículos - Estacionamento</h3>
                </div>
                <div class="card-body">
                    <div class="text-end mb-3">
                        <a href="Cadastrar.aspx" class="btn btn-success">+ Novo Veículo</a>
                    </div>

                    <asp:GridView ID="gvVeiculos" runat="server" CssClass="table table-striped table-hover text-center" AutoGenerateColumns="False">
                        <Columns>
                            <asp:BoundField DataField="Id" HeaderText="ID" />
                            <asp:BoundField DataField="Placa" HeaderText="Placa" />
                            <asp:BoundField DataField="Modelo" HeaderText="Modelo" />
                            <asp:BoundField DataField="Cor" HeaderText="Cor" />
                            <asp:BoundField DataField="DataEntrada" HeaderText="Entrada" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:BoundField DataField="HoraEntrada" HeaderText="Hora" />
                            <asp:BoundField DataField="DataSaida" HeaderText="Saída" DataFormatString="{0:dd/MM/yyyy}" />
                            <asp:BoundField DataField="ValorPago" HeaderText="Valor Pago" DataFormatString="{0:C}" />
                            <asp:BoundField DataField="Status" HeaderText="Status" />
 <asp:TemplateField HeaderText="Ações">
    <ItemTemplate>
        <%# 
            Eval("Status").ToString() == "Finalizado" 
            ? "<a href='Excluir.aspx?id=" + Eval("Id") + "' class='btn btn-danger btn-sm'>Excluir</a>" 
            : "<a href='Editar.aspx?id=" + Eval("Id") + "' class='btn btn-warning btn-sm me-1'>Editar</a>" +
              "<a href='Excluir.aspx?id=" + Eval("Id") + "' class='btn btn-danger btn-sm me-1'>Excluir</a>" +
              "<a href='BaixaSaida.aspx?id=" + Eval("Id") + "' class='btn btn-secondary btn-sm'>Dar Baixa</a>"
        %>
    </ItemTemplate>
</asp:TemplateField>


                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
