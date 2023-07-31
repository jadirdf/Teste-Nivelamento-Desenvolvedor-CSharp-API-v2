using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Mvc;
using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Services.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContaCorrenteController : ControllerBase
    {
        private static SqliteConnection _connection;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ContaCorrenteController> _logger;

        public ContaCorrenteController(ILogger<ContaCorrenteController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            CreateConnectionSQLite();
        }

        [HttpGet(Name = "GeMovimentacao")]
        public string Get(string idRequisicao, int numeroContaCorrente, double vlrMovimento, string tipoMovimento)
        {
            try
            {
                var contaCorrente = ValidarDados(idRequisicao, numeroContaCorrente, vlrMovimento, tipoMovimento);
                var movimento = PersistirDados(idRequisicao, contaCorrente.IdContaCorrente, numeroContaCorrente, vlrMovimento, tipoMovimento);

                Response.StatusCode = 200;
                var msgSucesso = string.Format("Id do movimento gerado: {0}", movimento.IdMovimento);
                return msgSucesso;
            }
            catch (Exception e)
            {
                Response.StatusCode = 400;
                return e.Message;
            }
        }

        private void CreateConnectionSQLite()
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var connectionString = _configuration["DatabaseName"];

            _connection = new SqliteConnection(connectionString);
        }

        private ContaCorrente ValidarDados(string idRequisicao, int numeroContaCorrente, double vlrMovimento, string tipoMovimento)
        {
            var contaCorrente = _connection.Query<ContaCorrente>("SELECT * FROM CONTACORRENTE")
                .Where(x => x.Numero.Equals(numeroContaCorrente))
                .FirstOrDefault();

            if (contaCorrente == null)
                throw new BadHttpRequestException("Número da Conta Corrente inválido. TIPO: INVALID_ACCOUNT");

            if (!contaCorrente.Ativo)
                throw new BadHttpRequestException("Conta Corrente não está ativa. TIPO: INACTIVE_ACCOUNT");

            if (vlrMovimento <= 0)
                throw new BadHttpRequestException("O valor informado não é válido. TIPO: INVALID_VALUE");

            var tiposMovimento = contaCorrente.GetTipoMovimento();

            if (!tiposMovimento.Contains(tipoMovimento.ToUpper()))
                throw new BadHttpRequestException("Tipo de movimentação inválido. TIPO: INVALID_TYPE");

            return contaCorrente;
        }

        private Movimento PersistirDados(string idRequisicao, string idContaCorrente, int numeroContaCorrente, double vlrMovimento, string tipoMovimento)
        {
            // Geração do IdMovimento
            Guid guid = Guid.NewGuid();
            var idMovimento = Guid.NewGuid().ToString().ToUpper();

            // Motagem do SQL de insersão
            var sql = string.Format("INSERT INTO MOVIMENTO (IDMOVIMENTO,IDCONTACORRENTE,DATAMOVIMENTO,TIPOMOVIMENTO,VALOR) VALUES ('{0}','{1}','{2}','{3}',{4})", idMovimento, idContaCorrente, DateTime.Now.Date.ToString("dd/MM/yyyy"), tipoMovimento.ToUpper(), vlrMovimento.ToString().Replace(",", "."));

            // Insersão dos dados
            _connection.Query<Movimento>(sql);

            // Consulta do movimento que acabou de ser inserido
            var sqlSelect = String.Format("SELECT * FROM MOVIMENTO WHERE IDMOVIMENTO = '{0}'", idMovimento);
            var movimento = _connection.Query<Movimento>(sqlSelect).FirstOrDefault();

            return movimento;
        }

    }
}
