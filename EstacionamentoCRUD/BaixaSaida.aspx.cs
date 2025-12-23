using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using EstacionamentoCRUD.DAL;

namespace EstacionamentoCRUD
{
    public partial class BaixaSaida : System.Web.UI.Page
    {
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
            string sql = "SELECT DataEntrada, HoraEntrada " +
                "FROM Veiculos " +
                "WHERE Placa = @Placa " +
                "AND Status = 'Estacionado'";
            var parameters = new[] { new SqlParameter("@Placa", placa) };
            DataTable dt = DataAccess.ExecuteDataTable(sql, parameters);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                DateTime dataEntrada = Convert.ToDateTime(dr["DataEntrada"]);
                TimeSpan horaEntrada = (TimeSpan)dr["HoraEntrada"];

                DateTime entradaCompleta = dataEntrada.Date + horaEntrada;
                DateTime saida = DateTime.Now;

                if (saida < entradaCompleta)
                {
                    lblMensagem.Text = " Data/hora de entrada é maior que a de saída. Verifique o registro.";
                    lblMensagem.CssClass = "text-warning";
                    txtValorPago.Text = "0,00";
                    return;
                }

                TimeSpan tempoPermanencia = saida - entradaCompleta;

                if (tempoPermanencia.TotalMinutes <= 15)
                {
                    txtValorPago.Text = "0,00";
                    lblMensagem.Text = " Permanência até 15 minutos — Não cobrar tarifa";
                    lblMensagem.CssClass = "text-success";
                    return;
                }
                int horas = (int)Math.Ceiling(tempoPermanencia.TotalHours);
                if (horas < 1) horas = 1;

                decimal valor;
                if (horas <= 2)
                {
                    valor = 18.00m;
                }
                else
                {
                    valor = 18.00m + ((horas - 2) * 5.00m);
                }

                txtValorPago.Text = valor.ToString("F2", CultureInfo.GetCultureInfo("pt-BR"));
                lblMensagem.Text = $" Permanência: {horas:F0} hora(s)";
                lblMensagem.CssClass = "text-info";
            }
            else
            {
                lblMensagem.Text = " Veículo não encontrado ou já deu baixa.";
                lblMensagem.CssClass = "text-danger";
                txtValorPago.Text = string.Empty;
            }
        }

        protected void btnDarBaixa_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim();

            if (string.IsNullOrEmpty(placa))
            {
                lblMensagem.Text = " Digite a placa do veículo.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            decimal valorPago;
            if (!decimal.TryParse(txtValorPago.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out valorPago))
            {
                lblMensagem.Text = " Valor inválido. Clique em Calcular primeiro.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            try
            {
                // 1. Antes de tudo, descobrir qual vaga este veículo ocupa
                object vagaIdObj = DataAccess.ExecuteScalar("SELECT VagaId FROM Veiculos " +
                    "WHERE Placa = @Placa " +
                    "AND Status = 'Estacionado'", new[] { new SqlParameter("@Placa", placa) });

                // 2. Atualiza o status do veículo para 'Finalizado'
                string sqlUpdateVeiculo = @"UPDATE Veiculos 
                                           SET DataSaida = GETDATE(), ValorPago = @ValorPago, 
                                           Status = 'Finalizado'
                                           WHERE Placa = @Placa AND Status = 'Estacionado'";

                var veiculoParams = new[]
                {
                    new SqlParameter("@Placa", placa),
                    new SqlParameter("@ValorPago", valorPago)
                };

                int rows = DataAccess.ExecuteNonQuery(sqlUpdateVeiculo, veiculoParams);

                if (rows > 0)
                {
                    // 3. Se o veículo foi baixado com sucesso E ele tinha uma vaga, libera a vaga
                    if (vagaIdObj != null && vagaIdObj != DBNull.Value)
                    {
                        int vagaId = Convert.ToInt32(vagaIdObj);
                        string sqlUpdateVaga = "UPDATE Vagas " +
                            "SET Status = 'Livre' " +
                            "WHERE Id = @VagaId";
                        DataAccess.ExecuteNonQuery(sqlUpdateVaga, new[] { new SqlParameter("@VagaId", vagaId) });
                    }

                    lblMensagem.Text = " Saída registrada com sucesso! A vaga agora está livre.";
                    lblMensagem.CssClass = "text-success";
                    Response.AddHeader("REFRESH", "2;URL=Home.aspx");
                }
                else
                {
                    lblMensagem.Text = " Erro: veículo não encontrado ou já finalizado.";
                    lblMensagem.CssClass = "text-danger";
                }
            }
            catch (Exception ex)
            {
                lblMensagem.Text = " Ocorreu um erro inesperado ao dar baixa.";
                lblMensagem.CssClass = "text-danger";
            }
        }
    }
}
