namespace TelegramExtensions;

using TL;
using WTelegram;

public class TelegramService
{
    public async Task<int> NewMessage(
        long chatId,
        string? message)
    {
        await using var client = new Client(ConfigProvider);
        await client.LoginUserIfNeeded();
        var chats = await client.Messages_GetAllChats();
        var target = chats.chats[chatId];
        var messageResult = await client.SendMessageAsync(target, message);
        return messageResult.ID;
    }

    public async Task<User> Login()
    {
        await using var client = new Client(ConfigProvider);
        var user = await client.LoginUserIfNeeded();
        return user;
    }

    private string? ConfigProvider(string arg)
    {
        return arg switch
        {
            "api_id" => "",
            "api_hash" => "",
            "phone_number" => "userPhoneNumber.Text",
            "verification_code" => "telegramCode.Text",
            "password" => "password.Text",
            _ => null
        };
    }
}