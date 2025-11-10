using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Estacionamento
{
    public partial class Home : Page
    {
        string conexao = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=EstacionamentoDB;Data Source=DESKTOP-GLQ18K5";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CarregarVeiculos();
        }

        private void CarregarVeiculos()
        {
            using (SqlConnection con = new SqlConnection(conexao))
            {
                string sql = "SELECT Id, Placa, Modelo, Cor, DataEntrada, HoraEntrada, DataSaida, ValorPago, Status FROM Veiculos";
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvVeiculos.DataSource = dt;
                gvVeiculos.DataBind();
            }
        }
    }
}
