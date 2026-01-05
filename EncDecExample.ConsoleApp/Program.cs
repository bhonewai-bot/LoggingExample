using System.Text;
using Effortless.Net.Encryption;

/*byte[] key = Bytes.GenerateKey();
byte[] iv = Bytes.GenerateIV();*/

byte[] key = Encoding.ASCII.GetBytes("");
byte[] iv = Encoding.ASCII.GetBytes("");
string encrypted = Strings.Encrypt("Secret", key, iv);
Console.WriteLine(encrypted);
string decrypted = Strings.Decrypt(encrypted, key, iv);
Console.WriteLine(decrypted);