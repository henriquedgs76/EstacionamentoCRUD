using System.Data;
using EstacionamentoCRUD.DAL;

namespace EstacionamentoCRUD.DAL
{
    public static class DashboardDAL
    {
        public static DataTable FaturamentoPorDia()
        {
            string sql = @"
                SELECT 
                    CAST(DataEntrada AS DATE) AS Dia,
                    SUM(Valor) AS Total
                FROM Movimentacao
                WHERE Valor IS NOT NULL
                GROUP BY CAST(DataEntrada AS DATE)
                ORDER BY Dia";

            return DataAccess.ExecuteDataTable(sql, null);
        }

        public static DataTable VeiculosPorDia()
        {
            string sql = @"
                SELECT 
                    CAST(DataEntrada AS DATE) AS Dia,
                    COUNT(*) AS Total
                FROM Movimentacao
                GROUP BY CAST(DataEntrada AS DATE)
                ORDER BY Dia";

            return DataAccess.ExecuteDataTable(sql, null);
        }

        public static DataTable StatusVeiculos()
        {
            string sql = @"
                SELECT 
                    SUM(CASE WHEN DataSaida IS NULL THEN 1 ELSE 0 END) AS Ativos,
                    SUM(CASE WHEN DataSaida IS NOT NULL THEN 1 ELSE 0 END) AS Finalizados
                FROM Movimentacao";

            return DataAccess.ExecuteDataTable(sql, null);
        }
    }
}
