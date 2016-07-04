﻿/*
    _                _      _  ____   _                           _____
   / \    _ __  ___ | |__  (_)/ ___| | |_  ___   __ _  _ __ ___  |  ___|__ _  _ __  _ __ ___
  / _ \  | '__|/ __|| '_ \ | |\___ \ | __|/ _ \ / _` || '_ ` _ \ | |_  / _` || '__|| '_ ` _ \
 / ___ \ | |  | (__ | | | || | ___) || |_|  __/| (_| || | | | | ||  _|| (_| || |   | | | | | |
/_/   \_\|_|   \___||_| |_||_||____/  \__|\___| \__,_||_| |_| |_||_|   \__,_||_|   |_| |_| |_|

 Copyright 2015-2016 Łukasz "JustArchi" Domeradzki
 Contact: JustArchi@JustArchi.net

 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0
					
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.

*/

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace ArchiSteamFarm {
	internal static class Logging {
		private const string Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss}|${level:uppercase=true}|${message}${onexception:inner= ${exception:format=toString,Data}}";

		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		internal static void Init() {
			LoggingConfiguration config = new LoggingConfiguration();

			if (Program.GlobalConfig.LogToFile) {
				FileTarget fileTarget = new FileTarget("File") {
					FileName = Program.LogFile,
					Layout = Layout
				};

				config.AddTarget(fileTarget);
				config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, fileTarget));
			}

			if (Program.IsRunningAsService) {
				EventLogTarget eventLogTarget = new EventLogTarget("EventLog") {
					Layout = Layout
				};

				config.AddTarget(eventLogTarget);
				config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, eventLogTarget));
			}

			ColoredConsoleTarget consoleTarget = new ColoredConsoleTarget("Console") {
				Layout = Layout
			};

			config.AddTarget(consoleTarget);
			config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, consoleTarget));

			LogManager.Configuration = config;
		}

		// Ideally, those two should disable/enable only console logging
		// But for some reason removing console rule/target doesn't seem to work
		internal static void OnUserInputStart() => LogManager.DisableLogging();
		internal static void OnUserInputEnd() => LogManager.EnableLogging();

		internal static void LogGenericWTF(string message, string botName = "Main", [CallerMemberName] string previousMethodName = null) {
			if (string.IsNullOrEmpty(message)) {
				LogNullError(nameof(message), botName);
				return;
			}

			Logger.Error($"{botName}|{previousMethodName}() {message}");
		}

		internal static void LogGenericError(string message, string botName = "Main", [CallerMemberName] string previousMethodName = null) {
			if (string.IsNullOrEmpty(message)) {
				LogNullError(nameof(message), botName);
				return;
			}

			Logger.Error($"{botName}|{previousMethodName}() {message}");
		}

		internal static void LogGenericException(Exception exception, string botName = "Main", [CallerMemberName] string previousMethodName = null) {
			if (exception == null) {
				LogNullError(nameof(exception), botName);
				return;
			}

			Logger.Error(exception, $"{botName}|{previousMethodName}()");
		}

		internal static void LogGenericWarning(string message, string botName = "Main", [CallerMemberName] string previousMethodName = null) {
			if (string.IsNullOrEmpty(message)) {
				LogNullError(nameof(message), botName);
				return;
			}

			Logger.Warn($"{botName}|{previousMethodName}() {message}");
		}

		internal static void LogGenericInfo(string message, string botName = "Main", [CallerMemberName] string previousMethodName = null) {
			if (string.IsNullOrEmpty(message)) {
				LogNullError(nameof(message), botName);
				return;
			}

			Logger.Info($"{botName}|{previousMethodName}() {message}");
		}

		internal static void LogNullError(string nullObjectName, string botName = "Main", [CallerMemberName] string previousMethodName = null) {
			if (string.IsNullOrEmpty(nullObjectName)) {
				return;
			}

			Logger.Error($"{botName}|{previousMethodName}() {nullObjectName} is null!");
		}

		[Conditional("DEBUG")]
		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		internal static void LogGenericDebug(string message, string botName = "Main", [CallerMemberName] string previousMethodName = null) {
			if (string.IsNullOrEmpty(message)) {
				LogNullError(nameof(message), botName);
				return;
			}

			Logger.Debug($"{botName}|{previousMethodName}() {message}");
		}
	}
}
