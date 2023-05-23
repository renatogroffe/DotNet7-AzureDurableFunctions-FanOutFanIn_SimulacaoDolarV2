using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using FunctionAppProcessarMoedas.Models;

namespace FunctionAppProcessarMoedas;

public static class ActivityNotificarAzureQueueStorage
{
    [Function(nameof(ActivityNotificarAzureQueueStorage))]
    public async static Task<bool> NotificarAzureQueueStorage(
        [ActivityTrigger] DadosCotacao dadosCotacao,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(ActivityNotificarAzureQueueStorage));
        logger.LogInformation(
            $"{nameof(ActivityNotificarAzureQueueStorage)} - Iniciando a execucao...");
        var jsonCotacao = JsonSerializer.Serialize(dadosCotacao);

        try
        {
            var client = new QueueClient(
                Environment.GetEnvironmentVariable("AzureStorageNotifications"),
                Environment.GetEnvironmentVariable("QueueAzureStorageNotifications"));
            await client.SendMessageAsync(jsonCotacao);
        }
        catch (Exception ex)
        {
            logger.LogError($"{nameof(ActivityNotificarAzureQueueStorage)} - Erro: " +
                $"{ex.GetType().Name} - {ex.Message}");
            return false;
        }

        logger.LogInformation(
            $"{nameof(ActivityNotificarAzureQueueStorage)} - Dados transmitidos: {jsonCotacao}");
        return true;
    }
}