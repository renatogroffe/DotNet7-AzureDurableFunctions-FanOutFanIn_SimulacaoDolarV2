using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using FunctionAppProcessarMoedas.Models;

namespace FunctionAppProcessarMoedas;

public static class ProcessarSimulacaoDolarTimerStart
{
    [Function("ProcessarSimulacaoDolar_TimerStart")]
    public static async Task TimerStart(
        [DurableClient] DurableTaskClient client,
        [TimerTrigger("*/30 * * * * *")] FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("ProcessarSimulacaoDolarTimerStart");

        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(ProcessarSimulacaoDolar),
            new ParametrosExecucao()
            {
                Moeda = "USD",
                DataReferencia = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}"
            });

        logger.LogInformation($"{nameof(TimerStart)} - Iniciada orquestração com ID = '{instanceId}'.");
    }
}