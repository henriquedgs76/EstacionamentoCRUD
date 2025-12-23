using System;
using System.Data;
using EstacionamentoCRUD.DAL;

namespace EstacionamentoCRUD.Pages
{
    public partial class Listar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string sql = @"
                    SELECT 
                        V.*, 
                        VG.NumeroDaVaga 
                    FROM 
                        Veiculos V
                        LEFT JOIN Vagas VG ON V.VagaId = VG.Id
                    ORDER BY 
                        V.DataEntrada DESC, 
                        V.HoraEntrada DESC";
                GridView1.DataSource = DataAccess.ExecuteDataTable(sql, null);
                GridView1.DataBind();
            }
        }
    }
}
