using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Applications.Autenticacao;
using API.Domains;
using API.DTOs.AutenticacaoDto;
using API.Exceptions;
using API.Interfaces;

namespace API.Applications.Services
{
    public class AutenticacaoService
    {
        private readonly IUsuarioRepository _repository;
        private readonly GeradorTokenJwt _tokenJwt;
        private readonly GeradorTokenRecuperacaoSenha _tokenSenha;
        private readonly ITokenRedefinicaoSenhaRepository _tokenRepository;
        private readonly IEmailService _emailService;

        public AutenticacaoService(
            IUsuarioRepository repository,
            GeradorTokenJwt tokenJwt,
            ITokenRedefinicaoSenhaRepository tokenRepository,
            GeradorTokenRecuperacaoSenha tokenSenha,
            IEmailService emailService)
        {
            _repository = repository;
            _tokenJwt = tokenJwt;
            _tokenRepository = tokenRepository;
            _tokenSenha = tokenSenha;
            _emailService = emailService;
        }

        private static bool VerificarSenha(
            string senhaDigitada,
            byte[] senhaHashBanco)
        {
            using var sha = SHA256.Create();

            byte[] hashDigitado = sha.ComputeHash(
                Encoding.UTF8.GetBytes(senhaDigitada));

            return hashDigitado.SequenceEqual(senhaHashBanco);
        }

        public async Task<TokenDto> Login(LoginDto loginDto)
        {
            Usuario? usuario = await _repository.BuscarEmail(loginDto.Email);

            if (usuario == null ||
                !usuario.Ativo ||
                usuario.Senha == null ||
                !VerificarSenha(loginDto.Senha, usuario.Senha))
            {
                throw new DomainException("E-mail ou senha inválidos.");
            }

            string token = _tokenJwt.GerarToken(usuario);

            return new TokenDto
            {
                Token = token
            };
        }

        public async Task<bool> SolicitarRedefinicaoSenhaAsync(
            SolicitarRedefinicaoSenhaDto dto)
        {
            Usuario? usuario = await _repository.BuscarEmail(dto.Email);

            if (usuario == null)
            {
                return false;
            }

            string token = _tokenSenha.GerarToken();
            string tokenHash = _tokenSenha.GerarHashToken(token);

            var tokenRedefinicao = new TokenRedefinicaoSenha
            {
                UsuarioId = usuario.UsuarioId,
                TokenHash = tokenHash,
                ExpiraEm = DateTime.UtcNow.AddMinutes(60),
                Utilizado = false
            };

            await _tokenRepository.CadastrarAsync(tokenRedefinicao);

            await _emailService.EnviarEmailRedefinicaoSenhaAsync(
                usuario.EmailPessoal,
                token);

            return true;
        }

        public async Task<bool> RedefinirSenhaAsync(
            RedefinirSenhaDto dto)
        {
            Usuario? usuario = await _repository.BuscarEmail(dto.Email);

            if (usuario == null || string.IsNullOrWhiteSpace(dto.NovaSenha))
            {
                return false;
            }

            string tokenHash = _tokenSenha.GerarHashToken(dto.Token);

            TokenRedefinicaoSenha? token =
                await _tokenRepository.BuscarPorHashAsync(tokenHash);

            if (token == null ||
                token.UsuarioId != usuario.UsuarioId ||
                token.Utilizado ||
                token.ExpiraEm <= DateTime.UtcNow)
            {
                return false;
            }

            byte[] senhaHash =
                CriptografiaUsuario.CriptografarSenha(dto.NovaSenha);

            bool senhaAtualizada = await _repository.AtualizarSenha(
                usuario.UsuarioId,
                senhaHash);

            if (!senhaAtualizada)
            {
                return false;
            }

            token.Utilizado = true;
            await _tokenRepository.AtualizarAsync(token);

            return true;
        }
    }
}