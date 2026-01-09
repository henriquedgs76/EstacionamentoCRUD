using System;
using System.Data;
using EstacionamentoCRUD.DAL;

namespace EstacionamentoCRUD.Pages
{
    public partial class Listar : System.Web.UI.Page
    {
        // Quando a página carrega pela primeira vez...
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // ...a gente monta uma consulta SQL pra pegar todos os veículos que estão ativos no pátio.
                // A consulta também busca o nome da marca e o número da vaga pra mostrar tudo junto.
                string sql = @"       
                    SELECT 
                        V.Id,
                        V.Placa,
                        M.Nome AS Marca,
                        V.Modelo,
                        V.Cor,
                        V.DataEntrada,
                        V.HoraEntrada,
                        V.Status,
                        VG.NumeroDaVaga 
                    FROM 
                        Veiculos V
                        LEFT JOIN Vagas VG ON V.VagaId = VG.Id
                        LEFT JOIN Marcas M ON V.MarcaId = M.Id
                    WHERE
                        V.Ativo = 1 
                    ORDER BY 
                        V.DataEntrada DESC, 
                        V.HoraEntrada DESC";
                //aqui é pra listar os veiculos


                // Depois de buscar os dados, a gente joga eles dentro do GridView pra mostrar na tela.
                GridView1.DataSource = DataAccess.ExecuteDataTable(sql, null);
                GridView1.DataBind();
            }
        }
    }
}
