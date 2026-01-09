using EstacionamentoCRUD.BLL;
using Microsoft.Ajax.Utilities;
using System;
using System.Data;
using System.Linq;

namespace EstacionamentoCRUD
{
    public partial class ImprimirTicket : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string ticketId = Request.QueryString["ticket"];
                if (!string.IsNullOrEmpty(ticketId))
                {
                    CarregarInformacoesTicket(ticketId);

                    //chama a impressão quando a página carregada
                    ClientScript.RegisterStartupScript(this.GetType(), 
                        "Print", "window.onload = function() " +
                        "{ window.print(); };", true);
                }
                else
                {
                    litPlaca.Text = "TICKET INVÁLIDO";
                }
            }
        }

        private void CarregarInformacoesTicket(string ticketId) // carrego informações do ticket
        {
            try
            {
                var veiculoBLL = new VeiculoBLL();
                var dt = veiculoBLL.BuscarVeiculoPorTicket(ticketId);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    string ticketIdValue = row["TicketId"].ToString();
                    litTicketId.Text = ticketIdValue;
                    litPlaca.Text = row["Placa"].ToString().ToUpper();
                    litTelefone.Text = row["Telefone"] != DBNull.Value ? row["Telefone"].ToString() : "N/A";
                    litMarca.Text = row["Marca"].ToString();
                    litModelo.Text = row["Modelo"].ToString();
                    litServico.Text = row["NomeServico"].ToString().ToUpper();
                    DateTime dataEntrada = Convert.ToDateTime(row["DataEntrada"]);
                    TimeSpan horaEntrada = (TimeSpan)row["HoraEntrada"];

                    litDataEntrada.Text = dataEntrada.ToString("dd/MM/yyyy");
                    litHoraEntrada.Text = horaEntrada.ToString(@"hh\:mm\:ss");

                    string telefone = row["Telefone"].ToString();
                    if (!string.IsNullOrEmpty(telefone) && telefone != "N/A")
                    {
                        // Remove espaços, parênteses e traços
                        string telLimpo = new string(telefone.Where(char.IsDigit).ToArray());

                        // Adiciona o código do país se não tiver
                        if (!telLimpo.StartsWith("55")) telLimpo = "55" + telLimpo;

                        // Monta a mensagem profissional para o Baronesa
                        string mensagem = $"*LAVA RÁPIDO BARONESA*%0A%0AOlá! Confirmamos a entrada do seu *{row["Modelo"]}* (Placa: {row["Placa"]}).%0AServiço: {row["NomeServico"]}.%0AAvisaremos quando estiver pronto! ✨";

                        // Atribui ao HyperLink que está no ASPX
                        lnkWhatsApp.NavigateUrl = $"https://wa.me/{telLimpo}?text={mensagem}";
                        lnkWhatsApp.Visible = true;
                    }
                    else
                    {
                        lnkWhatsApp.Visible = false;
                    }


                    //aqui cria o codigo de barras 
                    string script = $@"
                        document.addEventListener('DOMContentLoaded', function () {{
                            try {{
                                JsBarcode('#barcode', '{ticketIdValue}', {{
                                    format: 'CODE128',
                                    lineColor: '#000',
                                    width: 2,
                                    height: 50,
                                    displayValue: true
                                }});
                            }} catch (e) {{
                                console.error('Falha ao gerar código de barras:', e);
                            }}
                        }});";
                    ClientScript.RegisterStartupScript(this.GetType(), 
                        "BarcodeScript", script, true);
                }
                else
                {
                    litTicketId.Text = "N/A";
                    litPlaca.Text = "NÃO ENCONTRADO";
                    litDataEntrada.Text = "N/A";
                    litHoraEntrada.Text = "N/A";
                    litMarca.Text = "N/A";
                    litModelo.Text = "N/A";
                    litServico.Text = "N/A";
                }


            }
            catch (Exception ex)
            {
                litPlaca.Text = "Erro: " + ex.Message;

            }
        }
    }
}
