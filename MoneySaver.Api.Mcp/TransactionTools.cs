using ModelContextProtocol.Server;
using MoneySaver.Api.Mcp.Models;
using System.ComponentModel;
using System.Text.Json;

namespace MoneySaver.Api.Mcp
{
    [McpServerToolType]
    public static class TransactionTools
    {
        [McpServerTool, Description("" +
                "Create a new transaction record. " +
                "Mandatory properties which needs to be received are DateTime (format yyyy-MM-dd), Amount, Category name." +
            "Not mandatory is Additional note" +
                "This will return a JSON format string as a result"
                )]
        public static async Task<string> AddTransaction(
            TransactionService transactionService,
            [Description("The category name which is mandatory")] string categoryName,
            [Description("Spent amount which is mandatory")] double amount,
            [Description("Datetime when the transaction was made (﻿format yyyy-MM-dd)")] string dateTime,
            [Description("Addtional note added")] string additionalNote
            )
        {

            var res = await transactionService.CreateTransaction(
                    categoryName,
                    dateTime,
                    amount,
                    additionalNote
                );

            if (res != null) {
                return JsonSerializer.Serialize(res);
            }

            return "Fail to create transaction.";
        }
    }
}
