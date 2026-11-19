using Microsoft.AspNetCore.DataProtection;

namespace API.Applications.Services.SonarCube;

public class CriptografiaService
{
    private readonly IDataProtector _protetor;

    public CriptografiaService(IDataProtectionProvider provider) =>
        _protetor = provider.CreateProtector("SkaL4ble.Sonar.Tokens");

    public string Proteger(string textoPlano) => _protetor.Protect(textoPlano);

    public string Desproteger(string textoCifrado) => _protetor.Unprotect(textoCifrado);
}