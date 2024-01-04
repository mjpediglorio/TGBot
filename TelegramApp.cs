using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TL;
using WTelegram;

namespace Solamis
{
    public class TelegramApp
    {

        public int ApiId { get; set; }
        public string ApiHash { get; set; }
        public TelegramApp()
        {
        }

        public async Task<string> Login(string ContactNumber, WTelegram.Client? client)
        {
            while (client.User == null)
                switch (await client.Login(ContactNumber)) // returns which config is needed to continue login
                {
                    case "verification_code": Console.Write("Code: "); ContactNumber = Console.ReadLine(); break;
                    default: ContactNumber = null; break;
                }
            var chats = await client.Messages_GetAllChats();
            Console.Clear();
            Console.WriteLine($"We are logged-in as {client.User} (id {client.User.id})");
            Console.WriteLine("");
            foreach (var (id, chat) in chats.chats)
                if (chat.IsActive)
                    Console.WriteLine($"{id,10}: {chat}");
            Console.WriteLine();
            Console.WriteLine("Input the Channel ID you wanna copy trade: \n");
            return Console.ReadLine();
        }
        public async Task SendToBonkBot(WTelegram.Client? client, string TokenAddress)
        {
            var resolved = await client.Contacts_ResolveUsername("shelbytrades10");
            await client.SendMessageAsync(resolved, TokenAddress);
        }
    }
}