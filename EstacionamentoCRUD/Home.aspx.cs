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
            string sql = @"
                SELECT 
                    V.*, 
                    VG.NumeroDaVaga 
                FROM 
                    Veiculos V
                    LEFT JOIN Vagas VG ON V.VagaId = VG.Id
                ORDER BY 
                    CASE WHEN V.Status = 'Estacionado' THEN 0 ELSE 1 END, 
                    V.DataEntrada DESC, 
                    V.HoraEntrada DESC";
            gvVeiculos.DataSource = DataAccess.ExecuteDataTable(sql, null);
            gvVeiculos.DataBind();
        }
    }
}
