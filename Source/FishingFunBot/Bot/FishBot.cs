using FishingFun.Configuration;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace FishingFun
{
    public class FishingBot
    {
        public static ILog logger = LogManager.GetLogger("Fishbot");

        private readonly BotConfiguration config;
        private ConsoleKey castKey;
        private List<ConsoleKey> macroKeys;
        private DateTime StartTime = DateTime.Now;

        private IBobberFinder bobberFinder;
        private IBiteWatcher biteWatcher;
        private Stopwatch stopwatch = new Stopwatch();
        private static Random random = new Random();
        private CancellationToken cancellationToken;

        public event EventHandler<FishingEvent> FishingEventHandler;

        public FishingBot(IBobberFinder bobberFinder, IBiteWatcher biteWatcher, BotConfiguration configuration)
        {
            this.bobberFinder = bobberFinder;
            this.biteWatcher = biteWatcher;
            this.config = configuration;
            this.castKey = configuration.KeyBinds.CastKey;
            this.macroKeys = new List<ConsoleKey> { configuration.KeyBinds.Macro1Key, configuration.KeyBinds.Macro2Key };

            logger.Info("FishBot Created.");

            FishingEventHandler += (s, e) => { };
        }

        // Legacy constructor for backward compatibility
        public FishingBot(IBobberFinder bobberFinder, IBiteWatcher biteWatcher, ConsoleKey castKey, List<ConsoleKey> macroKeys, int macroTimer)
            : this(bobberFinder, biteWatcher, ConfigurationManager.Instance.Current)
        {
            this.castKey = castKey;
            this.macroKeys = macroKeys;
            this.config.Timing.MacroInterval = macroTimer * 60 * 1000;
        }

        public void Start(CancellationToken ct)
        {
            this.cancellationToken = ct;
            biteWatcher.FishingEventHandler = (e) => FishingEventHandler?.Invoke(this, e);

            DoMacroKeys();

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    logger.Info($"Pressing key {castKey} to Cast.");

                    PressMacroKeysIfDue();

                    FishingEventHandler?.Invoke(this, new FishingEvent { Action = FishingAction.Cast });
                    WowProcess.PressKey(castKey);

                    Watch(config.Timing.CastWatchDelay, ct);

                    WaitForBite(ct);
                }
                catch (OperationCanceledException)
                {
                    logger.Info("Bot operation cancelled.");
                    break;
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    Sleep(2000);
                }
            }

            logger.Info("Bot has Stopped.");
        }

        // Legacy Start method for backward compatibility
        public void Start()
        {
            Start(CancellationToken.None);
        }

        public void SetCastKey(ConsoleKey castKey)
        {
            this.castKey = castKey;
        }

        public void SetMacro1Key(ConsoleKey castKey)
        {
            this.macroKeys[0] = castKey;
        }

        public void SetMacro2Key(ConsoleKey castKey)
        {
            this.macroKeys[1] = castKey;
        }

        public void SetMacroTimer(int time)
        {
            this.config.Timing.MacroInterval = time * 60 * 1000;
        }

        private void Watch(int milliseconds, CancellationToken ct)
        {
            bobberFinder.Reset();
            stopwatch.Reset();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < milliseconds && !ct.IsCancellationRequested)
            {
                bobberFinder.Find(ct);
            }
            stopwatch.Stop();
        }

        private void WaitForBite(CancellationToken ct)
        {
            bobberFinder.Reset();

            var bobberPosition = FindBobber(ct);
            if (bobberPosition == Point.Empty)
            {
                return;
            }

            this.biteWatcher.Reset(bobberPosition);

            logger.Info("Bobber start position: " + bobberPosition);

            var timedTask = new TimedAction((a) => { logger.Info("Fishing timed out!"); }, config.Timing.FishingTimeout, config.Timing.FishingTimeout / 1000);

            // Wait for the bobber to move
            while (!ct.IsCancellationRequested)
            {
                var currentBobberPosition = FindBobber(ct);
                if (currentBobberPosition == Point.Empty || currentBobberPosition.X == 0) { return; }

                if (this.biteWatcher.IsBite(currentBobberPosition))
                {
                    Loot(bobberPosition);
                    PressMacroKeysIfDue();
                    return;
                }

                if (!timedTask.ExecuteIfDue(ct)) { return; }
            }
        }

        private void PressMacroKeysIfDue()
        {
            if (macroKeys.Count <= 0)
            {
                return;
            }

            //  Use seconds to get fidelity with the slush timer.
            //  Issue #35: There was potential for the few seconds it takes to cast lure to not be waited on for second lure,
            //  causing every other lure application to fail.
            if ((DateTime.Now - StartTime).TotalMilliseconds > config.Timing.MacroInterval + (config.Timing.MacroExecutionDelay * 1000))
            {
                DoMacroKeys();
            }
        }

        /// <summary>
        /// Ten minute key can do anything you want e.g.
        /// Macro to apply a lure: 
        /// /use Bright Baubles
        /// /use 16
        /// 
        /// Or a macro to delete junk:
        /// /run for b=0,4 do for s=1,GetContainerNumSlots(b) do local n=GetContainerItemLink(b,s) if n and (strfind(n,"Raw R") or strfind(n,"Raw Spot") or strfind(n,"Raw Glo") or strfind(n,"roup")) then PickupContainerItem(b,s) DeleteCursorItem() end end end
        /// </summary>
        private void DoMacroKeys()
        {
            StartTime = DateTime.Now;

            if (macroKeys.Count == 0)
            {
                logger.Info($"Ten Minute Key:  No keys defined in tenMinKey, so nothing to do (Define in call to FishingBot constructor).");
            }

            FishingEventHandler?.Invoke(this, new FishingEvent { Action = FishingAction.Cast });

            foreach (var key in macroKeys)
            {
                logger.Info($"Ten Minute Key: Pressing key {key} to run a macro, delete junk fish or apply a lure etc.");
                WowProcess.PressKey(key);
            }
        }

        private void Loot(Point bobberPosition)
        {
            logger.Info($"Right clicking mouse to Loot.");
            WowProcess.RightClickMouse(logger, bobberPosition);
        }

        public static void Sleep(int ms)
        {
            var config = ConfigurationManager.Instance.Current;
            ms += random.Next(0, config.Timing.SleepMaxRandomness);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.Elapsed.TotalMilliseconds < ms)
            {
                FlushBuffers();
                Thread.Sleep(Constants.DefaultSleepCheckInterval);
            }
        }

        public static void FlushBuffers()
        {
            ILog log = LogManager.GetLogger("Fishbot");
            var logger = log.Logger as Logger;
            if (logger != null)
            {
                foreach (IAppender appender in logger.Appenders)
                {
                    var buffered = appender as BufferingAppenderSkeleton;
                    if (buffered != null)
                    {
                        buffered.Flush();
                    }
                }
            }
        }

        private Point FindBobber(CancellationToken ct)
        {
            var timer = new TimedAction(
                (a) => { logger.Info("Waited seconds for target: " + a.ElapsedSecs); },
                config.Detection.BobberSearchInterval,
                config.Detection.BobberSearchTimeout / 1000);

            while (!ct.IsCancellationRequested)
            {
                var target = this.bobberFinder.Find(ct);
                if (target != Point.Empty || !timer.ExecuteIfDue(ct)) { return target; }
            }

            return Point.Empty;
        }
    }
}