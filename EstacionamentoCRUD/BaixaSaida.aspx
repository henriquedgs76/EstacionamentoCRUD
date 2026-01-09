<%@ Page Title="Baixa de Saída" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BaixaSaida.aspx.cs" Inherits="EstacionamentoCRUD.BaixaSaida" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootswatch@5.3.3/dist/darkly/bootstrap.min.css" rel="stylesheet" />
    <div class="col-md-6 mx-auto">
        <div class="card shadow-lg p-4">
            <h2 class="text-center mb-4 text-primary">Saída de Veículo</h2>

            <div class="row">
                <div class="col-md-9 mb-3">
                    <label class="form-label">Buscar por Ticket:</label>
                    <asp:TextBox ID="txtTicketId" runat="server" CssClass="form-control" placeholder="Digite o número do ticket"></asp:TextBox>
                </div>
                <div class="col-md-3 d-flex align-items-end mb-3">
                    <asp:Button ID="btnBuscarTicket" runat="server" CssClass="btn btn-primary w-100" Text="Buscar" OnClick="btnBuscarTicket_Click" />
                </div>
            </div>

            <div class="text-center my-2">
                <span class="text-muted">OU</span>
            </div>

            <div class="row">
                <div class="col-md-9 mb-3">
                    <label class="form-label">Placa do Veículo:</label>
                    <asp:TextBox ID="txtPlaca" runat="server" CssClass="form-control" placeholder="Digite a placa do veículo"></asp:TextBox>
                </div>
                <div class="col-md-3 d-flex align-items-end mb-3">
                    <asp:Button ID="btnCalcular" runat="server" CssClass="btn btn-info w-100" Text="Cobrar R$" OnClick="btnCalcular_Click" />
                </div>
            </div>

            <div class="mb-3">
                <label class="form-label">Valor a Pagar (R$):</label>
                <asp:TextBox ID="txtValorPago" runat="server" CssClass="form-control fw-bold" ReadOnly="true"></asp:TextBox>
            </div>

            <div class="mb-3">
                <label class="form-label"><strong>Forma de Pagamento:</strong></label>
                <asp:DropDownList ID="ddlFormaPagamento" runat="server" CssClass="form-select" onchange="calcularTroco()">
                    <asp:ListItem Text="Dinheiro" Value="Dinheiro"></asp:ListItem>
                    <asp:ListItem Text="Cartão de Crédito" Value="Credito"></asp:ListItem>
                    <asp:ListItem Text="Cartão de Débito" Value="Debito"></asp:ListItem>
                    <asp:ListItem Text="Pix" Value="Pix"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="mb-3">
                <label class="form-label"><strong>Valor Recebido:</strong></label>
                <asp:TextBox ID="txtValorRecebido" runat="server" CssClass="form-control" placeholder="0,00" onkeyup="calcularTroco()"></asp:TextBox>
            </div>

            <div class="mb-4 text-center border-top pt-3">
                <span class="text-muted d-block">Troco a Devolver:</span>
                <span id="lblTrocoExibicao" class="h3 fw-bold text-success">R$ 0,00</span>
            </div>

            <div class="mb-3 d-grid">
                <asp:Button ID="btnDarBaixa" runat="server" CssClass="btn btn-success" Text=" Confirmar Saída" OnClick="btnDarBaixa_Click" />
            </div>

            <asp:Label ID="lblMensagem" runat="server" CssClass="text-center fw-bold mt-3 d-block"></asp:Label>

            <div class="text-center mt-3">
                <a href="Home.aspx" class="btn btn-secondary">Voltar para Home</a>
            </div>
        </div>
    </div>

    <div class="row mt-3">
        <div class="col-12">
            <div id="reader" class="mx-auto" style="border: none;"></div>
        </div>
    </div>
    <button type="button" id="btnIniciarCamera" class="btn btn-primary w-100 mb-3">
        <i class="bi bi-camera me-2"></i>Ler Ticket pela Câmera
   
    </button>

    <script src="https://unpkg.com/html5-qrcode"></script>
    <script type="text/javascript">
        // Variável global para o scanner da câmera
        var html5QrcodeScanner;

        // Função para calcular o troco e gerenciar o formulário de pagamento
        function calcularTroco() {
            var ddlForma = document.getElementById('<%= ddlFormaPagamento.ClientID %>');
            var txtTotal = document.getElementById('<%= txtValorPago.ClientID %>');
            var txtRecebido = document.getElementById('<%= txtValorRecebido.ClientID %>');
            var displayTroco = document.getElementById('lblTrocoExibicao');

            var valorPago = parseFloat(txtTotal.value.replace('R$', '').replace(/\./g, '').replace(',', '.').trim()) || 0;

            if (ddlForma.value !== "Dinheiro") {
                txtRecebido.value = valorPago > 0 ? valorPago.toFixed(2).replace('.', ',') : "0,00";
                txtRecebido.readOnly = true;
                displayTroco.innerHTML = "R$ 0,00";
                displayTroco.className = "h3 fw-bold text-muted";
                return;
            } else {
                txtRecebido.readOnly = false;
            }

            var valorRecebido = parseFloat(txtRecebido.value.replace(/\./g, '').replace(',', '.').trim()) || 0;

            if (valorRecebido > 0) {
                var troco = valorRecebido - valorPago;
                if (troco >= 0) {
                    displayTroco.innerHTML = "R$ " + troco.toFixed(2).replace('.', ',');
                    displayTroco.className = "h3 fw-bold text-success";
                } else {
                    displayTroco.innerHTML = "Valor insuficiente...";
                    displayTroco.className = "h3 fw-bold text-danger";
                }
            } else {
                displayTroco.innerHTML = "R$ 0,00";
                displayTroco.className = "h3 fw-bold text-muted";
            }
        }

        // Funções para o scanner de QR Code (câmera)
        function onScanSuccess(decodedText, decodedResult) {
            if (html5QrcodeScanner && html5QrcodeScanner.getState() === 2) { // 2: SCANNING
                html5QrcodeScanner.clear().then(_ => {
                    console.log("QR Code scanner stopped.");
                }).catch(err => {
                    console.error("Failed to clear scanner: ", err);
                });
            }
            document.getElementById('<%= txtTicketId.ClientID %>').value = decodedText;
            document.getElementById('<%= btnBuscarTicket.ClientID %>').click();
        }

        function onScanFailure(error) {
            // Apenas ignora falhas de leitura (ex: não achou QR code na imagem)
            // console.warn(`Code scan error = ${error}`);
        }

        // Event listener principal que roda quando o conteúdo da página é carregado
        document.addEventListener("DOMContentLoaded", function () {
            
            // --- LÓGICA DO PAGAMENTO ---
            var ddlPagamento = document.getElementById('<%= ddlFormaPagamento.ClientID %>');
            var txtRecebido = document.getElementById('<%= txtValorRecebido.ClientID %>');
            if(ddlPagamento) ddlPagamento.addEventListener('change', calcularTroco);
            if(txtRecebido) txtRecebido.addEventListener('keyup', calcularTroco);

            // --- LÓGICA DO LEITOR DE CÓDIGO DE BARRAS (FÍSICO OU TECLADO) ---
            var ticketInput = document.getElementById('<%= txtTicketId.ClientID %>');
            var btnBuscar = document.getElementById('<%= btnBuscarTicket.ClientID %>');
            
            if (ticketInput && btnBuscar) {
                ticketInput.addEventListener('keypress', function (e) {
                    if (e.key === 'Enter' || e.keyCode === 13) {
                        e.preventDefault(); // Impede o envio do formulário
                        btnBuscar.click();  // Clica no botão "Buscar"
                    }
                });
            }

            // --- LÓGICA PARA INICIAR O SCANNER PELA CÂMERA ---
            var btnCamera = document.getElementById('btnIniciarCamera');
            if (btnCamera) {
                btnCamera.addEventListener('click', function () {
                    // Inicializa o scanner se não estiver ativo
                    if (!html5QrcodeScanner || html5QrcodeScanner.getState() !== 2) {
                        html5QrcodeScanner = new Html5QrcodeScanner(
                            "reader", 
                            { 
                                fps: 10, 
                                qrbox: { width: 250, height: 150 },
                                supportedScanTypes: [Html5QrcodeScanType.SCAN_TYPE_CAMERA]
                            },
                            false // verbose
                        );
                        html5QrcodeScanner.render(onScanSuccess, onScanFailure);
                    }
                });
            }
        });
    </script>
</asp:Content>
