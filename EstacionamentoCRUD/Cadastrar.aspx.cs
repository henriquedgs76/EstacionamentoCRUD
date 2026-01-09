using EstacionamentoCRUD.BLL;
using EstacionamentoCRUD.DAL;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Estacionamento
{
    public partial class Cadastrar : System.Web.UI.Page
    {
        // Quando a página é carregada pela primeira vez 
        // a gente já manda carregar as vagas, marcas, serviços e a lista dos carros que já tão no pátio.
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarVagasLivres();
                CarregarMarcas();
                CarregarServicos();
                CarregarGrid();
            }
        }
        // Busca no banco os 5 últimos carros que entraram e ainda estão no pátio ('Estacionado').
        // E aí mostra essa lista na telinha pra gente ver.
        private void CarregarGrid()
        {
            try
            {
                
                // Isso garante que assim que o carro virar 'Finalizado', ele some do Grid automaticamente
                string sql = @"SELECT TOP 5 
                        V.Placa, 
                        M.Nome AS Marca, 
                        V.Modelo, 
                        V.Cor, 
                        VG.NumeroDaVaga AS Vaga, 
                        V.DataEntrada
                       FROM Veiculos V
                       LEFT JOIN Marcas M ON V.MarcaId = M.Id
                       INNER JOIN Vagas VG ON V.VagaId = VG.Id
                       WHERE V.Status = 'Estacionado' 
                       ORDER BY V.Id DESC";

                DataTable dt = DataAccess.ExecuteDataTable(sql, null);
                gvVeiculos.DataSource = dt;
                gvVeiculos.DataBind();
            }
            catch (Exception ex)
            {
                lblMensagem.Text = "Erro: " + ex.Message;
            }
        }

        // quando o usuário termina de digitar a placa e sai do campo,
        // o sistema vai no banco e vê se esse carro já veio aqui antes.
        // Se já veio, ele preenche o modelo, a cor e a marca automaticamente pra adiantar o trabalho.
        protected void txtPlaca_TextChanged(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim();

            if (placa.Length >= 7)
            {
                try
                {
                    string sql = @"SELECT TOP 1 MarcaId, Modelo, Cor , Telefone
                           FROM Veiculos 
                           WHERE Placa = @Placa 
                           ORDER BY Id DESC";

                    // Criando o parâmetro da forma que o SQL Server entende melhor
                    System.Data.SqlClient.SqlParameter[] parametros = {
                new System.Data.SqlClient.SqlParameter("@Placa", placa)
            };

                    DataTable dt = DataAccess.ExecuteDataTable(sql, parametros);

                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        txtModelo.Text = row["Modelo"].ToString();
                        txtCor.Text = row["Cor"].ToString();
                        // Preenche o telefone se ele já for cliente
                        if (dt.Columns.Contains("Telefone"))
                        {
                            txtTelefone.Text = row["Telefone"].ToString();
                        }


                        if (row["MarcaId"] != DBNull.Value)
                        {
                            // Tenta selecionar a marca no DropDown
                            string marcaId = row["MarcaId"].ToString();
                            if (ddlMarcas.Items.FindByValue(marcaId) != null)
                            {
                                ddlMarcas.SelectedValue = marcaId;
                            }
                        }

                        lblMensagem.Text = "Veículo já cadastrado! Só selecionar a vaga.";
                        lblMensagem.CssClass = "text-info";
                        ddlVagas.Focus();
                    }
                }
                catch (Exception ex)
                {
                    lblMensagem.Text = "Erro ao buscar dados: " + ex.Message;
                }
            }
        }

        // Busca no banco a lista de serviços que o lava-rápido oferece (ex: Lavagem Simples, Cera, etc.)
        // e coloca tudo num dropdown (caixinha de seleção).
        private void CarregarServicos()
        {
            try
            {
                string sql = "SELECT Id, NomeServico, PrecoPadrao FROM Servicos ORDER BY NomeServico";
                DataTable dt = DataAccess.ExecuteDataTable(sql, null);

                
                ddlServicos.DataSource = dt;
                ddlServicos.DataValueField = "Id";
                // Vamos exibir o nome e o preço para facilitar
                ddlServicos.DataTextField = "NomeServico";
                ddlServicos.DataBind();

                ddlServicos.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Selecione o Serviço...", ""));
            }
            catch (Exception ex)
            {
                lblMensagem.Text = "Erro ao carregar serviços: " + ex.Message;
            }
        }

        // Busca a lista de marcas de carro e carrega no dropdown de marcas.
        private void CarregarMarcas()
        {
            try
            {
                var marcaBLL = new MarcaBLL();
                DataTable dt = marcaBLL.GetMarcas();

                ddlMarcas.DataSource = dt;
                ddlMarcas.DataValueField = "Id";
                ddlMarcas.DataTextField = "Nome";
                ddlMarcas.DataBind();

                ddlMarcas.Items.Insert(0, new System.Web.UI.WebControls.ListItem
                    ("Selecione uma marca...", ""));
            }
            catch (Exception ex)
            {
                lblMensagem.Text = " Erro ao carregar as marcas.";
                lblMensagem.CssClass = "text-danger" + ex.Message;
            }
        }

        // Pega só as vagas que tão com status 'Livre' e mostra no dropdown pra gente poder escolher uma.
        private void CarregarVagasLivres()
        {
            try
            {
                string sql = "SELECT Id, " +
                    "NumeroDaVaga FROM Vagas " +
                    "WHERE Status = 'Livre' " +
                    "ORDER BY NumeroDaVaga";
                DataTable dt = DataAccess.ExecuteDataTable(sql, null);

                ddlVagas.DataSource = dt;
                ddlVagas.DataValueField = "Id";
                ddlVagas.DataTextField = "NumeroDaVaga";
                ddlVagas.DataBind();

                ddlVagas.Items.Insert(0,
                    new System.Web.UI.WebControls.ListItem
                    ("Selecione uma vaga...", ""));
            }
            catch (Exception ex)
            {
                lblMensagem.Text = " Erro ao carregar as vagas disponíveis." + ex.Message;
                lblMensagem.CssClass = "text-danger";
            }
        }



        
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            //  Antes de tudo, confere se o usuário ainda tá logado. Se a sessão caiu, manda ele pro login de novo.
            if (Session["UsuarioId"] == null)
            {
                lblMensagem.Text = "Sua sessão expirou. Faça login novamente.";
                lblMensagem.CssClass = "text-danger";
                Response.AddHeader("REFRESH", "1;URL=Login.aspx");
                return;
            }

            // Pega todas as informações que o usuário preencheu nos campos da tela.
            string placa = txtPlaca.Text.Trim();
            string modelo = txtModelo.Text.Trim();
            string cor = txtCor.Text.Trim();
            string vagaId = ddlVagas.SelectedValue;
            string marcaIdStr = ddlMarcas.SelectedValue;
            string servicoIdStr = ddlServicos.SelectedValue;
            string telefone = txtTelefone.Text.Trim();


            // Dá uma checada básica pra ver se os campos mais importantes (placa, vaga, serviço) foram preenchidos.
            if (string.IsNullOrEmpty(placa) || string.IsNullOrEmpty(vagaId) || string.IsNullOrEmpty(servicoIdStr))
            {
                lblMensagem.Text = "Preencha Placa, Vaga e o Serviços!";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            try
            {
                // Prepara os IDs pra mandar pra camada de negócio.
                int? marcaId = string.IsNullOrEmpty(marcaIdStr) ? (int?)null : Convert.ToInt32(marcaIdStr);
                int servicoId = Convert.ToInt32(servicoIdStr);
                var veiculoBLL = new VeiculoBLL();

                // Chama o método `RegistrarEntrada` lá da nossa BLL.
                // É a BLL que tem toda a regra de como salvar. A gente só entrega os dados pra ela.
                Tuple<string, int> resultado = veiculoBLL.RegistrarEntrada(
                    placa,
                    marcaId,
                    modelo,
                    cor,
                    Convert.ToInt32(vagaId),
                    servicoId,
                    telefone
                );

                string ticketId = resultado.Item1;
                int novoVeiculoId = resultado.Item2;
       
                // Depois que salva, a gente registra na auditoria que o usuário X deu entrada no carro Y.
                int usuarioId = Convert.ToInt32(Session["UsuarioId"]);
                string nomeUsuario = Session["UsuarioLogado"]?.ToString() ?? "Usuário Desconhecido";

                DataAccess.RegistrarAuditoria(usuarioId, "Lavagem iniciada por: " + nomeUsuario, "Veiculos", novoVeiculoId);

                // Se deu tudo certo, manda o usuário direto pra tela de imprimir o ticket.
                Response.Redirect($"ImprimirTicket.aspx?ticket={ticketId}", true);
            }
            catch (Exception ex)
            {
                // Se qualquer coisa der errado no meio do caminho, pega o erro e mostra uma mensagem vermelha na tela.
                lblMensagem.Text = "Erro: " + ex.Message;
                lblMensagem.CssClass = "text-danger";
            }

        }
    }
}