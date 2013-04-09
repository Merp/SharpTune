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
using System.Collections.Generic;
using System.IO;
using Com.Centerkey.Utils;
using Java.Awt;
using Javax.Swing;
using Org.Eclipse.Jgit.Api;
using Org.Eclipse.Jgit.Api.Errors;
using Org.Eclipse.Jgit.Lib;
using Org.Eclipse.Jgit.Storage.File;
using RomRaider;
using RomRaider.Swing;
using Sharpen;

namespace RomRaider.Definition
{
	[System.Serializable]
	public sealed class DefinitionRepoManager : AbstractFrame
	{
		private const long serialVersionUID = -8376649924531989081L;

		private Settings settings = ECUExec.settings;

		private static Repository gitRepo;

		private static Git git;

		private JWindow startStatus;

		private readonly JLabel startText = new JLabel(" Initializing Defintion Repo");

		private JProgressBar progressBar;

		public DefinitionRepoManager()
		{
			progressBar = Startbar();
		}

		public void Load()
		{
			UpdateStatus("Checking Definition Repo Status...", 10);
			try
			{
				if (!InitAndCheckRepoExists())
				{
					UpdateStatus("Downloading Definition Repo...", 50);
					DownloadRepo();
				}
				else
				{
					UpdateStatus("Updating Definition Repo...", 75);
					SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
					FetchAll();
					CheckoutBranch(settings.GetGitBranch());
					SetCursor(null);
				}
				this.startStatus.Dispose();
			}
			catch (Exception e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
				JOptionPane.ShowMessageDialog(this, "Error configuring definition repository, configure definitions manually!\nError: "
					 + e.Message, "Definition repository configuration failed.", JOptionPane.INFORMATION_MESSAGE
					);
				if (settings.GetEcuDefinitionFiles().Count <= 0)
				{
					// no ECU definitions configured - let user choose to get latest or configure later
					object[] options = new object[] { "Yes", "No" };
					int answer = JOptionPane.ShowOptionDialog(null, "Unable to configure ECU definition repository.\nGo online to download the latest definition files?"
						, "Editor Configuration", JOptionPane.DEFAULT_OPTION, JOptionPane.WARNING_MESSAGE
						, null, options, options[0]);
					if (answer == 0)
					{
						BareBonesBrowserLaunch.OpenURL(Version.ECU_DEFS_URL);
					}
					else
					{
						JOptionPane.ShowMessageDialog(this, "ECU definition files need to be configured before ROM images can be opened.\nMenu: ECU Definitions > ECU Definition Manager..."
							, "Editor Configuration", JOptionPane.INFORMATION_MESSAGE);
					}
				}
			}
		}

		/// <summary>Checks that we have a git repo containing our desired branch</summary>
		/// <returns></returns>
		public bool InitAndCheckRepoExists()
		{
			try
			{
				gitRepo = InitRepo(Settings.GetGitDefsBaseDir());
				git = new Git(gitRepo);
				return CheckLocalBranchExists(gitRepo, settings.GetGitBranch());
			}
			catch (IOException e1)
			{
				// TODO Auto-generated catch block
				Sharpen.Runtime.PrintStackTrace(e1);
				return false;
			}
		}

