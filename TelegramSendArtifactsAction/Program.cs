using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using CommandLine;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace TelegramSendArtifactsAction
{
    public static class Program
    {
        private static async Task Job(Options options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.Files = options.Files.SelectMany(e => e.Split())
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .SelectMany(e => Directory.GetFiles(Directory.GetCurrentDirectory(), e))
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .OrderBy(e => e)
                .ToArray();

            var bot = new TelegramBotClient(options.BotKey);
            await bot.SetWebhookAsync(string.Empty);

            var paths = options.Files;

            await paths
                .ToObservable()
                .Select(
                    e =>
                    {
                        var s = File.OpenRead(e);
                        var fileName = Path.GetFileName(e);

                        var media = new InputMediaDocument(new InputMedia(s, fileName));

                        var commitMessage = string.IsNullOrWhiteSpace(options.CommitMessage)
                            ? ""
                            : $@"
<code>{options.CommitMessage}</code>
";

                        media.Caption = $@"
{IconHelper.GetRandomIcon()}  <b>{options.Event}</b>

<b>{options.Source}</b>
{options.Ref}

version code <code>{options.VersionCode}</code>

<code>{options.CommitHash}</code>
" + commitMessage;

                        media.ParseMode = ParseMode.Html;

                        return (fileName, path: e, media, stream: s);
                    }
                )
                .ToList()
                .Select(
                    e =>
                        bot.SendMediaGroupAsync(
                                options.ChatId,
                                e
                                    .OrderBy(c => c.fileName)
                                    .Select(c => c.media),
                                true
                            )
                            .ToObservable()
                            .Select(c => e)
                )
                .Merge()
                .FirstAsync()
                .Do(
                    e =>
                    {
                        foreach (var (_, _, _, stream) in e) stream.Dispose();
                    }
                )
                .IgnoreElements()
                .ToList();
        }

        private static async Task Main(string[] args)
        {
            var parse = Parser.Default.ParseArguments<Options>(args);

            parse.WithNotParsed(e => { });
            await parse.WithParsedAsync(Job);
        }
    }
}
