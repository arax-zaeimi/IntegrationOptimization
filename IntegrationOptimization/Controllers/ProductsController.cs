using IntegrationOptimization.Commands;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationOptimization.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProcessProdcutsUseCase processProductsUseCase, ILogger<ProductsController> logger) : ControllerBase
{
    [HttpPost("nonoptimized-process")]
    public IActionResult ProcessProductsNonOptimized()
    {
        try
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await processProductsUseCase.ProcessNonOptimizedAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in background task during non-optimized processing");
                }
            });

            return Ok(new { Message = "Product processing started successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "An error occurred while starting product processing", Details = ex.Message });
        }
    }

    [HttpPost("optimized-process")]
    public IActionResult ProcessProductsOptimized()
    {
        try
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await processProductsUseCase.ProcessOptimizedAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in background task during optimized processing");
                }
            });

            return Ok(new { Message = "Product processing started successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "An error occurred while starting product processing", Details = ex.Message });
        }
    }

    [HttpPost("superoptimized-process")]
    public IActionResult ProcessProductsSuperOptimized()
    {
        try
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await processProductsUseCase.ProcessSuperOptimizedAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in background task during super-optimized processing");
                }
            });
            return Ok(new { Message = "Product processing started successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "An error occurred while starting product processing", Details = ex.Message });
        }
    }
}