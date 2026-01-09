<%@ Page Title="Dashboard"
    Language="C#"
    MasterPageFile="~/Site.master"
    AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs"
    Inherits="EstacionamentoCRUD.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootswatch@5.3.3/dist/darkly/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

    <style>
        .dashboard {
            display: grid;
            grid-template-columns: repeat(2, 1fr); /* Ajustado para 2 colunas para os 2 gráficos */
            gap: 20px;
            margin-top: 20px;
        }

        .card {
            background: #303030;
            border-radius: 12px;
            padding: 20px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.3);
            border: 1px solid #444;
        }

        h4 {
            color: #fff;
            font-weight: 600;
            margin-bottom: 15px;
        }

        canvas {
            width: 100% !important;
            height: 300px !important;
        }

        .kpi-container {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 20px;
            margin-top: 20px;
            margin-bottom: 30px;
        }

        .kpi-card {
            background: #303030;
            border: 1px solid #444;
            border-radius: 10px;
            padding: 20px;
            text-align: center;
        }

        .kpi-title {
            font-size: 13px;
            color: #aaa;
            text-transform: uppercase;
        }

        .kpi-value {
            font-size: 24px;
            font-weight: bold;
            color: #fff;
        }

        .form-select:hover {
            border-color: #375a7f !important;
            transition: 0.3s;
        }
    </style>

    <div class="row mb-4 mt-3">
        <div class="col-md-12 d-flex justify-content-end align-items-center">
            <div class="input-group w-auto shadow-sm">
                <span class="input-group-text bg-primary text-white border-primary">
                    <i class="fas fa-calendar-alt"></i>
                </span>
                <asp:DropDownList ID="ddlPeriodo" runat="server" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlPeriodo_SelectedIndexChanged"
                    CssClass="form-select bg-dark text-white border-primary"
                    Style="cursor: pointer;">
                    <asp:ListItem Text="Hoje" Value="0" />
                    <asp:ListItem Text="Últimos 7 dias" Value="7" Selected="True" />
                    <asp:ListItem Text="Últimos 15 dias" Value="15" />
                    <asp:ListItem Text="Últimos 30 dias" Value="30" />
                    <asp:ListItem Text="Últimos 60 dias" Value="60" />
                </asp:DropDownList>
            </div>
        </div>
    </div>

    <div class="kpi-container">
        <div class="kpi-card">
            <div class="kpi-title">Faturamento Hoje</div>
            <div class="kpi-value">
                <asp:Label ID="lblFaturamentoHoje" runat="server" />
            </div>
        </div>
        <div class="kpi-card">
            <div class="kpi-title">Veículos Estacionados</div>
            <div class="kpi-value">
                <asp:Label ID="lblVeiculosAtivos" runat="server" />
            </div>
        </div>
        <div class="kpi-card">
            <div class="kpi-title">Ticket Médio</div>
            <div class="kpi-value">
                <asp:Label ID="lblTicketMedio" runat="server" />
            </div>
        </div>
        <div class="kpi-card">
            <div class="kpi-title">Entradas Hoje</div>
            <div class="kpi-value">
                <asp:Label ID="lblEntradasHoje" runat="server" />
            </div>
        </div>
    </div>
    <div class="kpi-card">
        <div class="kpi-title">Ocupação (50 vagas)</div>
        <div class="kpi-value">
            <asp:Label ID="lblOcupacao" runat="server" />
        </div>
    </div>
    <div class="kpi-card" onclick="window.location='RelatorioClientes.aspx';" style="cursor: pointer; border: 1px solid #e74c3c;">
        <div class="kpi-title" style="color: #ff6b6b;">Clientes Ausentes (+15 dias)</div>
        <div class="kpi-value text-danger">
            <asp:Label ID="lblClientesAusentes" runat="server" Text="0" />
        </div>
        <small style="font-size: 10px; color: #aaa;">Clique para ver a lista</small>
    </div>



    <div class="dashboard">
        <div class="card">
            <h4>Faturamento por Dia</h4>
            <canvas id="graficoFaturamento"></canvas>
        </div>
        <div class="card">
            <h4>Entradas e Saídas</h4>
            <canvas id="graficoVeiculos"></canvas>
        </div>
    </div>

    <div style="margin-top: 20px; text-align: center;">
        <asp:Button ID="btnVoltarHome" runat="server" Text="Voltar para Home" OnClick="btnVoltarHome_Click" CssClass="btn btn-primary" />
    </div>

    <asp:HiddenField ID="hfFaturamento" runat="server" />
    <asp:HiddenField ID="hfVeiculos" runat="server" />
    <asp:HiddenField ID="hfStatus" runat="server" />

    <script>
        const delayedAnimation = {
            delay: (context) => {
                let delay = 0;
                if (context.type === 'data' && context.mode === 'default') {
                    delay = context.dataIndex * 150 + context.datasetIndex * 200;
                }
                return delay;
            },
            duration: 2000, // Ajustado para 2s para o efeito ser mais fluido
            easing: 'easeOutElastic'
        };

        document.addEventListener("DOMContentLoaded", function () {

            // --- GRÁFICO FATURAMENTO ---
            var hfFat = document.getElementById('<%= hfFaturamento.ClientID %>');
            if (hfFat && hfFat.value) {
                var faturamento = JSON.parse(hfFat.value);
                new Chart(document.getElementById('graficoFaturamento'), {
                    type: 'line',
                    data: {
                        labels: faturamento.labels,
                        datasets: [{
                            label: 'Faturamento',
                            data: faturamento.values,
                            borderColor: '#375a7f',
                            backgroundColor: 'rgba(55, 90, 127, 0.2)',
                            pointRadius: 4,
                            tension: 0.1,
                            fill: true
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: { labels: { color: '#fff' } },
                            tooltip: {
                                callbacks: {
                                    label: function (context) {
                                        let val = context.parsed.y || 0;
                                        return 'Total: ' + val.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
                                    }
                                }
                            }
                        },
                        scales: {
                            y: {
                                beginAtZero: true,
                                ticks: {
                                    color: '#fff',
                                    callback: function (value) {
                                        return 'R$ ' + value.toLocaleString('pt-BR', { minimumFractionDigits: 2 });
                                    }
                                }
                            },
                            x: { ticks: { color: '#fff' } }
                        }
                    }
                });
            }


            // --- GRÁFICO VEÍCULOS ---
            var hfVei = document.getElementById('<%= hfVeiculos.ClientID %>');
            if (hfVei && hfVei.value) {
                var veiculos = JSON.parse(hfVei.value);
                new Chart(document.getElementById('graficoVeiculos'), {
                    type: 'bar',
                    data: {
                        labels: veiculos.labels,
                        datasets: [
                            { label: 'Entradas', data: veiculos.entradas, backgroundColor: '#00bc8c' },
                            { label: 'Saídas', data: veiculos.saidas, backgroundColor: '#e74c3c' }
                        ]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        animation: delayedAnimation,
                        animations: {
                            y: {
                                from: 500, // Barras subindo
                                type: 'number'
                            }
                        },
                        plugins: { legend: { labels: { color: '#fff' } } },
                        scales: {
                            y: { beginAtZero: true, ticks: { color: '#fff' } },
                            x: { ticks: { color: '#fff' } }
                        }
                    }
                });
            }
        });
    </script>
</asp:Content>

<%--Status--%>
<%--/*var statusString = document.getElementById('<%= hfStatus.ClientID %>').value; 
        var statusArray = statusString.replace('[', '').replace(']', '').split(','); 
        var statusData = statusArray.map(Number); 

        new Chart(document.getElementById('graficoStatus'), {
            type: 'doughnut',
            data: {
                labels: ['Ativos', 'Finalizados'],
                datasets: [{
                    label: 'Status dos Veículos',
                    data: statusData,
                    backgroundColor: [
                        '#3b82f6',
                        '#16a34a',
                        '#f59e0b'
                    ],

                    hoverOffset: 0
                }]
            }
        });--%>
        

