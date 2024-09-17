using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;

class Program
{
    private static readonly string BotToken = "7416280107:AAFSIX8a3T483eG80gr8WytJCvmCCEXa96g";
    private static TelegramBotClient botClient;

    // Dicionários para perguntas e respostas
    private static readonly Dictionary<int, string> Perguntas = new Dictionary<int, string>
    {
        { 1, "Horários de atendimento" },
        { 2, "Suporte técnico" },
        { 3, "Sede da empresa" },
        { 4, "Produtos" },
        { 5, "Política de devolução" },
        { 6, "Formas de pagamento" },
        { 7, "Acompanhar meu pedido" },
        { 8, "Frete grátis" },
        { 9, "Programa de fidelidade" },
        { 10, "Cadastrar na newsletter" }
    };

    private static readonly Dictionary<string, string> Respostas = new Dictionary<string, string>
    {
        { "horários de atendimento", "Nossos horários de atendimento são de segunda a sexta-feira, das 9h às 18h. Estamos fechados aos sábados e domingos." },
        { "suporte técnico", "Você pode entrar em contato com nosso suporte técnico pelo e-mail suporte@empresa.com.br ou pelo telefone (21) 1234-5678." },
        { "sede da empresa", "A sede da nossa empresa está localizada na Rua Exemplo, 123, Bairro, Cidade, Estado, CEP 12345-678." },
        { "produtos", "Oferecemos uma ampla gama de produtos, incluindo [listar alguns produtos principais]. Para ver a lista completa, visite nosso site em www.empresa.com.br." },
        { "política de devolução", "Sim, temos uma política de devolução de 30 dias. Para mais detalhes, consulte nossa política de devolução em nosso site ou entre em contato com o suporte." },
        { "formas de pagamento", "Aceitamos pagamentos por cartão de crédito, débito, boleto bancário e transferência bancária. Confira mais opções durante o processo de compra em nosso site." },
        { "acompanhar meu pedido", "Após o envio, você receberá um e-mail com o código de rastreamento do seu pedido. Você pode acompanhar o status do seu pedido no nosso site usando o código de rastreamento." },
        { "frete grátis", "Oferecemos frete grátis para pedidos acima de R$200. Para pedidos abaixo desse valor, o frete será calculado com base no endereço de entrega." },
        { "programa de fidelidade", "Sim, temos um programa de fidelidade que oferece benefícios exclusivos para nossos clientes. Saiba mais sobre o programa e como participar em nosso site." },
        { "cadastrar na newsletter", "Você pode se cadastrar na nossa newsletter visitando o site e preenchendo o formulário de inscrição na seção de notícias ou no rodapé da página." }
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
