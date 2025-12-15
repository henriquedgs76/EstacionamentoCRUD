using System;
using System.Data;
using System.Web.UI;
using EstacionamentoCRUD.DAL;

namespace Estacionamento
{
    public partial class Home : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarVeiculos();
            }
        }

        private void CarregarVeiculos()
        {
            // Order by status to show active ones first, then by entry date
            string sql = "SELECT * FROM Veiculos ORDER BY CASE WHEN Status = 'Estacionado' THEN 0 ELSE 1 END, DataEntrada DESC, HoraEntrada DESC";
            gvVeiculos.DataSource = DataAccess.ExecuteDataTable(sql, null);
            gvVeiculos.DataBind();
        }
    }
}
