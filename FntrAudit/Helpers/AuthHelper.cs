using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FntrAudit.Helpers
{
    public class AuthHelper : IAuthHelpers
    {

        public async Task<string> GenerateRandomStringAsync()
        {
            return await Task.Run(() =>
            {
                // Les caractères que vous souhaitez inclure dans la chaîne aléatoire
                const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@!:;=)ç_?";

                // Créez une instance de la classe Random pour générer des indices aléatoires
                Random random = new Random();

                // Génère la chaîne aléatoire en sélectionnant des caractères aléatoires du pool de caractères
                char[] randomString = new char[10];
                for (int i = 0; i < 10; i++)
                {
                    randomString[i] = characters[random.Next(characters.Length)];
                }

                return new string(randomString);
            });
        }

    }
}
