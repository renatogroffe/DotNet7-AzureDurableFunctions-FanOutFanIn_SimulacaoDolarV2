using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using FunctionAppProcessarMoedas.Models;

namespace FunctionAppProcessarMoedas;

public static class ActivityNotificarAzureServiceBus
{
    [Function(nameof(ActivityNotificarAzureServiceBus))]
    public async static Task<bool> NotificarAzureServiceBus(
        [ActivityTrigger] DadosCotacao dadosCotacao,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(ActivityNotificarAzureServiceBus));
        logger.LogInformation(
            $"{nameof(ActivityNotificarAzureServiceBus)} - Iniciando a execucao...");
        var jsonCotacao = JsonSerializer.Serialize(dadosCotacao);

        try
        {
            var clientOptions = new ServiceBusClientOptions() { TransportType = ServiceBusTransportType.AmqpWebSockets };
            var client = new ServiceBusClient(
                connectionString: Environment.GetEnvironmentVariable("AzureServiceBusNotifications"),
                options: clientOptions);
            var sender = client.CreateSender(
                Environment.GetEnvironmentVariable("QueueAzureServiceBusNotifications"));

            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
            messageBatch.TryAddMessage(new ServiceBusMessage(jsonCotacao));
            await sender.SendMessagesAsync(messageBatch);
        }
        catch (Exception ex)
        {
            logger.LogError($"{nameof(ActivityNotificarAzureServiceBus)} - Erro: " +
                $"{ex.GetType().Name} - {ex.Message}");
            return false;
        }

        logger.LogInformation(
            $"{nameof(ActivityNotificarAzureServiceBus)} - Dados transmitidos: {jsonCotacao}");
        return true;
    }
}