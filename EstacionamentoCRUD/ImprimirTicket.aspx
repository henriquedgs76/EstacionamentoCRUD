<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImprimirTicket.aspx.cs" Inherits="EstacionamentoCRUD.ImprimirTicket" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Ticket de Estacionamento</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body {
            background-color: #f8f9fa;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
        }

        .ticket {
            width: 350px;
            padding: 20px;
            border: 2px dashed #333;
            background-color: #FFFFCC; /* Fundo amarelo do ticke */
            font-family: 'Courier New', Courier, monospace;
        }

        .ticket-header {
            text-align: center;
            margin-bottom: 20px;
        }

            .ticket-header h4 {
                margin: 0;
                font-weight: bold;
            }

        .ticket-info p {
            margin: 5px 0;
            font-size: 16px;
        }

        .ticket-info strong {
            display: inline-block;
            width: 120px;
        }

        .ticket-footer {
            text-align: center;
            margin-top: 20px;
            font-size: 12px;
        }

        @media print {
            body {
                background-color: #fff;
            }
            /* Adicione isso logo abaixo do fechamento do @media print { ... } */
            .btn-whatsapp {
                background-color: #25d366;
                color: white !important;
                font-weight: bold;
                border: none;
                margin-bottom: 10px;
                width: 100%;
                display: block;
            }

                .btn-whatsapp:hover {
                    background-color: #128c7e;
                }

            .no-print {
                display: none;
            }

            .ticket {
                border: none;
                width: 100%;
                box-shadow: none;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div class="ticket">
                <div class="ticket-header">
                    <h4>LAVA RÁPIDO BARONESA</h4>
                    <p>Comprovante de Entrada</p>
                    <br />
                </div>
                <hr />
                <div class="ticket-info">
                    <p>
                        <strong>Ticket:</strong>
                        <asp:Literal ID="litTicketId" runat="server"></asp:Literal>
                    </p>

                    <p>
                        <strong>Telefone:</strong>
                        <asp:Literal ID="litTelefone" runat="server"></asp:Literal>
                    </p>

                    <p>
                        <strong>Placa:</strong>
                        <asp:Literal ID="litPlaca" runat="server"></asp:Literal>
                    </p>
                    <p>
                        <strong>Marca:</strong>
                        <asp:Literal ID="litMarca" runat="server"></asp:Literal>
                    </p>
                    <p>
                        <strong>Modelo:</strong>
                        <asp:Literal ID="litModelo" runat="server"></asp:Literal>
                    </p>
                    <p>
                        <strong>Serviço:</strong>
                        <asp:Literal ID="litServico" runat="server"></asp:Literal>
                    </p>
                    <p>
                        <strong>Data:</strong>
                        <asp:Literal ID="litDataEntrada" runat="server"></asp:Literal>
                    </p>
                    <p>
                        <strong>Hora:</strong>
                        <asp:Literal ID="litHoraEntrada" runat="server"></asp:Literal>
                    </p>
                </div>
                <hr />
                <div class="text-center my-3">
                    <svg id="barcode"></svg>
                </div>
                <div class="ticket-footer">
                    <p>Apresente este ticket na saída para pagamento.</p>
                    <p>Obrigado e volte sempre!</p>
                    <p>CNPJ: 34.34.344.769/0001-49</p>
                    <br />
                </div>
            </div>
            <div class="text-center mt-3 no-print">
                <asp:HyperLink ID="lnkWhatsApp" runat="server" CssClass="btn btn-lg w-100 mb-3"
                    Target="_blank" Style="background-color: #25D366; color: white; font-weight: bold; border: none;">
    <i class="bi bi-whatsapp"></i> ENVIAR AVISO WHATSAPP
</asp:HyperLink>

                <div class="d-flex justify-content-center gap-2">
                    <button type="button" onclick="window.print();" class="btn btn-primary">
                        <i class="bi bi-printer"></i>Imprimir Ticket
                    </button>

                    <a href="Home.aspx" class="btn btn-secondary">
                        <i class="bi bi-house-door"></i>Voltar para Home
                    </a>
                </div>
            </div>
        </div>
    </form>
    <script src="https://cdn.jsdelivr.net/npm/jsbarcode@3.11.5/dist/JsBarcode.all.min.js"></script>
</body>
</html>
