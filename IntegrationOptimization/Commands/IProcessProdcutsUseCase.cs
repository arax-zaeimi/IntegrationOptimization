
namespace IntegrationOptimization.Commands
{
    public interface IProcessProdcutsUseCase
    {
        Task ProcessInvoice();
        Task ProcessNonOptimizedAsync();
        Task ProcessOptimizedAsync();
        Task ProcessSuperOptimizedAsync();
    }
}