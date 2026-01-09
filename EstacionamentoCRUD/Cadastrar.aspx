<%@ Page Title="Cadastrar Veículo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cadastrar.aspx.cs" Inherits="Estacionamento.Cadastrar" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootswatch@5.3.3/dist/darkly/bootstrap.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/tesseract.js@4/dist/tesseract.min.js"></script>

    <div class="col-md-6 mx-auto mt-4">
        <div class="card shadow">
            <div class="card-header bg-success text-white text-center">
                <i class="bi bi-car-front-fill fs-1"></i>
                <h4 class="mt-2">Adicionar Veículo</h4>
            </div>
            <div class="card-body">
                <div class="mb-3">
                    <label class="form-label">Placa</label>
                    <div class="input-group">
                        <asp:TextBox ID="txtPlaca" runat="server" CssClass="form-control text-uppercase"
                            placeholder="AAA0A00" AutoPostBack="true" OnTextChanged="txtPlaca_TextChanged" />
                        <button type="button" class="btn btn-primary" onclick="abrirCamera()">
                            <i class="bi bi-camera mb-2"></i>
                        </button>
                    </div>
                </div>

                <div class="mb-2">
                    <label class="form-label">Telefone (WhatsApp)</label>
                    <div class="input-group">
                        <span class="input-group-text"><i class="fab fa-whatsapp"></i></span>
