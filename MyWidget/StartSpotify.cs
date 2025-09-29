using NPSMLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWidget
{
    public static class StartSpotify
    {
        public static void Start()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "spotify",
                UseShellExecute = true,
            };
            Process.Start(psi);

            Thread.Sleep(4000);

            NowPlayingSessionManager player = new NowPlayingSessionManager();

            NowPlayingSession[] sessions = player.GetSessions();
            var sessionInfos = sessions
                .Where(x =>
                    x.SourceAppId == "Spotify.exe"
                    || x.SourceAppId.Contains("spotify")
                    || x.SourceAppId.Contains("Spotify")
                    || x.SourceDeviceId == "Local"
                )
                .Select(x => x.GetSessionInfo())
                .ToList();
            if (sessionInfos.Count == 0)
            {
                return;
            }
            else
            {
                player.SetCurrentSession(sessionInfos[0]);
            }
            NowPlayingSession currentSession = player.CurrentSession;

            var x = currentSession.ActivateMediaPlaybackDataSource();
            var z = x.GetMediaPlaybackInfo();

            if (z.PlaybackState.ToString() == "Playing")
            {
                x.SendMediaPlaybackCommand(MediaPlaybackCommands.Stop);
                x.SendMediaPlaybackCommand(MediaPlaybackCommands.Pause);
                // string lastpath = GetImageDir("stop.png", false);

            }
            else
            {
                x.SendMediaPlaybackCommand(MediaPlaybackCommands.Play);
                x.SendMediaPlaybackCommand(MediaPlaybackCommands.Pause);
                x.SendMediaPlaybackCommand(MediaPlaybackCommands.Stop);

            }
            Thread.Sleep(1000);
        }
    }
}
