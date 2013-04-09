/*
 * This code is derived from the Java version of RomRaider
 *
 * RomRaider Open-Source Tuning, Logging and Reflashing
 * Copyright (C) 2006-2012 RomRaider.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;
using Com.Sun.Speech.Freetts;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Tts
{
	public class Speaker
	{
		private static readonly string VOICE_NAME = "kevin16";

		private static readonly VoiceManager VOICE_MANAGER = VoiceManager.GetInstance();

		private static readonly Voice VOICE = VOICE_MANAGER.GetVoice(VOICE_NAME);

		static Speaker()
		{
			VOICE.Allocate();
		}

		public Speaker()
		{
			throw new NotSupportedException();
		}

		public static void Say(string message)
		{
			ThreadUtil.RunAsDaemon(new _Runnable_40(message));
		}

		private sealed class _Runnable_40 : Runnable
		{
			public _Runnable_40(string message)
			{
				this.message = message;
			}

			public void Run()
			{
				try
				{
					RomRaider.Tts.Speaker.VOICE.Speak(message);
				}
				catch (Exception)
				{
				}
			}

			private readonly string message;
		}

		// ignore
		public static void End()
		{
			VOICE.Deallocate();
		}
	}
}
