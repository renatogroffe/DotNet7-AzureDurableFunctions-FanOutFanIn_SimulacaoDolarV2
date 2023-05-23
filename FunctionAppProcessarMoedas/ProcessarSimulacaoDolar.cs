using FunctionAppProcessarMoedas.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace FunctionAppProcessarMoedas;

public static class ProcessarSimulacaoDolar
{
    [Function(nameof(ProcessarSimulacaoDolar))]
    public static async Task RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var logger = context.CreateReplaySafeLogger(nameof(ProcessarSimulacaoDolar));
        logger.LogInformation(
            $"{nameof(ProcessarSimulacaoDolar)} - Iniciando a execucao do metodo {nameof(RunOrchestrator)}...");

        var parametrosExecucao = context.GetInput<ParametrosExecucao>();

        var dadosCotacao = await context.CallActivityAsync<DadosCotacao>(
            nameof(ActivitySimularCotacaoDolar), parametrosExecucao);
        var tasks = new Task<bool>[3];
        tasks[0] = context.CallActivityAsync<bool>(
            nameof(ActivityNotificarAzureQueueStorage), dadosCotacao);
        tasks[1] = context.CallActivityAsync<bool>(
            nameof(ActivityNotificarAzureServiceBus), dadosCotacao);
        tasks[2] = context.CallActivityAsync<bool>(
            nameof(ActivityNotificarAzureQueueStorageB), dadosCotacao);
        await Task.WhenAll(tasks);

        logger.LogInformation(
            $"{nameof(ProcessarSimulacaoDolar)} - Concluindo a execucao do metodo {nameof(RunOrchestrator)}.");
    }
}