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
	/// Welcome Module For DOLNancyWeb
	/// Display Defaut Home Page with route "/"
	/// </summary>
	public class DOLNancyWelcome : BasePublicModule
	{
		public DOLNancyWelcome()
			: base()
		{
			// Get Model for Page Display
			var model = new WelcomeModel(this);
			
			Get["/"] = parameters => View["public/welcome.sshtml", model];
		}
		
		class PageModel
		{
			private readonly NancyModule m_module;
			public string Title;
			public bool Authenticated
			{
				get { return m_module.Context != null && m_module.Context.CurrentUser != null && !string.IsNullOrEmpty(m_module.Context.CurrentUser.UserName); }
			}
			
			public PageModel(NancyModule module)
			{
				m_module = module;
			}
		}
		
		class ServerModel
		{
			public string Name;
			public int PlayerCount;
			public uint CurrentTime;
		}
		
		class WelcomeModel
		{
			public PageModel Page;
			public ServerModel Server;
			
			public WelcomeModel(NancyModule welcome)
			{
				Page = new PageModel(welcome);
				Server = new ServerModel();
				
				Page.Title = string.Format("{0} - {1}", "Welcome", GameServer.Instance.Configuration.ServerName);
				
				Server.Name = GameServer.Instance.Configuration.ServerName;
				Server.PlayerCount = WorldMgr.GetAllClientsCount();
				Server.CurrentTime = WorldMgr.GetCurrentGameTime();
			}
		}
	}
}
