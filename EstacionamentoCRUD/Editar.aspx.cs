using System;
using System.Data;
using System.Data.SqlClient;
using EstacionamentoCRUD.DAL;
using EstacionamentoCRUD.BLL;

namespace EstacionamentoCRUD
{
    public partial class Editar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            if (Session["UsuarioLogado"] == null || Session["PerfilId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            int perfilId = (int)Session["PerfilId"];

            // Admin (1) e Supervisor (3)
            if (perfilId != 1 && perfilId != 3)
            {
                Response.Redirect("~/Home.aspx");
                return;
            }


            if (!IsPostBack)
            {
                CarregarMarcas(); // marcas primeiro


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

        private void CarregarMarcas()
        {
            try
            {
                var marcaBLL = new MarcaBLL();
                ddlMarcas.DataSource = marcaBLL.GetMarcas();
                ddlMarcas.DataTextField = "Nome";
                ddlMarcas.DataValueField = "Id";
                ddlMarcas.DataBind();
                ddlMarcas.Items.Insert(0, new System.Web.UI.WebControls.ListItem
                    ("Selecione uma marca...", ""));
            }
            catch (Exception ex)
            {
                lblMensagem.Text = "Erro ao carregar marcas: " + ex.Message;
                lblMensagem.CssClass = "text-danger";
            }
        }

        private void CarregarVeiculo(int id)
        {
            string sql = "SELECT Placa, Modelo, Cor, DataEntrada, HoraEntrada, MarcaId " +
                "FROM Veiculos " +
                "WHERE Id = @Id";
            var parameters = new[] { new SqlParameter("@Id", id) };
            DataTable dt = DataAccess.ExecuteDataTable(sql, parameters);

            if (dt.Rows.Count > 0)
            {
                hfVeiculoId.Value = id.ToString(); // GUARDA O ID AQUI
                PreencherFormulario(dt.Rows[0]);
                txtPlaca.ReadOnly = true;
                btnBuscar.Enabled = true;
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

            if (dr["MarcaId"] != DBNull.Value)
            {
                ddlMarcas.SelectedValue = dr["MarcaId"].ToString();
            }

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

            string sql = "SELECT Placa, Modelo, Cor, DataEntrada, HoraEntrada, MarcaId " +
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
            if (Session["UsuarioId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Validação do ID
            if (string.IsNullOrEmpty(hfVeiculoId.Value))
            {
                lblMensagem.Text = " Id do veículo perdido. Tente recarregar a página.";
                lblMensagem.CssClass = "text-danger";
                return;
            }

            int veiculoId = Convert.ToInt32(hfVeiculoId.Value);
            string modelo = txtModelo.Text.Trim();
            string cor = txtCor.Text.Trim();
            string marcaIdStr = ddlMarcas.SelectedValue;

            // Validação da Hora
            TimeSpan horaEntrada;
            if (!TimeSpan.TryParse(txtHoraEntrada.Text, out horaEntrada))
            {
                lblMensagem.Text = "Hora inválida.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            // Validação da Data (Garante que datas anteriores funcionem)
            DateTime dataEntrada;
            if (!DateTime.TryParse(txtDataEntrada.Text, out dataEntrada))
            {
                lblMensagem.Text = "Data inválida.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            int? marcaId = !string.IsNullOrEmpty(marcaIdStr) ? Convert.ToInt32(marcaIdStr) : (int?)null;

            try
            {
                string sqlUpdate = @"UPDATE Veiculos 
                            SET Modelo=@Modelo, Cor=@Cor, DataEntrada=@DataEntrada, 
                                HoraEntrada=@HoraEntrada, MarcaId=@MarcaId
                            WHERE Id=@Id";

                var parameters = new[]
                {
            new SqlParameter("@Modelo", modelo),
            new SqlParameter("@Cor", cor),
            // Definindo explicitamente como Date para o SQL
            new SqlParameter("@DataEntrada", SqlDbType.Date) { Value = dataEntrada },
            new SqlParameter("@HoraEntrada", horaEntrada),
            new SqlParameter("@MarcaId", (object)marcaId ?? DBNull.Value),
            new SqlParameter("@Id", veiculoId)
        };

                int rows = DataAccess.ExecuteNonQuery(sqlUpdate, parameters);

                if (rows > 0)
                {

                    //Recupera os dados da sessão
                    int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
                    string nomeUsuario = Session["UsuarioLogado"]?.ToString() ?? "Usuário Desconhecido";

                    // Registra a auditoria usando o nome na descrição
                    // Mantemos o usuarioId no primeiro parâmetro 
                    // E usamos o nomeUsuario na string de texto o que aparece no layout/relatório
                    DataAccess.RegistrarAuditoria(usuarioId,"Veículo editado pelo usuário: " + nomeUsuario, 
                        "Veiculos",veiculoId);

                    // Redirecionamento limpo para atualizar o layout da Home
                    Response.Redirect("~/Home.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    lblMensagem.Text = "Nenhuma alteração foi feita no banco de dados.";
                    lblMensagem.CssClass = "text-warning";
                }
            }
            catch (Exception ex)
            {
                lblMensagem.Text = "Erro técnico: " + ex.Message;
                lblMensagem.CssClass = "text-danger";
            }
        }
    }
}
