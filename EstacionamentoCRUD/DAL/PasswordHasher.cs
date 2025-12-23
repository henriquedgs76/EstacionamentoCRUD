using System;
using System.Security.Cryptography;

namespace EstacionamentoCRUD.DAL
{
    public static class PasswordHasher
    {
        private const int TamanhoDoSalt = 16; // 128 bit
        private const int TamanhoDoHash = 20; // 160 bit
        private const int Iteracoes = 10000;

        // Cria um hash a partir de uma senha
        public static (byte[] hash, byte[] salt) HashPassword(string senha)
        {
            // 1. Gera um salt aleatório
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[TamanhoDoSalt]);

            // 2. Gera o hash usando PBKDF2
            var pbkdf2 = new Rfc2898DeriveBytes(senha, salt, Iteracoes);
            byte[] hash = pbkdf2.GetBytes(TamanhoDoHash);

            return (hash, salt);
        }

        // Verifica se a senha digitada corresponde ao hash salvo
        public static bool VerifyPassword(string senha, byte[] hashArmazenado, byte[] saltArmazenado)
        {
            // 1. Gera um hash da senha digitada usando o salt que já estava salvo no banco
            var pbkdf2 = new Rfc2898DeriveBytes(senha, saltArmazenado, Iteracoes);
            byte[] hashParaComparar = pbkdf2.GetBytes(TamanhoDoHash);

            // 2. Compara os dois hashes byte a byte
            for (int i = 0; i < hashParaComparar.Length; i++)
            {
                if (hashParaComparar[i] != hashArmazenado[i])
                {
                    return false; // Se qualquer byte for diferente, a senha está incorreta
                }
            }
            return true; // Se todos os bytes forem iguais, a senha está correta
        }
    }
}
