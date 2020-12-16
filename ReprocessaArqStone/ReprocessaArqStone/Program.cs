using System;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Refit;

namespace ReprocessaArqStone
{
    class Program
    {
        static async Task Main(string[] args)
        {

            Console.WriteLine("Digite a data que deseja iniciar o reprocessamento: (dd/MM/yyyy)");
            string resposta = Console.ReadLine();
            DateTime dataIniResposta = new DateTime();
            while (!DateTime.TryParse(resposta, out dataIniResposta))
            {
                Console.WriteLine("Data invalida, tente novamente");
                dataIniResposta = new DateTime();
                resposta = Console.ReadLine();
            }

            DateTime dataFimResposta = new DateTime();
            Console.WriteLine("Agora digite a data final que deseja encerrar o reprocessamaneto: ");
            resposta = Console.ReadLine();
            while (!DateTime.TryParse(resposta, out dataFimResposta))
            {
                Console.WriteLine("Data invalida, tente novamente");
                dataFimResposta = new DateTime();
                resposta = Console.ReadLine();
            }

            resposta = string.Empty;
            while (resposta.ToUpper() != "S" && resposta.ToUpper() != "N")
            {
                Console.WriteLine("Deseja informar um código de exportação? (S/N)");
                resposta = Console.ReadLine();
            }

            int codExportacaoValido = 0;
            int? codExportacao = null;
            if (resposta.ToUpper() == "S")
            {
                Console.WriteLine("Então informe: ");
                resposta = Console.ReadLine();
                while (!int.TryParse(resposta, out codExportacaoValido))
                {
                    Console.WriteLine("Número ivalido, tente novamente");
                    codExportacaoValido = 0;
                    resposta = Console.ReadLine();
                }
                codExportacao = codExportacaoValido;
            }

            Console.WriteLine("Agora faça login em produção e informe o token do sistema: ");
            var token = Console.ReadLine();

            await ReprocessaArquivos(dataIniResposta, dataFimResposta, codExportacao, token);

            Console.WriteLine("Processo finalizado. [APERTE ENTER PARA FINALIZAR]");
            Console.ReadLine();
        }

        private static async Task ReprocessaArquivos(DateTime dataInicial, DateTime dataFinal, int? codExportacao, string token)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("INICIANDO PROCESSO DE CONCILIAÇÃO");

                if (codExportacao == null)
                {
                    using SqlDataReader listCodExportacao = ParoquiaApp.GetAllCodExportacao();
                    while (listCodExportacao.Read())
                    {
                        ReprocessarArquivosConciliacaoViewModel parmsArquivo = new ReprocessarArquivosConciliacaoViewModel
                        {
                            DataInicial = dataInicial.Date.ToString(),
                            DataFinal = dataFinal.Date.ToString(),
                            CodigoExportacao = Convert.ToInt32(listCodExportacao.GetDecimal(0)),
                            AtualizarConciliacao = true,
                            AtualizarRelatorio = true
                        };

                        await Execute(parmsArquivo, token);
                    }
                }
                else
                {
                    ReprocessarArquivosConciliacaoViewModel parmsArquivo = new ReprocessarArquivosConciliacaoViewModel
                    {
                        DataInicial = dataInicial.Date.ToString(),
                        DataFinal = dataFinal.Date.ToString(),
                        CodigoExportacao = codExportacao.GetValueOrDefault(),
                        AtualizarConciliacao = true,
                        AtualizarRelatorio = true
                    };

                    await Execute(parmsArquivo, token);
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static async Task Execute(ReprocessarArquivosConciliacaoViewModel arq, string token)
        {
            try
            {               
                var client = new HttpClient() { BaseAddress = new Uri("https://eclesial.theos.com.br/eclesialparoquia") };
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var eclesialService = RestService.For<IEclesialParoquiaInterface>(client);

                var resultado = await eclesialService.ReprocessaArquivosStone(arq);


                if (!string.IsNullOrEmpty(resultado.MensagemErro))
                {
                    Console.WriteLine("---------------------------------------------------------------------------------------");
                    Console.WriteLine($"Data de Início...: {arq.DataInicial}");
                    Console.WriteLine($"Data Final.......: {arq.DataFinal}");
                    Console.WriteLine($"Código Exportação: {arq.CodigoExportacao}");
                    Console.WriteLine($"Mensagem.........: {resultado.MensagemErro}");
                    Console.WriteLine("---------------------------------------------------------------------------------------");
                }
                else
                if (!string.IsNullOrEmpty(resultado.MensagemSucesso))
                {
                    Console.WriteLine("---------------------------------------------------------------------------------------");
                    Console.WriteLine($"Data de Início...: {arq.DataInicial}");
                    Console.WriteLine($"Data Final.......: {arq.DataFinal}");
                    Console.WriteLine($"Código Exportação: {arq.CodigoExportacao}");
                    Console.WriteLine($"Mensagem.........: {resultado.MensagemSucesso}");
                    Console.WriteLine("---------------------------------------------------------------------------------------");
                }                
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("---------------------------------------------------------------------------------------");
                Console.WriteLine($"Data de Início...: {arq.DataInicial}");
                Console.WriteLine($"Data Final.......: {arq.DataFinal}");
                Console.WriteLine($"Código Exportação: {arq.CodigoExportacao}");
                Console.WriteLine($"Mensagem.........: {ex.Message}");
                Console.WriteLine("---------------------------------------------------------------------------------------");
            }
        }
    }
}
