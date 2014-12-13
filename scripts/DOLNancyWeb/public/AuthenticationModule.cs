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
using Nancy.Authentication.Forms;
using Nancy.ModelBinding;

using DOL.GS;
using DOL.Database;
using DOL.GS.PacketHandler.Client.v168;

namespace DOLNancyWeb
{
	/// <summary>
	/// This module Handle authentication forms
	/// </summary>
	public class AuthenticationModule : NancyModule
	{
		public AuthenticationModule()
		{
			var loginModel = new {
				Username = string.Empty,
				Password = string.Empty,
			};
			
			Get["/login"] = parameters => View["public/login.sshtml", new { Message = string.Empty, }];
			/*
			Get["/logout"] = parameters => {
			// Called when the user clicks the sign out button in the application. Should
			// perform one of the Logout actions (see below)
			};*/
			
			Post["/login"] = parameters => {
				var loginParams = this.Bind<LoginParams>();
				
				string errorString = string.Empty;
				bool loggedIn = false;
				
				// Try to find the player
				var playerAccount = GameServer.Database.FindObjectByKey<Account>(GameServer.Database.Escape(loginParams.Username));
				var guid = new Guid();
				
				if (playerAccount != null)
				{
					if (playerAccount.Password.Equals(LoginRequestHandler.CryptPassword(loginParams.Password)))
					{
						loggedIn = true;
						DOLUserMapper.AddAuthenticatedUser(guid, new DOLUserIdentity(playerAccount))
					}
				}
				
				
				
				if (!loggedIn)
					return View["public/login.sshtml", new { Message = string.Format("Error While Authenticating User {0} - {1}", loginParams.Username, errorString), }];
				
				return this.LoginAndRedirect(guid, fallbackRedirectUrl: "/");
			};
		}
	}
	
	public class LoginParams
	{
		public string Username;
		public string Password;
	}
}
