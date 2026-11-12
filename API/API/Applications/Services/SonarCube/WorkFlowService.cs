using API.DTOs.SonarQubeDto;
using API.DTOs.SonarQubeDto.WorkFlowDto;
using API.Exceptions;

namespace API.Applications.Services.SonarCube;

// Gera o .yml do GitHub Actions para cada tipo de projeto, no estilo das telas do SonarQube.
public static class ModeloWorkflowSonar
{
    // Confira a versão mais recente em https://github.com/marketplace/actions/official-sonarqube-scan
    private const string VersaoScanAction = "v5";

    public static readonly IReadOnlyList<TipoWorkFlowDto> Tipos = new List<TipoWorkFlowDto>
    {
        new() { Valor = "maven",  Nome = "Maven" },
        new() { Valor = "gradle", Nome = "Gradle" },
        new() { Valor = "js",     Nome = "JS/TS & Web" },
        new() { Valor = "dotnet", Nome = ".NET" },
        new() { Valor = "python", Nome = "Python" },
        new() { Valor = "rust",   Nome = "Rust/Cargo" },
        new() { Valor = "outro",  Nome = "Other (Go, PHP, ...)" }
    };

    // "tipo" precisa ser um dos valores de Tipos; "branch" precisa já estar validada por quem chama.
    public static string Gerar(string tipo, string chaveProjeto, string branch)
    {
        var corpo = tipo switch
        {
            "dotnet" => CorpoDotnet,
            "maven" => CorpoMaven,
            "js" or "python" or "rust" or "outro" => CorpoScanAction,
            _ => throw new DomainException("Tipo de projeto inválido")
        };

        return (Cabecalho + "\n\n" + corpo + "\n")
            .Replace("\r\n", "\n")
            .Replace("__CHAVE__", chaveProjeto)
            .Replace("__BRANCH__", branch)
            .Replace("__SCAN__", VersaoScanAction);
    }

    private const string Cabecalho = """
        name: Análise SonarQube

        on:
          push:
            branches:
              - __BRANCH__
          pull_request:
            types: [opened, synchronize, reopened]
        """;

    private const string CorpoDotnet = """
        jobs:
          analise:
            name: Build e análise
            runs-on: windows-latest
            steps:
              - name: Configurar JDK 17
                uses: actions/setup-java@v4
                with:
                  java-version: 17
                  distribution: zulu

              - uses: actions/checkout@v4
                with:
                  fetch-depth: 0

              # Descomente e ajuste se o projeto exigir uma versão específica do .NET:
              # - uses: actions/setup-dotnet@v4
              #   with:
              #     dotnet-version: 10.0.x

              - name: Instalar o scanner do SonarQube
                shell: powershell
                run: dotnet tool install --global dotnet-sonarscanner

              - name: Build e análise
                env:
                  SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
                  SONAR_HOST_URL: ${{ secrets.SONAR_HOST_URL }}
                shell: powershell
                run: |
                  dotnet-sonarscanner begin /k:"__CHAVE__" /d:sonar.token="$env:SONAR_TOKEN" /d:sonar.host.url="$env:SONAR_HOST_URL"
                  dotnet build
                  dotnet-sonarscanner end /d:sonar.token="$env:SONAR_TOKEN"
        """;

    private const string CorpoMaven = """
        name: Build

        on:
          push:
            branches:
              - main


        jobs:
          build:
            name: Build and analyze
            runs-on: windows-latest

            steps:
              - uses: actions/checkout@34e114876b0b11c390a56381ad16ebd13914f8d5 # v4.3.1
                with:
                  fetch-depth: 0  # Shallow clones should be disabled for a better relevancy of analysis
              - name: Set up JDK 21
                uses: actions/setup-java@c1e323688fd81a25caa38c78aa6df2d33d3e20d9 # v4.8.0
                with:
                  java-version: 21
                  distribution: 'zulu' # Alternative distribution options are available.
              - name: Cache SonarQube packages
                uses: actions/cache@0057852bfaa89a56745cba8c7296529d2fc39830 # v4.3.0
                with:
                  path: ~/.sonar/cache
                  key: ${{ runner.os }}-sonar
                  restore-keys: ${{ runner.os }}-sonar
              - name: Cache Maven packages
                uses: actions/cache@0057852bfaa89a56745cba8c7296529d2fc39830 # v4.3.0
                with:
                  path: ~/.m2
                  key: ${{ runner.os }}-m2-${{ hashFiles('**/pom.xml') }}
                  restore-keys: ${{ runner.os }}-m2
              - name: Build and analyze
                env:
                  SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
                  SONAR_HOST_URL: ${{ secrets.SONAR_HOST_URL }}
                run: mvn -B verify org.sonarsource.scanner.maven:sonar-maven-plugin:sonar -Dsonar.projectKey=T__CHAVE__ -Dsonar.projectName='__PROJETO__'
        """;


    // Usado por JS/TS & Web, Python, Rust/Cargo e Other (Go, PHP, ...)
    private const string CorpoScanAction = """
        jobs:
          analise:
            name: Análise
            runs-on: windows-latest
            steps:
              - uses: actions/checkout@v4
                with:
                  fetch-depth: 0

              - name: Análise SonarQube
                uses: SonarSource/sonarqube-scan-action@__SCAN__
                env:
                  SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
                  SONAR_HOST_URL: ${{ secrets.SONAR_HOST_URL }}
                with:
                  args: >
                    -Dsonar.projectKey=__CHAVE__
        """;
}