using System.Globalization;

namespace Questao1
{
    class ContaBancaria
    {
        private int _numero;
        private double _saldo;
        private string _titular;
        private double taxa = 3.50;

        public ContaBancaria(int numero, string titular, double depositoInicial)
        {
            _numero = numero;
            _titular = titular;
            _saldo = depositoInicial;
        }
        public ContaBancaria(int numero, string titular)
        {
            _numero = numero;
            _titular = titular;
            _saldo = 0;
        }

        public void Deposito(double quantia)
        {
            _saldo += quantia;
        }

        public void Saque(double quantia)
        {
            _saldo -= (quantia + taxa);
        }

        public int Numero
        {
            get { return _numero; }
        }

        public string Titular
        {
            get { return _titular; }
            set { _titular = value; }
        }

        public double Saldo
        {
            get { return _saldo; }
        }

    }
}