		public void DownloadRepo()
		{
			JOptionPane.ShowMessageDialog(this, "Definition files are missing, downloading most up to date set from: "
				 + Settings.defaultGitUrl + " This may take a few minutes!!", "Definition Configuration"
				, JOptionPane.INFORMATION_MESSAGE);
			try
			{
				SetCursor(Cursor.GetPredefinedCursor(Cursor.WAIT_CURSOR));
				GitClone(Settings.defaultGitUrl, Settings.defaultGitRemote, Settings.GetGitDefsBaseDir
					(), settings.GetGitBranch());
				SetCursor(null);
				JOptionPane.ShowMessageDialog(this, "Definitions successfully updated!", "Definition Configuration"
					, JOptionPane.INFORMATION_MESSAGE);
			}
			catch (IOException e)
			{
				// TODO Auto-generated catch block
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}

		public void UpdateDefRepo(string remote, string branch)
		{
			try
			{
				git.Fetch().SetRemote(remote).SetRemoveDeletedRefs(true).Call();
				this.UpdateAllBranches(ECUExec.settings.GetGitRemotes().Get(remote), branch);
			}
			catch (Exception e)
			{
				// TODO Auto-generated catch block
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		public void GitClone(string url, string remote, string path, string checkoutBranch
			)
		{
			try
			{
				DelDir(new FilePath(path));
				git = Git.CloneRepository().SetRemote(remote).SetURI(url).SetDirectory(new FilePath
					(path + "/")).SetCloneAllBranches(true).SetTimeout(10000).Call();
				FetchAll();
			}
			catch (GitAPIException e)
			{
				// TODO Auto-generated catch block
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}

		private void FetchAll()
		{
			IList<Ref> bl;
			try
			{
				bl = git.BranchList().SetListMode(ListBranchCommand.ListMode.REMOTE).Call();
				IList<string> tl = new AList<string>();
				foreach (Ref r in bl)
				{
					string[] bra = r.GetName().Split("/");
					string rem = bra[bra.Length - 2];
					if (!tl.Contains(rem))
					{
						tl.AddItem(rem);
						git.Fetch().SetRemote(rem);
					}
				}
			}
			catch (GitAPIException e)
			{
				// TODO Auto-generated catch block
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}

		/// <exception cref="Org.Eclipse.Jgit.Api.Errors.GitAPIException"></exception>
		private void UpdateAllBranches(string url, string checkoutBranch)
		{
			IList<Ref> bl = git.BranchList().SetListMode(ListBranchCommand.ListMode.REMOTE).Call
				();
			foreach (Ref r in bl)
			{
				try
				{
					UpdateBranch(r);
				}
				catch (GitAPIException e)
				{
					// TODO Auto-generated catch block
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
			CheckoutBranch(settings.GetGitBranch());
		}

		/// <exception cref="Org.Eclipse.Jgit.Api.Errors.RefAlreadyExistsException"></exception>
		/// <exception cref="Org.Eclipse.Jgit.Api.Errors.RefNotFoundException"></exception>
		/// <exception cref="Org.Eclipse.Jgit.Api.Errors.InvalidRefNameException"></exception>
		/// <exception cref="Org.Eclipse.Jgit.Api.Errors.GitAPIException"></exception>
		private void UpdateBranch(Ref r)
		{
			string sbranch = Repository.ShortenRefName(r.GetName());
			git.BranchCreate().SetForce(true).SetName(sbranch).SetUpstreamMode(CreateBranchCommand.SetupUpstreamMode
				.TRACK).SetStartPoint(sbranch).Call();
			settings.SetGitBranch(sbranch);
		}

		public void CheckoutBranch(string s)
		{
			git = new Git(gitRepo);
			try
			{
				git.Checkout().SetName(s).SetUpstreamMode(CreateBranchCommand.SetupUpstreamMode.TRACK
					).Call();
				settings.SetGitBranch(s);
			}
			catch (GitAPIException e)
			{
				// TODO Auto-generated catch block
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}

		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="Org.Eclipse.Jgit.Api.Errors.InvalidRemoteException"></exception>
		/// <exception cref="Org.Eclipse.Jgit.Api.Errors.TransportException"></exception>
		/// <exception cref="Org.Eclipse.Jgit.Api.Errors.GitAPIException"></exception>
		public static void GitClone(string url, string path, IList<string> branches)
		{
			Git.CloneRepository().SetURI(url).SetDirectory(new FilePath(path + "/")).SetBranchesToClone
				(branches).SetTimeout(10000).Call();
			gitRepo = InitRepo(path);
		}

		/// <exception cref="System.IO.IOException"></exception>
		public static Repository InitRepo(string path)
		{
			FileRepositoryBuilder builder = new FileRepositoryBuilder();
			Repository repository = null;
			repository = builder.SetGitDir(new FilePath(path + "/.git")).ReadEnvironment().FindGitDir
				().Build();
			return repository;
		}

		/// <exception cref="System.IO.IOException"></exception>
		public bool CheckLocalBranchExists(Repository repo, string branch)
		{
			Ref hurr;
			hurr = repo.GetRef(branch);
			if (hurr == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool DelDir(FilePath directory)
		{
			if (directory.Exists())
			{
				FilePath[] files = directory.ListFiles();
				if (null != files)
				{
					for (int i = 0; i < files.Length; i++)
					{
						if (files[i].IsDirectory())
						{
							DelDir(files[i]);
						}
						else
						{
							files[i].Delete();
						}
					}
				}
			}
			return (directory.Delete());
		}

		public void UpdateStatus(string s, int i)
		{
			progressBar.SetValue(i);
			startText.SetText(s);
		}

		private JProgressBar Startbar()
		{
			startStatus = new JWindow();
			startStatus.SetSize(300, 50);
			startStatus.SetAlwaysOnTop(true);
			startStatus.SetLocation((int)(settings.GetLoggerWindowSize().GetWidth() / 2 + settings
				.GetLoggerWindowLocation().GetX() - 150), (int)(settings.GetLoggerWindowSize().GetHeight
				() / 2 + settings.GetLoggerWindowLocation().GetY() - 36));
			JProgressBar progressBar = new JProgressBar(0, 100);
			progressBar.SetValue(0);
			progressBar.SetIndeterminate(false);
			progressBar.SetOpaque(true);
			startText.SetOpaque(true);
			JPanel panel = new JPanel();
			panel.SetLayout(new BorderLayout());
			panel.SetBorder(BorderFactory.CreateEtchedBorder());
			panel.Add(progressBar, BorderLayout.CENTER);
			panel.Add(startText, BorderLayout.SOUTH);
			startStatus.GetContentPane().Add(panel);
			startStatus.Pack();
			startStatus.SetVisible(true);
			progressBar.SetVisible(true);
			return progressBar;
		}

		public Vector<string> GetAvailableLocalBranches()
		{
			// TODO Auto-generated method stub
			Vector<string> tv = new Vector<string>();
			try
			{
				IList<Ref> trl = git.BranchList().SetListMode(ListBranchCommand.ListMode.ALL).Call
					();
				foreach (Ref r in trl)
				{
					if (!r.GetName().Contains("remotes"))
					{
						tv.AddItem(r.GetName().Replace("refs/heads/", string.Empty));
					}
				}
			}
			catch (GitAPIException e)
			{
				// TODO Auto-generated catch block
				Sharpen.Runtime.PrintStackTrace(e);
			}
			return tv;
		}

		public Vector<string> GetAvailableBranches()
		{
			// TODO Auto-generated method stub
			Vector<string> tv = new Vector<string>();
			try
			{
				IList<Ref> trl = git.BranchList().SetListMode(ListBranchCommand.ListMode.REMOTE).
					Call();
				foreach (Ref r in trl)
				{
					tv.AddItem(r.GetName());
				}
			}
			catch (GitAPIException e)
			{
				//Repository.shortenRefName(r.getName()));
				// TODO Auto-generated catch block
				Sharpen.Runtime.PrintStackTrace(e);
			}
			return tv;
		}

		public string GetCurrentBranch()
		{
			try
			{
				return gitRepo.GetFullBranch();
			}
			catch (IOException e)
			{
				// TODO Auto-generated catch block
				Sharpen.Runtime.PrintStackTrace(e);
				return null;
			}
		}

		public void AddRemote(string name, string url)
		{
			StoredConfig config = git.GetRepository().GetConfig();
			config.SetString("remote", name, "url", url);
			config.SetString("remote", name, "fetch", "+refs/heads/*:refs/remotes/" + name + 
				"/*");
			try
			{
				config.Save();
				RomRaider.Definition.DefinitionRepoManager.InitRepo(Settings.GetGitDefsBaseDir());
				Git.Init().Call();
				git.Fetch().SetRemote(name).Call();
				settings.AddGitRemote(url, name);
			}
			catch (Exception e)
			{
				// TODO Auto-generated catch block
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}
	}
}
