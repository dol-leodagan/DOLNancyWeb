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

using Nancy;

using DOL.GS;

namespace DOLNancyWeb
{
	/// <summary>
	/// Default Model to subclass for displaying application wide data.
	/// Subclass this Model to implement display within a Master Page.
	/// </summary>
	public class DOLNancyDefaultModel
	{
		public PageModel Page = new PageModel();
		public ServerModel Server = new ServerModel();
		
		/// <summary>
		/// Default Constructor with NancyModule Reference.
		/// </summary>
		/// <param name="module"></param>
		public DOLNancyDefaultModel(NancyModule module)
		{
		}
		
		/// <summary>
		/// Contains Server Global Info
		/// </summary>
		public class ServerModel
		{
			public string Name;
			public int PlayerCount;
			public uint CurrentTime;
			
			/// <summary>
			/// Default Constructor for Server Model
			/// </summary>
			public ServerModel()
			{
				Name = GameServer.Instance.Configuration.ServerName;
				PlayerCount = WorldMgr.GetAllClientsCount();
				CurrentTime = WorldMgr.GetCurrentGameTime();
			}
		}
		
		/// <summary>
		/// Contains Basic Page Data
		/// </summary>
		public class PageModel
		{
			public string Title;
			
			/// <summary>
			/// Default Constructor for Page Model
			/// </summary>
			public PageModel()
			{
				Title = string.Format("{0} - {1}", "Welcome", GameServer.Instance.Configuration.ServerName);
			}
		}

	}
}
