using System;
using System.Data.SqlClient;
using EstacionamentoCRUD.DAL;

namespace EstacionamentoCRUD
{
    public partial class Excluir : System.Web.UI.Page
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
                        CarregarPlaca(veiculoId);
                    }
                }
            }
        }

        private void CarregarPlaca(int id)
        {
            string sql = "SELECT Placa FROM Veiculos WHERE Id = @Id";
            var parameters = new[] { new SqlParameter("@Id", id) };
            object placa = DataAccess.ExecuteScalar(sql, parameters);

            if (placa != null)
            {
                txtPlaca.Text = placa.ToString();
                txtPlaca.ReadOnly = true;
                btnBuscar.Enabled = false;
                lblMensagem.Text = "⚠️ Confirme a exclusão do veículo com a placa acima.";
                lblMensagem.CssClass = "text-warning";
            }
            else
            {
                lblMensagem.Text = "❌ Veículo não encontrado.";
                lblMensagem.CssClass = "text-danger";
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim();

            if (string.IsNullOrEmpty(placa))
            {
                lblMensagem.Text = "⚠️ Digite a placa.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            string sql = "SELECT COUNT(*) FROM Veiculos WHERE Placa = @Placa";
            var parameters = new[] { new SqlParameter("@Placa", placa) };
            int count = Convert.ToInt32(DataAccess.ExecuteScalar(sql, parameters));

            if (count > 0)
            {
                lblMensagem.Text = "⚠️ Veículo encontrado. Clique em 'Excluir' para confirmar.";
                lblMensagem.CssClass = "text-danger";
            }
            else
            {
                lblMensagem.Text = "❌ Veículo não encontrado.";
                lblMensagem.CssClass = "text-danger";
            }
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim();
            if (string.IsNullOrEmpty(placa))
            {
                lblMensagem.Text = "⚠️ Digite a placa.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            string sql = "DELETE FROM Veiculos WHERE Placa = @Placa";
            var parameters = new[] { new SqlParameter("@Placa", placa) };
            int rows = DataAccess.ExecuteNonQuery(sql, parameters);

            if (rows > 0)
            {
                lblMensagem.Text = "✅ Veículo excluído com sucesso!";
                lblMensagem.CssClass = "text-success";
                Response.AddHeader("REFRESH", "2;URL=Home.aspx");
            }
            else
            {
                lblMensagem.Text = "❌ Erro: veículo não encontrado ou já excluído.";
                lblMensagem.CssClass = "text-danger";
            }
        }
    }
}
