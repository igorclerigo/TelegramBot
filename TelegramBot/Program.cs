using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;

class Program
{
    private static readonly string BotToken = "7416280107:AAFSIX8a3T483eG80gr8WytJCvmCCEXa96g";
    private static TelegramBotClient botClient;

    private static readonly Dictionary<int, string> Perguntas = new Dictionary<int, string>
{
    { 1, "Horários de funcionamento" },
    { 2, "Cardápio de comidas futuristas" },
    { 3, "Localização do restaurante" },
    { 4, "Formas de pagamento" },
    { 5, "Política de entregas com drones" },
    { 6, "Reservas para jantares espaciais" },
    { 7, "Acompanhamento do pedido" },
    { 8, "Benefícios do programa de fidelidade galáctico" },
    { 9, "Frete para Marte e outros planetas" },
    { 10, "Assinar o boletim de novidades cósmicas" }
};


    private static readonly Dictionary<string, string> Respostas = new Dictionary<string, string>
{
    { "horários de funcionamento", "Nosso restaurante está aberto todos os dias, 24 horas por dia, com robôs prontos para atendê-lo a qualquer momento!" },
    { "cardápio de comidas futuristas", "Nosso cardápio inclui pratos como 'Espaguete Intergaláctico', 'Hambúrguer de Nanotecnologia' e 'Salada de Algas Alienígenas'. Confira o menu completo no nosso site futurista!" },
    { "localização do restaurante", "Estamos localizados na base orbital, Plataforma Alpha-7, mas você pode fazer um tour virtual no nosso site." },
    { "formas de pagamento", "Aceitamos pagamentos com criptomoedas, créditos galácticos e transferência via NeuralNet. Para outras opções, consulte nosso portal de pagamentos." },
    { "política de entregas com drones", "Nossos drones autônomos entregam sua refeição onde quer que você esteja, incluindo naves espaciais e estações orbitais. Frete grátis para distâncias até 10.000 km!" },
    { "reservas para jantares espaciais", "Para reservar um jantar sob as estrelas, acesse nosso sistema de reservas interplanetário ou ligue para nosso assistente robótico." },
    { "acompanhamento do pedido", "Rastreie seu pedido em tempo real com nossa tecnologia de holograma 3D. Basta usar o código fornecido ao finalizar seu pedido." },
    { "benefícios do programa de fidelidade galáctico", "Nosso programa oferece viagens interestelares e descontos em alimentos de outras galáxias! Participe agora e acumule créditos cósmicos!" },
    { "frete para marte e outros planetas", "Oferecemos entregas para Marte e outras colônias espaciais por um valor adicional. O prazo de entrega varia conforme a localização." },
    { "assinar o boletim de novidades cósmicas", "Cadastre-se na nossa newsletter para receber atualizações sobre novos pratos interplanetários e eventos espaciais exclusivos!" }
};


    private static HashSet<long> usuariosQueIniciaram = new HashSet<long>();

    static async Task Main(string[] args)
    {
        botClient = new TelegramBotClient(BotToken);

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions);

        var me = await botClient.GetMeAsync();
        Console.WriteLine($"Bot iniciado como {me.Username}. Pressione qualquer tecla para parar o bot...");
        Console.ReadKey();

        await botClient.CloseAsync();
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message?.Text != null)
        {
            var message = update.Message;
            long chatId = message.Chat.Id;

            if (!usuariosQueIniciaram.Contains(chatId))
            {
                usuariosQueIniciaram.Add(chatId);
                await EnviarListaDePerguntas(botClient, chatId);
                return;
            }

            if (message.Text.Equals("/ajuda", StringComparison.OrdinalIgnoreCase))
            {
                await EnviarListaDePerguntas(botClient, chatId);
                return;
            }

            string resposta = ProcessarPerguntaPorNumero(message.Text);
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: resposta
            );
        }
    }

    private static async Task EnviarListaDePerguntas(ITelegramBotClient botClient, long chatId)
    {
        string mensagemBoasVindas = "Olá! Eu sou o bot de atendimento. Aqui estão algumas perguntas que você pode fazer:\n";
        foreach (var pergunta in Perguntas)
        {
            mensagemBoasVindas += $"{pergunta.Key}. {pergunta.Value}\n";
        }
        mensagemBoasVindas += "\nPor favor, digite o número da pergunta que você deseja fazer.";

        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: mensagemBoasVindas
        );
    }

    private static string ProcessarPerguntaPorNumero(string input)
    {
        if (int.TryParse(input, out int numero) && Perguntas.ContainsKey(numero))
        {
            string chavePergunta = Perguntas[numero].ToLower();
            if (Respostas.ContainsKey(chavePergunta))
            {
                return Respostas[chavePergunta];
            }
            else
            {
                return "Desculpe, não temos uma resposta para essa pergunta.";
            }
        }
        else
        {
            return "Número inválido. Por favor, escolha um número de 1 a 10.";
        }
    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Erro na API do Telegram:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}
