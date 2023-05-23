using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using FunctionAppProcessarMoedas.Models;

namespace FunctionAppProcessarMoedas;

public static class ActivityNotificarAzureQueueStorageB
{
    [Function(nameof(ActivityNotificarAzureQueueStorageB))]
    public async static Task<bool> NotificarAzureQueueStorageB(
        [ActivityTrigger] DadosCotacao dadosCotacao,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(ActivityNotificarAzureQueueStorageB));
        logger.LogInformation(
            $"{nameof(ActivityNotificarAzureQueueStorageB)} - Iniciando a execucao...");
        var txtCotacao =
                $"{dadosCotacao.Sigla}|{dadosCotacao.Horario:yyyMMdd-HHmmss}|" +
                  JsonSerializer.Serialize(dadosCotacao.Valor);
        try
        {
            var client = new QueueClient(
                Environment.GetEnvironmentVariable("AzureStorageNotificationsB"),
                Environment.GetEnvironmentVariable("QueueAzureStorageNotificationsB"));
            await client.SendMessageAsync(txtCotacao);
        }
        catch (Exception ex)
        {
            logger.LogError($"{nameof(ActivityNotificarAzureQueueStorageB)} - Erro: " +
                $"{ex.GetType().Name} - {ex.Message}");
            return false;
        }

        logger.LogInformation(
            $"{nameof(ActivityNotificarAzureQueueStorageB)} - Dados transmitidos: {txtCotacao}");
        return true;
    }
}