using System.Text;
using Effortless.Net.Encryption;

namespace EncDecExample.WebApi3.Services;

public class EncDecService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncDecService(IConfiguration configuration)
    {
        _key = Encoding.ASCII.GetBytes(configuration["Security:Key"]!);
        _iv = Encoding.ASCII.GetBytes(configuration["Security:IV"]!);
    }

    public string Encrypt(string plainText)
    {
        return Strings.Encrypt(plainText, _key, _iv);
    }

    public string Decrypt(string cipherText)
    {
        return Strings.Decrypt(cipherText, _key, _iv);
    }
}