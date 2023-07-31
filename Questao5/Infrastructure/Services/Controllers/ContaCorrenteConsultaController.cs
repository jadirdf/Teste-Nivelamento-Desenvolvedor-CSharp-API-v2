using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Mvc;
using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Services.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContaCorrenteConsultaController : ControllerBase
    {
        private static SqliteConnection _connection;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ContaCorrenteConsultaController> _logger;

        public ContaCorrenteConsultaController(ILogger<ContaCorrenteConsultaController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            CreateConnectionSQLite();
        }

        [HttpGet(Name = "GetConsulta")]
        public string Get(int numeroContaCorrente)
        {
            try
            {
                var contaCorrente = ValidarDados(numeroContaCorrente);

                var items = ConsultaDados(contaCorrente);

                var vlrCredito = 0.00;
                var vlrDebito = 0.00;

                if (items.Count() > 0)
                {
                    var itemCredito = items.Where(x => x.TipoMovimento.Equals("C")).FirstOrDefault();
                    if (itemCredito != null)
                        vlrCredito = itemCredito.Valor;

                    var itemDebito = items.Where(x => x.TipoMovimento.Equals("D")).FirstOrDefault();
                    if (itemDebito != null)
                        vlrDebito = itemDebito.Valor;

                }

                var vlrSaldo = (vlrCredito - vlrDebito);

                Response.StatusCode = 200;

                var msgRetorno = "Número da conta corrente: {0} - Nome do titular da conta corrente: {1} - Data e hora da consulta: {2} - Valor do saldo atual: {3}";
                var msgSucesso = string.Format(msgRetorno, contaCorrente.Numero, contaCorrente.Nome, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), vlrSaldo.ToString("N2"));

                return msgSucesso;
            }
            catch (Exception e)
            {
                Response.StatusCode = 400;
                return e.Message;
            }
        }

        private ContaCorrente ValidarDados(int numeroContaCorrente)
        {
            var contaCorrente = _connection.Query<ContaCorrente>("SELECT * FROM CONTACORRENTE")
                .Where(x => x.Numero.Equals(numeroContaCorrente))
                .FirstOrDefault();

            if (contaCorrente == null)
                throw new BadHttpRequestException("Número da Conta Corrente inválido. TIPO: INVALID_ACCOUNT");

            if (!contaCorrente.Ativo)
                throw new BadHttpRequestException("Conta Corrente não está ativa. TIPO: INACTIVE_ACCOUNT");

            return contaCorrente;
        }

        private List<Movimento> ConsultaDados(ContaCorrente contaCorrente)
        {
            var sqlList = new List<string>();
            sqlList.Add("SELECT IDCONTACORRENTE, TIPOMOVIMENTO, SUM(VALOR) AS VALOR");
            sqlList.Add("FROM MOVIMENTO");
            sqlList.Add("WHERE IDCONTACORRENTE = '{0}'");
            sqlList.Add("GROUP BY IDCONTACORRENTE, TIPOMOVIMENTO");
            sqlList.Add("ORDER BY TIPOMOVIMENTO");

            var sqlConsulta = string.Format(String.Join(" ", sqlList.ToArray()), contaCorrente.IdContaCorrente);

            var items = _connection.Query<Movimento>(sqlConsulta)
                .ToList();

            return items;
        }

        private void CreateConnectionSQLite()
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var connectionString = _configuration["DatabaseName"];

            _connection = new SqliteConnection(connectionString);
        }


    }
}