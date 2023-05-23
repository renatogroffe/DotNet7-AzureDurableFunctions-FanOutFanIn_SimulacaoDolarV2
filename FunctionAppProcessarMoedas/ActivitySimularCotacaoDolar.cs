using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using FunctionAppProcessarMoedas.Models;

namespace FunctionAppProcessarMoedas;

public static class ActivitySimularCotacaoDolar
{
    private const decimal VALOR_BASE = 5.09m;

    [Function(nameof(ActivitySimularCotacaoDolar))]
    public static DadosCotacao SimularCotacaoDolar([ActivityTrigger] ParametrosExecucao parametrosExecucao,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger(nameof(ActivitySimularCotacaoDolar));
        logger.LogInformation(
            $"{nameof(ActivitySimularCotacaoDolar)} - Iniciando a execucao...");
        var cotacao = new DadosCotacao()
        {
            Sigla = parametrosExecucao.Moeda,
            Horario = parametrosExecucao.DataReferencia,
            Valor = Math.Round(VALOR_BASE + new Random().Next(0, 21) / 1000m, 3)
        };
        logger.LogInformation(
            $"{nameof(ActivitySimularCotacaoDolar)} - Dados gerados: " +
            JsonSerializer.Serialize(cotacao));
        return cotacao;
    }
}
