using Telegram.Bot.Types;
using TelegramBotPGFRC;
using static System.Net.Mime.MediaTypeNames;

TelegramBotClient botClient = new TelegramBotClient("5975891448:AAGw74g78o5PoWeBYrweW4pnvhnbJdKmgps");

var metBot = new BotEngine(botClient);


await metBot.ListenForMessagesAsync();


public class BotEngine
{


    //Messages and user info


    private readonly TelegramBotClient _botClient;

    public BotEngine(TelegramBotClient botClient)
    {
        _botClient = botClient;
    }
    public async Task ListenForMessagesAsync()
    {
        using var cts = new CancellationTokenSource();


        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
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

    private Dictionary<long, PatientsAntropometrics> PatientsAntropometrics = new Dictionary<long, PatientsAntropometrics>();

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {

        switch (update.Type)
        {
            case UpdateType.Message:
                await ProcessMessageUpdateAsync(update, botClient, cancellationToken);
                break;

            case UpdateType.CallbackQuery:
                await ProcessCallbackQueryAsync(update, botClient, cancellationToken);
                break;

            case UpdateType.Unknown:
                Console.WriteLine("сработал Unknown");
                break;
        }

        async Task ProcessMessageUpdateAsync(Update update, ITelegramBotClient botClient, CancellationToken cancellationToken)
        {
            if (update.Message?.Text == "/start")
            {
                await SendStartMenu(update.Message.Chat.Id, botClient, cancellationToken);
            }
            else
            {
                string userText = update.Message.Text;
                Console.WriteLine($"Пользователь ввел: {userText}");
            }
        }

        async Task ProcessCallbackQueryAsync(Update update, ITelegramBotClient botClient, CancellationToken cancellationToken)
        {
            if (update.CallbackQuery?.Data == "Рост")
            {
                await botClient.SendTextMessageAsync(
                    chatId: update.CallbackQuery.Message.Chat.Id,
                    replyMarkup: new ForceReplyMarkup(),
                    text: "Введите Рост",
                    cancellationToken: cancellationToken
                );

                bool statement = await GetUserInputAsync(update, botClient, cancellationToken, "Пользователь ввел рост:", height => PatientsAntropometrics[update.CallbackQuery.Message.Chat.Id] = new PatientsAntropometrics { Height = height });

                if (statement)
                {
                    await SendActionKeyboardAsync(update, botClient, cancellationToken, "Креатинин");
                }
            }
            else if (update.CallbackQuery?.Data == "Креатинин")
            {
                await botClient.SendTextMessageAsync(
                    chatId: update.CallbackQuery.Message.Chat.Id,
                    replyMarkup: new ForceReplyMarkup(),
                    text: "Введите концентрацию креатинина",
                    cancellationToken: cancellationToken
                );

                bool statement = await GetUserInputAsync(update, botClient, cancellationToken, "Пользователь ввел концентрацию креатинина:", screatinine => PatientsAntropometrics[update.CallbackQuery.Message.Chat.Id].SCreatinine = screatinine);

                if (statement)
                {
                    await SendActionKeyboardAsync(update, botClient, cancellationToken, "Цистатин С");
                }
            }
            else if (update.CallbackQuery?.Data == "Цистатин С")
            {
                await botClient.SendTextMessageAsync(
                    chatId: update.CallbackQuery.Message.Chat.Id,
                    replyMarkup: new ForceReplyMarkup(),
                    text: "Введите концентрацию Цистатина С",
                    cancellationToken: cancellationToken
                );

                bool statement = await GetUserInputAsync(update, botClient, cancellationToken, "Пользователь ввел концентрацию Цистатина С:", sCystatinC => PatientsAntropometrics[update.CallbackQuery.Message.Chat.Id].SCystatinC = sCystatinC);

                if (statement)
                {
                    await SendActionKeyboardAsync(update, botClient, cancellationToken, "Рассёт");
                }
            }
            else if (update.CallbackQuery?.Data == "Пропустить")
            {
                await SendActionKeyboardAsync(update, botClient, cancellationToken, "Рассёт");
            }
            else if (update.CallbackQuery?.Data == "Рассёт")
            {
                PatientsAntropometrics userAntropometrics = PatientsAntropometrics[update.CallbackQuery.Message.Chat.Id];
                var creatinineEstimator = new CreatinineBedsideSchwartz();
                decimal creatinineEstimation = creatinineEstimator.Calculate(userAntropometrics);

                await botClient.SendTextMessageAsync(
                    chatId: update.CallbackQuery.Message.Chat.Id,
                    text: $"Рост: {userAntropometrics.Height}\nКреатинин: {userAntropometrics.SCreatinine}\nЦистатин С: {userAntropometrics.SCystatinC}\nОценка креатинина: {creatinineEstimation}",
                    cancellationToken: cancellationToken
                );
            }
        }

        async Task<bool> GetUserInputAsync(Update update, ITelegramBotClient botClient, CancellationToken cancellationToken, string promptMessage, Action<decimal> saveValueAction)
        {
            bool statement = false;
            int maxAttempts = 3;
            int attempts = 0;

            while (!statement && attempts < maxAttempts)
            {
                var updates = await botClient.GetUpdatesAsync(
                    offset: update.Id + 1,
                    allowedUpdates: new List<UpdateType> { UpdateType.Message },
                    cancellationToken: cancellationToken
                );

                foreach (var newUpdate in updates)
                {
                    if (newUpdate.Message?.Chat.Id == update.CallbackQuery.Message.Chat.Id && newUpdate.Message?.Text != null)
                    {
                        Console.WriteLine($"{promptMessage} {newUpdate.Message.Text}");

                        if (decimal.TryParse(newUpdate.Message.Text, out decimal value))
                        {
                            saveValueAction(value);
                            statement = true;
                            break;
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(chatId: newUpdate.Message?.Chat.Id, text: "Вы ввели некорректные данные", cancellationToken: cancellationToken);
                            attempts++;
                            await Task.Delay(10000);
                        }
                    }

                    await Task.Delay(3000); // Pause to avoid overloading the processor
                    Console.WriteLine(" Жду ввода...");
                }
            }

            return statement;
        }

        async Task SendActionKeyboardAsync(Update update, ITelegramBotClient botClient, CancellationToken cancellationToken, string action)
        {
            var keyboard = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData(action) } });

            await botClient.SendTextMessageAsync(
                chatId: update.CallbackQuery.Message.Chat.Id,
                text: "Выберите действие:",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken
            );
        }

        if (update.Message is not { } message)
        {
            Console.WriteLine("Null сработал");
            return;
        }
        if (message.Text is not { } messageText)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Вы или тут", cancellationToken: cancellationToken);
            return;
        }
        
        
        Console.WriteLine($"Received a '{messageText}' message in chat {message.Chat.Id} with Id№ '{message.MessageId}' ");


    }

   

    private async Task SendStartMenu(long chatId, ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
     new[]
     {
         InlineKeyboardButton.WithCallbackData("Рост"),

     }
 });

        await botClient.SendTextMessageAsync(
           chatId: chatId, replyMarkup: keyboard,
           text: "Выберите действие:",
            cancellationToken: cancellationToken
        );
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
}



