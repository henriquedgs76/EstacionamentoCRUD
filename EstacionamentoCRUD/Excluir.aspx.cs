using System;
using System.Data; 
using System.Data.SqlClient;
using EstacionamentoCRUD.DAL;

namespace EstacionamentoCRUD
{
    public partial class Excluir : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            // Primeiro, checa se o usuário está logado.
            if (Session["UsuarioLogado"] == null || Session["PerfilId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Pega o perfil do usuário da sessão.
            int perfilId = Convert.ToInt32(Session["PerfilId"]);

            // Se o perfil for 'Operador' 
            if (perfilId == 2)
            {
                // ...mostra uma mensagem de erro, desabilita os botões e para tudo.
                lblMensagem.Text = "Acesso negado. Operadores não podem excluir registros.";
                lblMensagem.CssClass = "text-danger";
                btnBuscar.Enabled = false;
                btnExcluir.Enabled = false;
                txtPlaca.Enabled = false;
                return;
            }
            

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
                lblMensagem.Text = " Confirme a exclusão do veículo com a placa acima.";
                lblMensagem.CssClass = "text-warning";
            }
            else
            {
                lblMensagem.Text = " Veículo não encontrado.";
                lblMensagem.CssClass = "text-danger";
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim();

            if (string.IsNullOrEmpty(placa))
            {
                lblMensagem.Text = " Digite a placa.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            string sql = "SELECT COUNT(*) " +
                "FROM Veiculos " +
                "WHERE Placa = @Placa";
            var parameters = new[]
            {
                new SqlParameter("@Placa", placa)
            };
            int count = Convert.ToInt32(DataAccess.ExecuteScalar(sql, parameters));

            if (count > 0)
            {
                lblMensagem.Text = " Veículo encontrado. Clique em 'Excluir' para confirmar.";
                lblMensagem.CssClass = "text-danger";
            }
            else
            {
                lblMensagem.Text = " Veículo não encontrado.";
                lblMensagem.CssClass = "text-danger";
            }
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim();
            if (string.IsNullOrEmpty(placa))
            {
                lblMensagem.Text = "Digite a placa.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            if (Session["UsuarioId"] == null)
            {
                lblMensagem.Text = "Sua sessão expirou. Por favor, faça o login novamente.";
                lblMensagem.CssClass = "text-danger";
                Response.AddHeader("REFRESH", "0;URL=Login.aspx");
                return;
            }

            // Usar uma transação para garantir que todas as operações funcionem ou nenhuma delas.
            using (var connection = new SqlConnection(DataAccess.GetConnectionString()))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    //  Buscar o ID do veículo e o ID da vaga que ele ocupa.
                    string sqlGetInfo = "SELECT Id, VagaId FROM Veiculos WHERE Placa = @Placa AND Ativo = 1";
                    var paramGetInfo = new[] { new SqlParameter("@Placa", placa) };
                    
                    int? veiculoId = null;
                    int? vagaId = null;

                    // Usamos um DataTable para pegar os dois valores de uma vez.
                    DataTable dt = DataAccess.ExecuteDataTable(sqlGetInfo, paramGetInfo, transaction);

                    if (dt.Rows.Count > 0)
                    {
                        veiculoId = Convert.ToInt32(dt.Rows[0]["Id"]);
                        if (dt.Rows[0]["VagaId"] != DBNull.Value)
                        {
                            vagaId = Convert.ToInt32(dt.Rows[0]["VagaId"]);
                        }
                    }

                    if (veiculoId == null)
                    {
                        lblMensagem.Text = "Erro: veículo não encontrado ou já inativo.";
                        lblMensagem.CssClass = "text-danger";
                        transaction.Rollback();
                        return;
                    }

                    // Se o veículo estava em uma vaga, liberar a vaga.
                    if (vagaId.HasValue)
                    {
                        string sqlUpdateVaga = "UPDATE Vagas SET Status = 'Livre' WHERE Id = @VagaId";
                        var paramUpdateVaga = new[] { new SqlParameter("@VagaId", vagaId.Value) };
                        DataAccess.ExecuteNonQuery(sqlUpdateVaga, paramUpdateVaga, transaction);
                    }

                    // Inativar o veículo, mudar seu status e remover a referência à vaga.
                    string sqlUpdateVeiculo = "UPDATE Veiculos SET Ativo = 0, Status = 'Excluído', VagaId = NULL WHERE Id = @Id";
                    var paramUpdateVeiculo = new[] { new SqlParameter("@Id", veiculoId.Value) };
                    int rows = DataAccess.ExecuteNonQuery(sqlUpdateVeiculo, paramUpdateVeiculo, transaction);

                    if (rows > 0)
                    {
                        // Registrar a auditoria.
                        int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
                        string nomeUsuario = Session["UsuarioLogado"]?.ToString() ?? "Usuário Desconhecido";
                        DataAccess.RegistrarAuditoria(usuarioId, "Veículo excluído pelo usuário: " + nomeUsuario, "Veiculos", veiculoId.Value, transaction);

                        // Se tudo deu certo, comitar a transação.
                        transaction.Commit();

                        lblMensagem.Text = "Veículo marcado como inativo e vaga liberada com sucesso!";
                        lblMensagem.CssClass = "text-success";
                        Response.AddHeader("REFRESH", "2;URL=Home.aspx");
                    }
                    else
                    {
                        lblMensagem.Text = "Erro: Nenhuma linha foi atualizada. O veículo pode já estar inativo.";
                        lblMensagem.CssClass = "text-danger";
                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    // Se qualquer coisa der errado, desfaz tudo.
                    transaction.Rollback();
                    lblMensagem.Text = "Ocorreu um erro inesperado durante a exclusão: " + ex.Message;
                    lblMensagem.CssClass = "text-danger";
                }
            }
        }
    }
}
