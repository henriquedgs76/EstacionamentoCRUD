using EstacionamentoCRUD.BLL;
using EstacionamentoCRUD.DAL;
using Microsoft.Ajax.Utilities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls.WebParts;

namespace EstacionamentoCRUD
{
    public partial class BaixaSaida : System.Web.UI.Page
    {
        // quando a página carrega, ela vê se veio um 'id' na URL 
        // se veio, já chama a função pra carregar os dados desse carro na tela.
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    int veiculoId;
                    if (int.TryParse(Request.QueryString["id"], out veiculoId))
                    {
                        CarregarVeiculo(veiculoId);
                    }

                }
            }
        }

        // busca no banco a placa do carro usando o ID que a gente recebeu.
        // se achar, coloca a placa no campo de texto pra facilitar pro usuário.
        private void CarregarVeiculo(int id)
        {
            string sql = "SELECT Placa " +
                "FROM Veiculos " +
                "WHERE Id = @Id " +
                "AND Status = 'Estacionado'";
            var parameters = new[] { new SqlParameter("@Id", id) };
            object placaObj = DataAccess.ExecuteScalar(sql, parameters);

            if (placaObj != null)
            {
                string placa = placaObj.ToString();
                txtPlaca.Text = placa;
                txtPlaca.ReadOnly = true;
                txtValorPago.Text = string.Empty;
                lblMensagem.Text = string.Empty;
            }
            else
            {
                lblMensagem.Text = " Veículo não encontrado ou já deu baixa.";
                lblMensagem.CssClass = "text-danger";
            }
        }

        protected void btnCalcular_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim();
            if (string.IsNullOrEmpty(placa))
            {
                lblMensagem.Text = " Digite a placa do veículo.";
                lblMensagem.CssClass = "text-warning";
                return;
            }
            CalcularValor(placa);
        }

        private void CalcularValor(string placa)
        {
            string sql = @"SELECT V.DataEntrada, V.HoraEntrada, S.NomeServico, S.PrecoPadrao 
                   FROM Veiculos V 
                   INNER JOIN Servicos S ON V.ServicoId = S.Id 
                   WHERE V.Placa = @Placa AND V.Status = 'Estacionado'";

            var parameters = new[] { new SqlParameter("@Placa", placa) };
            DataTable dt = DataAccess.ExecuteDataTable(sql, parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                string nomeServico = dr["NomeServico"].ToString();
                decimal precoBase = Convert.ToDecimal(dr["PrecoPadrao"]);
                DateTime dataEntrada = Convert.ToDateTime(dr["DataEntrada"]);
                TimeSpan horaEntrada = (TimeSpan)dr["HoraEntrada"];

                DateTime entradaCompleta = dataEntrada.Date + horaEntrada;
                DateTime saida = DateTime.Now;

                // previne que o campo de valor seja somente leitura por padrão
                txtValorPago.ReadOnly = true;

                if (saida < entradaCompleta)
                {
                    lblMensagem.Text = "Data de entrada maior que a de saída!";
                    lblMensagem.CssClass = "text-warning";
                    txtValorPago.Text = string.Empty; // Limpa o campo
                    return;
                }

                
                // se o preço base for 0 E não for o serviço de Estacionamento (que tem cálculo próprio)
                if (precoBase == 0 && !nomeServico.Equals("Estacionamento", StringComparison.OrdinalIgnoreCase))
                {
                    lblMensagem.Text = $"Serviço '{nomeServico}': Por favor, insira o valor manualmente.";
                    lblMensagem.CssClass = "text-warning";
                    txtValorPago.Text = string.Empty; // apaga qualquer valor calculado anteriormente
                    txtValorPago.ReadOnly = false; // deixa edição manual
                    txtValorPago.Focus(); // cria o foco para o operador digitar
                    return; // aqui saimos do método, aguardando entrada manual
                }
               


                decimal valorFinal = 0;

                //a logica para ESTACIONAMENTO (Cálculo por hora)
                if (nomeServico.Equals("Estacionamento", StringComparison.OrdinalIgnoreCase))
                {
                    TimeSpan tempoPermanencia = saida - entradaCompleta;
                    int horas = (int)Math.Ceiling(tempoPermanencia.TotalHours);
                    if (horas < 1) horas = 1;

                    if (tempoPermanencia.TotalMinutes <= 15)
                    {
                        valorFinal = 0;
                        lblMensagem.Text = "Permanência até 15 min - Isento";
                    }
                    else if (horas <= 2)
                    {
                        valorFinal = precoBase; // usa o precoBase do estacionamento
                    }
                    else
                    {
                        valorFinal = precoBase + ((horas - 2) * 5.00m); // preço base + adicional por hora
                    }
                    lblMensagem.Text += $" | Serviço: {nomeServico} | Tempo: {horas}h";
                }
                // Lógica para QUALQUER OUTRO SERVIÇO (Valor Fixo do Banco, PrecoPadrao > 0)
                else
                {
                    valorFinal = precoBase; // Usa o preço padrão fixo do serviço
                    lblMensagem.Text = $"Serviço: {nomeServico}";
                }

                // Preenche o campo de valor, já que não retornou antes (preço fixo ou estacionamento)
                txtValorPago.Text = valorFinal.ToString("N2", CultureInfo.GetCultureInfo("pt-BR"));
                lblMensagem.CssClass = "text-info";
            }
            else
            {
                lblMensagem.Text = "Veículo não encontrado ou já deu baixa.";
                lblMensagem.CssClass = "text-danger";
                txtValorPago.Text = string.Empty; // Limpa o campo
                txtValorPago.ReadOnly = true; // Mantém somente leitura
            }
        }

        // Uma ajudinha pro usuário: ele digita o número do ticket, a gente acha a placa do carro pra ele
        // e já chama a função de calcular o valor automaticamente.
        protected void btnBuscarTicket_Click(object sender, EventArgs e)
        {
            string ticketId = txtTicketId.Text.Trim();
            if (string.IsNullOrEmpty(ticketId))
            {
                lblMensagem.Text = "Por favor, insira um número de ticket.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            try
            {
                var veiculoBLL = new EstacionamentoCRUD.BLL.VeiculoBLL();
                string placa = veiculoBLL.BuscarPlacaPorTicket(ticketId);

                if (!string.IsNullOrEmpty(placa))
                {
                    txtPlaca.Text = placa;
                    CalcularValor(placa); //calcula automatico com os dados do sistema
                                         
                    btnDarBaixa.Focus(); //foco no botão de Dar Baixa para agilizar

                    lblMensagem.Text = "Ticket identificado!";
                    lblMensagem.CssClass = "text-success";

                }
                else
                {
                    txtPlaca.Text = string.Empty;
                    txtValorPago.Text = string.Empty;
                    lblMensagem.Text = "Ticket não encontrado para um veículo estacionado.";
                    lblMensagem.CssClass = "text-danger";
                }
            }
            catch (Exception ex)
            {
                lblMensagem.Text = "Ocorreu um erro ao buscar o ticket." + ex.Message;
                lblMensagem.CssClass = "text-danger";

            }
        }

     
        protected void btnDarBaixa_Click(object sender, EventArgs e)
        {
            //confere se o usuário ainda tá logado.
            if (Session["UsuarioId"] == null)
            {
                lblMensagem.Text = "Sua sessão expirou. Faça login novamente.";
                lblMensagem.CssClass = "text-danger";
                return;
            }

            string placa = txtPlaca.Text.Trim();

            //se a placa foi preenchida.
            if (string.IsNullOrEmpty(placa))
            {
                lblMensagem.Text = " Digite a placa do veículo.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            // se o valor do pagamento é um número válido.
            decimal valorPago;
            if (!decimal.TryParse(txtValorPago.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out valorPago))
            {
                lblMensagem.Text = " Valor inválido. Clique em Calcular primeiro.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            try
            {
                var veiculoBLL = new EstacionamentoCRUD.BLL.VeiculoBLL();

                //pega o ID do veículo ANTES de dar baixa, pra gente poder usar na auditoria.
                string sqlId = "SELECT Id FROM Veiculos WHERE Placa = @Placa AND Status = 'Estacionado'";
                object idObj = DataAccess.ExecuteScalar(sqlId, new[] { new SqlParameter("@Placa", placa) });

                //chama o `RegistrarSaida` lá da pagina BLL, que faz o trabalho de verdade (mudar status, liberar vaga, etc).
                string formaPagamento = ddlFormaPagamento.SelectedValue;
                veiculoBLL.RegistrarSaida(placa, valorPago, formaPagamento);

                // salva na auditoria que o usuário que deu baixa no carro, o valor e a forma de pagamento.
                if (idObj != null)
                {
                    int veiculoId = Convert.ToInt32(idObj);
                    int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
                    string usuarioLogado = Session["UsuarioLogado"]?.ToString() ?? "Desconhecido";

                    string mensagemAuditoria = $"Baixa efetuada por: {usuarioLogado} | Valor: R$ {valorPago:F2} | Pagamento: {formaPagamento}";
                    DataAccess.RegistrarAuditoria(usuarioId, mensagemAuditoria, "Veiculos", veiculoId);
                }

                // cria uma mensagem de sucesso e manda o usuário de volta pra página inicial depois de 2 segundos.
                lblMensagem.Text = " Saída registrada com sucesso! A vaga agora está livre.";
                lblMensagem.CssClass = "text-success text-center d-block fw-bold";

                Response.AddHeader("REFRESH", "2;URL=Home.aspx");
            }
            catch (Exception ex)
            {
                //se algo der errado, mostra a mensagem de erro na tela.
                lblMensagem.Text = "Erro ao processar saída: " + ex.Message;
                lblMensagem.CssClass = "text-danger";
            }
        }
    }
}