﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSSBot.Commands
{
    /// <summary>
    /// Just some basic commands to get started
    /// use an empty name attribute because these are default
    /// </summary>
    [Name("")]
    public class BasicCommands : ModuleBase
    {
        // as far as I'm concerned, Dependency Injection is black magic
        // here are the docs I referenced to get this working
        // https://discord.foxbot.me/docs/guides/commands/commands.html#dependency-injection
        private readonly CommandService _commandService;
        private readonly DiscordSocketClient _client;

        public BasicCommands(CommandService commands, DiscordSocketClient client)
        {
            _commandService = commands;
            _client = client;
        }

        /// <summary>
        /// Basic Ping/Pong command
        /// </summary>
        /// <returns></returns>
        [Command("Ping"), Summary("A simple ping/pong command, for checking if the bot works.")]
        public async Task Ping()
        {
            await ReplyAsync("Pong!");
        }

        /// <summary>
        /// Echo command
        /// </summary>
        /// <returns></returns>
        [Command("Echo"), Summary("A simple echo command.")]
        public async Task Echo([Name("Text"), Summary("The text to echo back."), Remainder] string text)
        {
            await ReplyAsync(Context.User.Mention + " : " + text);
        }

        /// <summary>
        /// About text command
        /// Includes a link to the repo
        /// </summary>
        /// <returns></returns>
        [Command("About"), Summary("Replies back with some help text.")]
        [Alias("GitHub")]
        public async Task About()
        {
            string txt = string.Format("🤔 CSSBot 🤔\nGitHub: https://github.com/Chris-Johnston/CSSBot");
            await ReplyAsync(txt);
        }

        [Command("Cleanup", RunMode = RunMode.Async)]
        [Summary("Deletes a specified number of the bot's recent messages.")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireContext(ContextType.Guild)]
        public async Task CleanupRecent([Name("NumMsg")] int amountToCleanup = 10)
        {
            // ensure bounds
            // don't allow deleting more than 25 because that's a lot to delete
            // and we don't want to spam api either
            if (amountToCleanup < 0 || amountToCleanup > 25) amountToCleanup = 10;
            foreach( var message in await Context.Channel.GetMessagesAsync(Context.Message.Id, Direction.Before, amountToCleanup).FlattenAsync())
            {
                if(message.Author.Id == Context.Client.CurrentUser.Id)
                    await message.DeleteAsync();
            }

            // delete the message that started the command as well
            await Context.Message.DeleteAsync();
        }

        [Command("InviteLink")]
        [Summary("Gets the invite link for the bot.")]
        [RequireUserPermission(GuildPermission.SendMessages | GuildPermission.EmbedLinks)]
        public async Task InviteLink()
        {
            await ReplyAsync($"A user with the 'Manage Server' permission can add me to your server using the following link: https://discordapp.com/oauth2/authorize?client_id={_client.CurrentUser.Id}&scope=bot");
        }

    }
}
