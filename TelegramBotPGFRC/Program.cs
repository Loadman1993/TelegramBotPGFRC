

var botClient = new TelegramBotClient("5975891448:AAFuxHEN7gcvByfsQr2xkWKTO5SEwvIMVew");

var metBot = new BotEngine(botClient);

// Listen for messages sent to the bot
await metBot.ListenForMessagesAsync();


public class BotEngine
{
          
        private readonly TelegramBotClient _botClient;

        public BotEngine(TelegramBotClient botClient)
        {
            _botClient = botClient;
        }
    public async Task ListenForMessagesAsync()
    {
        using var cts = new CancellationTokenSource();
       
        _botClient.OnCallbackQuery += async (sender, callbackQuery) =>
        {
            await HandleCallbackQueryAsync(_botClient, callbackQuery, cts.Token);
        };
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
        };
        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
            );

        var me = await _botClient.GetMeAsync();

        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();
    }
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
      
        if (update.Message is not { } message)
        {
            return;
        }

        // Only process text messages
        if (message.Text is not { } messageText)
        {
            return;
        }
        if (message.Text == "/start")
        {
            await SendStartMenu(message.Chat.Id);
        }

        Console.WriteLine($"Received a '{messageText}' message in chat {message.Chat.Id}.");
    }
    private async Task SendStartMenu(long chatId)
    {
        // Создаем клавиатуру с кнопками
        var keyboard = new InlineKeyboardMarkup(new[]
        {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Кнопка 1"),
            InlineKeyboardButton.WithCallbackData("Кнопка 2")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Кнопка 3")
        }
    });

        // Отправляем сообщение с клавиатурой
        await _botClient.SendTextMessageAsync(chatId, "Выберите действие:", replyMarkup: keyboard);
    }
    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
    private async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        // Получаем данные о нажатой кнопке
        var data = callbackQuery.Data;

        // Обрабатываем действие в зависимости от данных о нажатой кнопке
        if (data == "Кнопка 1")
        {
            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Вы выбрали Кнопку 1");
        }
        else if (data == "Кнопка 2")
        {
            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Вы выбрали Кнопку 2");
        }
        else if (data == "Кнопка 3")
        {
            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Вы выбрали Кнопку 3");
        }
    }

}