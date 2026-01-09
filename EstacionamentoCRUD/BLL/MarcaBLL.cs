using System.Data;
using EstacionamentoCRUD.DAL;

namespace EstacionamentoCRUD.BLL
{
    public class MarcaBLL
    {
        // pega no banco a lista de todas as marcas de carro pra gente usar nos dropdowns aquelas caixinhas de seleção
        public DataTable GetMarcas()
        {
            string sql = "SELECT Id, Nome FROM Marcas ORDER BY Nome";
            return DataAccess.ExecuteDataTable(sql, null);
        }
    }
}
