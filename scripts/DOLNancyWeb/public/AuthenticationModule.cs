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

namespace DOLNancyWeb
{
	/// <summary>
	/// This module Handle authentication forms
	/// </summary>
	public class AuthenticationModule : BasePublicModule
	{
		public AuthenticationModule()
			: base()
		{
			// Login Form
			Get["/login"] = parameters => View["public/login.sshtml", new { Message = string.Empty, }];

			// Logout URI
			Get["/logout"] = parameters => {
				
				// check is user is logged.
				if (this.Context != null && this.Context.CurrentUser != null && this.Context.CurrentUser is DOLUserIdentity)
					DOLUserMapper.LogoutUser(((DOLUserIdentity)this.Context.CurrentUser).UserGuid);
				
				return this.Logout("~/");
			};
			
			// Login Form Posting
			Post["/login"] = parameters => {
				// Param From Login Form
				var loginParams = this.Bind<LoginParams>();
				
				string errorMessage;
				Guid guid;
				bool ok = DOLUserMapper.ValidateUser(loginParams.Username, loginParams.Password, out guid, out errorMessage);
								
				// Wrong login display form with error message
				if (!ok)
					return View["public/login.sshtml", new { Message = string.Format("Error While Authenticating User {0} - {1}", loginParams.Username, errorMessage), }];
				
				// Login successful redirect and continue !
				return this.Login(guid);
			};
		}
		
		public class LoginParams
		{
			public string Username { get; set; }
			public string Password { get; set; }
		}

	}
}
