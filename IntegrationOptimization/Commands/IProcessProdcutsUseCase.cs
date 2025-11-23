
namespace IntegrationOptimization.Commands
{
    public interface IProcessProdcutsUseCase
    {
        Task ProcessNonOptimizedAsync();
        Task ProcessOptimizedAsync();
    }
}