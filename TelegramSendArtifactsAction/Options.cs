using System.Collections.Generic;
using CommandLine;

namespace TelegramSendArtifactsAction
{
    public class Options
    {
        [Option("bot-key", Required = true)]
        public string BotKey { get; set; }

        [Option("chat-id", Required = true)]
        public string ChatId { get; set; }

        [Option('s', "source", Required = true)]
        public string Source { get; set; }

        [Option('e', "event", Required = true)]
        public string Event { get; set; }

        [Option('r', "ref", Required = true)]
        public string Ref { get; set; }

        [Option('h', "commit-hash", Required = true)]
        public string CommitHash { get; set; }

        [Option('m', "commit-message", Required = false)]
        public string CommitMessage { get; set; }

        [Option('c', "version-code", Required = true)]
        public string VersionCode { get; set; }

        [Option('f', "files", Required = true)]
        public IEnumerable<string> Files { get; set; }

        [Option("s3AccessKeyId", Required = false)]
        public string S3AccessKeyId { get; set; }

        [Option("s3SecretAccessKey", Required = false)]

        public string S3SecretAccessKey { get; set; }

        [Option("s3EndpointUrl", Required = false)]
        public string S3EndpointUrl { get; set; }

        [Option("s3Space", Required = false)]
        public string S3Space { get; set; }
    }
}
