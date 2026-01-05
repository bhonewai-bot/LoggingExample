using System.Text;
using Effortless.Net.Encryption;

namespace EncDecExample.WebApi.Services;

public class EncDecService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncDecService(IConfiguration configuration)
    {
        _key = Encoding.ASCII.GetBytes(configuration["Security:Key"]!);
        _iv = Encoding.ASCII.GetBytes(configuration["Security:Iv"]!);
    }

    public string Encrypt(string plainText)
    {
        return Strings.Encrypt(plainText, _key, _iv);
    }

    public string Decrypt(string encryptedText)
    {
        return Strings.Decrypt(encryptedText, _key, _iv);
    }
    
    
}