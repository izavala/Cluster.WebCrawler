// -----------------------------------------------------------------------
// <copyright file="AkkaStartupTasks.cs" company="Petabridge, LLC">
//      Copyright (C) 2015 - 2019 Petabridge, LLC <https://petabridge.com>
// </copyright>
// -----------------------------------------------------------------------

using Akka.Actor;
using Akka.Routing;
using WebCrawler.Shared.Config;
using WebCrawler.Shared.DevOps;
using WebCrawler.Web.Actors;

namespace WebCrawler.Web
{
    public static class AkkaStartupTasks
    {
        public static ActorSystem StartAkka()
        {
            var config = HoconLoader.ParseConfig("web.hocon");
            SystemActors.ActorSystem = ActorSystem.Create("webcrawler", config.ApplyOpsConfig()).StartPbm();
            var router = SystemActors.ActorSystem.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), "tasker");
            var processor = SystemActors.CommandProcessor = SystemActors.ActorSystem.ActorOf(
                Props.Create(() => new CommandProcessor(router)),
                "commands");
            SystemActors.SignalRActor =
                SystemActors.ActorSystem.ActorOf(Props.Create(() => new SignalRActor(processor)), "signalr");
            return SystemActors.ActorSystem;
        }
    }
}