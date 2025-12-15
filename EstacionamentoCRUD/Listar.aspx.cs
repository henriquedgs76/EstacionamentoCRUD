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
                GridView1.DataSource = DataAccess.ExecuteDataTable("SELECT * FROM Veiculos ORDER BY DataEntrada DESC, HoraEntrada DESC", null);
                GridView1.DataBind();
            }
        }
    }
}
