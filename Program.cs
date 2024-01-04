using Solamis;
using Solamis.Telegram;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using TL;
using WTelegram;

namespace Solamis
{
    static class Program
    {
        static WTelegram.Client Client;
        static User My;
        private static string ContactNumber;
        private static int ApiId;
        private static string ApiHash;
        private static long ChannelId;
        static readonly Dictionary<long, User> Users = new();
        static readonly Dictionary<long, ChatBase> Chats = new();
        private static string User(long id) => Users.TryGetValue(id, out var user) ? user.ToString() : $"User {id}";
        private static string Chat(long id) => Chats.TryGetValue(id, out var chat) ? chat.ToString() : $"Chat {id}";
        private static string Peer(Peer peer) => peer is null ? null : peer is PeerUser user ? User(user.user_id)
            : peer is PeerChat or PeerChannel ? Chat(peer.ID) : $"Peer {peer.ID}";
        public async static Task Main(string[] args)
        {
            Console.WriteLine($"Welcome to Telegram Copy Trader! \n" +
                $"What is your Contact Number? \n" +
                $"Include Country Code ex. +63992211231 \n " +
                $"\n" +
                $"Contact Number:");
            ContactNumber = Console.ReadLine();
            Console.Clear();
            Console.WriteLine($"Contact Number: {ContactNumber}" +
                $"\n" +
                $"\n" +
                $"Please input your ApiId and ApiHash." +
                $"\n");
            ApiId = Int32.Parse(Console.ReadLine());
            ApiHash = Console.ReadLine();

            Console.WriteLine($"These are your details: \n" +
                $"Contact Number: {ContactNumber} \n" +
                $"ApiId: {ApiId} \n" +
                $"ApiHash: {ApiHash} \n" +
                $"\n" +
                $"Press any key to continue...");
            Console.ReadKey();
            ITGApp _tg = new TGApp(ApiId, ApiHash, ContactNumber);
            //WTelegram.Client client = new WTelegram.Client(ApiId, ApiHash);
            //TelegramApp tg = new TelegramApp();
            //using (client)
            //{
            //    ChannelId = long.Parse(await tg.Login(ContactNumber, client));
            //    client.OnUpdate += Client_OnUpdate;
            //    var dialogs = await client.Messages_GetAllDialogs(); // dialogs = groups/channels/users
            //    dialogs.CollectUsersChats(Users, Chats);
            //    Console.ReadKey();
            //}
        }
        private static async Task Client_OnUpdate(UpdatesBase updates)
        {
            updates.CollectUsersChats(Users, Chats);
            if (updates is UpdateShortMessage usm && !Users.ContainsKey(usm.user_id))
                (await Client.Updates_GetDifference(usm.pts - usm.pts_count, usm.date, 0)).CollectUsersChats(Users, Chats);
            //else if (updates is UpdateShortChatMessage uscm && (!Users.ContainsKey(uscm.from_id) || !Chats.ContainsKey(uscm.chat_id)))
            //    (await Client.Updates_GetDifference(uscm.pts - uscm.pts_count, uscm.date, 0)).CollectUsersChats(Users, Chats);
            foreach (var update in updates.UpdateList)
            {
                switch (update)
                {
                    case UpdateNewMessage unm: await HandleMessage(unm.message); break;
                        //default: Console.WriteLine(update.GetType().Name); break; // there are much more update types than the above example cases
                }
            }
        }

        // in this example method, we're not using async/await, so we just return Task.CompletedTask
        private static Task HandleMessage(MessageBase messageBase, bool edit = false)
        {
            if (edit) Console.Write("(Edit): ");
            if (messageBase.Peer.ID == ChannelId)
            {
                WTelegram.Client newClient = new WTelegram.Client(ApiId, ApiHash);
                TelegramApp tg = new TelegramApp();
                Console.WriteLine("Filtering works!");
                string input = "";
                SmartContract contract = new SmartContract();
                input = Console.ReadLine();
                var ListString = contract.ConvertToList(input);
                foreach (string str in ListString)
                {
                    tg.SendToBonkBot(newClient, str);
                }
                switch (messageBase)
                {
                    case Message m:
                        Console.WriteLine($"{Peer(m.from_id) ?? m.post_author} in {Peer(m.peer_id)}> {m.message}");
                        break;
                    case MessageService ms: Console.WriteLine($"{Peer(ms.from_id)} in {Peer(ms.peer_id)} [{ms.action.GetType().Name[13..]}]"); break;
                }
            }
            return Task.CompletedTask;
        }
    }
}