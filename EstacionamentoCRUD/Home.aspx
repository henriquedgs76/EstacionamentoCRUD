<%@ Page Title="Página Inicial" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Estacionamento.Home" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .horizonte-container {
            margin-bottom: 2rem;
            padding: 1.5rem;
            background-color: #454d55; /* Cor de fundo escura para tema Darkly */
            border-radius: .5rem;
            box-shadow: 0 4px 8px rgba(0,0,0,0.2);
        }

        .timeline {
            display: flex;
            height: 60px;
            width: 100%;
            border: 1px solid #6c757d;
            border-radius: .25rem;
            overflow: hidden;
        }

        .hour-block {
            flex-grow: 1;
            position: relative;
            transition: all 0.2s ease-in-out;
            cursor: pointer;
        }

            .hour-block:hover {
                transform: scaleY(1.1);
                z-index: 10;
            }

            .hour-block .tooltip-text {
                visibility: hidden;
                width: 160px;
                background-color: #212529;
                color: #fff;
                text-align: center;
                border-radius: 6px;
                padding: 5px 0;
                position: absolute;
                z-index: 1;
                bottom: 125%;
                left: 50%;
                margin-left: -80px;
                opacity: 0;
                transition: opacity 0.3s;
            }

            .hour-block:hover .tooltip-text {
                visibility: visible;
                opacity: 1;
            }

        .timeline-labels {
            display: flex;
            justify-content: space-between;
            padding: 0 10px;
            margin-top: 5px;
            font-size: 0.75rem;
            color: #adb5bd;
        }
    </style>

    <link href="https://cdn.jsdelivr.net/npm/bootswatch@5.3.3/dist/darkly/bootstrap.min.css" rel="stylesheet" />

    <div class="horizonte-container">
        <h3 class="mb-3">Mapa de Fluxo do Estacionamento</h3>
        <p id="alertasPreditivos" class="alert alert-info"><%= AlertasPreditivos %></p>
        <div id="timeline" class="timeline"></div>
        <div class="timeline-labels">
            <span>00:00</span>
            <span>06:00</span>
            <span>12:00</span>
            <span>18:00</span>
            <span>23:59</span>
        </div>
    </div>

   <div class="row mb-4">
    <div class="col-md-4">
        <div class="card bg-primary text-white text-center shadow-sm">
            <div class="card-body p-2">
                <h6 class="card-title mb-0">No Pátio</h6>
                <h3 class="fw-bold"><asp:Literal ID="litPatio" runat="server">0</asp:Literal></h3>
            </div>
        </div>
    </div>

    <div class="col-md-4">
        <div class="card bg-warning text-dark text-center shadow-sm">
            <div class="card-body p-2">
                <h6 class="card-title mb-0">Lavagens Pendentes</h6>
                <h3 class="fw-bold"><asp:Literal ID="litPendentes" runat="server">0</asp:Literal></h3>

            </div>
        </div>
    </div>

    <div class="col-md-4">
        <div class="card bg-danger text-white text-center shadow-sm" style="cursor: pointer;" onclick="location.href='RelatorioClientes.aspx'">
            <div class="card-body p-2">
                <h6 class="card-title mb-0">Recuperar Clientes   (+15 dias sem vir)</h6>
                <h3 class="fw-bold"><asp:Literal ID="litAusentes" runat="server">0</asp:Literal></h3>
                
            </div>
        </div>
    </div>
</div>


    <div class="d-flex justify-content-between align-items-center mb-3">
        <h3>Veículos Ativos</h3>
        <a href="Cadastrar.aspx" class="btn btn-success"><i class="bi bi-car-front-fill me-2"></i>Adicionar Veículo</a>
    </div>



    <asp:GridView ID="gvVeiculos" runat="server" CssClass="table table-striped table-hover text-center" AutoGenerateColumns="False" GridLines="None">
        <Columns>
            <asp:BoundField DataField="Placa" HeaderText="Placa" />
            <asp:BoundField DataField="Marca" HeaderText="Marca" />
            <asp:BoundField DataField="Modelo" HeaderText="Modelo" />
            <asp:BoundField DataField="NumeroDaVaga" HeaderText="Vaga" />
            <asp:BoundField DataField="Status" HeaderText="Status" />
            <asp:BoundField DataField="DataEntrada" HeaderText="Entrada" DataFormatString="{0:dd/MM/yyyy}" />
            <asp:BoundField DataField="HoraEntrada" HeaderText="Hora" />
           <asp:TemplateField HeaderText="Tipo de Serviço">
    <ItemTemplate>
        <%# Eval("NomeServico").ToString() == "Estacionamento" ? 
            "<span class='badge bg-info'><i class='bi bi-p-circle me-1'></i> Estacionamento</span>" : 
            "<span class='badge " + (Eval("StatusLavagem").ToString() == "Pendente" || Eval("StatusLavagem").ToString() == "Aguardando" ? "bg-warning text-dark" : "bg-success") + "'>" + 
            "<i class='bi bi-droplet-fill me-1'></i> " + Eval("NomeServico") + ": " + Eval("StatusLavagem") + "</span>" 
        %>
    </ItemTemplate>
</asp:TemplateField>
            <asp:TemplateField HeaderText="Ações">
                <ItemTemplate>
                    <asp:HyperLink runat="server" NavigateUrl='<%# Eval("Status").ToString() == "Finalizado" ? "" : "Editar.aspx?id=" + Eval("Id") %>'
                        CssClass='<%# Eval("Status").ToString() == "Finalizado" ? "btn btn-warning btn-sm me-1 disabled" : "btn btn-warning btn-sm me-1" %>' Text=" Editar" />
                    <asp:HyperLink runat="server" NavigateUrl='<%# "Excluir.aspx?id=" + Eval("Id") %>'
                        CssClass="btn btn-danger btn-sm me-1" Text=" Excluir" />
                    <asp:HyperLink runat="server" NavigateUrl='<%# Eval("Status").ToString() == "Finalizado" ? "" : "BaixaSaida.aspx?id=" + Eval("Id") %>'
                        CssClass='<%# Eval("Status").ToString() == "Finalizado" ? "btn btn-secondary btn-sm disabled" : "btn btn-secondary btn-sm" %>' Text=" Saída" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", function () {
            // Pega os dados JSON do code-behind
            var horizonteData = <%= HorizonteJsonData %>;
            var timelineContainer = document.getElementById("timeline");

            // Função para converter o nível de "calor" (0 a 1) em uma cor HSL (azul para vermelho)
            function getHeatColor(calor) {
                // HSL: Hue(matiz) de 240 (azul) a 0 (vermelho)
                var hue = 240 - (calor * 240);
                return "hsl(" + hue + ", 75%, 50%)";
            }

            // Gera os blocos de hora
            horizonteData.forEach(function (data) {
                var hourBlock = document.createElement("div");
                hourBlock.className = "hour-block";

                var calor = data.Calor;
                hourBlock.style.backgroundColor = getHeatColor(calor);

                // Adiciona o tooltip
                var tooltip = document.createElement("span");
                tooltip.className = "tooltip-text";
                tooltip.innerHTML = "<b>" + String(data.Hora).padStart(2, '0') + "h-" + String(data.Hora + 1).padStart(2, '0') + "h</b><br/>" +
                    "Atividade: " + data.TotalAtividade + " eventos";
                hourBlock.appendChild(tooltip);

                timelineContainer.appendChild(hourBlock);
            });
        });
    </script>
</asp:Content>
