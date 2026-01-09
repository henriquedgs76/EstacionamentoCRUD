using System;
using System.Web.UI;
using EstacionamentoCRUD.BLL; 

namespace EstacionamentoCRUD
{
    public partial class RelatorioClientes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarRelatorio();
            }
        }

        private void CarregarRelatorio()
        {
            try
            {
                // Aqui usamos a classe que você já tem para buscar os dados
                VeiculoBLL bll = new VeiculoBLL();
                gvClientesAusentes.DataSource = bll.ListarClientesAusentes(15);
                gvClientesAusentes.DataBind();
            }
            catch (Exception ex)
            {
                // Exibe o erro de forma amigável para debug
                Response.Write("<script>alert('Erro ao carregar dados: " + ex.Message + "');</script>");
            }
        }

        // Este método evita o erro de "GridView deve estar dentro de um Form"
        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Confirma a renderização */
        }
    }
}