<asp:TextBox ID="txtTelefone" runat="server" CssClass="form-control"
                            placeholder="(00) 90000-0000" type="tel" />
                    </div>
                </div>

                <div class="mb-3">
                    <label class="form-label">Serviços:</label>
                    <asp:DropDownList ID="ddlServicos" runat="server" CssClass="form-select"></asp:DropDownList>
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

    <div class="col-md-10 mx-auto mt-5">
        <div class="card shadow">
            <div class="card-header bg-primary text-white">
                <h5 class="mb-0"><i class="fas fa-history me-2"></i>Últimos Veículos Cadastrados</h5>
            </div>
            <div class="card-body p-0">
                <asp:GridView ID="gvVeiculos" runat="server" AutoGenerateColumns="False"
                    CssClass="table table-dark table-striped table-hover mb-0" GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="Placa" HeaderText="Placa" ItemStyle-CssClass="fw-bold text-info" />
                        <asp:BoundField DataField="Marca" HeaderText="Marca" />
                        <asp:BoundField DataField="Modelo" HeaderText="Modelo" />
                        <asp:BoundField DataField="Cor" HeaderText="Cor" />
                        <asp:BoundField DataField="Vaga" HeaderText="Vaga" />
                        <asp:BoundField DataField="DataEntrada" HeaderText="Entrada" DataFormatString="{0:dd/MM HH:mm}" />
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="text-center p-4 text-muted">Nenhum veículo registrado hoje.</div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>
    </div>


    <div id="cameraContainer" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.9); z-index: 9999; flex-direction: column; align-items: center; justify-content: center; padding: 20px;">
        <h5 class="text-white mb-3">Aponte para a Placa</h5>
        <video id="video" style="width: 100%; max-width: 500px; border: 2px solid #00bc8c; border-radius: 10px;"></video>
        <canvas id="canvas" style="display: none;"></canvas>

        <div class="mt-4 d-flex gap-2">
            <button type="button" class="btn btn-lg btn-success" onclick="capturarFoto()">
                <i class="fas fa-check-circle me-1"></i>Capturar
            </button>
            <button type="button" class="btn btn-lg btn-danger" onclick="fecharCamera()">
                <i class="fas fa-times me-1"></i>Cancelar
            </button>
        </div>
        <div id="loaderOcr" class="mt-3 text-info" style="display: none;">
            <div class="spinner-border spinner-border-sm" role="status"></div>
            Processando imagem...
        </div>
    </div>

    <script>
        let videoStream;
        let ocrWorker = null;
        let lendoOCR = false;

        const inputArquivo = document.createElement('input');
        inputArquivo.type = 'file';
        inputArquivo.accept = 'image/*';
        inputArquivo.onchange = e => processarArquivoTeste(e);

        async function iniciarOCR() {
            if (ocrWorker) return;

            ocrWorker = await Tesseract.createWorker();
            await ocrWorker.loadLanguage('eng');
            await ocrWorker.initialize('eng');
            await ocrWorker.setParameters({
                tessedit_char_whitelist: 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789'
            });
        }

        async function abrirCamera() {
            const video = document.getElementById('video');
            const container = document.getElementById('cameraContainer');

            // Limpeza de segurança: se já houver um stream ativo, para ele antes de começar
            if (videoStream) {
                videoStream.getTracks().forEach(t => t.stop());
            }

            try {
                // Usamos o padrão mais simples e compatível com Android/Chrome
                const constraints = {
                    video: {
                        facingMode: "environment" // Força a câmera traseira
                    }
                };

                videoStream = await navigator.mediaDevices.getUserMedia(constraints);
                container.style.display = 'flex';
                video.srcObject = videoStream;

                // Pequeno atraso para garantir que o vídeo carregue antes de dar play
                video.onloadedmetadata = () => {
                    video.play();
                };
            } catch (err) {
                console.error("Erro ao abrir câmera: ", err);
                // Se falhar (ex: usuário negou permissão), abre o seletor de arquivos
                inputArquivo.click();
            }
        }

        function fecharCamera() {
            if (videoStream) videoStream.getTracks().forEach(t => t.stop());
            document.getElementById('cameraContainer').style.display = 'none';
        }

        async function processarLeitura(imagemBase64) {
            if (lendoOCR) return false;
            lendoOCR = true;

            const loader = document.getElementById('loaderOcr');
            const txtPlaca = document.getElementById('<%= txtPlaca.ClientID %>');
            loader.style.display = 'block';

            try {
                await iniciarOCR();
                const { data: { text } } = await ocrWorker.recognize(imagemBase64);

                // 1. Limpeza básica (Mantém apenas o que interessa)
                let textoLimpo = text.toUpperCase().replace(/[^A-Z0-9]/g, '');

                // 2. Remove sujeira comum de fotos de internet/placas
                const lixo = ['BRASIL', 'MERCOSUL', 'FREEPIK', 'OMP'];
                lixo.forEach(palavra => { textoLimpo = textoLimpo.replace(palavra, ''); });

                let placa = '';

                // 3. Tenta encontrar os padrões exatos primeiro (Sem trocar nada)
                const mercosul = textoLimpo.match(/[A-Z]{3}[0-9][A-Z][0-9]{2}/);
                const antiga = textoLimpo.match(/[A-Z]{3}[0-9]{4}/);

                if (mercosul) {
                    placa = mercosul[0];
                } else if (antiga) {
                    placa = antiga[0];
                } else {
                    // 4. FALLBACK: Se não achou, procura qualquer bloco de 7
                    // Aqui você pode aplicar trocas se quiser, mas só como última tentativa
                    const fallback = textoLimpo.match(/[A-Z0-9]{7}/);
                    if (fallback) placa = fallback[0];
                }

                if (placa) {
                    txtPlaca.value = placa;
                    // Notifica o usuário com uma vibração (se o celular suportar)
                    if (navigator.vibrate) navigator.vibrate(100);

                    __doPostBack('<%= txtPlaca.UniqueID %>', '');
                    return true;
                }

                alert('Não foi possível validar a placa. Tente enquadrar melhor o texto.');
                return false;

            } catch (err) {
                console.error(err);
                return false;
            } finally {
                loader.style.display = 'none';
                lendoOCR = false;
            }
        }

        async function capturarFoto() {
            const video = document.getElementById('video');
            const canvas = document.getElementById('canvas');

            canvas.width = video.videoWidth;
            canvas.height = video.videoHeight;
            canvas.getContext('2d').drawImage(video, 0, 0);

            const sucesso = await processarLeitura(canvas.toDataURL('image/png'));
            if (sucesso) fecharCamera();
        }

        function processarArquivoTeste(event) {
            const reader = new FileReader();
            reader.onload = async e => await processarLeitura(e.target.result);
            reader.readAsDataURL(event.target.files[0]);
        }

        function limparFormulario() {
            document.getElementById('<%= txtPlaca.ClientID %>').value = '';
            document.getElementById('<%= txtModelo.ClientID %>').value = '';
            document.getElementById('<%= txtCor.ClientID %>').value = '';
        }
    </script>

</asp:Content>
