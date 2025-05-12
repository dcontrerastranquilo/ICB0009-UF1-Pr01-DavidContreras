using System;
using System.Text;
using System.Security.Cryptography;
using ClaveSimetricaClass;
using ClaveAsimetricaClass;

namespace SimuladorEnvioRecepcion
{
    class Program
    {
        static string UserName;
        static byte[] SecurePassHash;
        static byte[] Salt;
        static byte[] Firma;
        static byte[] ClaveSimetricaKeyCifrada;
        static byte[] ClaveSimetricaIVCifrada;

        static ClaveAsimetrica Emisor = new ClaveAsimetrica();
        static ClaveAsimetrica Receptor = new ClaveAsimetrica();
        static ClaveSimetrica ClaveSimetricaEmisor = new ClaveSimetrica();
        static ClaveSimetrica ClaveSimetricaReceptor = new ClaveSimetrica();

        static string TextoAEnviar = "Me he dado cuenta que incluso las personas que dicen que todo está predestinado y que no podemos hacer nada para cambiar nuestro destino igual miran antes de cruzar la calle. Stephen Hawking.";

        static void Main(string[] args)
        {
            /**** PARTE 1: Registro/Login ****/

            Console.WriteLine("¿Deseas registrarte? (S/N)");
            string registro = Console.ReadLine();

            if (registro.Equals("S", StringComparison.OrdinalIgnoreCase))
            {
                Registro();
            }

            bool login = Login();

            /*** FIN PARTE 1 ***/

            if (login)
            {
                byte[] TextoAEnviar_Bytes = Encoding.UTF8.GetBytes(TextoAEnviar);
                Console.WriteLine("Texto a enviar bytes: {0}", BytesToStringHex(TextoAEnviar_Bytes));

                // ------------------ LADO EMISOR ------------------

                // Firmar mensaje
                Firma = Emisor.FirmarMensaje(TextoAEnviar_Bytes);

                // Cifrar mensaje con clave simétrica
                byte[] TextoCifrado = ClaveSimetricaEmisor.CifrarMensaje(TextoAEnviar);

                // Cifrar clave simétrica con clave pública del receptor
                ClaveSimetricaKeyCifrada = Emisor.CifrarMensaje(ClaveSimetricaEmisor.Key, Receptor.PublicKey);
                ClaveSimetricaIVCifrada = Emisor.CifrarMensaje(ClaveSimetricaEmisor.IV, Receptor.PublicKey);

                // Mostrar datos
                Console.WriteLine("\n--- DATOS ENVIADOS ---");
                Console.WriteLine("Firma: {0}", BytesToStringHex(Firma));
                Console.WriteLine("Texto cifrado: {0}", BytesToStringHex(TextoCifrado));
                Console.WriteLine("Clave simétrica cifrada (Key): {0}", BytesToStringHex(ClaveSimetricaKeyCifrada));
                Console.WriteLine("Clave simétrica cifrada (IV): {0}", BytesToStringHex(ClaveSimetricaIVCifrada));

                // ------------------ LADO RECEPTOR ------------------

                // Descifrar clave e IV
                byte[] ClaveSimetricaKey = Receptor.DescifrarMensaje(ClaveSimetricaKeyCifrada);
                byte[] ClaveSimetricaIV = Receptor.DescifrarMensaje(ClaveSimetricaIVCifrada);

                ClaveSimetricaReceptor.Key = ClaveSimetricaKey;
                ClaveSimetricaReceptor.IV = ClaveSimetricaIV;

                //Descifrar mensaje
                string TextoDescifrado = ClaveSimetricaReceptor.DescifrarMensaje(TextoCifrado);
                byte[] TextoDescifradoBytes = Encoding.UTF8.GetBytes(TextoDescifrado);

                //Comprobar firma
                bool firmaCorrecta = Receptor.ComprobarFirma(Firma, TextoDescifradoBytes, Emisor.PublicKey);

                // Mostrar resultados
                Console.WriteLine("\n--- RECEPCIÓN ---");
                Console.WriteLine("Texto descifrado: {0}", TextoDescifrado);
                Console.WriteLine("Firma válida: {0}", firmaCorrecta);
            }
        }

        public static void Registro()
        {
            Console.WriteLine("Indica tu nombre de usuario:");
            UserName = Console.ReadLine();

            Console.WriteLine("Indica tu password:");
            string passwordRegister = Console.ReadLine();

            // SALT
            Salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(Salt);
            }

            // HASH + SALT
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(passwordRegister);
                byte[] passwordWithSalt = Salt.Concat(passwordBytes).ToArray();
                SecurePassHash = sha512.ComputeHash(passwordWithSalt);
            }

            Console.WriteLine("Registro exitoso con password seguro.");
        }

        public static bool Login()
        {
            bool auxlogin = false;

            do
            {
                Console.WriteLine("Acceso a la aplicación");
                Console.WriteLine("Usuario: ");
                string userName = Console.ReadLine();

                Console.WriteLine("Password: ");
                string Password = Console.ReadLine();

                if (userName.Equals(UserName, StringComparison.OrdinalIgnoreCase))
                {
                    using (SHA512 sha512 = SHA512.Create())
                    {
                        byte[] inputPasswordBytes = Encoding.UTF8.GetBytes(Password);
                        byte[] inputWithSalt = Salt.Concat(inputPasswordBytes).ToArray();
                        byte[] hashToCompare = sha512.ComputeHash(inputWithSalt);

                        if (hashToCompare.SequenceEqual(SecurePassHash))
                        {
                            Console.WriteLine("Login correcto.");
                            auxlogin = true;
                        }
                        else
                        {
                            Console.WriteLine("Password incorrecto.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Usuario incorrecto.");
                }

            } while (!auxlogin);

            return auxlogin;
        }

        static string BytesToStringHex(byte[] result)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (byte b in result)
                stringBuilder.AppendFormat("{0:x2}", b);

            return stringBuilder.ToString();
        }
    }
}
