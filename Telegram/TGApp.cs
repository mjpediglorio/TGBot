using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL;
using WTelegram;

namespace Solamis.Telegram
{
    public class TGApp : ITGApp
    {
        static WTelegram.Client Client;
        private readonly int _ApiId;
        private readonly string _ApiHash;
        private static string User(long id) => Users.TryGetValue(id, out var user) ? user.ToString() : $"User {id}";
        private static string Chat(long id) => Chats.TryGetValue(id, out var chat) ? chat.ToString() : $"Chat {id}";
        private static string Peer(Peer peer) => peer is null ? null : peer is PeerUser user ? User(user.user_id)
            : peer is PeerChat or PeerChannel ? Chat(peer.ID) : $"Peer {peer.ID}";
        private static long ChannelId;
        static readonly Dictionary<long, User> Users = new();
        static readonly Dictionary<long, ChatBase> Chats = new();
        private static string ContactNumber;
        private readonly WTelegram.Client _client;
        public TGApp(int ApiId, string HashId, string ContactNumber)
        {
            _ApiHash = HashId;
            _ApiId = ApiId;
            _client = new WTelegram.Client(_ApiId, _ApiHash);
            Function(ContactNumber);
        }

        private async void Function(string ContactNumber)
        {
            using (_client)
            {
                ChannelId = long.Parse(await Login(ContactNumber, _client));
                _client.OnUpdate += Client_OnUpdate;
                var dialogs = await _client.Messages_GetAllDialogs(); // dialogs = groups/channels/users
                dialogs.CollectUsersChats(Users, Chats);
                Console.ReadKey();
            }
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
                TelegramApp tg = new TelegramApp();
                Console.WriteLine("Filtering works!");
                string input = "";
                SmartContract contract = new SmartContract();
                input = Console.ReadLine();
                var ListString = contract.ConvertToList(input);
                //foreach (string str in ListString)
                //{
                //    tg.SendToBonkBot(str);
                //}
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
