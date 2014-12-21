/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */
using System;
using System.Collections.Generic;
using System.Linq;

using Nancy;

using DOL.GS;
using DOL.GS.ServerProperties;
using DOL.Database;

namespace DOLNancyWeb
{
	/// <summary>
	/// Default Model to subclass for displaying application wide data.
	/// Subclass this Model to implement display within a Master Page.
	/// </summary>
	public class DOLNancyDefaultModel
	{
		/// <summary>
		/// Module related to this model
		/// </summary>
		private NancyContext m_context;
		
		/// <summary>
		/// Return Server Instance
		/// </summary>
		public GameServer Server
		{
			get
			{
				return GameServer.Instance;
			}
		}
		
		/// <summary>
		/// Return Player Instance if Player exists
		/// </summary>
		public GamePlayer Player
		{
			get
			{
				if (m_context != null && m_context.CurrentUser != null)
				{
					var client = WorldMgr.GetClientByAccountName(m_context.CurrentUser.UserName, true);
					if (client != null && client.IsPlaying)
					{
						return client.Player;
					}
				}
				
				return null;
			}
		}
		
		/// <summary>
		/// Human Readable Game Time
		/// </summary>
		public string GameTime 
		{
			get
			{
				uint current = WorldMgr.GetCurrentGameTime();
				return string.Format("{0}h{2}{1}", current / 1000 / 60 / 60, current / 1000 / 60 % 60, (current / 1000 / 60 % 60) < 10 ? "0" : "");
			}
		}

		public IDictionary<string, object> ServerProperties
		{
			get; set;
		}
		
		/// <summary>
		/// Page Title
		/// </summary>
		public virtual string Title { get; set; }
		
		public virtual List<string> StyleSheets
		{
			get; set;
		}
		
		public virtual List<string> Scripts
		{
			get; set;
		}
		
		/// <summary>
		/// Default Constructor with NancyModule Reference.
		/// </summary>
		/// <param name="module"></param>
		public DOLNancyDefaultModel(NancyContext context)
		{
			m_context = context;
			ServerProperties = Properties.AllCurrentProperties;
			
			StyleSheets = new List<string>();
			StyleSheets.Add("/static/style.css");
			
			Scripts = new List<string>();
			Scripts.Add("/static/jquery-2.1.1.min.js");
		}
	}
}
