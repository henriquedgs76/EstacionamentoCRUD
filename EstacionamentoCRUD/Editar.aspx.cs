using System;
using System.Data;
using System.Data.SqlClient;
using EstacionamentoCRUD.DAL;

namespace EstacionamentoCRUD
{
    public partial class Editar : System.Web.UI.Page
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
            string sql = "SELECT Placa, Modelo, Cor, DataEntrada, HoraEntrada " +
                "FROM Veiculos " +
                "WHERE Id = @Id";
            var parameters = new[] { new SqlParameter("@Id", id) };
            DataTable dt = DataAccess.ExecuteDataTable(sql, parameters);

            if (dt.Rows.Count > 0)
            {
                PreencherFormulario(dt.Rows[0]);
                txtPlaca.ReadOnly = true;
                btnBuscar.Enabled = false;
            }
            else
            {
                lblMensagem.Text = " Veículo não encontrado.";
                lblMensagem.CssClass = "text-danger";
            }
        }

        private void PreencherFormulario(DataRow dr)
        {
            txtPlaca.Text = dr["Placa"].ToString();
            txtModelo.Text = dr["Modelo"].ToString();
            txtCor.Text = dr["Cor"].ToString();
            txtDataEntrada.Text = Convert.ToDateTime(dr["DataEntrada"]).ToString("yyyy-MM-dd");
            txtHoraEntrada.Text = ((TimeSpan)dr["HoraEntrada"]).ToString(@"hh\:mm");
            lblMensagem.Text = " Veículo encontrado! Você pode editar os dados.";
            lblMensagem.CssClass = "text-success";
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim();

            if (string.IsNullOrEmpty(placa))
            {
                lblMensagem.Text = " Digite a placa do veículo.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            string sql = "SELECT Placa, Modelo, Cor, DataEntrada, HoraEntrada " +
                "FROM Veiculos " +
                "WHERE Placa = @Placa";
            var parameters = new[] { new SqlParameter("@Placa", placa) };
            DataTable dt = DataAccess.ExecuteDataTable(sql, parameters);

            if (dt.Rows.Count > 0)
            {
                PreencherFormulario(dt.Rows[0]);
            }
            else
            {
                lblMensagem.Text = " Veículo não encontrado.";
                lblMensagem.CssClass = "text-danger";
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim();
            string modelo = txtModelo.Text.Trim();
            string cor = txtCor.Text.Trim();
            
            if (string.IsNullOrEmpty(placa) || string.IsNullOrEmpty(modelo))
            {
                lblMensagem.Text = " Preencha todos os campos obrigatórios.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            TimeSpan horaEntrada;
            if (!TimeSpan.TryParse(txtHoraEntrada.Text, out horaEntrada))
            {
                lblMensagem.Text = " Formato de hora inválido. Use HH:mm.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            string sql = @"UPDATE Veiculos 
                           SET Modelo=@Modelo, Cor=@Cor, DataEntrada=@DataEntrada, HoraEntrada=@HoraEntrada
                           WHERE Placa=@Placa";

            var parameters = new[]
            {
                new SqlParameter("@Modelo", modelo),
                new SqlParameter("@Cor", cor),
                new SqlParameter("@DataEntrada", Convert.ToDateTime(txtDataEntrada.Text)),
                new SqlParameter("@HoraEntrada", horaEntrada),
                new SqlParameter("@Placa", placa)
            };

            int rows = DataAccess.ExecuteNonQuery(sql, parameters);
            if (rows > 0)
            {
                lblMensagem.Text = " Alterações salvas com sucesso!";
                lblMensagem.CssClass = "text-success";
                Response.AddHeader("REFRESH", "2;URL=Home.aspx");
            }
            else
            {
                lblMensagem.Text = " Erro ao salvar alterações. Verifique a placa.";
                lblMensagem.CssClass = "text-danger";
            }
        }
    }
}
