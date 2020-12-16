using System;
namespace ReprocessaArqStone
{
    public class ReprocessarArquivosConciliacaoViewModel
    {
        public string DataInicial { get; set; }        
        public string DataFinal { get; set; }
        public int CodigoExportacao { get; set; }
        public bool AtualizarRelatorio { get; set; }
        public bool AtualizarConciliacao { get; set; }
    }
}
