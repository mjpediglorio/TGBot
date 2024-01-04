using Solamis;
using System;
using System.Threading.Tasks;
using TL;
using WTelegram;

namespace Solamis
{
    static class Program
    {
        static WTelegram.Client Client;
        static readonly Dictionary<long, User> Users = new();
        static readonly Dictionary<long, ChatBase> Chats = new();
        private static int ChannelId;
        public async static Task Main(string[] args)
        {
            using var Client = new WTelegram.Client();
            {
                var myself = await Client.LoginUserIfNeeded();
                Client.OnUpdate += Client_OnUpdate;
                Console.WriteLine($"We are logged-in as {myself.username ?? myself.first_name + " " + myself.last_name} (id {myself.id})");
                var chats = await Client.Messages_GetAllChats();
                Console.Clear();
                Console.WriteLine("This user has joined the following:");
                foreach (var (id, chat) in chats.chats)
                    if (chat.IsActive)
                        Console.WriteLine($"{id,10}: {chat}");
                Console.Write("\n Enter Channel Id you want to copy: ");
                ChannelId = Int32.Parse(Console.ReadLine());
                var dialogs = await Client.Messages_GetAllDialogs(); // dialogs = groups/channels/users
                dialogs.CollectUsersChats(Users, Chats);
                Console.ReadKey();
            }
        }

        private static async Task Client_OnUpdate(UpdatesBase updates)
        {
            foreach (var update in updates.UpdateList)
                switch (update)
                {
                    case UpdateNewMessage unm: await HandleMessage(unm.message); break;
                }
        }

        // in this example method, we're not using async/await, so we just return Task.CompletedTask
        private static Task HandleMessage(MessageBase messageBase, bool edit = false)
        {
            if (messageBase.Peer.ID == ChannelId)
            {
                SendToBonkBot("test");
                if (edit) Console.Write("(Edit): ");
                switch (messageBase)
                {
                    case Message m: Console.WriteLine($"{Peer(m.from_id) ?? m.post_author} in {Peer(m.peer_id)}> {m.message}"); break;
                    case MessageService ms: Console.WriteLine($"{Peer(ms.from_id)} in {Peer(ms.peer_id)} [{ms.action.GetType().Name[13..]}]"); break;
                }
            }
            return Task.CompletedTask;
        }
        public static async Task SendToBonkBot(string TokenAddress)
        {
            var resolved = await Client.Contacts_ResolveUsername("shelbytrades10");
            await Client.SendMessageAsync(resolved, TokenAddress);
        }
        private static string User(long id) => Users.TryGetValue(id, out var user) ? user.ToString() : $"User {id}";
        private static string Chat(long id) => Chats.TryGetValue(id, out var chat) ? chat.ToString() : $"Chat {id}";
        private static string Peer(Peer peer) => peer is null ? null : peer is PeerUser user ? User(user.user_id)
            : peer is PeerChat or PeerChannel ? Chat(peer.ID) : $"Peer {peer.ID}";
    }
}