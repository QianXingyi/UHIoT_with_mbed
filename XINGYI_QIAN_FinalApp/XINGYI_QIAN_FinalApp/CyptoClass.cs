using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Security;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace XINGYI_QIAN_FinalApp
{
    public static class CryptoClass
    {
        #region RSA Encryption Methods
        public static CryptographicKey generateSessionKey()
        {
            CryptographicKey keyPair;
            keyPair = (AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1)).CreateKeyPair(2048);
            //Size of key is 2048

            return keyPair;
        }

        public static string getPreGeneratedPublicKey(CryptographicKey keyPair)
        {
            return CryptographicBuffer.EncodeToBase64String(keyPair.ExportPublicKey());
            //key(buffer)->string
        }

        public static string RSA_Encrypt(string input, string publicKey)
        {
            AsymmetricKeyAlgorithmProvider RSAProvider = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);
            //choose the encrypt algorithm as RSA
            CryptographicKey PK = RSAProvider.ImportPublicKey(CryptographicBuffer.DecodeFromBase64String(publicKey));
            //Import the public key

            IBuffer encData = CryptographicEngine.Encrypt(PK, CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(input)), null);
            //encrypted data
            return CryptographicBuffer.EncodeToBase64String(encData);
            //Cyphertext(buffer->string)
        }

        public static string RSA_Decrypt(string input, CryptographicKey keyPair)
        {
            byte[] Decrypted;
            AsymmetricKeyAlgorithmProvider RSAProvider = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);
            CryptographicBuffer.CopyToByteArray(CryptographicEngine.Decrypt(keyPair, CryptographicBuffer.DecodeFromBase64String(input), null), out Decrypted);

            return Encoding.UTF8.GetString(Decrypted, 0, Decrypted.Length);
        }
        #endregion
        #region AES Encryption Methods
        static IBuffer IV;
        static void GenerateIV()
        {
            IV = CryptographicBuffer.GenerateRandom(32);
            //Random IV

        }
        public static string AES_Ecrypt(string input, string pass)
        {
            GenerateIV();
            SymmetricKeyAlgorithmProvider SAP = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            CryptographicKey AES;
            //Key
            HashAlgorithmProvider HAP = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            CryptographicHash Hash_AES = HAP.CreateHash();

            string encrypted = "";
            try
            {
                byte[] hash = new byte[32];
                Hash_AES.Append(CryptographicBuffer.CreateFromByteArray(System.Text.Encoding.UTF8.GetBytes(pass)));
                //pass->key
                byte[] temp;
                CryptographicBuffer.CopyToByteArray(Hash_AES.GetValueAndReset(), out temp);

                Array.Copy(temp, 0, hash, 0, 16);
                Array.Copy(temp, 0, hash, 16, 16);
                //totally 32 bits,16 bits once,temp-> hash

                AES = SAP.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(hash));
                IBuffer Buffer = CryptographicBuffer.CreateFromByteArray(System.Text.Encoding.UTF8.GetBytes(input));
                encrypted = CryptographicBuffer.EncodeToBase64String(CryptographicEngine.Encrypt(AES, Buffer, IV));
                //key,data,Initialization vector
                return encrypted + "!" + CryptographicBuffer.EncodeToBase64String(IV);
                //line break
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public static string AES_Decrypt(string input, string pass)
        {
            //opposite process
            string[] strInput = input.Split('!');
            //Split IV and cyphertext
            IBuffer BufferIV = CryptographicBuffer.DecodeFromBase64String(strInput[1]);
            SymmetricKeyAlgorithmProvider SAP = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
            CryptographicKey AES;
            HashAlgorithmProvider HAP = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            CryptographicHash Hash_AES = HAP.CreateHash();

            string decrypted = "";
            try
            {
                byte[] hash = new byte[32];
                Hash_AES.Append(CryptographicBuffer.CreateFromByteArray(System.Text.Encoding.UTF8.GetBytes(pass)));
                byte[] temp;
                CryptographicBuffer.CopyToByteArray(Hash_AES.GetValueAndReset(), out temp);

                Array.Copy(temp, 0, hash, 0, 16);
                Array.Copy(temp, 0, hash, 16, 16);
                //temp->hash

                AES = SAP.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(hash));

                IBuffer Buffer = CryptographicBuffer.DecodeFromBase64String(strInput[0]);
                byte[] Decrypted;
                CryptographicBuffer.CopyToByteArray(CryptographicEngine.Decrypt(AES, Buffer, BufferIV), out Decrypted);
                decrypted = System.Text.Encoding.UTF8.GetString(Decrypted, 0, Decrypted.Length);
                return decrypted;
            }
            catch (Exception ex)
            {
                return null;
            }
            #endregion
        }


        private static char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
         'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
         'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};

        public static string GenerateRandomString()
        {
            string checkCode = String.Empty;
            Random rd = new Random();
            for (int i = 0; i < 16; i++)
            {
                checkCode += constant[rd.Next(62)].ToString();
            }
            return checkCode;
        }


    }
}
