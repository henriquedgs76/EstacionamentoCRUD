using System;
using System.Data.SqlClient;
using System.Globalization;

namespace EstacionamentoCRUD
{
    public partial class BaixaSaida : System.Web.UI.Page
    {
        string connectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=EstacionamentoDB;Data Source=DESKTOP-GLQ18K5";

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnCalcular_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim();

            if (string.IsNullOrEmpty(placa))
            {
                lblMensagem.Text = "⚠️ Digite a placa do veículo.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = "SELECT DataEntrada, HoraEntrada FROM Veiculos WHERE Placa = @Placa AND Status = 'Estacionado'";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Placa", placa);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    DateTime dataEntrada = Convert.ToDateTime(dr["DataEntrada"]);
                    string horaEntradaStr = dr["HoraEntrada"].ToString();
                    TimeSpan horaEntrada;

                    if (!TimeSpan.TryParse(horaEntradaStr, out horaEntrada))
                    {
                        DateTime temp;
                        if (DateTime.TryParse(horaEntradaStr, out temp))
                            horaEntrada = temp.TimeOfDay;
                        else
                            horaEntrada = new TimeSpan(0, 0, 0);
                    }

                    DateTime entradaCompleta = dataEntrada.Date + horaEntrada;
                    dr.Close();

                    DateTime saida = DateTime.Now;

                    if (saida < entradaCompleta)
                    {
                        lblMensagem.Text = "⚠️ Data/hora de entrada é maior que a de saída. Verifique o registro.";
                        lblMensagem.CssClass = "text-warning";
                        txtValorPago.Text = "0,00";
                        return;
                    }

                    TimeSpan tempoPermanencia = saida - entradaCompleta;

                    double horas = Math.Ceiling(tempoPermanencia.TotalHours);
                    if (horas < 1) horas = 1;

                    double valor = horas * 5.0;

                    txtValorPago.Text = valor.ToString("F2", CultureInfo.GetCultureInfo("pt-BR"));
                    lblMensagem.Text = $"🕒 Permanência: {horas} hora(s)";
                    lblMensagem.CssClass = "text-info";
                }
                else
                {
                    lblMensagem.Text = "❌ Veículo não encontrado ou já deu baixa.";
                    lblMensagem.CssClass = "text-danger";
                }
            }
        }

        protected void btnDarBaixa_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim();

            if (string.IsNullOrEmpty(placa))
            {
                lblMensagem.Text = "⚠️ Digite a placa do veículo.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            decimal valorPago;
            if (!decimal.TryParse(txtValorPago.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out valorPago))
            {
                lblMensagem.Text = "⚠️ Valor inválido. Verifique o campo Valor Pago.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = @"UPDATE Veiculos 
                               SET DataSaida = GETDATE(), ValorPago = @ValorPago, Status = 'Finalizado'
                               WHERE Placa = @Placa AND Status = 'Estacionado'";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Placa", placa);
                cmd.Parameters.AddWithValue("@ValorPago", valorPago);

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    lblMensagem.Text = "✅ Saída registrada com sucesso!";
                    lblMensagem.CssClass = "text-success";
                    Response.AddHeader("REFRESH", "1;URL=Home.aspx");
                }
                else
                {
                    lblMensagem.Text = "❌ Erro: veículo não encontrado ou já finalizado.";
                    lblMensagem.CssClass = "text-danger";
                }
            }
        }
    }
}
