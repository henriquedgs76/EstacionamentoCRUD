using System;
using System.Security.Cryptography;

namespace EstacionamentoCRUD.DAL
{
    public static class PasswordHasher
    {
        private const int TamanhoDoSalt = 16; // 128 bit
        private const int TamanhoDoHash = 20; // 160 bit
        private const int Iteracoes = 10000;

        // Pega a senha que o usuário digitou e transforma ela num monte de código (hash) que não dá pra ler.
        // Também cria um "sal" (salt), que é tipo um tempero aleatório pra senha ficar mais segura.
        public static (byte[] hash, byte[] salt) HashPassword(string senha)
        {
            // Gera um salt aleatório pra cada senha nova.
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[TamanhoDoSalt]);

            // Usa um algoritmo chamado PBKDF2 pra criar o hash da senha junto com o salt.
            var pbkdf2 = new Rfc2898DeriveBytes(senha, salt, Iteracoes);
            byte[] hash = pbkdf2.GetBytes(TamanhoDoHash);

            return (hash, salt);
        }

        // Esse aqui faz o caminho contrário na hora do login.
        public static bool VerifyPassword(string senha, byte[] hashArmazenado, byte[] saltArmazenado)
        {
            // Pega a senha que o usuário tentou usar e o sal que a gente guardou no banco...
            // ... e gera um hash novo pra comparar.
            var pbkdf2 = new Rfc2898DeriveBytes(senha, saltArmazenado, Iteracoes);
            byte[] hashParaComparar = pbkdf2.GetBytes(TamanhoDoHash);

            // Compara o hash novo com o que estava no banco.
            // Se forem idênticos, a senha tá certa.
            for (int i = 0; i < hashParaComparar.Length; i++)
            {
                if (hashParaComparar[i] != hashArmazenado[i])
                {
                    return false; // Se um pedacinho for diferente, já era, senha errada.
                }
            }
            return true; // Se passou no teste todo, a senha tá correta.
        }
    }
}
