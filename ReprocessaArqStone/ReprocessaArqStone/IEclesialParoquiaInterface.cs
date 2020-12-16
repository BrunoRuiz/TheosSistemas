using System.Threading.Tasks;
using Refit;

namespace ReprocessaArqStone
{
    public interface IEclesialParoquiaInterface
    {
        [Post("/api/v1/stoneconciliacao")]       
        Task<EclesialReprocessaReponse> ReprocessaArquivosStone([Body]ReprocessarArquivosConciliacaoViewModel parms);        
    }
}
