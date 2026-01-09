using System;
using System.Web.UI;

namespace EstacionamentoCRUD
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Define a visibilidade padrão de todos os links, exceto para as páginas de login/cadastro
            if (!Page.AppRelativeVirtualPath.Contains("Login.aspx") &&
                !Page.AppRelativeVirtualPath.Contains("CadastrarUsuario.aspx"))
            {
                // Se a sessão do usuário não existir, redireciona para a página de login
                if (Session["UsuarioLogado"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                    return; // Interrompe a execução para evitar processamento desnecessário
                }

                // aqui é pra mostrar o nome do usuário logado
                var lblUsuario = FindControl("lblUsuarioLogado") as System.Web.UI.HtmlControls.HtmlGenericControl;
                if (lblUsuario != null)
                {
                    lblUsuario.InnerText = "Usuário: " + Session["UsuarioLogado"].ToString();
                }

                // aqui permito pelo nivel de acesso
                string nivelAcesso = Session["NivelAcesso"]?.ToString();

                if (string.IsNullOrEmpty(nivelAcesso))
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }


                // não mostra o dashboard e nem relatorio pro Operador
                if (string.Equals(nivelAcesso, "Operador", StringComparison.OrdinalIgnoreCase))
                {
                    var navDashboard = FindControl("navLinkDashboard");
                    if (navDashboard != null) navDashboard.Visible = false;

                    var navRelatorios = FindControl("navLinkRelatorios");
                    if (navRelatorios != null) navRelatorios.Visible = false;

                    // operador consegue editar, mais salva na tabela auditoria
                    // quem fez a ação
                    var navEditar = FindControl("navLinkEditar");
                    if (navEditar != null) navEditar.Visible = false;
                }
                //se for adm ou supervisor tem acesso a tudo
            }
            else
            {
                // Se estiver na página de login ou cadastro,
                // esconde todos os links de navegação e o label do usuário
                var navPlaceholder = FindControl("mainNavLinksPlaceholder");
                if (navPlaceholder != null) navPlaceholder.Visible = false;

                var lblUsuario = FindControl("lblUsuarioLogado");
                if (lblUsuario != null) lblUsuario.Visible = false;
            }
        }
        protected void lnkSair_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }
    }
}