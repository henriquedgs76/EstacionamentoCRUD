using System.Configuration;
using System.Data.SqlClient;

namespace EstacionamentoCRUD.DAL
{
    public class Conexao
    {
        public static SqlConnection Conectar()
        {
            string strCon = ConfigurationManager.ConnectionStrings["EstacionamentoDB"].ConnectionString;
            SqlConnection con = new SqlConnection(strCon);
            con.Open();
            return con;
        }
    }
}
